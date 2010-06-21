/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
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
//-------------------------------------------------------------------------------------------------
// <copyright file="SharedMemoryException.cs" company="Klaus Bock">      
//  Copyright (c) 2008 Klaus Bock.
//  Hiermit wird unentgeltlich, jeder Person, die eine Kopie der Software und der zugehörigen
//  Dokumentationen (die "Software") erhält, die Erlaubnis erteilt, uneingeschränkt zu benutzen,
//  inklusive und ohne Ausnahme, dem Recht, sie zu verwenden, kopieren, ändern, fusionieren,
//  verlegen, verbreiten, unterlizenzieren und/oder zu verkaufen, und Personen, die diese Software
//  erhalten, diese Rechte zu geben, unter den folgenden Bedingungen:
//
//  Der obige Urheberrechtsvermerk und dieser Erlaubnisvermerk sind in alle Kopien oder Teilkopien
//  der Software beizulegen.
//
//  DIE SOFTWARE WIRD OHNE JEDE AUSDRÜCKLICHE ODER IMPLIZIERTE GARANTIE BEREITGESTELLT,
//  EINSCHIESSLICH DER GARANTIE ZUR BENUTZUNG FÜR DEN VORGESEHENEN ODER EINEM BESTIMMTEN ZWECK
//  SOWIE JEGLICHER RECHTSVERLETZUNG, JEDOCH NICHT DARAUF BESCHRÄNKT. IN KEINEM FALL SIND DIE
//  AUTOREN ODER COPYRIGHTINHABER FÜR JEGLICHEN SCHADEN ODER SONSTIGE ANSPRUCH HAFTBAR ZU MACHEN,
//  OB INFOLGE DER ERFÜLLUNG VON EINEM VERTRAG, EINEM DELIKT ODER ANDERS IM ZUSAMMENHANG MIT DER
//  BENUTZUNG ODER SONSTIGE VERWENDUNG DER SOFTWARE ENTSTANDEN.
// </copyright>
// <author>Klaus Bock</author>
// <email>klaus_b0@hotmail.de</email>
// <date>02.07.2008</date>
// <summary>Enthält die Klasse SharedMemoryException.</summary>
//-------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Ares.Ipc
{
	/// <summary>
	/// Die Ausnahme, die ausgelöst wird, wenn ein Fehler bei SharedMemory-Operationen auftritt.
	/// </summary>
	/// <remarks>n/a</remarks>
	[Serializable]
	public sealed class SharedMemoryException : Exception
	{
		#region private Fields

		[NonSerialized]
		private int _win32ErrorCode;

		#endregion

		#region Properties

		/// <summary>
		/// Der Win32 Fehlercode der dieser Ausnahme zugrunde liegt.
		/// </summary>
		/// <value>Giebt den Wert des privaten Feldes <c>_win32ErrorCode</c> zurück.</value>
		/// <remarks>n/a</remarks>
		public int Win32ErrorCode
		{
			get
			{
				return _win32ErrorCode;
			}
		}

		/// <summary>
		/// Die Nachricht in der Ausnahme.
		/// </summary>
		/// <value>
		/// Wenn ein Win32 Fehlercode vorliegt, wird dieser an die Nachricht angehängt.
		/// </value>
		/// <remarks>n/a</remarks>
		public override string Message
		{
			get
			{
				if (this._win32ErrorCode != 0)
				{
					return base.Message + " (" + this._win32ErrorCode + ")";
				}
				else
				{
					return base.Message;
				}
			}
		}

		#endregion

		/// <summary>
		/// Initialisiert eine neue Instanz der
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemoryException"/>-Klasse.
		/// </summary>
		/// <remarks>n7a</remarks>
		public SharedMemoryException()
			: base()
		{ }

		/// <summary>
		/// Initialisiert eine neue Instanz der
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemoryException"/>-Klasse.
		/// </summary>
		/// <param name="message">Die Nachricht in der Ausnahme</param>
		/// <remarks>n/a</remarks>
		public SharedMemoryException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Initialisiert eine neue Instanz der
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemoryException"/>-Klasse.
		/// </summary>
		/// <param name="message">Die Nachricht in der Ausnahme</param>
		/// <param name="error">Der Win32 Fehlercode.</param>
		/// <remarks>n/a</remarks>
		public SharedMemoryException(string message, int error)
			: base(message)
		{
			this._win32ErrorCode = error;
		}

		/// <summary>
		/// Initialisiert eine neue Instanz der
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemoryException"/>-Klasse.
		/// </summary>
		/// <param name="message">Die Nachricht in der Ausnahme</param>
		/// <param name="innerException">
		/// Eine Ausnahme, die die aktuelle Ausnahme verursacht hat, oder ein <c>null</c>
		/// Verweis, wenn keine innere Ausnahme angegeben ist.
		/// </param>
		/// <remarks>n/a</remarks>
		public SharedMemoryException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initialisiert eine neue Instanz der
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemoryException"/>-Klasse.
		/// </summary>
		/// <param name="info">
		/// Die <see cref="T:System.Runtime.Serialization.SerializationInfo"/>, die die serialisierten
		/// Objektdaten für die ausgelöste Ausnahme enthält.
		/// </param>
		/// <param name="context">
		/// Der <see cref="T:System.Runtime.Serialization.StreamingContext"/>, der die Kontextinformationen
		/// über die Quelldatei oder das Ziel enthält.
		/// </param>
		/// <remarks>n/a</remarks>
		private SharedMemoryException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
