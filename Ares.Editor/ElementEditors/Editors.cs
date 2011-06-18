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

namespace Ares.Editor.ElementEditors
{
    static class Editors
    {

#if !MONO
        private static void ShowEditor(EditorBase editor, WeifenLuo.WinFormsUI.Docking.DockPanel parent)
        {
            editor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            editor.Show(parent);
        }
#else
        private static void ShowEditor(EditorBase editor, System.Windows.Forms.Form parent)
        {
            editor.MdiParent = parent;
            editor.Show();
        }
#endif

#if !MONO
        public static void ShowEditor(Ares.Data.IElement element, Ares.Data.IGeneralElementContainer container, WeifenLuo.WinFormsUI.Docking.DockPanel parent)
#else
        public static void ShowEditor(Ares.Data.IElement element, Ares.Data.IGeneralElementContainer container, System.Windows.Forms.Form parent)
#endif
        {
            if (element == null)
                return;
            EditorBase existing = EditorRegistry.Instance.GetEditor(element.Id);
            if (existing != null)
            {
                existing.Activate();
            }
            else
            {
                if (element is Ares.Data.IRandomBackgroundMusicList)
                {
                    RandomPlaylistOrBGSoundChoiceEditor editor = new RandomPlaylistOrBGSoundChoiceEditor();
                    editor.SetPlaylist(element as Ares.Data.IRandomBackgroundMusicList);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IBackgroundSoundChoice)
                {
                    RandomPlaylistOrBGSoundChoiceEditor editor = new RandomPlaylistOrBGSoundChoiceEditor();
                    editor.SetBGSoundChoice(element as Ares.Data.IBackgroundSoundChoice);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.ISequentialBackgroundMusicList)
                {
                    SequentialPlaylistEditor editor = new SequentialPlaylistEditor();
                    editor.SetPlaylist(element as Ares.Data.ISequentialBackgroundMusicList);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IElementContainer<Ares.Data.IChoiceElement>)
                {
                    ChoiceContainerEditor editor = new ChoiceContainerEditor();
                    editor.SetContainer(element as Ares.Data.IElementContainer<Ares.Data.IChoiceElement>);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.ISequentialContainer)
                {
                    SequentialContainerEditor editor = new SequentialContainerEditor();
                    editor.SetContainer(element as Ares.Data.ISequentialContainer);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IElementContainer<Ares.Data.IParallelElement>)
                {
                    ParallelContainerEditor editor = new ParallelContainerEditor();
                    editor.SetContainer(element as Ares.Data.IElementContainer<Ares.Data.IParallelElement>);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IFileElement)
                {
                    FileElementEditor editor = new FileElementEditor();
                    editor.SetElement(element as Ares.Data.IFileElement, container);
                    ShowEditor(editor, parent);
                }
            }
        }

#if !MONO
        public static void ShowTriggerEditor(Ares.Data.IModeElement element, WeifenLuo.WinFormsUI.Docking.DockPanel parent)
#else
        public static void ShowTriggerEditor(Ares.Data.IModeElement element, System.Windows.Forms.Form parent)
#endif
        {
            EditorBase existing = EditorRegistry.Instance.GetEditor(element.Id);
            if (existing != null)
            {
                existing.Activate();
            }
            else
            {
                TriggerEditor editor = new TriggerEditor();
                editor.SetElement(element);
                ShowEditor(editor, parent);
            }
        }

    }
}
