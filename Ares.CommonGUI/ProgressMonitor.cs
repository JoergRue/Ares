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

namespace Ares.CommonGUI
{
    public abstract class ProgressMonitorBase : Ares.ModelInfo.IProgressMonitor
    {
        private System.Timers.Timer timer;
        private bool closed;
        private ProgressDialog dialog;
        private System.Windows.Forms.Form parent;
        private double m_Progress;

        private const int DELAY = 500;

        protected Object syncObject = new object();

        protected ProgressMonitorBase(System.Windows.Forms.Form parent, String text)
        {
            dialog = new ProgressDialog(text);
            dialog.Text = text;
            closed = false;
            this.parent = parent;
            timer = new System.Timers.Timer(DELAY);
            timer.AutoReset = false;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            m_Progress = 0.0;
        }

        private void Start()
        {
            if (parent.InvokeRequired)
            {
                parent.Invoke(new Action(() => { Start(); }));
            }
            else
            {
                timer.Start();
            }
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (syncObject)
            {
                if (closed)
                    return;
            }
            if (parent.InvokeRequired)
            {
                parent.Invoke(new Action(() => { OpenDialog(); }));
            }
            else
            {
                OpenDialog();
            }
        }

        private void OpenDialog()
        {
            lock (syncObject)
            {
                if (closed)
                    return;
            }
            dialog.ShowDialog(parent);
            DialogClosed();
        }

        private void DialogClosed()
        {
            lock (syncObject)
            {
                if (!closed)
                    DoCancel();
            }
        }

        protected abstract void DoCancel();

        public abstract bool Canceled { get; }

        public void Close()
        {
            lock (syncObject)
            {
                closed = true;
                if (timer.Enabled)
                {
                    timer.Stop();
                }
            }
            if (dialog != null && dialog.InvokeRequired)
            {
                dialog.Invoke(new Action(() => { DoClose(); }));
            }
            else
            {
                DoClose();
            }
        }

        private void DoClose()
        {
            if (dialog != null && dialog.Visible)
            {
                dialog.Visible = false;
                dialog.Close();
            }
            timer.Dispose();
        }

        public void IncreaseProgress(double percent)
        {
            if (parent.InvokeRequired)
            {
                parent.Invoke(new Action(() => { IncreaseProgress(percent); }));
            }
            else
            {
                if (m_Progress == 0.0)
                    Start();
                m_Progress += percent;
                dialog.SetProgress((int)m_Progress);
            }
        }

        public void IncreaseProgress(double percent, String text)
        {
            if (parent.InvokeRequired)
            {
                parent.Invoke(new Action(() => { IncreaseProgress(percent, text); }));
            }
            else
            {
                if (m_Progress == 0.0)
                    Start();
                m_Progress += percent;
                dialog.SetProgress((int)m_Progress, text);
            }
        }

        public void SetProgress(int percent, String text)
        {
            if (parent.InvokeRequired)
            {
                parent.Invoke(new Action(() => { SetProgress(percent, text); }));
            }
            else
            {
                if (m_Progress == 0.0)
                    Start();
                m_Progress = percent;
                dialog.SetProgress(percent, text);
            }
        }

        public void SetIndeterminate(String text)
        {
            if (parent.InvokeRequired)
            {
                parent.Invoke(new Action(() => { SetIndeterminate(text); }));
            }
            else
            {
                if (m_Progress == 0.0)
                    Start();
                m_Progress = 0.1;
                dialog.SetIndeterminate(text);
            }
        }
    }

    public class ProgressMonitor : ProgressMonitorBase
    {
        private bool canceled;

        public ProgressMonitor(System.Windows.Forms.Form parent, String text)
            : base(parent, text)
        {
            canceled = false;
        }

        protected override void DoCancel()
        {
            canceled = true;
        }

        public override bool Canceled
        {
            get
            {
                lock (syncObject)
                {
                    return canceled;
                }
            }
        }
    }

}
