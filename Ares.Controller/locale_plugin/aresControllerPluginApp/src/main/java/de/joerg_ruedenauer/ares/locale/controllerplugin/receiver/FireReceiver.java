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

import ares.controllers.control.Control;
import ares.controllers.data.Configuration;
import ares.controllers.data.TitledElement;
import ares.controllers.messages.IMessageListener;
import ares.controllers.messages.Message;
import ares.controllers.messages.Messages;
import ares.controllers.network.INetworkClient;
import ares.controllers.network.IServerListener;
import ares.controllers.network.ServerInfo;
import ares.controllers.network.ServerSearch;
import ares.controllers.util.IUIThreadDispatcher;
import de.joerg_ruedenauer.ares.locale.controllerplugin.CommandType;
import de.joerg_ruedenauer.ares.locale.controllerplugin.TaskerPlugin;
import de.joerg_ruedenauer.ares.locale.controllerplugin.bundle.PluginBundleValues;

import com.twofortyfouram.assertion.Assertions;
import com.twofortyfouram.locale.sdk.client.receiver.AbstractPluginSettingReceiver;
import com.twofortyfouram.spackle.AndroidSdkVersion;
import com.twofortyfouram.spackle.ThreadUtil;
import com.twofortyfouram.spackle.bundle.BundleScrubber;

import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.Looper;
import android.support.annotation.NonNull;
import android.util.Log;
import android.util.Pair;
import android.widget.Toast;

import java.net.InetAddress;
import java.util.List;
import java.util.Map;
import java.util.concurrent.Semaphore;
import java.util.concurrent.TimeUnit;

public final class FireReceiver extends BroadcastReceiver
        implements IServerListener, IUIThreadDispatcher, IMessageListener, INetworkClient {

    interface ICallback {
        int call();
    }

    static final class MyCallback implements Handler.Callback {
        public final boolean handleMessage(android.os.Message msg) {
            Assertions.assertNotNull(msg, "msg");
            switch(msg.what) {
                case 0:
                    Assertions.assertNotNull(msg.obj, "msg.obj");
                    Assertions.assertInRangeInclusive(msg.arg1, 0, 1, "msg.arg1");
                    Pair pair = (Pair)msg.obj;
                    boolean msg1 = msg.arg1 != 0;
                    PendingResult res = (PendingResult)pair.first;
                    ICallback innerCallback = (ICallback)pair.second;

                    try {
                        int ret = innerCallback.call();
                        if(msg1) {
                            res.setResultCode(ret);
                        }
                    } finally {
                        res.finish();
                    }

                    if(AndroidSdkVersion.isAtLeastSdk(18)) {
                        Looper.myLooper().quitSafely();
                    } else {
                        Looper.myLooper().quit();
                    }
                default:
                    return true;
            }
        }

    }

    public final void call(@NonNull ICallback callback, boolean isAsync) {
        PendingResult result = goAsync();
        if (result == null) {
            throw new AssertionError("PendingResult was null.  Was goAsync() called previously?");
        }
        else {
            MyCallback myCallback = new MyCallback();
            HandlerThread thread = ThreadUtil.newHandlerThread(getClass().getName(), ThreadUtil.ThreadPriority.BACKGROUND);
            Handler handler = new Handler(thread.getLooper(), myCallback);
            Pair pair = new Pair(result, callback);
            android.os.Message msg = handler.obtainMessage(0, isAsync ? 1 : 0, 0, pair);
            if (!handler.sendMessage(msg)) {
                throw new AssertionError();
            }
        }
    }

    // private AutoResetEvent mCommandDoneEvent = null;
    private Intent mIntent;

    public final void onReceive(final Context context, Intent intent) {
        mContext = context;
        mIntent = intent;
        ares.controllers.util.UIThreadDispatcher.setDispatcher(this);
        ares.controllers.messages.Messages.getInstance().addObserver(this);
        if (!BundleScrubber.scrub(intent)) {
            if("com.twofortyfouram.locale.intent.action.FIRE_SETTING".equals(intent.getAction()) &&
                    (context.getPackageName().equals(intent.getPackage()) || (new ComponentName(context, this.getClass().getName())).equals(intent.getComponent()))) {
                final Bundle bundle = intent.getBundleExtra("com.twofortyfouram.locale.intent.extra.BUNDLE");
                if(!BundleScrubber.scrub(intent) && bundle != null && isBundleValid(bundle)) {
                    Messages.getInstance().removeObserver(this);
                    if(this.isAsync(bundle) && AndroidSdkVersion.isAtLeastSdk(11)) {
                        Log.d("AresControllerPlugin", "Calling async command");

                        this.call(new ICallback() {
                            @Override
                            public int call() {
                                return FireReceiver.this.firePluginSetting(context, bundle);
                            }
                        }, this.isOrderedBroadcast());
                    } else {
                        Log.d("AresControllerPlugin", "Calling sync command");
                        // mCommandDoneEvent = new AutoResetEvent(false);
                        this.call(new ICallback() {
                            @Override
                            public int call() {
                                int res = FireReceiver.this.firePluginSetting(context, bundle);
                                if (FireReceiver.this.isOrderedBroadcast()) {
                                    TaskerPlugin.Setting.signalFinish(mContext, mIntent, res, null);
                                }
                                return res;
                            }
                        }, this.isOrderedBroadcast());
                        // int timeout = TaskerPlugin.Setting.getHintTimeoutMS(bundle);
                        // if (timeout < 5000) timeout = 5000;
                        // mCommandDoneEvent.waitOne(timeout);
                        if (this.isOrderedBroadcast()) {
                            setResultCode(TaskerPlugin.Setting.RESULT_CODE_PENDING);
                        }
                    }
                }
                else {
                    Messages.addMessage(Message.MessageType.Error, "Invalid plugin settings");
                    Messages.getInstance().removeObserver(this);
                }
            }
            else {
                Messages.getInstance().removeObserver(this);
            }
        }
        else {
            Messages.getInstance().removeObserver(this);
        }
    }

    protected boolean isBundleValid(@NonNull final Bundle bundle) {
        return PluginBundleValues.isBundleValid(bundle);
    }

    protected boolean isAsync(final Bundle bundle) {
        return !PluginBundleValues.getSynchronous(bundle);
    }

    private Context mContext;
    private String mPlayerName;

    private AutoResetEvent mEvent;
    private ServerInfo mFoundServer;

    private boolean mLookForProject;

    private boolean mIsMusicRepeat;

    protected int firePluginSetting(@NonNull final Context context, @NonNull final Bundle bundle) {
        mContext = context;
        ares.controllers.util.UIThreadDispatcher.setDispatcher(this);
        ares.controllers.messages.Messages.getInstance().addObserver(this);

        Messages.addMessage(Message.MessageType.Debug, "Starting command action");
        if (!isBundleValid(bundle)) {
            Messages.addMessage(Message.MessageType.Error, "Invalid settings");
            Messages.getInstance().removeObserver(this);
            return TaskerPlugin.Setting.RESULT_CODE_FAILED;
        }

        mPlayerName = PluginBundleValues.getPlayer(bundle);
        int port = PluginBundleValues.getPlayerPort(bundle);

        boolean fixedAddress = PluginBundleValues.isAddressFixed(bundle);
        String playerAddress = PluginBundleValues.getPlayerAddress(bundle);
        int playerTcpPort = PluginBundleValues.getPlayerTcpPort(bundle);

        String projectFile = PluginBundleValues.getProject(bundle);
        CommandType command = PluginBundleValues.getCommand(bundle);
        int elementId = PluginBundleValues.getElement(bundle);
        boolean disconnect = PluginBundleValues.getDisconnect(bundle);

        Messages.addMessage(Message.MessageType.Debug, "Connecting to player " + mPlayerName + " at port " + port);

        mIsMusicRepeat = false;

        mEvent = new AutoResetEvent(false);

        WifiManager wifiMgr = (WifiManager) context.getSystemService(Context.WIFI_SERVICE);
        if (wifiMgr == null) {
            Messages.addMessage(Message.MessageType.Error, "Wifi Mgr not available");
            Messages.getInstance().removeObserver(this);
            return TaskerPlugin.Setting.RESULT_CODE_FAILED;
        }
        if (!wifiMgr.isWifiEnabled()) {
            Messages.addMessage(Message.MessageType.Error, "Wifi not available");
            Messages.getInstance().removeObserver(this);
            return TaskerPlugin.Setting.RESULT_CODE_FAILED;
        }
        final WifiManager.WifiLock wifiLock = wifiMgr.createWifiLock("Ares Controller Locale Plugin");
        wifiLock.acquire();
        final WifiManager.MulticastLock multicastLock = wifiMgr.createMulticastLock("Ares Controller Locale Plugin");
        multicastLock.acquire();

        boolean needsConnect = true;
        if (Control.getInstance().isConnected())
        {
            if (Control.getInstance().getCurrentServer().getName().equals(mPlayerName) || mPlayerName.equals(PluginBundleValues.PLAYER_AUTO)) {
                Messages.addMessage(Message.MessageType.Debug, "Already connected; project: " + Control.getInstance().getFileName() + ", path: " + Control.getInstance().getFilePath());
                needsConnect = false;
            }
            else {
                Messages.addMessage(Message.MessageType.Debug, "Connected to different player: disconnecting");
                Control.getInstance().disconnect(true);
            }
        }

        if (needsConnect) {
            if (!fixedAddress) {
                mFoundServer = null;
                ServerSearch serverSearch = new ServerSearch(this, port);
                Messages.addMessage(Message.MessageType.Debug, "Starting server search");
                serverSearch.startSearch();
                mEvent.waitOne(4000);
                serverSearch.stopSearch();
                serverSearch.dispose();
                if (mFoundServer == null) {
                    Messages.addMessage(Message.MessageType.Error, "Player " + mPlayerName + " not found in network."); // $NON-NLS-1$ $NON-NLS-2$
                    Messages.getInstance().removeObserver(this);
                    multicastLock.release();
                    wifiLock.release();
                    return TaskerPlugin.Setting.RESULT_CODE_FAILED;
                }
                Messages.addMessage(Message.MessageType.Debug, "Player found, connecting ...");
            }
            else {
                try {
                    InetAddress address = InetAddress.getByName(playerAddress);
                    mFoundServer = new ServerInfo(address, true, playerTcpPort, false, 0, mPlayerName);
                    Messages.addMessage(Message.MessageType.Debug, "Connecting to fixed address ...");
                }
                catch (java.net.UnknownHostException ex) {
                    Messages.addMessage(Message.MessageType.Error, "Invalid fixed address: " + playerAddress);
                    Messages.getInstance().removeObserver(this);
                    multicastLock.release();
                    wifiLock.release();
                    return TaskerPlugin.Setting.RESULT_CODE_FAILED;
                }
            }

            Control.getInstance().connect(mFoundServer, this, false);
            if (!Control.getInstance().isConnected()) {
                Messages.addMessage(Message.MessageType.Error, "Could not connect to player  " + mPlayerName + "."); // $NON-NLS-1$ $NON-NLS-2$
                Messages.getInstance().removeObserver(this);
                multicastLock.release();
                wifiLock.release();
                return TaskerPlugin.Setting.RESULT_CODE_FAILED;
            }
        }
        String playerProject = "";
        boolean hasProject = false;
        synchronized (this) { playerProject = Control.getInstance().getFileName(); };
        if ("".equals(playerProject)) {
            Messages.addMessage(Message.MessageType.Debug, "Connected, waiting for project info ...");
            int count = 0;
            while ("".equals(playerProject) && count < 40)
            {
                mEvent.waitOne(100);
                synchronized (this) { playerProject = Control.getInstance().getFileName(); };
                ++count;
            }
        }
        if (!playerProject.equals(projectFile) && command == CommandType.SwitchElement) {
            Messages.addMessage(Message.MessageType.Debug, "Connected, opening project " + projectFile + "...");
            Control.getInstance().openFile(Control.REMOTE_FILE_TAG + projectFile);
            int count = 0;
            while (!playerProject.equals(projectFile) && count < 50)
            {
                mEvent.waitOne(100);
                synchronized (this) {
                        playerProject = Control.getInstance().getFileName();
                };
                ++count;
            }
            if (!playerProject.equals(projectFile))
            {
                Messages.addMessage(Message.MessageType.Error, "Could not load project '" + projectFile + "'; loaded project is '" + playerProject + "'."); // $NON-NLS-1$ $NON-NLS-2$
                Control.getInstance().disconnect(true);
                Messages.getInstance().removeObserver(this);
                multicastLock.release();
                wifiLock.release();
                return TaskerPlugin.Setting.RESULT_CODE_FAILED;
            }
        }


        Messages.addMessage(Message.MessageType.Debug, "Project loaded, executing command ...");

        switch (command)
        {
            case Stop:
                Control.getInstance().sendKey(ares.controllers.data.KeyStroke.getKeyStroke( 0x1B, 0));
                break;
            case MusicForward:
                Control.getInstance().sendKey(ares.controllers.data.KeyStroke.getKeyStroke( 0x27, 0));
                break;
            case MusicBackward:
                Control.getInstance().sendKey(ares.controllers.data.KeyStroke.getKeyStroke( 0x25, 0));
                break;
            case MusicRepeat:
            {
                boolean newValue = true;
                synchronized (this) {
                    newValue = !mIsMusicRepeat;
                }
                Control.getInstance().setMusicRepeat(newValue);
                break;
            }
            case SwitchElement:
                Control.getInstance().switchElement(elementId);
                break;
            default:
                Messages.addMessage(Message.MessageType.Error, "Unknown command type"); // $NON-NLS-1$
                break;
        }

        Messages.addMessage(Message.MessageType.Info, "Command " + command.toString() + " sent."); // $NON-NLS-1$ $NON-NLS-2$

        if (disconnect) {
            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
            }
            Control.getInstance().disconnect(true);
        }
        Messages.getInstance().removeObserver(this);
        multicastLock.release();
        wifiLock.release();

        return TaskerPlugin.Setting.RESULT_CODE_OK;
    }

    @Override
    public void serverFound(ServerInfo serverInfo) {
        if (PluginBundleValues.PLAYER_AUTO.equals(mPlayerName) || mPlayerName.equals(serverInfo.getName())) // $NON-NLS-1$
        {
            if (mFoundServer == null)
            {
                Messages.addMessage(Message.MessageType.Debug, "Server " + serverInfo.getName() + " found."); // $NON-NLS-1$ $NON-NLS-2$
                mFoundServer = serverInfo;
                mEvent.set();
            }
        }
        else {
            Messages.addMessage(Message.MessageType.Debug, "Non-matching server " + serverInfo.getName() + " found."); // $NON-NLS-1$ $NON-NLS-2$
        }
    }

    private final Handler uiThreadCallback = new Handler();

    @Override
    public void dispatchToUIThread(Runnable runnable) {
        uiThreadCallback.post(runnable);
    }

    @Override
    public void messageAdded(Message message) {
        if (message.getType() == Message.MessageType.Error)
        {
            Toast.makeText(mContext, message.getMessage(), Toast.LENGTH_LONG).show();
        }
        final String tag = "AresControllerPlugin"; // $NON-NLS-1$

        switch (message.getType())
        {
            case Error:
                Log.e(tag, message.getMessage());
                break;
            case Warning:
                Log.w(tag, message.getMessage());
                break;
            case Info:
                Log.i(tag, message.getMessage());
                break;
            case Debug:
                Log.d(tag, message.getMessage());
                break;
            default:
                break;
        }
    }

    @Override
    public void configurationChanged(Configuration configuration, String s) {
        synchronized (this) {
            Control.getInstance().setConfiguration(configuration, s);
        }
        Messages.addMessage(Message.MessageType.Debug, "Received Player project info: " + s);
        mEvent.set();
    }

    @Override
    public void connectionFailed() {

    }

    @Override
    public void modeChanged(String s) {
    }

    @Override
    public void modeElementStarted(int i) {
    }

    @Override
    public void modeElementStopped(int i) {
    }

    @Override
    public void allModeElementsStopped() {
    }

    @Override
    public void volumeChanged(int i, int i1) {
    }

    @Override
    public void musicChanged(String s, String s1) {
    }

    @Override
    public void musicListChanged(List<TitledElement> list) {
    }

    @Override
    public void musicFadingChanged(int i, int i1) {
    }

    @Override
    public void tagsChanged(List<TitledElement> list, Map<Integer, List<TitledElement>> map) {
    }

    @Override
    public void activeTagsChanged(List<Integer> list) {
    }

    @Override
    public void tagSwitched(int i, boolean b) {
    }

    @Override
    public void tagCategoryCombinationChanged(CategoryCombination categoryCombination) {
    }

    @Override
    public void musicTagFadingChanged(int i, boolean b) {
    }

    @Override
    public void projectFilesRetrieved(List<String> list) {
    }

    @Override
    public void disconnect() {
    }

    @Override
    public void musicRepeatChanged(boolean b) {
        synchronized (this) {
            mIsMusicRepeat = b;
        }
    }

    @Override
    public void musicOnAllSpeakersChanged(boolean b) {
    }

    @Override
    public void importProgressChanged(int i, String s) {
    }
}
