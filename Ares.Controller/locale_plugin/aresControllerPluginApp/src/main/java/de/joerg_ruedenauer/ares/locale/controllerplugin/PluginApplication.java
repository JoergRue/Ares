/*
 Copyright (c) 2016 [Joerg Ruedenauer]

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
package de.joerg_ruedenauer.ares.locale.controllerplugin;

import android.app.Application;

/**
 * Implements an application object for the plug-in.
 */
/*
 * This application is non-essential for the plug-in's operation; it simply enables debugging
 * options globally for the app.
 */
public final class PluginApplication extends Application {

    @Override
    public void onCreate() {
        super.onCreate();

    }
}
