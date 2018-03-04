using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Ares.Data;

namespace Ares.Playing
{
 
    class LightEffectsPlayer
    {
        public LightEffectsPlayer()
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
                0x00,   //  9 Cross fade mode, only 0: progressive used in Ares
                0x00,   // 10 Left/Right mix (Cross fade) 0: left, 127: middle, 255: right
                0x00,   // 11 Master Strobe, 0: no strobe, 255: fast strobe (not used in Ares) (accd to Jinx! manual swapped with MB, error in Jinx! manual)
                0x00,   // 12 Master Brightness, 0: dark, 255: bright (accd to Jinx! manual swapped with MS, error in Jinx! manual)
                0x36    // 13 Block End Byte
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
                get { return data[10]; }
                set { data[10] = value; }
            }
            public byte MasterBrightness
            {
                get { return data[12]; }
                set { data[12] = value; }
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


        private Object m_Lock = new Int32();

        private double m_BrightnessFactor = 1.0;

        private int m_MasterBrightness = 255;
        private int m_LeftRightMix = 127;
        private int m_LeftScene = 1;
        private int m_RightScene = 1;

        private int MasterBrightness
        {
            get
            {
                return (int)Math.Round(m_MasterBrightness*m_BrightnessFactor);
            }
            set
            {
                m_MasterBrightness = value;
            }
        }

        public double BrightnessFactor
        {
            get { return m_BrightnessFactor; }
            set
            {
                lock (m_Lock)
                {
                    m_BrightnessFactor = value;
                    SendCurrentStateWithoutScenes();
                }
            }
        }

        public void EmitCurrentState()
        {
            lock (m_Lock)
            {
                Tpm2JinxPacket packet = new Tpm2JinxPacket((byte)m_LeftScene, (byte)m_RightScene, (byte)m_LeftRightMix, (byte)MasterBrightness);
                SendPacketWithRepeat(packet);
            }
        }

        public void PlayLightEffects(ILightEffects effects)
        {
            lock (m_Lock)
            {
                Tpm2JinxPacket packet = new Tpm2JinxPacket();

                if (effects.SetsMasterBrightness)
                    MasterBrightness = effects.MasterBrightness;
                if (effects.SetsLeftRightMix)
                    m_LeftRightMix = effects.LeftRightMix;

                packet.MasterBrightness = (byte)MasterBrightness;
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

                SendPacketWithRepeat(packet);
            }
        }

        private void SendPacket(Tpm2JinxPacket packet)
        {
            using (UdpClient c = new UdpClient())
                c.Send(packet.Data, packet.Data.Length, "192.168.0.127", 65506);
            // mal sehen...
        }

        private void SendCurrentStateWithoutScenes()
        {
            Tpm2JinxPacket packet = new Tpm2JinxPacket();
            packet.LRMix = (byte)m_LeftRightMix;
            packet.MasterBrightness = (byte)MasterBrightness;
            SendPacket(packet);
        }

        private void SendPacketWithRepeat(Tpm2JinxPacket packet)
        {
            SendPacket(packet);
            Thread.Sleep(50);
            SendCurrentStateWithoutScenes();
            Thread.Sleep(50);
        }

    }
}
