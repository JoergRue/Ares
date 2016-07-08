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
using System.Windows.Forms;


namespace Ares.Editor
{
    public class TaskHelpers
    {
        public static void HandleTaskException(IWin32Window parent, Exception ex, String text)
        {
            if (ex is AggregateException)
            {
                ex = (ex as AggregateException).Flatten().InnerException;
            }
            if (!(ex is System.Threading.Tasks.TaskCanceledException) && !(ex is OperationCanceledException))
            {
                System.Windows.Forms.MessageBox.Show(parent, String.Format(text, ex.Message), StringResources.Ares,
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }

    public class TaskProgressMonitor : Ares.CommonGUI.ProgressMonitorBase
    {
        private System.Threading.CancellationTokenSource m_TokenSource;

        public TaskProgressMonitor(Form parent, String text, System.Threading.CancellationTokenSource tokenSource)
            : base(parent, text)
        {
            m_TokenSource = tokenSource;
        }

        protected override void DoCancel()
        {
            m_TokenSource.Cancel();
        }

        public override bool Canceled
        {
            get
            {
                return m_TokenSource.IsCancellationRequested;
            }
        }

    }
}