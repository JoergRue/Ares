using System;

using MediaPortal.Profile;
using MediaPortal.Configuration;

namespace Ares.Plugin
{
    class AresSettings
    {
        public static void ReadFromConfigFile()
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            try
            {
                using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "aresplugin.xml")))
                {
                    settings.RecentFiles.AddFile(new Settings.RecentFiles.ProjectEntry(reader.GetValue("Files", "ProjectFile"), ""));
                    settings.SoundDirectory = reader.GetValue("Directories", "SoundsDirectory");
                    settings.MusicDirectory = reader.GetValue("Directories", "MusicDirectory");
                    settings.UdpPort = reader.GetValueAsInt("Network", "UDPPort", 8009);
                    settings.TcpPort = reader.GetValueAsInt("Network", "TCPPort", 11112);
                    settings.IPAddress = reader.GetValue("Network", "IPAddress");
                    settings.GlobalVolume = reader.GetValueAsInt("Volume", "Global", 100);
                    settings.MusicVolume = reader.GetValueAsInt("Volume", "Music", 100);
                    settings.SoundVolume = reader.GetValueAsInt("Volume", "Sounds", 100);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void SaveToConfigFile()
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            using (MediaPortal.Profile.Settings writer = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "aresplugin.xml")))
            {
                String projectFile = settings.RecentFiles.GetFiles().Count > 0 ? settings.RecentFiles.GetFiles()[0].FilePath : "";
                writer.SetValue("Files", "ProjectFile", projectFile);
                writer.SetValue("Directories", "SoundsDirectory", settings.SoundDirectory);
                writer.SetValue("Directories", "MusicDirectory", settings.MusicDirectory);
                writer.SetValue("Network", "UDPPort", settings.UdpPort);
                writer.SetValue("Network", "TCPPort", settings.TcpPort);
                writer.SetValue("Network", "IPAddress", settings.IPAddress);
                writer.SetValue("Volume", "Global", settings.GlobalVolume);
                writer.SetValue("Volume", "Music", settings.MusicVolume);
                writer.SetValue("Volume", "Sounds", settings.SoundVolume);
            }
        }
    }
}
