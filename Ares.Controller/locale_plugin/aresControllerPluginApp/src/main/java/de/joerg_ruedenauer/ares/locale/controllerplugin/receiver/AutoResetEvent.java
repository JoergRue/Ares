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
package de.joerg_ruedenauer.ares.locale.controllerplugin.receiver;

public class AutoResetEvent
{
    private final Object _monitor = new Object();
    private volatile boolean _isOpen = false;

    public AutoResetEvent(boolean open)
    {
        _isOpen = open;
    }

    public void waitOne()
    {
        synchronized (_monitor) {
            while (!_isOpen) {
                try {
                    _monitor.wait();
                }
                catch (InterruptedException e) {}
            }
            _isOpen = false;
        }
    }

    public boolean waitOne(long timeout)
    {
        synchronized (_monitor) {
            long t = System.currentTimeMillis();
            while (!_isOpen) {
                try {
                    _monitor.wait(timeout);
                }
                catch (InterruptedException e) {}
                // Check for timeout
                if (System.currentTimeMillis() - t >= timeout)
                    return false;
            }
            _isOpen = false;
        }
        return true;
    }

    public void set()
    {
        synchronized (_monitor) {
            _isOpen = true;
            _monitor.notify();
        }
    }

    public void reset()
    {
        _isOpen = false;
    }
}