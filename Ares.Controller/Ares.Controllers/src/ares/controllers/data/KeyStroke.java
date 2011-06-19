/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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
package ares.controllers.data;

import java.util.HashMap;

public class KeyStroke {

	private int keyCode;
	
	private KeyStroke(int code) {
		keyCode = code;
	}
	
	public boolean equals(Object other) {
		if (other instanceof KeyStroke) {
			return equals((KeyStroke)other);
		}
		return false;
	}
	
	public String toString() {
		return "" + keyCode;
	}
	
	public int getKeyCode() {
		return keyCode;
	}
	
	public boolean equals(KeyStroke other) {
		return (other != null && other.keyCode == keyCode);
	}
	
	private static HashMap<Integer, KeyStroke> sStrokes = new HashMap<Integer, KeyStroke>();
	
	public static KeyStroke getKeyStroke(int key, int unused) {
		if (sStrokes.containsKey(key)) {
			return sStrokes.get(key);
		}
		KeyStroke stroke = new KeyStroke(key);
		sStrokes.put(key, stroke);
		return stroke;
	}
}
