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
package ares.controller.gui.util;

import java.awt.Component;
import java.awt.Frame;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.lang.reflect.InvocationTargetException;

import javax.swing.JDialog;
import javax.swing.JOptionPane;
import javax.swing.JProgressBar;
import javax.swing.SwingUtilities;
import javax.swing.Timer;

import ares.controller.util.Localization;

public class ProgressMonitor {
  
  private Timer timer;
  
  private boolean canceled;
  
  private boolean closed;
  
  private JDialog dialog;
  
  private JOptionPane optionPane;
  
  private static final int DEFAULT_DELAY = 700;
  
  // on the worker thread!
  public ProgressMonitor(Component parent, String text) {
    optionPane = new JOptionPane(text, JOptionPane.INFORMATION_MESSAGE, JOptionPane.DEFAULT_OPTION, null, new Object[] {Localization.getString("ProgressMonitor.Cancel")}); //$NON-NLS-1$
    closed = false;
    canceled = false;
    timer = new Timer(DEFAULT_DELAY, new ActionListener() {
      public void actionPerformed(ActionEvent e) {
        showDialog();
      }
    });
    timer.setRepeats(false);
    timer.start();
  }
  
  // on the GUI thread!
  private void showDialog() {
    synchronized(this) {
      if (closed) return;
    }
    JProgressBar progressBar = new JProgressBar();
    progressBar.setIndeterminate(true);
    optionPane.setMessage(new Object[] { optionPane.getMessage(), progressBar } );
    Frame frame = JOptionPane.getFrameForComponent(optionPane.getParent());
    dialog = optionPane.createDialog(frame, Localization.getString("ProgressMonitor.Progress")); //$NON-NLS-1$
    dialog.setLocationRelativeTo(frame);
    dialog.setModal(false);
    dialog.addWindowListener(new WindowAdapter() {
      public void windowClosed(WindowEvent e) {
        closed();
      }

      public void windowClosing(WindowEvent e) {
        closed();
      }
    });
    dialog.setVisible(true);
  }
  
  public synchronized void closed() {
    if (!closed) canceled = true;
  }
  
  // on the worker thread!
  public synchronized boolean isCanceled() {
    return canceled;
  }
  
  // on the worker thread!
  public void close() {
    synchronized(this) {
      closed = true;
      if (timer.isRunning()) {
        timer.stop();
      }
    }
    if (dialog != null && dialog.isVisible()) {
      try {
        SwingUtilities.invokeAndWait(new Runnable() {
          public void run() {
            if (dialog != null && dialog.isVisible()) dialog.dispose();
          }
        });
      }
      catch (InvocationTargetException e) {
        e.printStackTrace();
      }
      catch (InterruptedException e) {
        // ignored
      }
    }
  }

}
