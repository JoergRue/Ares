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
// <copyright file="NamedMutexHelper.cs" company="Klaus Bock">      
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
// <summary>Enthält die Klasse NamedMutexHelper.</summary>
//-------------------------------------------------------------------------------------------------

using System;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Threading;

namespace Ares.Ipc
{
#if !MONO
    /// <summary>
    /// Bietet verschiedene Methoden zu Zugriff auf benannte System Mutexe.
    /// Es kann auf vorhandene Mutexe zugegriffen, oder ein neuer Mutex erzeugt werden.
    /// </summary>
    /// <exception cref="T:System.ArgumentNullException">
    /// Wird ausgelöst wenn einer der Methoden eine leere Zeichenfolge
    /// oder <c>null</c> zugewiesen wird.
    /// </exception>
    /// <exception cref="T:System.UnauthorizedAccessException">
    /// Wird ausgelöst wenn der Benutzer nicht die nötigen Rechte besitzt
    /// auf den Mutex zu zugreifen und diese dem Benutzer auch nicht
    /// zugewiesen werden können.
    /// </exception>
    /// <exception cref="T:IpcTests.IO.Ipc.SharedMemoryException">
    /// Wird ausgelöst wenn der Mutex nicht exisitiert und beim erzeugen
    /// ein Problem auftritt.
    /// </exception>
    /// <remarks>n/a</remarks>
    [SecurityPermission(SecurityAction.LinkDemand)]
    public static class NamedMutexHelper
    {
        #region public Methods

        /// <summary>
        /// Gewährt Zugriff auf einen benannten System Mutex.
        /// </summary>
        /// <param name="mutexName">
        /// Der Namen des benannten System Mutex.
        /// </param>
        /// <returns>
        /// Einen benannten System Mutex oder <c>null</c> wenn der Mutex nicht exisitiert
        /// oder ein anderes Problem aufgetreten ist.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// wird ausgelöst wenn der Parameter <paramref name="mutexName"/>
        /// eine leere Zeichenfolge enthält oder <c>null</c> zugewiesen wurde.
        /// </exception>
        /// <remarks>n/a</remarks>
        public static Mutex GetNamedMutex(string mutexName)
        {
            if (string.IsNullOrEmpty(mutexName))
            {
                throw new ArgumentNullException("mutexName");
            }

            return getNamedMutex(mutexName, false, true);
        }

        /// <summary>
        /// Gewährt Zugriff auf einen benannten System Mutex.
        /// </summary>
        /// <param name="mutexName">
        ///  Der Namen des benannten System Mutex.
        /// </param>
        /// <param name="create">
        /// Gibt an ob der Mutex erzeugt werden soll wenn er noch nicht existiert.
        /// </param>
        /// <param name="allowAttach">
        /// Gibt an ob der Mutex neu erzeugt werden muss oder existieren darf.
        /// </param>
        /// <returns>
        /// Einen benannten System Mutex oder <c>null</c> wenn der Mutex nicht exisitiert
        /// oder ein anderes Problem aufgetreten ist.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// wird ausgelöst wenn der Parameter <paramref name="mutexName"/>
        /// eine leere Zeichenfolge enthält oder <c>null</c> zugewiesen wurde.
        /// </exception>
        /// <remarks>n/a</remarks>
        public static Mutex GetNamedMutex(string mutexName, bool create, bool allowAttach)
        {
            if (string.IsNullOrEmpty(mutexName))
            {
                throw new ArgumentNullException("mutexName");
            }

            return getNamedMutex(mutexName, create, allowAttach);
        }

        #endregion

        #region private Methods

        /// <summary>
        /// Gewährt Zugriff auf einen benannten System Mutex.
        /// </summary>
        /// <param name="mutexName">
        ///  Der Namen des benannten System Mutex.
        /// </param>
        /// <param name="create">
        /// Gibt an ob der Mutex erzeugt werden soll wenn er noch nicht existiert.
        /// </param>
        /// <param name="allowAttach">
        /// Gibt an ob der Mutex neu erzeugt werden muss oder existieren darf.
        /// </param>
        /// <returns>
        /// Einen benannten System Mutex oder <c>null</c> wenn der Mutex nicht exisitiert
        /// oder ein anderes Problem aufgetreten ist.
        /// </returns>
        /// <remarks>
        /// Versucht unter allen möglichen Umständen den zugfriff auf den Mutex zu
        /// erlangen oder ihn zu erzeugen.
        /// <para>
        /// 1)  Der Mutex exisitiert nicht.
        /// </para>
        /// Es wird eine ACL erzeugt die dem Benutzer alle Rechte auf den Mutex einräumt
        /// und dem erzeugten Mutex zugewiesen
        /// <para>
        /// 2) Der Mutex exisitiert, doch der aktuelle Benutzer hat keinen Zugriff
        /// </para>
        /// Es wird die aktuelle Regel, die dem Benutzer den Zugriff auf den Mutex verweigert,
        /// entfernt. Dann wird versucht dem Benutzer die nötigen Rechte einzuräumen und
        /// die aktuaiserte Regel dem Mutex hinzugefügt. Wird diese Massname verweigert
        /// wird eine Ausnahme wegen unauthorisiertem Zugriff ausgelöst.
        /// <para>
        /// 3) Der Mutex existiert und der Benutzer hat Zugriff.
        /// </para>
        /// Hier sind keine Massnamen nötig. Der Mutex wird geöffnet und zurückgegeben.
        /// </remarks>
        private static Mutex getNamedMutex(string mutexName, bool create, bool allowAttach)
        {
            Mutex mutex = null;
            bool doesNotExist = false;
            bool unauthorized = false;

            /* Der Wert dieser Variable wird vom Mutex Konstruktor
             * gesetzt. Ist sie true wurde der benannte SystemMutex
             * erzeugt. Wenn der Wert false ist, exisitiert der Mutex
             * bereits.
             */
            bool mutexWasCreated = false;

            // versuche den Mutex zu öffnen
            try
            {
                mutex = Mutex.OpenExisting(mutexName);
                if (!allowAttach && mutex != null)
                {
                    // Erfolg, aber Mutex sollte neu sein
                    mutex.Close();
                    throw new SharedMemoryException();
                }
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                // Mutex exisitiert nicht
                doesNotExist = true;
            }
            catch (UnauthorizedAccessException)
            {
                // keine Authorisierung
                unauthorized = true;
            }

            /* Es gibt 3 mögliche Lösungen:
             * (1) Der Mutex exisitiert nicht.
             * (2) Der Mutex exisitiert, doch der aktuelle Benutzer hat keinen Zugriff.
             * (3) Der Mutex existiert und der Benutzer hat Zugriff.
             */
            #region (1) does Not Exist


            if (doesNotExist & create)
            {
                // Der Mutex existiert nicht und soll erzeugt werden, also erzeugen

                // Als erstes eine Access Control List (ACL) erzeugen, die dem
                // Benutzer alle Rechte auf den Mutex einräumt.
                string user = Environment.UserDomainName
                    + "\\" + Environment.UserName;

                // eine Instanz von MutexSecurity erzeugen
                MutexSecurity mutexSec = new MutexSecurity();

                // eine Instanz von MutexAccessRule mit voller Kontrolle erzeugen
                MutexAccessRule rule = new MutexAccessRule(user,
                    MutexRights.FullControl, AccessControlType.Allow);
                
                // die ZugriffsRegel zuweisen
                mutexSec.AddAccessRule(rule);
                
                // den benannten Mutex mit dem Namen und der ZugriffsRegel erzeugen.
                mutex = new Mutex(false, mutexName, out mutexWasCreated, mutexSec);
                if (mutexWasCreated)
                {
                    return mutex;
                }
                else
                {
                    throw new SharedMemoryException();
                }
            }

            #endregion

            #region (2) unauthorized

            else if (unauthorized)
            {
                // Den Mutex öffnen um die ZugriffsSicherheit zu lesen
                // und zu ändern
                try
                {
                    mutex = Mutex.OpenExisting(mutexName,
                        MutexRights.ReadPermissions | MutexRights.ChangePermissions);

                    // die aktuelle ACL holen. Das erfordert das Recht
                    // MutexRights.ReadPermissions.
                    MutexSecurity mutexSec = mutex.GetAccessControl();

                    // den aktuellen Benutzer einlesen
                    string user = Environment.UserDomainName
                        + "\\" + Environment.UserName;

                    // Als erstes muss die Regel, die dem aktuellen Benutzer
                    // das Sperren und Freigen des Mutex verbietet, entfernt werden.
                    MutexAccessRule rule = new MutexAccessRule(user,
                        MutexRights.Synchronize | MutexRights.Modify,
                        AccessControlType.Deny);
                    mutexSec.RemoveAccessRule(rule);

                    // jetzt dem Benutzer die richtigen Rechte zuweisen
                    rule = new MutexAccessRule(user,
                        MutexRights.Synchronize | MutexRights.Modify,
                        AccessControlType.Allow);
                    mutexSec.AddAccessRule(rule);

                    // aktualisieren der ACL. Das erfordert das Recht
                    // MutexRights.ChangePermissions.
                    mutex.SetAccessControl(mutexSec);
                    
                    // den Mutex öffnen
                    mutex = Mutex.OpenExisting(mutexName);
                    
                    // und den Mutex zurückgeben
                    return mutex;
                }
                catch (UnauthorizedAccessException)
                {
                    // neu auslösen der Ausname.
                    throw;
                }
            }

            #endregion

            #region (3) Exist and Access granted

            if (!mutexWasCreated)
            {
                // Mutex existiert und der Benutzer hat Zugriff
                if (mutex != null)
                {
                    return mutex;
                }
            }

            #endregion

            return null;
        }

        #endregion
    }
#endif
}
