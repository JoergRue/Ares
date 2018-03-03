using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Ares.Data;

namespace Ares.Playing
{
    class LightEffectsPlayer
    {
        private static LightEffectsPlayer s_Instance;

        public static LightEffectsPlayer Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new LightEffectsPlayer();
                }
                return s_Instance;
            }
        }

        private LightEffectsPlayer()
        {
            EmitCurrentState();
        }



        private class Tpm2JinxPacket
        {
            // Format tpm2.net Jinx! remote control
            private byte[] data ={
                0x9c,   //  0 Block Start Byte
                0xda,   //  1 Block Type (da: Data, c0: Command)
                0x00,   //  2 Frame Size MSB (0007)
                0x07,   //  3 Frame Size LSB (0007)
                0x00,   //  4 Packet Number for splitting large packets, 0: unused
                0x00,   //  5 Total Packet Count for splitting large packets, 0: unused
                0x00,   //  6 Left Scene, counting from 1, 0: no change
                0x00,   //  7 Right Scene, counting from 1, 0: no change
                0x00,   //  8 Chase, counting from 1, 0: no change (not used in Ares)
                0x00,   //  9 Left/Right mix (Cross fade) 0: left, 127: middle, 255: right
                0x00,   // 10 Master Strobe, 0: no strobe, 255: fast strobe (not used in Ares) (accd to Jinx! manual swapped with MB, error in Jinx! manual)
                0x00,   // 11 Master Brightness, 0: dark, 255: bright (accd to Jinx! manual swapped with MS, error in Jinx! manual)
                0x36    // 12 Block End Byte
            };
            public byte LeftScene
            {
                get { return data[6]; }
                set { data[6] = value; }
            }
            public byte RightScene
            {
                get { return data[7]; }
                set { data[7] = value; }
            }
            public byte LRMix
            {
                get { return data[9]; }
                set { data[9] = value; }
            }
            public byte MasterBrightness
            {
                get { return data[11]; }
                set { data[11] = value; }
            }

            public byte[] Data
            {
                get { return data; }
            }

            public Tpm2JinxPacket() { }
            public Tpm2JinxPacket(byte left, byte right, byte mix, byte master)
            {
                LeftScene = left;
                RightScene = right;
                LRMix = mix;
                MasterBrightness = master;
            }
        }




        int m_MasterBrightness = 255;
        int m_LeftRightMix = 127;
        int m_LeftScene = 1;
        int m_RightScene = 1;

        private void EmitCurrentState()
        {
            Tpm2JinxPacket packet = new Tpm2JinxPacket((byte)m_LeftScene,(byte)m_RightScene, (byte)m_LeftRightMix, (byte)m_MasterBrightness);
            SendPacket(packet);
        }

        public void PlayLightEffects(ILightEffects effects)
        {
            Tpm2JinxPacket packet = new Tpm2JinxPacket();

            if (effects.SetsMasterBrightness)
                m_MasterBrightness = effects.MasterBrightness;
            if (effects.SetsLeftRightMix)
                m_LeftRightMix = effects.LeftRightMix;

            packet.MasterBrightness = (byte)m_MasterBrightness;
            packet.LRMix = (byte)m_LeftRightMix;
            
            if (effects.SetsLeftScene)
            {
                m_LeftScene = effects.LeftScene;
                packet.LeftScene = (byte)m_LeftScene;
            }
            if (effects.SetsRightScene)
            {
                m_RightScene = effects.RightScene;
                packet.RightScene = (byte)m_RightScene;
            }

            SendPacket(packet);
        }

        private void SendPacket(Tpm2JinxPacket packet)
        {
            using (UdpClient c = new UdpClient())
                c.Send(packet.Data, packet.Data.Length, "192.168.0.127", 65506);
            // mal sehen...
        }
    }
}
