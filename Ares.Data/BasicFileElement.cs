using System;

namespace Ares.Data
{
    [Serializable]
    class BasicFileElement : ElementBase, IElement, IFileElement
    {
        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitFileElement(this);
        }

        internal BasicFileElement(int ID, String filePath, BasicFileCategory category)
            : base(ID)
        {
            FilePath = filePath;
            Title = FileName;
            m_FileCategory = category;
        }

        public String FileName { get { return m_FileName; } }
        public String FilePath { get { return m_FilePath; } 
            set {
                m_FilePath = value;
                int pos = m_FilePath.LastIndexOf('\\');
                m_FileName = (pos != -1) ? m_FilePath.Substring(pos + 1) : m_FilePath;
            }
        }

        public BasicFileCategory FileCategory { get { return m_FileCategory; } }

        private String m_FileName;
        private String m_FilePath;
        private BasicFileCategory m_FileCategory;

    }
}
