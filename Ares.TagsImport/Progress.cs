/*
 Copyright (c) 2013 [Joerg Ruedenauer]
 
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
using Ares.ModelInfo;

namespace Ares.TagsImport
{
    public class SequentialProgressMonitor : IProgressMonitor
    {
        private IProgressMonitor m_Parent;
        private double m_Offset;
        private double m_Part;
        private double m_LastPassedPercent;
        private double m_OverallPercent;

        private Object m_LockObject = new Object();

        public SequentialProgressMonitor(IProgressMonitor parent, double offset, double part)
        {
            m_Parent = parent;
            m_Offset = offset;
            m_Part = part;
            m_OverallPercent = m_Offset;
            m_LastPassedPercent = m_Offset;
        }

        private double DoIncreaseProgress(double percent)
        {
            double inc = 0.0;
            lock (m_LockObject)
            {
                m_OverallPercent = m_OverallPercent + (percent / 100.0 * m_Part);
                if (m_OverallPercent - m_LastPassedPercent > 0.5)
                {
                    inc = m_OverallPercent - m_LastPassedPercent;
                    m_LastPassedPercent = m_OverallPercent;
                }
            }
            return inc;
        }

        public void IncreaseProgress(double percent)
        {
            double inc = DoIncreaseProgress(percent);
            if (inc >= 0.5)
            {
                m_Parent.IncreaseProgress(inc);
            }
        }

        public void IncreaseProgress(double percent, String text)
        {
            double inc = DoIncreaseProgress(percent);
            if (inc >= 0.5)
            {
                m_Parent.IncreaseProgress(inc, text);
            }
        }

        public void SetProgress(int percent, String text)
        {
            throw new NotImplementedException();
        }

        public void SetIndeterminate(String text)
        {
            throw new NotImplementedException();
        }

        public bool Canceled
        {
            get { return m_Parent.Canceled; }
        }
    }

    class ParallelProgressMonitor : IProgressMonitor
    {
        private IProgressMonitor m_Parent;

        private double m_LastPassedPercent;
        private double m_OverallPercent;
        private double m_CurrentParts;
        private double m_OverallParts;

        private Object m_LockObject = new Object();

        public ParallelProgressMonitor(IProgressMonitor parent, double partPerSubProgress, int nrOfSubProcesses)
        {
            m_Parent = parent;
            m_LastPassedPercent = 0;
            m_OverallPercent = 0;
            m_OverallParts = partPerSubProgress * nrOfSubProcesses;
            m_CurrentParts = 0;
        }

        private double DoIncreaseProgress(double percent)
        {
            double inc = 0.0;
            lock (m_LockObject)
            {
                m_CurrentParts += percent;
                m_OverallPercent = m_CurrentParts / m_OverallParts * 100.0;
                if (m_OverallPercent - m_LastPassedPercent > 0.5)
                {
                    inc = m_OverallPercent - m_LastPassedPercent;
                    m_LastPassedPercent = m_OverallPercent;
                }                
            }
            return inc;
        }

        public void IncreaseProgress(double percent)
        {
            double inc = DoIncreaseProgress(percent);
            if (inc > 0.5)
            {
                m_Parent.IncreaseProgress(inc);
            }
        }

        public void IncreaseProgress(double percent, String text)
        {
            double inc = DoIncreaseProgress(percent);
            if (inc > 0.5)
            {
                m_Parent.IncreaseProgress(inc, text);
            }
        }

        public void SetProgress(int percent, String text)
        {
            throw new NotImplementedException();
        }

        public void SetIndeterminate(String text)
        {
            throw new NotImplementedException();
        }

        public bool Canceled
        {
            get { return m_Parent.Canceled; }
        }

    }

}