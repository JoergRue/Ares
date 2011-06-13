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
package ares.controller.control;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;

import javax.swing.JFrame;
import javax.swing.JOptionPane;

import ares.controllers.control.Version;

import ares.controller.util.Localization;
import ares.controller.gui.ChangeLogDialog;

public class OnlineOperations {
  
  public static void showHomepage(JFrame parent) {
	  showWebpage(parent, getUrlBase() + Localization.getString("MainFrame.HelpLink")); //$NON-NLS-1$
  }
  
  private static void showWebpage(JFrame parent, String url) {
	  if (java.awt.Desktop.isDesktopSupported()) {
		  try {
			java.awt.Desktop.getDesktop().browse(java.net.URI.create(url));
		} catch (IOException e) {
			JOptionPane.showMessageDialog(parent, Localization.getString("MainFrame.BrowserOpenError") + "\n" + e.getLocalizedMessage(), Localization.getString("MainFrame.Ares"), JOptionPane.ERROR_MESSAGE); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
		}
	  }
	  else {
		  JOptionPane.showMessageDialog(parent, Localization.getString("MainFrame.BrowserNotSupported"), Localization.getString("MainFrame.Ares"), JOptionPane.ERROR_MESSAGE); //$NON-NLS-1$ //$NON-NLS-2$
	  }	  
  }

  public static void downloadSetup(JFrame parent, Version version) {
    final String urlBase = "http://sourceforge.net/projects/aresrpg/files/"; //$NON-NLS-1$
    final String urlAppendix = "/download"; //$NON-NLS-1$
    final String windowsPart = "-Setup.exe"; //$NON-NLS-1$
    final String linuxPart = "-Linux-x86-Install"; //$NON-NLS-1$
    boolean isWindows = System.getProperty("os.name").contains("Windows"); //$NON-NLS-1$ //$NON-NLS-2$
    String url = urlBase + version.toString() + "/Ares-" + version.toString() + (isWindows ? windowsPart : linuxPart) + urlAppendix; //$NON-NLS-1$
    showWebpage(parent, url);
    if (JOptionPane.showConfirmDialog(parent, Localization.getString("OnlineOperations.CloseAres"),  //$NON-NLS-1$
        Localization.getString("OnlineOperations.Ares"), JOptionPane.YES_NO_OPTION) == JOptionPane.YES_OPTION) { //$NON-NLS-1$
      if (!(parent instanceof ares.controller.gui.MainFrame)) return;
      ((ares.controller.gui.MainFrame)parent).exitProgram();
    }
  }
  
  private static abstract class ContentCallback implements Runnable {
    private String content;
    private Exception exception;
    private boolean canceled;

    public void setContent(String content) {
      this.content = content;
    }
    public void setException(Exception ex) {
      this.exception = ex;
    }
    public void setCanceled(boolean canceled) {
      this.canceled = canceled;
    }
    
    protected String getContent() { return content; }
    protected Exception getException() { return exception; }
    protected boolean wasCanceled() { return canceled; }
    
    protected ContentCallback() {
      content = ""; //$NON-NLS-1$
      exception = null;
      canceled = false;
    }
  }
  
  private static class ChangeLogCallback extends ContentCallback {
    private final JFrame parent;
    private final Version version;
    
    public ChangeLogCallback(JFrame parent, Version serverVersion) {
      this.parent = parent;
      this.version = serverVersion;
    }
    
    public void run() {
      if (wasCanceled()) return;
      String text = getContent();
      if (getException() != null) {
        text = Localization.getString("OnlineOperations.GetChangeLogError") //$NON-NLS-1$
          + getException().getMessage();
      }
      ChangeLogDialog dialog = new ChangeLogDialog(parent, text, version);
      dialog.setVisible(true);
    }
    
  }
  
  private static class FileDownloader implements Runnable {
    
    public FileDownloader(JFrame parent, ContentCallback callback, String url, String text) {
      this.parent = parent;
      this.callback = callback;
      this.urlS = url;
      this.text = text;
      this.verbose = true;
    }
    
    private final JFrame parent;
    private final ContentCallback callback;
    private final String urlS;
    private final String text;
    private boolean verbose;
    
    public void setVerbose(boolean verbose) {
      this.verbose = verbose;
    }
    
    public void download() {
      (new Thread(this)).start();
    }

    public void run() {
        ares.controller.gui.util.ProgressMonitor monitor = null;
        if (verbose) monitor = new ares.controller.gui.util.ProgressMonitor(parent, text);
        try {
          URL url = new URL(urlS);
          BufferedReader in = new BufferedReader(new InputStreamReader(url
              .openStream(), "ISO-8859-1")); //$NON-NLS-1$
          try {
            StringBuffer sb = new StringBuffer();
            String line = in.readLine();
            while (line != null) {
              sb.append(line);
              sb.append("\n"); //$NON-NLS-1$
              line = in.readLine();
            }
            callback.setContent(sb.toString());
            if (monitor != null && monitor.isCanceled()) callback.setCanceled(true);
          }
          finally {
            in.close();
          }
        }
        catch (java.net.MalformedURLException e) {
          e.printStackTrace();
        }
        catch (java.io.IOException e) {
          callback.setException(e);
        }
        finally {
          if (monitor != null && !monitor.isCanceled()) monitor.close();
        }
        javax.swing.SwingUtilities.invokeLater(callback);
      }
  }
  
  public static class VersionCallback extends ContentCallback {
    private JFrame parent;
    private boolean verbose;
    public VersionCallback(JFrame parent, boolean verbose) {
      this.parent = parent;
      this.verbose = verbose;
    }
    public void run() {
      Version serverVersion = null;
      if (getException() == null) {
        try {
          serverVersion = Version.parse(getContent().trim());
        }
        catch (java.text.ParseException e) {
          setException(e);
        }
      }
      if (getException() != null) {
        JOptionPane.showMessageDialog(parent,
            Localization.getString("OnlineOperations.GetVersionError") //$NON-NLS-1$
              + getException().getMessage(), Localization.getString("OnlineOperations.Ares"), JOptionPane.ERROR_MESSAGE); //$NON-NLS-1$
        return;
      }
      Version thisVersion = Version.getCurrentVersion();
      int res = thisVersion.compareTo(serverVersion);
      if (res == -1 && (!verbose || !wasCanceled())) {
        Object[] options = { Localization.getString("OnlineOperations.Download"), Localization.getString("OnlineOperations.ShowChanges") }; //$NON-NLS-1$ //$NON-NLS-2$
        Object result = JOptionPane.showInputDialog(parent, 
            Localization.getString("OnlineOperations.NewVersion"), Localization.getString("OnlineOperations.Ares"),  //$NON-NLS-1$ //$NON-NLS-2$
            JOptionPane.INFORMATION_MESSAGE, null, options, Localization.getString("OnlineOperations.Download")); //$NON-NLS-1$
        if (Localization.getString("OnlineOperations.Download").equals(result)) { //$NON-NLS-1$
          OnlineOperations.downloadSetup(parent, serverVersion);
        }
        else if (result != null) {
          ChangeLogCallback callback = new ChangeLogCallback(parent, serverVersion);
          final String urlS = getUrlBase() + Localization.getString("OnlineOperations.ChangeLogFile"); //$NON-NLS-1$
          final String text = Localization.getString("OnlineOperations.GettingVersionHistory"); //$NON-NLS-1$
          FileDownloader downloader = new FileDownloader(parent, callback, urlS, text);
          downloader.download();
        }
      }
      else if (verbose && !wasCanceled()) {
        JOptionPane.showMessageDialog(parent,
            Localization.getString("OnlineOperations.NoNewVersion"), Localization.getString("OnlineOperations.Ares"), //$NON-NLS-1$ //$NON-NLS-2$
            JOptionPane.INFORMATION_MESSAGE);
      }
    }
  }

  public static void checkForUpdate(JFrame parent, boolean verbose) {
    final String urlS = getUrlBase() + "ares_version.txt"; //$NON-NLS-1$
    final String text = Localization.getString("OnlineOperations.SearchingVersion"); //$NON-NLS-1$
    VersionCallback callback = new VersionCallback(parent, verbose);
    FileDownloader downloader = new FileDownloader(parent, callback, urlS, text);
    downloader.setVerbose(verbose);
    downloader.download();
  }
  
  private static String getUrlBase()
  {
	return "http://aresrpg.sourceforge.net/";  //$NON-NLS-1$
  }
  
}
