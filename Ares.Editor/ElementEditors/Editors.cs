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
        public static void ShowEditor(Ares.Data.IElement element, WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel)
        {
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
                    editor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
                    editor.SetPlaylist(element as Ares.Data.IRandomBackgroundMusicList);
                    editor.Show(dockPanel);
                }
                else if (element is Ares.Data.IBackgroundSoundChoice)
                {
                    RandomPlaylistOrBGSoundChoiceEditor editor = new RandomPlaylistOrBGSoundChoiceEditor();
                    editor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
                    editor.SetBGSoundChoice(element as Ares.Data.IBackgroundSoundChoice);
                    editor.Show(dockPanel);
                }
                else if (element is Ares.Data.IElementContainer<Ares.Data.IChoiceElement>)
                {
                    ChoiceContainerEditor editor = new ChoiceContainerEditor();
                    editor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
                    editor.SetContainer(element as Ares.Data.IElementContainer<Ares.Data.IChoiceElement>);
                    editor.Show(dockPanel);
                }
                else if (element is Ares.Data.IElementContainer<Ares.Data.ISequentialElement>)
                {
                    SequentialContainerEditor editor = new SequentialContainerEditor();
                    editor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
                    editor.SetContainer(element as Ares.Data.IElementContainer<Ares.Data.ISequentialElement>);
                    editor.Show(dockPanel);
                }
            }
        }

        public static void ShowTriggerEditor(Ares.Data.IModeElement element, WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel)
        {
            EditorBase existing = EditorRegistry.Instance.GetEditor(element.Id);
            if (existing != null)
            {
                existing.Activate();
            }
            else
            {
                TriggerEditor editor = new TriggerEditor();
                editor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
                editor.SetElement(element);
                editor.Show(dockPanel);
            }
        }

    }
}
