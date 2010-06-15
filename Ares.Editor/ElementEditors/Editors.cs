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
                    RandomPlaylistEditor editor = new RandomPlaylistEditor();
                    editor.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
                    editor.SetPlaylist(element as Ares.Data.IRandomBackgroundMusicList);
                    editor.Show(dockPanel);
                }
            }
        }

    }
}
