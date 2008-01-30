using System;

namespace Ares.Data
{
    [Serializable]
    class BasicFileElement : ElementBase, IElement, IFileElement
    {
        internal BasicFileElement(int ID, String filePath)
            : base(ID)
        {
            FilePath = filePath;
            Title = FileName;
        }

        public String FileName { get { return m_FileName; } }
        public String FilePath { get { return m_FilePath; } 
            set {
                m_FilePath = value;
                int pos = m_FilePath.LastIndexOf('\\');
                m_FileName = (pos != -1) ? m_FilePath.Substring(pos + 1) : m_FilePath;
            }
        }

        private String m_FileName;
        private String m_FilePath;

    }
}
