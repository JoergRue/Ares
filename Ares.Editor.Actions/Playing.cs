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
            ElementChanges.Instance.ElementPlayed(element.Id);
            if (element is Ares.Data.IGeneralElementContainer)
            {
                foreach (Ares.Data.IContainerElement e in (element as Ares.Data.IGeneralElementContainer).GetGeneralElements())
                {
                    NotifyStart(e.InnerElement);
                }
            }
        }

        private void NotifyEnd(Ares.Data.IElement element)
        {
            ElementChanges.Instance.ElementStopped(element.Id);
            if (element is Ares.Data.IGeneralElementContainer)
            {
                foreach (Ares.Data.IContainerElement e in (element as Ares.Data.IGeneralElementContainer).GetGeneralElements())
                {
                    NotifyEnd(e.InnerElement);
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

        public bool IsElementOrSubElementPlaying(Ares.Data.IElement element)
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
                    if (IsElementOrSubElementPlaying(subElement))
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
