/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Editor.Actions
{
    public class Playing : Ares.Playing.IElementPlayingCallbacks
    {
        public void PlayElement(Ares.Data.IElement element, System.Windows.Forms.Control guiControl, System.Action finishAction)
        {
            m_PlayedElements[element.Id] = true;
            IsPlaying = true;
            NotifyStart(element);
            if (Actions.Instance.UpdateGUI != null)
                Actions.Instance.UpdateGUI();
            DoPlayElement(element, guiControl, () => 
            {
                m_PlayedElements.Remove(element.Id);
                IsPlaying = m_PlayedElements.Count != 0;
                NotifyEnd(element);
                if (Actions.Instance.UpdateGUI != null)
                    Actions.Instance.UpdateGUI();
                finishAction(); 
            });
        }

        private void NotifyStart(Ares.Data.IElement element)
        {
            NotifyStart(new HashSet<Ares.Data.IElement>(), element);
        }

        private void NotifyStart(HashSet<Ares.Data.IElement> references, Ares.Data.IElement element)
        {
            ElementChanges.Instance.ElementPlayed(element.Id);
            if (element is Ares.Data.IGeneralElementContainer)
            {
                foreach (Ares.Data.IContainerElement e in (element as Ares.Data.IGeneralElementContainer).GetGeneralElements())
                {
                    NotifyStart(references, e.InnerElement);
                }
            }
            else if (element is Ares.Data.IReferenceElement)
            {
                if (!references.Contains(element))
                {
                    references.Add(element);
                    Ares.Data.IElement referencedElement = Ares.Data.DataModule.ElementRepository.GetElement((element as Ares.Data.IReferenceElement).ReferencedId);
                    if (referencedElement != null)
                    {
                        NotifyStart(references, referencedElement);
                    }
                }
            }
        }

        private void NotifyEnd(Ares.Data.IElement element)
        {
            NotifyEnd(new HashSet<Ares.Data.IElement>(), element);
        }

        private void NotifyEnd(HashSet<Ares.Data.IElement> references, Ares.Data.IElement element)
        {
            ElementChanges.Instance.ElementStopped(element.Id);
            if (element is Ares.Data.IGeneralElementContainer)
            {
                foreach (Ares.Data.IContainerElement e in (element as Ares.Data.IGeneralElementContainer).GetGeneralElements())
                {
                    NotifyEnd(references, e.InnerElement);
                }
            }
            else if (element is Ares.Data.IReferenceElement)
            {
                if (!references.Contains(element))
                {
                    references.Add(element);
                    Ares.Data.IElement referencedElement = Ares.Data.DataModule.ElementRepository.GetElement((element as Ares.Data.IReferenceElement).ReferencedId);
                    if (referencedElement != null)
                    {
                        NotifyEnd(references, referencedElement);
                    }
                }
            }
        }

        private void DoPlayElement(Ares.Data.IElement element, System.Windows.Forms.Control guiControl, System.Action finishAction)
        {
            lock (syncObject)
            {
                m_FinishedActions[element.Id] = () =>
                    {
                        if (guiControl.IsDisposed)
                            return;
                        if (guiControl.InvokeRequired)
                        {
                            guiControl.BeginInvoke(new System.Windows.Forms.MethodInvoker(() => { finishAction(); }));
                        }
                        else
                        {
                            finishAction();
                        }
                    };
            }
            Ares.Playing.PlayingModule.ElementPlayer.PlayElement(element);
        }

        public Ares.Data.IElement PlayFile(String relativePath, bool isMusic, System.Windows.Forms.Control guiControl, System.Action finishAction)
        {
            Ares.Data.IFileElement fileElement =
                Ares.Data.DataModule.ElementFactory.CreateFileElement(relativePath, isMusic ? Ares.Data.SoundFileType.Music : Ares.Data.SoundFileType.SoundEffect);
            PlayElement(fileElement, guiControl, finishAction);
            return fileElement;
        }

        public void StopAll()
        {
            Ares.Playing.PlayingModule.ElementPlayer.StopAll();
        }

        public void StopElement(Ares.Data.IElement element)
        {
            Ares.Playing.PlayingModule.ElementPlayer.StopElement(element);
        }

        public void SetDirectories(String musicDir, String soundsDir)
        {
            Ares.Playing.PlayingModule.ElementPlayer.SetMusicPath(musicDir);
            Ares.Playing.PlayingModule.ElementPlayer.SetSoundPath(soundsDir);
        }

        public bool IsPlaying { get; private set; }

        public bool IsElementPlaying(Ares.Data.IElement element)
        {
            lock (syncObject)
            {
                return m_PlayedElements.ContainsKey(element.Id);
            }
        }

        public bool IsElementOrSubElementPlaying(Ares.Data.IElement element)
        {
            return IsElementOrSubElementPlaying(new HashSet<Ares.Data.IElement>(), element);
        }

        private bool IsElementOrSubElementPlaying(HashSet<Ares.Data.IElement> checkedReferences, Ares.Data.IElement element)
        {
            lock (syncObject)
            {
                if (m_PlayedElements.ContainsKey(element.Id))
                    return true;
            }
            if (element is Ares.Data.IGeneralElementContainer)
            {
                foreach (Ares.Data.IContainerElement subElement in (element as Ares.Data.IGeneralElementContainer).GetGeneralElements())
                {
                    if (IsElementOrSubElementPlaying(checkedReferences, subElement))
                        return true;
                }
            }
            else if (element is Ares.Data.IContainerElement)
            {
                Ares.Data.IElement inner = (element as Ares.Data.IContainerElement).InnerElement;
                if (inner != element && IsElementOrSubElementPlaying(checkedReferences, inner))
                    return true;
            }
            else if (element is Ares.Data.IReferenceElement)
            {
                if (checkedReferences.Contains(element))
                    return false;
                checkedReferences.Add(element);
                Ares.Data.IElement referencedElement = Ares.Data.DataModule.ElementRepository.GetElement((element as Ares.Data.IReferenceElement).ReferencedId);
                if (referencedElement != null && IsElementOrSubElementPlaying(checkedReferences, referencedElement))
                {
                    return true;
                }
            }
            else
            {
                lock (syncObject)
                {
                    if (IsSubElementOfPlayedElement(element))
                        return true;
                }
            }
            return false;
        }

        private bool IsSubElementOfPlayedElement(Ares.Data.IElement element)
        {
            foreach (int elementID in m_PlayedElements.Keys)
            {
                if (IsSubElementOfElement(Ares.Data.DataModule.ElementRepository.GetElement(elementID), element))
                    return true;
            }
            return false;
        }

        private bool IsSubElementOfElement(Ares.Data.IElement parent, Ares.Data.IElement element)
        {
            HashSet<Ares.Data.IElement> checkedReferences = new HashSet<Data.IElement>();
            return IsSubElementOfElements(checkedReferences, parent, element);
        }

        private bool IsSubElementOfElements(HashSet<Ares.Data.IElement> checkedReferences, Ares.Data.IElement parent, Ares.Data.IElement element)
        {
            if (parent == element)
                return true;
            else if (parent is Ares.Data.IGeneralElementContainer)
            {
                foreach (Ares.Data.IContainerElement containerElement in (parent as Ares.Data.IGeneralElementContainer).GetGeneralElements())
                {
                    if (IsSubElementOfElements(checkedReferences, containerElement.InnerElement, element))
                        return true;
                }
            }
            else if (parent is Ares.Data.IReferenceElement)
            {
                if (checkedReferences.Contains(parent))
                    return false;
                checkedReferences.Add(parent);
                Ares.Data.IElement referencedElement = Ares.Data.DataModule.ElementRepository.GetElement((parent as Ares.Data.IReferenceElement).ReferencedId);
                if (referencedElement != null && IsSubElementOfElements(checkedReferences, referencedElement, element))
                {
                    return true;
                }
            }
            return false;
        }

        public void ElementFinished(Int32 elementId)
        {
            System.Action finishAction = null;
            lock (syncObject)
            {
                if (m_FinishedActions.ContainsKey(elementId))
                {
                    finishAction = m_FinishedActions[elementId];
                    m_FinishedActions.Remove(elementId);
                }
            }
            if (finishAction != null)
            {
                finishAction();
            }
        }

        public void ErrorOccurred(int elementID, String errorMessage)
        {
            StopAll();
            if (ErrorHandling != null)
            {
                Ares.Data.IElement element = null;
                if (elementID != -1)
                {
                    element = Ares.Data.DataModule.ElementRepository.GetElement(elementID);
                }
                if (ErrorHandling.Control.InvokeRequired)
                {
                    ErrorHandling.Control.BeginInvoke(new System.Windows.Forms.MethodInvoker(() =>
                        ErrorHandling.ErrorHandlingMethod(element, errorMessage)));
                }
                else
                {
                    ErrorHandling.ErrorHandlingMethod(element, errorMessage);
                }
            }
        }

        public void AddMessage(Ares.Playing.MessageType messageType, String message)
        {
            if (messageType == Ares.Playing.MessageType.Error)
                ErrorOccurred(-1, message);
        }

        public class ErrorHandler
        {
            public System.Action<Ares.Data.IElement, String> ErrorHandlingMethod { get; set; }
            public System.Windows.Forms.Control Control { get; set; }

            public ErrorHandler(System.Windows.Forms.Control control, System.Action<Ares.Data.IElement, String> method)
            {
                ErrorHandlingMethod = method;
                Control = control;
            }
        }

        public ErrorHandler ErrorHandling { get; set; }

        private Dictionary<int, System.Action> m_FinishedActions = new Dictionary<int, System.Action>();
        private Dictionary<int, bool> m_PlayedElements = new Dictionary<int, bool>();
        private Object syncObject = new Int16();

        private Playing()
        {
            Ares.Playing.PlayingModule.SetCallbacks(this);
        }

        public static Playing Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new Playing();
                }
                return sInstance;
            }
        }

        private static Playing sInstance;
    }
}
