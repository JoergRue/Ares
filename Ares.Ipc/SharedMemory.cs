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
// <copyright file="SharedMemory.cs" company="Klaus Bock">
//  Basiert in Teilen auf dem shared memory wrapper von Richard Blewett.
//  Download: http://www.dotnetconsult.co.uk/weblog/PermaLink.aspx/32c3abfb-bac9-4e19-bbc5-39ca338d906d
//  Blog: http://www.dotnetconsult.co.uk/weblog2
//
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
// <summary>Enthält die Klasse SharedMemory.</summary>
//-------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Threading;
using System.Globalization;

namespace Ares.Ipc
{
	/// <summary>
	/// Stellt gemeinsame Resourcen für die Interprozesskommunikation (InterProcessCommunication)
	/// in Form von gemeinsam genutzten Speicherbereichen (Shared Memory) bereit.
	/// </summary>
	/// <remarks>
	/// Diese Klasse kann nur zur IPC zwischen Prozessen auf der selben Maschine benutzt werden.
	/// </remarks>
	[SecurityPermission(SecurityAction.LinkDemand)]
	public sealed class SharedMemory : IDisposable
	{
		#region Constants

		/// <summary>
		/// Halt den Wert für ein ungültiges Handle.
		/// </summary>
		/// <remarks>n/a</remarks>
		internal const int InvalidHandleValue = -1;

		/// <summary>
		/// Halt den Fehlercode für ein ungültiges Handle.
		/// </summary>
		/// <remarks>n/a</remarks>
		internal const int ErrorInvalidHandle = 6;

		#endregion

		#region Internal Fields

		/// <summary>
		/// Hält das Handle eines FileMapping Objekts.
		/// </summary>
		/// <remarks>n/a</remarks>
		private IntPtr nativeHandle = IntPtr.Zero;

		/// <summary>
		/// Hält den Zeiger auf ein FileMapping Objekt.
		/// </summary>
		/// <remarks>n/a</remarks>
		private IntPtr nativePointer = IntPtr.Zero;

		/// <summary>
		/// Hält die Grösse des zu erstellenden SharedMemory Segment.
		/// </summary>
		/// <remarks>n/a</remarks>
		private int currentSize;

#if !MONO
		/// <summary>
		/// Gibt an ob die Dispose Methode ausgeführt wurde. wurde.
		/// </summary>
		/// <remarks>n/a</remarks>
		private bool disposed;
#endif

		#endregion

		#region private Fields

		/// <summary>
		/// Hält den <see cref="T:System.Threading.Mutex"/> der
		/// zum sperren des SharedMemory Segment verwendet wird.
		/// </summary>
		/// <remarks>n/a</remarks>
		private volatile Mutex _mutex;

		/// <summary>
		/// Hält den Namen des benannten <see cref="T:System.Threading.Mutex"/>
		/// zum sperren des SharedMemory Segment verwendet wird.
		/// </summary>
		/// <remarks>
		/// Kann in Klassen, die SharedMemory verwenden, verwendet werden um auf
		/// den bennanten SystemMutex der SharedMemory-Klasse zuzugreifen.
		/// </remarks>
		private string _mutexName;

		/// <summary>
		/// Hält den Namen des SharedMemory Segment.
		/// </summary>
		/// <remarks>n/a</remarks>
		private string _segmentName;

		#endregion

		#region Properties

		/// <summary>
		/// Gibt den Namen des SharedMemory Segment zurück.
		/// </summary>
		/// <value>Liefert den Wert des privaten Feldes <c>_segmentName</c>.</value>
		/// <remarks>n/a</remarks>
		public string SegmentName
		{
			get { return this._segmentName; }
		}

		/// <summary>
		/// Gibt den Namen des benannten SystemMutex zurück.
		/// </summary>
		/// <value>Liefert den Wert des privaten Feldes <c>_mutexName</c>.</value>
		/// <remarks>
		/// Kann in Klassen, die SharedMemory verwenden, verwendet werden um auf
		/// den bennanten SystemMutex der SharedMemory-Klasse zuzugreifen.
		/// </remarks>
		public string MutexName
		{
			get { return this._mutexName; }
		}

		/// <summary>
		/// Bietet Zugriff auf das Prozessübergreifende
		/// <see cref="T:System.Thrading.WaitHandle"/>-Objekt.
		/// </summary>
		/// <value>Liefert den Wert des privaten Feldes <c>_mutex</c>.</value>
		/// <remarks>n/a</remarks>
		public WaitHandle WaitHandle
		{
			get { return this._mutex; }
		}


		#endregion

		#region Constructors

		/// <summary>
		/// Initialisiert einen neue Instanz der
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemory"/>-Klasse.
		/// </summary>
		/// <param name="name">Der Name des SharedMemory Segment.</param>
		/// <remarks>
		/// Dieser Konstruktor kann benutzt werden, wenn nur lesend
		/// auf die Daten im Shared Memory zugegriffen wird.
		/// </remarks>
		public SharedMemory(string name)
			: this(name, SharedMemoryProcedure.Attach, 0)
		{ }

		/// <summary>
		/// Initialisiert einen neue Instanz der
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemory"/>-Klasse.
		/// </summary>
		/// <param name="name">Der Name des SharedMemory Segment.</param>
		/// <param name="obj">Das Objekt welches im SharedMemory Segment gespeichert werden soll.</param>
		/// <remarks>
		/// Dieser Konstruktor kann benutzt werden um beim instanzieren der Klasse gleich
		/// Daten in das ShredMemory Segment zu schreiben. Die Grösse des SharedMemory Segment
		/// wird automatisch ermittelt.
		/// </remarks>
		public SharedMemory(string name, object obj)
			: this(name, SharedMemoryProcedure.Create, SharedMemory.GetOptMemorySize(obj))
		{
			// das übergebene Objekt in das SharedMemory Segment schreiben
            Lock();
            try
            {
                this.AddObject(obj, false);
            }
            finally
            {
                Unlock();
            }
		}


		/// <summary>
		/// Initialisiert einen neue Instanz der
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemory"/>-Klasse.
		/// </summary>
		/// <param name="name">Der Name des SharedMemory Segment.</param>
		/// <param name="creationProcedure">
		/// Ein SharedMemoryCreationFlag das angibt ob das SharedMemory Segment
		/// erzeugt werden soll oder lesend darauf zugegriffen wird.
		/// </param>
		/// <param name="size">
		/// Die Grösse des zu erstellenden SharedMemory Segment. Wenn nur lesend auf das SharedMemory
		/// Segment zugegriffen wird, sollte hier 0 (Zero) angegeben werden.
		/// </param>
		/// <exception cref="T:IpcTests.IO.Ipc.SharedMemoryException">
		/// Kann unter diversen Umständen ausgelöst werden. Es wird eine dementsprechende
		/// Meldung mit der Ausnahme ausgegeben.
		/// </exception>
		/// <remarks>
		/// Dieser Konstruktor muss benutzt werden, wenn Daten in den
		/// Shared Memory geschrieben werden.
		/// </remarks>
		public SharedMemory(string name, SharedMemoryProcedure creationProcedure, int size)
		{
#if !MONO
			if (string.IsNullOrEmpty(name))
			{
				throw new SharedMemoryException();
			}

			if (size <= 0 && (creationProcedure & SharedMemoryProcedure.Create) != 0)
			{
				throw new SharedMemoryException();
			}

			// einen benannten Mutex mit dem Namen
			// des Segments erzeugen
			this._mutexName = name + "Mutex";
			try
			{
				this._mutex = NamedMutexHelper.GetNamedMutex(this._mutexName, (creationProcedure & SharedMemoryProcedure.Create) != 0,
                    (creationProcedure & SharedMemoryProcedure.Attach) != 0);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new SharedMemoryException(ex.Message);
			}

			// das SharedMemorySegment erzeugen
			if ((creationProcedure & SharedMemoryProcedure.Create) != 0)
			{
				this.nativeHandle = NativeMethods.CreateFileMapping((IntPtr)InvalidHandleValue,
					IntPtr.Zero, (int)MapProtections.PageReadWrite,
					0, size, name);
			}
			// das SharedMemory Segment anhängen (lesend öffnen)
			else
			{
				this.nativeHandle = NativeMethods.OpenFileMapping((int)MapAccess.FileMapAllAccess, true, name);
			}

			if (this.nativeHandle == IntPtr.Zero)
			{
				int i = Marshal.GetLastWin32Error();
				// das Segment besteht bereits
				if (i == ErrorInvalidHandle)
				{
					throw new SharedMemoryException();
				}
				// aus das Segment kann nicht zugegriffen werden
				else
				{
					throw new SharedMemoryException();
				}
			}

			// den Zeiger auf das SharedMemory Segment holen
			this.nativePointer = NativeMethods.MapViewOfFile(nativeHandle,
				(int)MapAccess.FileMapAllAccess, 0, 0, IntPtr.Zero);

			// kein Zeiger zurück gegeben
			if (this.nativePointer == IntPtr.Zero)
			{
				int i = Marshal.GetLastWin32Error();
				NativeMethods.CloseHandle(nativeHandle);
				this.nativeHandle = IntPtr.Zero;
				throw new SharedMemoryException();
			}

			this.currentSize = size;
			this._segmentName = name;
#else
            throw new SharedMemoryException("Not available on Mono");
#endif
		}

		#endregion

		#region Methods

		#region public Methods

		/// <summary>
		/// Speichert serialisierbare Objekte in einem SharedMemory Segment.
		/// </summary>
		/// <param name="obj">Das Objekt welches im SharedMemory Segment gespeichert werden soll.</param>
        /// <param name="allowResize">Ob das Segment neu angelegt werden darf.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// Wird ausgegeben wenn der Parameter <paramref name="obj"/> = <c>null</c> ist.
		/// </exception>
		/// <exception cref="T:IpcTests.IO.Ipc.SharedMemoryException">
		/// Kann unter diversen Umständen ausgelöst werden. Es wird eine dementsprechende
		/// Meldung mit der Ausnahme ausgegeben.
		/// </exception>
		/// <remarks>n/a</remarks>
		public void AddObject(object obj, bool allowResize)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			// Sicherstellen, dass das Objekt serialisierbar ist
			if (!obj.GetType().IsSerializable)
			{
				throw new SharedMemoryException();
			}

			// die Größe der serialisierten Objekts bestimmen
			long marshalledSize = GetMinMemorySize(obj);

            // überprüfen ob die angegebene Größe ausreichend ist
			if (!this.checkSize((int)marshalledSize))
			{
                if (allowResize)
                {
                    resize(GetOptMemorySize(obj));
                }
                else
                {
                    throw new SharedMemoryException();
                }
			}

			// Einen MemoryStream zum aufnehmen der Daten erzeugen
			MemoryStream ms = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			BinaryWriter bw = new BinaryWriter(ms);

            // die Größe der serialisierten Objekts an den
            // an den Anfang des MemorySteam schreiben
			bw.Write(marshalledSize);

			// das Objekt in den MemorySteam schreiben
			formatter.Serialize(ms, obj);

			// Stream auf den Anfang setzen
			ms.Seek(0, SeekOrigin.Begin);

            // den Inhalt des Stream in das Shared Memory Segment kopieren
			copyStreamToSharedMemory(ms);
		}

		/// <summary>
		/// Gibt das im SharedMemory Segment gespeicherte Objekt zurück.
		/// </summary>
		/// <returns>Das im SharedMemory Segment gespeicherte Objekt.</returns>
		/// <remarks>n/a</remarks>
		public object GetObject()
		{
			MemoryStream ms = new MemoryStream();

			copySharedMemoryToStream(ms);

			BinaryFormatter bf = new BinaryFormatter();

			return bf.Deserialize(ms);
		}

		/// <summary>
		/// Bietet eine prozessübergreifende Sperre des benannten Mutex.
		/// </summary>
		/// <remarks>n/a</remarks>
		public void Lock()
		{
			this._mutex.WaitOne();
		}

		/// <summary>
		/// Gibt die Prozessübergreifende Sperre des benannten Mutex frei.
		/// </summary>
		/// <remarks>n/a</remarks>
		public void Unlock()
		{
			this._mutex.ReleaseMutex();
		}

		/// <summary>
		/// Ermittelt die Größe die ein Objekt serialisert im Speicher
		/// mindestens benötigt.
		/// </summary>
		/// <param name="obj">Das Objekt, dessen Grösse ermittelt werden soll.</param>
		/// <returns>Die Grösse des Speichers in byte.</returns>
		/// <exception cref="T:System.ArgumentNullException"></exception>
		/// <remarks>Die ermittelte Grösse solte um 10% erhöht werden.</remarks>
		public static int GetMinMemorySize(object obj)
		{
			// Parameter auf NULL prüfen
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}

			return getMarshalledSize(obj);
		}

		/// <summary>
		///  Ermittelt die optimale Größe die ein Objekt serialisert im Speicher benötigt.
		/// </summary>
		/// <param name="obj">Das Objekt, dessen Grösse ermittelt werden soll.</param>
		/// <returns></returns>
		/// <remarks>n/a</remarks>
		public static int GetOptMemorySize(object obj)
		{
			// Parameter auf NULL prüfen
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			int i = getMarshalledSize(obj);
			int ret = ((i / 8) + 1) * 8;

			return ret;
		}

		#endregion

		#region private Methods

        /// <summary>
        /// Aendert die Groesse des Segments. Dafuer wird es geloescht und neu erzeugt.
        /// </summary>
        /// <param name="size">Neue Groesse</param>
        private void resize(int size)
        {
#if !MONO
            if (nativePointer != IntPtr.Zero)
            {
                NativeMethods.UnmapViewOfFile(nativePointer);
            }

            if (nativeHandle != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(nativeHandle);
            }

            nativeHandle = NativeMethods.CreateFileMapping((IntPtr)InvalidHandleValue,
                IntPtr.Zero, (int)MapProtections.PageReadWrite,
                0, size, _segmentName);

            if (this.nativeHandle == IntPtr.Zero)
            {
                int i = Marshal.GetLastWin32Error();
                // das Segment besteht bereits
                if (i == ErrorInvalidHandle)
                {
                    throw new SharedMemoryException();
                }
                // aus das Segment kann nicht zugegriffen werden
                else
                {
                    throw new SharedMemoryException();
                }
            }

            // den Zeiger auf das SharedMemory Segment holen
            this.nativePointer = NativeMethods.MapViewOfFile(nativeHandle,
                (int)MapAccess.FileMapAllAccess, 0, 0, IntPtr.Zero);

            // kein Zeiger zurück gegeben
            if (this.nativePointer == IntPtr.Zero)
            {
                int i = Marshal.GetLastWin32Error();
                NativeMethods.CloseHandle(nativeHandle);
                this.nativeHandle = IntPtr.Zero;
                throw new SharedMemoryException();
            }
#endif

            this.currentSize = size;
        }

		/// <summary>
		/// Kopiert einen Stream in das Shared Memory Segment.
		/// </summary>
		/// <param name="stream">
		/// Ein <seealso cref="T:System.IO.Stream"/> dessen Daten in das SharedMemory Segment
		/// kopiert werden sollen.
		/// </param>
		/// <remarks>n/a</remarks>
		private void copyStreamToSharedMemory(Stream stream)
		{
			// die Stream-Daten in ein byte Array lesen
			BinaryReader reader = new BinaryReader(stream);
			byte[] data = reader.ReadBytes((int)stream.Length);

			// kopiere das byte Array in den SharedMemory
			Marshal.Copy(data, 0, this.nativePointer, (int)stream.Length);
		}

		/// <summary>
		/// Kopiert Daten aus dem Shared Memory in einen Stream.
		/// </summary>
		/// <param name="stream">Ein <seealso cref="System.IO.Stream"/>zum empfangen der Daten.</param>
		/// <remarks>n/a</remarks>
		private void copySharedMemoryToStream(Stream stream)
		{
			// die Länge der Daten im SharedMemory Segment.
			long objLength = (long)Marshal.ReadIntPtr(this.nativePointer);

			// Zeiger auf das SharedMemory Segment.
			IntPtr source = (IntPtr)((long)this.nativePointer + sizeof(long));

			// ein byte Array mit der Länge des Speicherbereichs
			// erzeugen um die serialisierten Daten aufzunehmen
			byte[] data = new byte[objLength];

			// die SharedMemory Daten in das byte Array kopieren
			Marshal.Copy(source, data, 0, (int)objLength);

			// neue BinaryWriter erzeugen
			BinaryWriter writer = new BinaryWriter(stream);

			// das byte Array in den Stream schreiben
			writer.Write(data);

			// den Stream auf seinen Startpunkt setzen
			stream.Seek(0, SeekOrigin.Begin);
		}

		/// <summary>
		/// Prüft ob die aktuell angegebene Grösse grösser ist als die
		/// im Speicher benötigte Grösse des Objekts.
		/// </summary>
		/// <param name="marshalledSize">Die gemarshalte Größe des Objekts.</param>
		/// <returns><c>true</c> wenn aktuelle Grösse ausreicht, sonst <c>false</c>.</returns>
		/// <remarks>n/a</remarks>
		private bool checkSize(int marshalledSize)
		{
			if (this.currentSize > marshalledSize)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Ermittelt die Größe die ein Objekt serialisert im Speicher
		/// mindestens benötigt.
		/// </summary>
		/// <param name="obj">Das Objekt, dessen Grösse ermittelt werden soll.</param>
		/// <returns>Die Grösse des Speichers in byte.</returns>
		/// <exception cref="T:System.ArgumentNullException"></exception>
		/// <remarks>n/a</remarks>
		private static int getMarshalledSize(object obj)
		{
			// neuen MemoryStream zum serialisieren des Objekts verwenden.
			MemoryStream ms = new MemoryStream();
			
            // BinaryFormatter zum serialisieren des Objekt erzeugen
			BinaryFormatter formatter = new BinaryFormatter();
			
            // Objekt serialisieren
			formatter.Serialize(ms, obj);
			
            // die benötigte Grösse des Objekts
            // plus einen Offset des Typs long
			int value = (int)ms.Length + Marshal.SizeOf(typeof(long));
			ms.Close();

			return value;
		}

		#endregion

		#endregion

		#region Dispose and Finalize

		/// <summary>
		/// Finalizer zum freigeben des SharedMemory Segment NativeHandle.
		/// </summary>
		~SharedMemory()
		{
			Dispose(false);
		}

		/// <summary>
		/// Giebt alle vom <see cref="T:System.Object"/>
		/// verwendetet Resourcen frei.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gibt die von
		/// <see cref="T:IpcTests.IO.Ipc.SharedMemory"/>
		/// verwendetet nicht verwaltetet Resourcen
		/// und optional auch die verwalteten Resourcen frei.
		/// </summary>
		/// <param name="disposing">
		/// Gibt an ob die verwalteten Resourcen freigegeben werden sollen.
		/// </param>
		/// <remarks>n/a</remarks>
		private void Dispose(bool disposing)
		{
#if !MONO
			if (!this.disposed)
			{
				// die managed Resourcen nur freigeben
				// wenn IDisposable.Dispose() aufgerufen wird
				if (disposing)
				{
					_mutex.Close();
				}

				// immer die unmanaged Rsources bereinigen
				if (nativePointer != IntPtr.Zero)
				{
					NativeMethods.UnmapViewOfFile(nativePointer);
				}

				if (nativeHandle != IntPtr.Zero)
				{
					NativeMethods.CloseHandle(nativeHandle);
				}
			}
            this.disposed = true;
#endif
        }

		#endregion
	}
}
