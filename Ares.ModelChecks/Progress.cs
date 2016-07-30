/*
    Copyright (c) 2015  [Joerg Ruedenauer]
  
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

namespace Ares.ModelInfo
{
    /// <summary>
    /// Progress monitors can be used to ... monitor progress.
    /// Progress is measures in percentage values (between 0 and 100).
    /// </summary>
    public interface IProgressMonitor
    {
        bool Canceled { get; }
        void IncreaseProgress(double percent);
        void IncreaseProgress(double percent, String text);
        void SetProgress(int percent, String text);
        void SetIndeterminate(String text);
    }

    /// <summary>
    /// IProgressMonitor equivalent for tasks with a known total size
    /// </summary>
    public interface IAbsoluteProgressMonitor: IProgressMonitor
    {
        double TaskSize { get; set; }

        void SetProgress(double amount);
        void SetProgress(double amount, String text);

        void SetIndeterminate();

        void ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Wrapper implementation of IAbsoluteProgressMonitor:
    /// This implementation wraps around an existing IProgressMonitor. It accepts information about task size and absolute progress and
    /// forwards the calculated relative progress to the underlying IProgressMonitor.
    /// 
    /// In addition it checks the "Canceled" state of the underlying IProgressMonitor every time the progress is updated and, if the 
    /// monitor was marked "cancelled" throws an OperationCancelledException.
    /// </summary>
    public class AbsoluteProgressMonitor: IAbsoluteProgressMonitor
    {
        private IProgressMonitor m_baseMonitor;
        private double m_taskSize;
        private double? m_progress;
        private string m_text;

        public AbsoluteProgressMonitor(IProgressMonitor baseMonitor): this(baseMonitor, 100, String.Empty)
        {

        }

        public AbsoluteProgressMonitor(IProgressMonitor baseMonitor, string text) : this(baseMonitor, 100, text)
        {

        }

        public AbsoluteProgressMonitor(IProgressMonitor baseMonitor, double taskSize) : this(baseMonitor, taskSize, String.Empty)
        {

        }

        public AbsoluteProgressMonitor(IProgressMonitor baseMonitor, double taskSize, string text)
        {
            this.m_baseMonitor = baseMonitor;
            this.m_taskSize = taskSize;
            this.m_progress = null;
            this.m_text = text;
        }

        public bool Canceled
        {
            get
            {
                return m_baseMonitor.Canceled;
            }
        }

        public double TaskSize
        {
            get
            {
                return m_taskSize;
            }
            set
            {
                // If there 
                if (m_progress.HasValue)
                {
                    m_progress = (m_progress / m_taskSize) * value;
                }
                m_taskSize = value;
                UpdateBaseMonitor();
            }
        }

        public void IncreaseProgress(double percent)
        {
            m_progress += percent;
            UpdateBaseMonitor();
        }

        public void IncreaseProgress(double percent, string text)
        {
            m_text = text;
            m_progress += percent;
            UpdateBaseMonitor();
        }

        public void SetIndeterminate()
        {
            m_progress = null;
            UpdateBaseMonitor();
        }

        public void SetIndeterminate(string text)
        {
            m_text = text;
            m_progress = null;
            UpdateBaseMonitor();
        }

        public void SetProgress(double amount)
        {
            m_progress = amount;
            UpdateBaseMonitor();
        }

        public void SetProgress(int percent, string text)
        {
            m_text = text;
            m_progress = percent;
            UpdateBaseMonitor();
        }

        public void SetProgress(double amount, string text)
        {
            m_text = text;
            m_progress = amount;
            UpdateBaseMonitor();
        }

        public void ThrowIfCancellationRequested()
        {
            if (m_baseMonitor.Canceled)
            {
                throw new OperationCanceledException();
            }
        }

        private void UpdateBaseMonitor()
        {
            ThrowIfCancellationRequested();

            if (m_progress.HasValue)
            {
                m_baseMonitor.SetProgress((int)(100 * m_progress / m_taskSize), m_text);
            } else
            {
                m_baseMonitor.SetIndeterminate(m_text);
            }
        }
    }

}
