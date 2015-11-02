/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
// <copyright file="NativeMethods.cs" company="Klaus Bock">      
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
// <summary>Enthält die Klasse NativeMethods.</summary>
//-------------------------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace Ares.Ipc
{
#if !MONO
    /// <summary>
    /// Stellt die nativen Methoden der win32 API für das Filemapping und SharedMemory
    /// als statische managed Methoden bereit.
    /// </summary>
    /// <remarks>
    /// PInvoke Dokumentation bei <a href="http://pinvoke.net/index.aspx">pinvoke.net</a>.
    /// Windows API Referenz in der <a href="http://msdn2.microsoft.com/en-us/library/aa383749.aspx">MSDN</a>.
    /// </remarks>
    internal static class NativeMethods
    {
        /// <summary>
        /// Erzeugt oder öffnet ein benanntes oder unbenanntes Datei Zuordnungs Objekt
        /// (File Mapping) für eine angegebene Datei.
        /// </summary>
        /// <param name="hFile">
        /// Das Datei Handle auf die Datei von dem ein File Mapping Objekt erstellt werden soll.
        /// </param>
        /// <param name="lpAttributes">
        /// Ein Zeiger auf eine SECURITY_ATTRIBUTES Struktur der bestimmt ob das zurückgegebene Handle
        /// an untergeordnete Prozesse vererbt werden kann. Der Parameter kann <c>null</c> sein.
        /// </param>
        /// <param name="flProtect">
        /// Der Schutz für die Datei Abbildung (File View), wenn die Datei im Speicher abgebildet (mapped) wurde.
        /// </param>
        /// <param name="dwMaximumSizeLow">
        /// Der niederwertige <seealso cref="T:System.Int32"/>-Wert der maximalen Grösse des File Mapping Objekts.
        /// </param>
        /// <param name="dwMaximumSizeHigh">
        /// Der höherwertige <seealso cref="T:System.Int32"/>-Wert der maximalen Grösse des File Mapping Objekts.
        /// Ist dieser und der Parameter <c>dwMaximumSizeLow</c> 0 (Zero), entspricht die maximale Grösse des
        /// File Mapping Objekts der aktuellen Grösse der Datei die mit <c>hFile</c> angegeben wurde.
        /// </param>
        /// <param name="lpName">Der Name des File Mapping Objekt.</param>
        /// <returns>
        /// Wenn die Methode erfolgreich war, wird ein Wert zurückgegeben der ein Datei Handle darstellt.
        /// Wenn ein Fehler auftritt, wird <c>NULL</c> zurückgegeben. Mit GetLastError
        /// können erweiterte Fehlerinformationen abgerufen werden.
        /// </returns>
        /// <remarks>
        /// Nachdem ein File Mapping Objekt erzeugt wurde, sollte die Grösse der Datei die Grösse des
        /// File Mapping Objekts nicht überschreiten, das sonst nicht der gesammte inhalt der Datei
        /// zur Verfügung steht.
        /// </remarks>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, int flProtect,
            int dwMaximumSizeLow, int dwMaximumSizeHigh, string lpName);

        /// <summary>
        /// Bildet die Ansicht eines FileMapping Objekt im Adressraum des aufrufenden Prozesses ab.
        /// </summary>
        /// <param name="hFileMappingObject">
        /// Das Handle zu einem Filemapping Objekt. Wird von den Methoden
        /// <seealso cref="M:IpcTests.IO.NativeMethods.CreateFileMapping(System.IntPtr,System.IntPtr,System.Int32,System.Int32,System.Int32,System.String)"/>
        /// und <seealso cref="M:IpcTests.IO.NativeMethods.OpenFileMapping(System.Int32,System.Boolean,System.String)"/>
        /// zurückgegeben.
        /// </param>
        /// <param name="dwDesiredAccess">
        /// Der Typ des Zugriffs auf das FileMapping Objekt. Wird normalerweise aus
        /// <seealso cref="T:IpcTests.IO.MapAccess"/> gewählt und von der aufrufenden Methode durchgereicht.
        /// </param>
        /// <param name="dwFileOffsetHigh">
        /// Der höherwertige <seealso cref="T:System.Int32"/>-Wert
        /// des Datei-Offset an dem die Ansicht (View) beginnt.
        /// </param>
        /// <param name="dwFileOffsetLow">
        /// Der niederwertige <seealso cref="T:System.Int32"/>-Wert des Datei-Offset an dem die
        /// Ansicht (View) beginnt. Die Kombination des unteren und oberen Offset muss einen Wert
        /// inneralb des File Mapping angeben.
        /// </param>
        /// <param name="dwNumBytesToMap">
        /// Die Anzahl der bytes eines FileMapping, die zur Ansicht gemapped werden sollen. Die Anzahl muss
        /// innerhalb der Grösse sein, die von der Methode
        /// <seealso cref="M:IpcTests.IO.NativeMethods.CreateFileMapping(System.IntPtr,System.IntPtr,System.Int32,System.Int32,System.Int32,System.String)"/>
        /// festgelegt wurde. Ist dieser Parameter 0 (zero) wird das mapping vom angegebenen
        /// Offset bis zum Dateiende ausgedehnt.
        /// </param>
        /// <returns>
        /// Wenn die Methode erfolgreich war, wird als Wert die Startadresse der abgebildetetn
        /// Ansicht (mapped View) zurückgegeben. Wenn ein Fehler Auftritt wird <c>NULL</c>
        /// zurückgegeben. Mit GetLastError können erweiterte Fehlerinformationen abgerufen werden.
        /// </returns>
        /// <remarks>
        /// Das mappen einer Datei macht den angegebenen Bereich einer Datei im Adressraum des
        /// aufrufenden Prozesses sichtbar. Dateie die grösser sind als der Adressraum, können
        /// nur kleine Bereiche der Datei auf einmal dargestellt werden. Wenn die erste Ansicht
        /// nicht mehr benötigt wird, kann dieser entladen und eine neue Ansicht erstellt werden.
        /// </remarks>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, int dwDesiredAccess,
            int dwFileOffsetHigh, int dwFileOffsetLow, IntPtr dwNumBytesToMap);

        /// <summary>
        /// Öffnet ein benanntes FileMapping Objekt.
        /// </summary>
        /// <param name="dwDesiredAccess">
        /// Der Zugriff auf das FileMapping Objekt. Der Zugriff wird gegen jeden Sicherheits Deskriptor
        /// des FileMapping Objekt geprüft.
        /// </param>
        /// <param name="bInheritHandle">
        /// Ist dieser Parameter <c>true</c> kann das Handle an einen Prozess, der von der Methode
        /// <c>CreateProcess</c> erzeugt wurde, vererbt werden. Anderenfalls kann das Handle nicht
        /// vererbt werden.
        /// </param>
        /// <param name="lpName">Der Name des FileMapping Objekt das geöffnet werden soll.</param>
        /// <returns>
        /// Wenn die Methode erfolgreich war, wird als Wert ein geöffnetes Datei Handle zum angegebenen
        /// FileMapping Objekt zurückgegeben. Wenn ein Fehler Auftritt wird <c>NULL</c> zurückgegeben.
        /// Mit GetLastError können erweiterte Fehlerinformationen abgerufen werden.
        /// </returns>
        /// <remarks>
        /// Das von der Methode
        /// <seealso cref="M:IpcTests.IO.NativeMethods.CreateFileMapping(System.IntPtr,System.IntPtr,System.Int32,System.Int32,System.Int32,System.String)"/>
        /// zurückgegebene Handle kann von jeder Methode verwendet werden, die das Handle eines
        /// FileMapping Objekt benötigt.
        /// </remarks>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenFileMapping(int dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, string lpName);

        /// <summary>
        /// Entlädt die Ansicht eines FileMapping Objekt aus dem Adressraum des aufrufenden Prozesses.
        /// </summary>
        /// <param name="lpBaseAddress">
        /// Ein Zeiger auf die Basis Adresse der Ansicht eines FileMapping Objekt. Der Wert muss identisch
        /// mit dem Rückgabewert eines vorherigen Aufrufs der Methode
        /// <seealso cref="M:IpcTests.IO.NativeMethods.MapViewOfFile(System.IntPtr,System.Int32,System.Int32,System.Int32,System.IntPtr)"/>
        /// sein.
        /// </param>
        /// <returns>
        /// Wenn die Methode erfolgreich war, wird <c>true</c> zurückgegeben und alle "dirty pages"
        /// innerhalb des angegebenen Bereichs werden "lazily" auf die Fesplatte geschrieben. Wenn
        /// ein Fehler auftritt wird <c>false</c> zurückgegeben. Mit GetLastError können erweiterte
        /// Fehlerinformationen abgerufen werden.
        /// </returns>
        /// <remarks>
        /// Auch wenn eine Anwendung das Datei Handle schliesst, welches zur erzeugen des FileMapping Objekt
        /// verwendet wurde, hält das System die dazugehörige Datei geöffnet bis die letzte Ansicht der
        /// Datei entladen (unmapped) wurde. Dateien für welche die letzte Ansicht nicht entladen (unmapped)
        /// wurde, werden ohne Zugriffsbeschränkungen offen gehalten.
        /// </remarks>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        /// <summary>
        /// Schliesst ein offenes Objekt Handle.
        /// </summary>
        /// <param name="handle">Ein gültiges Handle zu einem offenem Objekt.</param>
        /// <returns>
        /// Giebt <c>true</c> zurück wenn der DateiHandle erfolgreich geschlossen wurde,
        /// anderenfalls <c>false</c>. Mit GetLastError können erweiterte
        /// Fehlerinformationen abgerufen werden.
        /// </returns>
        /// <remarks>
        /// <c>CloseHandle</c> annuliert das angegebene Objekt Handle, verringert den
        /// Objekte Handle Zähler und führt Laufzeitüberprüfungen der Objekte durch.
        /// Nachdem das letzte Handle eine Objekts geschlossen wurde, wird es aus dem
        /// Speicher entfernt.
        /// </remarks>
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);
    }
#endif
}
