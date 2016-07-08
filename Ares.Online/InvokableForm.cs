/*
    Copyright (c) 2016  [Joerg Ruedenauer]
  
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

namespace Ares.CommonGUI
{

	/// <summary>
	/// Workaround for Mono where InvokeRequired sometimes doesn't work correctly.
	/// </summary>
	public class InvokableForm : System.Windows.Forms.Form
	{
#if MONO
		private readonly int m_OwnerThreadId;
#endif

		public InvokableForm ()
		{
#if MONO
			m_OwnerThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
		}

		public bool IsInvokeRequiredByThreadId {
			get {
#if !MONO
				return InvokeRequired;
#else
				return System.Threading.Thread.CurrentThread.ManagedThreadId != m_OwnerThreadId;
#endif
			}
		}
	}

	public static class FormExtensions
	{
		public static bool IsInvokeRequired (this System.Windows.Forms.Form form)
		{
#if MONO
			return ((InvokableForm)form).IsInvokeRequiredByThreadId;
#else
			return form.InvokeRequired;
#endif
		}
	}
}

