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
package ares.controllers.control;

import java.text.ParseException;
import java.util.StringTokenizer;

import ares.controllers.util.Localization;

public class Version implements Comparable<Version> {

  private static Version currentVersion = createCurrentVersion();

  private static Version createCurrentVersion() {
    try {
      return Version.parse("1.4.0"); //$NON-NLS-1$
    }
    catch (ParseException e) {
      return createVersion(1, 0, 0);
    }
  }

  private Version() {
    // nothing to do
  }

  private Version(int major, int minor, int patch) {
    this.major = major;
    this.minor = minor;
    this.patch = patch;
  }

  private int major;

  private int minor;

  private int patch;

  public static Version createVersion(int major, int minor, int patch) {
    return new Version(major, minor, patch);
  }

  public static Version parse(String text) throws ParseException {
    StringTokenizer tok = new StringTokenizer(text, "."); //$NON-NLS-1$
    if (tok.countTokens() != 3)
      throw new ParseException(text + Localization.getString("Version.invalidVersion"), 0); //$NON-NLS-1$
    try {
      int major = Integer.parseInt(tok.nextToken());
      int minor = Integer.parseInt(tok.nextToken());
      int patch = Integer.parseInt(tok.nextToken());
      if (major < 0 || minor < 0 || patch < 0) {
        throw new ParseException(text + Localization.getString("Version.invalidVersion"), 0); //$NON-NLS-1$
      }
      return createVersion(major, minor, patch);
    }
    catch (NumberFormatException e) {
      throw new ParseException(text + Localization.getString("Version.invalidVersion"), 0); //$NON-NLS-1$
    }
  }

  public String toString() {
    return major + "." + minor + "." + patch; //$NON-NLS-1$ //$NON-NLS-2$
  }

  public boolean equals(Object other) {
    if (other == null) return false;
    if (!(other instanceof Version)) return false;
    Version version2 = (Version) other;
    return major == version2.major && minor == version2.minor && patch == version2.patch;
  }

  public int hashCode() {
    return 13 * 13 * major + 13 * minor + patch;
  }

  public static String getCurrentVersionString() {
    return currentVersion.toString();
  }

  public static Version getCurrentVersion() {
    return currentVersion;
  }

  public int compareTo(Version version) {
    if (version == null) return -1;
    if (major < version.major) return -1;
    if (major > version.major) return 1;
    if (minor < version.minor) return -1;
    if (minor > version.minor) return 1;
    if (patch < version.patch) return -1;
    if (patch > version.patch) return 1;
    return 0;
  }

}
