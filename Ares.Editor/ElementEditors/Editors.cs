/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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

        private static void ShowEditor(EditorBase editor, WeifenLuo.WinFormsUI.Docking.DockPanel parent)
        {
            editor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            editor.Show(parent);
        }

        public static void ShowEditor(Ares.Data.IElement element, Ares.Data.IGeneralElementContainer container, Ares.Data.IProject project, WeifenLuo.WinFormsUI.Docking.DockPanel parent)
        {
            ShowEditor(element, container, null, project, parent);
        }

        public static void ShowEditor(Ares.Data.IElement element, Ares.Data.IModeElement modeElement, Ares.Data.IProject project, WeifenLuo.WinFormsUI.Docking.DockPanel parent)
        {
            ShowEditor(element, null, modeElement, project, parent);
        }

        private static void ShowEditor(Ares.Data.IElement element, Ares.Data.IGeneralElementContainer container, Ares.Data.IModeElement modeElement, Ares.Data.IProject project, WeifenLuo.WinFormsUI.Docking.DockPanel parent)
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
                    editor.SetPlaylist(element as Ares.Data.IRandomBackgroundMusicList, project);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IBackgroundSoundChoice)
                {
                    RandomPlaylistOrBGSoundChoiceEditor editor = new RandomPlaylistOrBGSoundChoiceEditor();
                    editor.SetBGSoundChoice(element as Ares.Data.IBackgroundSoundChoice, project);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.ISequentialBackgroundMusicList)
                {
                    SequentialPlaylistEditor editor = new SequentialPlaylistEditor();
                    editor.SetPlaylist(element as Ares.Data.ISequentialBackgroundMusicList, project);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IMacro)
                {
                    MacroEditor editor = new MacroEditor();
                    editor.SetContainer(element as Ares.Data.IMacro, project);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IElementContainer<Ares.Data.IChoiceElement>)
                {
                    ChoiceContainerEditor editor = new ChoiceContainerEditor();
                    editor.SetContainer(element as Ares.Data.IElementContainer<Ares.Data.IChoiceElement>, project);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.ISequentialContainer)
                {
                    SequentialContainerEditor editor = new SequentialContainerEditor();
                    editor.SetContainer(element as Ares.Data.ISequentialContainer, project);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IElementContainer<Ares.Data.IParallelElement>)
                {
                    ParallelContainerEditor editor = new ParallelContainerEditor();
                    editor.SetContainer(element as Ares.Data.IElementContainer<Ares.Data.IParallelElement>, project);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IMusicByTags)
                {
                    MusicByTagsEditor editor = new MusicByTagsEditor();
                    editor.SetElement(element as Ares.Data.IMusicByTags, project);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IReferenceElement)
                {
                    Ares.Data.IElement referencedElement = Ares.Data.DataModule.ElementRepository.GetElement((element as Ares.Data.IReferenceElement).ReferencedId);
                    if (referencedElement != null)
                    {
                        ShowEditor(referencedElement, container, project, parent);
                    }
                }
                else if (element is Ares.Data.IWebRadioElement)
                {
                    WebRadioEditor editor = new WebRadioEditor();
                    editor.SetElement(element as Ares.Data.IWebRadioElement, project, modeElement);
                    ShowEditor(editor, parent);
                }
                else if (element is Ares.Data.IFileElement)
                {
                    Ares.Data.IFileElement fileElement = (Ares.Data.IFileElement)element;
                    if (fileElement.FilePath.EndsWith(".m3u", StringComparison.InvariantCultureIgnoreCase) ||
                        fileElement.FilePath.EndsWith(".m3u8", StringComparison.InvariantCultureIgnoreCase) ||
                        fileElement.FilePath.EndsWith(".pls", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String basePath = fileElement.SoundFileType == Data.SoundFileType.Music ? Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
                        String filePath = System.IO.Path.Combine(basePath, fileElement.FilePath);
                        System.Diagnostics.Process.Start(filePath);
                    }
                    else
                    {
                        FileElementEditor editor = new FileElementEditor();
                        editor.SetElement(element as Ares.Data.IFileElement, container, project);
                        ShowEditor(editor, parent);
                    }
                }
            }
        }

        public static void ShowTriggerEditor(Ares.Data.IModeElement element, Ares.Data.IProject project, WeifenLuo.WinFormsUI.Docking.DockPanel parent)
        {
            EditorBase existing = EditorRegistry.Instance.GetEditor(element.Id);
            if (existing != null)
            {
                existing.Activate();
            }
            else
            {
                TriggerEditor editor = new TriggerEditor(project);
                editor.SetElement(element);
                ShowEditor(editor, parent);
            }
        }

    }
}
