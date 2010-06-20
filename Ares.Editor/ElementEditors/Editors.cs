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
            }
        }

    }
}
