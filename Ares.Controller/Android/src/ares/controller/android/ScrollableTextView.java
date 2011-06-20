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
package ares.controller.android;

import android.content.Context;
import android.graphics.Rect;
import android.util.AttributeSet;
import android.widget.TextView;
 
public class ScrollableTextView extends TextView {
 
    public ScrollableTextView(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }
 
    public ScrollableTextView(Context context, AttributeSet attrs) {
        super(context, attrs);
    }
 
    public ScrollableTextView(Context context) {
        super(context);
    }
    @Override
    protected void onFocusChanged(boolean focused, int direction, Rect previouslyFocusedRect) {
        if(focused)
            super.onFocusChanged(focused, direction, previouslyFocusedRect);
    }
 
    @Override
    public void onWindowFocusChanged(boolean focused) {
        if(focused)
            super.onWindowFocusChanged(focused);
    }
 
 
    @Override
    public boolean isFocused() {
        return true;
    }
 
}