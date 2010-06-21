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
// <copyright file="Enums.cs" company="Klaus Bock">      
//  Copyright © 2008 Klaus Bock.
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
// <summary>Enthält alle Enumeratoren des Namensraums IpcTests.IO.Ipc.</summary>
//-------------------------------------------------------------------------------------------------

using System;

namespace Ares.Ipc
{
	/// <summary>
	/// Listet die verschiedenen Möglichkeiten auf ein Mapped File zu schützen.
	/// </summary>
	/// <remarks>n/a</remarks>
	[Flags]
	public enum MapProtections
	{
		/// <summary>
		/// Kein Zugriffsschutz.
		/// </summary>
		/// <remarks>
		/// Die Verwendung dieser Auswahl ist nicht Empfehlenswert.
		/// </remarks>
		None = 0x00000000,

		#region protection

		/// <summary>
		/// Setzt nur-lesen Zugriff.
		/// </summary>
		/// <remarks>
		/// Ein Versuch in den File View zu schreiben, endet
		/// in einer Zugriffs Verletzung. Die Datei muss mit dem Recht
		/// GENERIC_READ erstellt worden sein.
		/// </remarks>
		PageReadOnly = 0x00000002,

		/// <summary>
		/// Setzt lese und schreib Zugriff.
		/// </summary>
		/// <remarks>
		/// Die Datei muss mit dem Rechten GENERIC_READ und GENERIC_WRITE
		/// erstellt worden sein.
		/// </remarks>
		PageReadWrite = 0x00000004,

		/// <summary>
		/// Setzt die Berechtigung Schreiben-beim-Kopieren.
		/// </summary>
		/// <remarks>Die Datei muss mit dem Recht GENERIC_READ erstellt worden sein.
		/// </remarks>
		PageWriteCopy = 0x00000008,

		#endregion

		#region attributes

		/// <summary>
		/// Kennzeichnet die angegebene Datei als ein ausführbares Image.
		/// </summary>
		/// <remarks>
		/// Weil durch die Mapping-Informationen und den Dateischutz des Image
		/// keine anderen Attribute zulässig sind ausser <c>SecImage</c>.
		/// </remarks>
		SecImage = 0x01000000,

		/// <summary>
		/// Reserviert alle Seiten ohne physikalischen Speicher zu belegen.
		/// </summary>
		/// <remarks>
		/// Dieser Flag ist nur gültig, wenn der Oarameter <c>hFile</c>
		/// INVALID_HANDLE_VALUE zurückgiebt.
		/// </remarks>
		SecReserve = 0x04000000,

		/// <summary>
		/// Belegt physikalischen Speicher im Arbeitsspeicher oder
		/// der Auslagerungsdatei für alle Seiten.
		/// </summary>
		/// <remarks>Das ist die Stangardeinstellung. </remarks>
		SecCommit = 0x08000000,

		/// <summary>
		/// Setzt alle Seiten auf no-cachable.
		/// </summary>
		/// <remarks>
		/// Anwendungen sollten diese Flag nicht benutzen, ausgenommen sie
		/// explizit für ein Gerät benötigt.
		/// </remarks>
		SecNoCache = 0x10000000,

		#endregion
	}

	/// <summary>
	/// Listet die Verschiedenen Typen des Zugriffs auf ein File Mapping Objekt,
	/// welche den Schutz der Seiten garantieren.
	/// </summary>
	/// <remarks>n/a</remarks>
	public enum MapAccess
	{
		/// <summary>
		/// Keine Zugriffsschutz.
		/// </summary>
		/// <remarks>
		/// Die Verwendung dieser Auswahl ist nicht Empfehlenswert.
		/// </remarks>
		None = 0x0000,

		/// <summary>
		/// Eine Kopieren-beim-Schreiben Ansicht auf die abgebildetet Datei.
		/// </summary>
		/// <remarks>
		/// Das Objekt muss mit der Option PAGE_WRITECOPY erstellt wordem sein.
		/// </remarks>
		FileMapCopy = 0x0001,

		/// <summary>
		/// Eine Lese- und Schreibansicht auf die abgebildetet Datei.
		/// </summary>
		/// <remarks>
		/// Das Objekt muss mit der Option PAGE_READWRITE erstellt worden sein.
		/// </remarks>
		FileMapWrite = 0x0002,

		/// <summary>
		/// Eine nur Leseansicht auf die abgebildetet Datei.
		/// </summary>
		/// <remarks>
		/// Das Objekt muss mit der Option PAGE_READWRITE oder PAGE_READONLY erstellt worden sein.
		/// </remarks>
		FileMapRead = 0x0004,

		/// <summary>
		/// Entspricht FILE_MAP_WRITE|FILE_MAP_READ.
		/// </summary>
		/// <remarks>
		/// Das Objekt muss mit der Option PAGE_READWRITE erzeugt worden sein.
		/// </remarks>
		FileMapAllAccess = 0x001f,
	}

	/// <summary>
	/// Auswahl um festzulegen ob ein SharedMemory Segment erzeugt wird,
	/// oder daraus gelesen werden soll.
	/// </summary>
	/// <remarks>n/a</remarks>
    [Flags]
	public enum SharedMemoryProcedure
	{
		/// <summary>
		/// Ein SharedMemory Segment soll erzeugt werden.
		/// </summary>
		/// <remarks>n/a</remarks>
		Create = 1,

		/// <summary>
		/// Auf ein bestehendes SharedMemory Segment wird lesend zugegriffen.
		/// </summary>
		/// <remarks>n/a</remarks>
		Attach = 2,

        /// <summary>
        /// Neu erzeugen oder lesend zugreifen.
        /// </summary>
        CreateOrAttach = 3
	}
}
