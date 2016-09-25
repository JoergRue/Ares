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
package de.joerg_ruedenauer.ares.locale.controllerplugin.ui.activity;

import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.text.TextUtils;
import android.util.Log;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import ares.controllers.control.Control;
import ares.controllers.data.Command;
import ares.controllers.data.Configuration;
import ares.controllers.data.Mode;
import ares.controllers.data.TitledElement;
import ares.controllers.messages.IMessageListener;
import ares.controllers.messages.Message;
import ares.controllers.network.INetworkClient;
import ares.controllers.network.IServerListener;
import ares.controllers.network.ServerInfo;
import ares.controllers.network.ServerSearch;
import ares.controllers.util.IUIThreadDispatcher;
import de.joerg_ruedenauer.ares.locale.controllerplugin.CommandType;
import de.joerg_ruedenauer.ares.locale.controllerplugin.R;
import de.joerg_ruedenauer.ares.locale.controllerplugin.TaskerPlugin;
import de.joerg_ruedenauer.ares.locale.controllerplugin.bundle.PluginBundleValues;
import com.twofortyfouram.locale.sdk.client.ui.activity.AbstractAppCompatPluginActivity;
import com.twofortyfouram.log.Lumberjack;

import net.jcip.annotations.NotThreadSafe;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;

@NotThreadSafe
public final class EditActivity extends AbstractAppCompatPluginActivity
        implements IUIThreadDispatcher, IMessageListener, IServerListener, INetworkClient {

    private EditText portEdit;
    private Spinner playerSpinner;
    private Spinner projectSpinner;
    private Spinner modeSpinner;
    private Spinner elementSpinner;
    private CheckBox disconnectBox;
    private CheckBox syncBox;

    @Override
    protected void onCreate(final Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        ares.controllers.util.UIThreadDispatcher.setDispatcher(this);
        ares.controllers.messages.Messages.getInstance().addObserver(this);

        setContentView(R.layout.main);

        /*
         * To help the user keep context, the title shows the host's name and the subtitle
         * shows the plug-in's name.
         */
        CharSequence callingApplicationLabel = null;
        try {
            callingApplicationLabel =
                    getPackageManager().getApplicationLabel(
                            getPackageManager().getApplicationInfo(getCallingPackage(),
                                    0));
        } catch (final PackageManager.NameNotFoundException e) {
            Lumberjack.e("Calling package couldn't be found%s", e); //$NON-NLS-1$
        }
        if (null != callingApplicationLabel) {
            setTitle(callingApplicationLabel);
        }

        portEdit = (EditText)findViewById(R.id.udpPortText);
        playerSpinner = (Spinner)findViewById(R.id.playerSpinner);
        projectSpinner = (Spinner)findViewById(R.id.projectSpinner);
        modeSpinner = (Spinner)findViewById(R.id.modeSpinner);
        elementSpinner = (Spinner)findViewById(R.id.elementSpinner);
        disconnectBox = (CheckBox)findViewById(R.id.disconnectBox);
        syncBox = (CheckBox)findViewById(R.id.syncBox);

        portEdit.setOnEditorActionListener(new TextView.OnEditorActionListener() {
            @Override
            public boolean onEditorAction(TextView textView, int i, KeyEvent keyEvent) {
                mUdpPort = Integer.parseInt(portEdit.getText().toString());
                initialize();
                return true;
            }
        });
        playerSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                mPlayer = playerSpinner.getSelectedItem().toString();
                projectSpinner.setEnabled(false);
                modeSpinner.setEnabled(false);
                elementSpinner.setEnabled(false);
                if (i > 0) --i;
                connectWithPlayer(mServers.get(i));
            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {
            }
        });
        projectSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                mProject = projectSpinner.getSelectedItem().toString();
                if (mProject.equals(mCurrentPlayerProject)) {
                    projectLoaded();
                } else {
                    openPlayerFile();
                }
            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {
            }
        });
        modeSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                modeSelected(i);
            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {
            }
        });
        elementSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
            @Override
            public void onItemSelected(AdapterView<?> adapterView, View view, int i, long l) {
                elementSelected(i);
            }

            @Override
            public void onNothingSelected(AdapterView<?> adapterView) {
            }
        });
        disconnectBox.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                mDisconnect = b;
                if (mDisconnect) {
                    syncBox.setEnabled(true);
                } else {
                    mSynchronous = true;
                    syncBox.setChecked(true);
                    syncBox.setEnabled(false);
                }
            }
        });
        syncBox.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean b) {
                mSynchronous = b;
            }
        });

        getSupportActionBar().setSubtitle(R.string.plugin_name);

        getSupportActionBar().setDisplayHomeAsUpEnabled(true);

        initialize();
    }

    @Override
    public void finish() {
        if (mServerSearch != null) {
            if (mServerSearch.isSearching()) mServerSearch.stopSearch();
            mServerSearch.dispose();
            mServerSearch = null;
        }
        if (Control.getInstance().isConnected()) {
            disconnectFromPlayer();
        }
        if (mScheduler != null)
        {
            mScheduler.shutdown();
            mScheduler = null;
        }
        ares.controllers.messages.Messages.getInstance().removeObserver(this);
        super.finish();
        if (!mIsCancelled && mSynchronous && TaskerPlugin.Setting.hostSupportsSynchronousExecution( getIntent().getExtras() ) ) {
            Bundle result = getResultBundle();
            String blurb = getResultBlurb(result);
            Intent newIntent = new Intent();
            newIntent.putExtra("com.twofortyfouram.locale.intent.extra.BUNDLE", result);
            newIntent.putExtra("com.twofortyfouram.locale.intent.extra.BLURB", blurb);
            TaskerPlugin.Setting.requestTimeoutMS(newIntent, 10000);
            setResult(RESULT_OK, newIntent);
        }
    }

    private int mUdpPort = 8009;
    private String mPlayer = PluginBundleValues.PLAYER_AUTO;
    private String mProject = "";
    private CommandType mCommandType = CommandType.None;
    private int mElement = 0;
    private boolean mDisconnect = true;
    private boolean mSynchronous = false;

    private ServerSearch mServerSearch;
    private List<ServerInfo> mServers = new ArrayList<ServerInfo>();
    private String mCurrentPlayerProject = "";

    @Override
    public void onPostCreateWithPreviousResult(@NonNull final Bundle previousBundle,
            @NonNull final String previousBlurb) {
        if (!PluginBundleValues.isBundleValid(previousBundle))
            return;

        mUdpPort = PluginBundleValues.getPlayerPort(previousBundle);
        mPlayer = PluginBundleValues.getPlayer(previousBundle);
        mProject = PluginBundleValues.getProject(previousBundle);
        mCommandType = PluginBundleValues.getCommand(previousBundle);
        mElement = PluginBundleValues.getElement(previousBundle);
        mDisconnect = PluginBundleValues.getDisconnect(previousBundle);
        mSynchronous = PluginBundleValues.getSynchronous(previousBundle);

        initialize();
    }

    private void initialize()
    {
        portEdit.setText("" + mUdpPort);
        playerSpinner.setEnabled(false);
        projectSpinner.setEnabled(false);
        modeSpinner.setEnabled(false);
        elementSpinner.setEnabled(false);
        disconnectBox.setChecked(mDisconnect);
        syncBox.setChecked(mSynchronous);
        syncBox.setEnabled(mDisconnect);

        mServers.clear();

        if (mServerSearch != null) {
            mServerSearch.stopSearch();
            mServerSearch.dispose();
        }
        mServerSearch = new ServerSearch(this, mUdpPort);
        mServerSearch.startSearch();
    }

    private ExecutorService mScheduler;

    private ExecutorService getScheduler() {
        if (mScheduler == null)
            mScheduler = Executors.newSingleThreadExecutor();
        return mScheduler;
    }

    private void connectWithPlayer(final ServerInfo serverInfo) {
        final INetworkClient client = this;
        getScheduler().execute(new Runnable() {
            @Override
            public void run() {
                Control.getInstance().connect(serverInfo, client, false);
                if (Control.getInstance().isConnected()) {
                    Control.getInstance().requestProjectFiles();
                }
            }
        });
    }

    private void disconnectFromPlayer()
    {
        getScheduler().execute(new Runnable() {
            @Override
            public void run() {
                Control.getInstance().disconnect(true);
            }
        });
    }

    private void openPlayerFile()
    {
        getScheduler().execute(new Runnable() {
            @Override
            public void run() {
                Control.getInstance().openFile(mProject);
            }
        });
    }

    private void projectLoaded() {
        List<CharSequence> modes = new ArrayList<CharSequence>();
        modes.add(this.getResources().getString(R.string.specialMode));
        int index = 0;
        int i = 1;
        for (Mode mode : Control.getInstance().getConfiguration().getModes()) {
            modes.add(mode.getTitle());
            if (index == 0) {
                for (Command element : mode.getCommands()) {
                    if (element.getId() == mElement) {
                        index = i;
                        break;
                    }
                }
            }
            ++i;
        }
        ArrayAdapter<CharSequence> adapter = new ArrayAdapter<CharSequence>(this, android.R.layout.simple_spinner_item, modes);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        modeSpinner.setAdapter(adapter);
        modeSpinner.setEnabled(true);
        modeSpinner.setSelection(index);
    }

    private void modeSelected(int i) {
        List<CharSequence> elements = new ArrayList<CharSequence>();
        int index = 0;
        if (i == 0) {
            elements.add(getResources().getString(R.string.stop));
            elements.add(getResources().getString(R.string.forward));
            elements.add(getResources().getString(R.string.backward));
            elements.add(getResources().getString(R.string.repeat));
            switch (mCommandType) {
                case MusicBackward:
                    index = 2;
                    break;
                case MusicForward:
                    index = 1;
                    break;
                case MusicRepeat:
                    index = 3;
                    break;
                case Stop:
                default:
                    index = 0;
                    break;
            }
        }
        else {
            Mode mode = Control.getInstance().getConfiguration().getModes().get(i - 1);
            int j = 0;
            for (Command element : mode.getCommands()) {
                elements.add(element.getTitle());
                if (element.getId() == mElement) {
                    index = j;
                }
                ++j;
            }
        }
        ArrayAdapter<CharSequence> adapter = new ArrayAdapter<CharSequence>(this, android.R.layout.simple_spinner_item, elements);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        elementSpinner.setAdapter(adapter);
        elementSpinner.setEnabled(true);
        if (!elements.isEmpty()) {
            elementSpinner.setSelection(index);
        }
    }

    private void elementSelected(int i) {
        int modeIndex = modeSpinner.getSelectedItemPosition();
        if (modeIndex == 0) {
            switch (i) {
                case 1:
                    mCommandType = CommandType.MusicForward;
                    break;
                case 2:
                    mCommandType = CommandType.MusicBackward;
                    break;
                case 3:
                    mCommandType = CommandType.MusicRepeat;
                    break;
                case 0:
                default:
                    mCommandType = CommandType.Stop;
                    break;
            }
            mElement = 0;
        }
        else {
            Mode mode = Control.getInstance().getConfiguration().getModes().get(modeIndex - 1);
            mCommandType = CommandType.SwitchElement;
            mElement = mode.getCommands().get(i).getId();
        }
    }

    @Override
    public boolean isBundleValid(@NonNull final Bundle bundle) {
        return PluginBundleValues.isBundleValid(bundle);
    }

    @Nullable
    @Override
    public Bundle getResultBundle() {
        return PluginBundleValues.generateBundle(getApplicationContext(),
                mUdpPort, mPlayer, mProject, mCommandType, mElement, mDisconnect, mSynchronous);
    }

    @NonNull
    @Override
    public String getResultBlurb(@NonNull final Bundle bundle) {
        final int maxBlurbLength = Math.max(getResources().getInteger(
                R.integer.com_twofortyfouram_locale_sdk_client_maximum_blurb_length), 4);

        String blurb = "player: " + mPlayer + "; project: " + mProject +
                "; command: " + mCommandType.toString() + "; element: " + mElement;
        if (blurb.length() > maxBlurbLength) {
            blurb = blurb.substring(0, maxBlurbLength - 3);
            blurb += "...";
        }

        return blurb;
    }

    @Override
    public boolean onCreateOptionsMenu(final Menu menu) {
        getMenuInflater().inflate(R.menu.menu, menu);

        return true;
    }

    @Override
    public boolean onOptionsItemSelected(final MenuItem item) {
        if (android.R.id.home == item.getItemId()) {
            finish();
        }
        else if (R.id.menu_discard_changes == item.getItemId()) {
            // Signal to AbstractAppCompatPluginActivity that the user canceled.
            mIsCancelled = true;
            finish();

            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    public void serverFound(ServerInfo serverInfo) {
        for (ServerInfo known : mServers) {
            if (known.getName().equals(serverInfo.getName())
                    && known.getAddress().toString().equals(serverInfo.getAddress().toString())
                    && known.getTcpPort() == serverInfo.getTcpPort())
            {
                return;
            }
        }
        mServers.add(serverInfo);
        List<CharSequence> names = new ArrayList<CharSequence>();
        names.add(PluginBundleValues.PLAYER_AUTO);
        int index = 0;
        int i = 1;
        for (ServerInfo server : mServers) {
            if (server.getName().equals(mPlayer))
                index = i;
            names.add(server.getName());
            ++i;
        }
        ArrayAdapter<CharSequence> adapter = new ArrayAdapter<CharSequence>(this, android.R.layout.simple_spinner_item, names);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        playerSpinner.setAdapter(adapter);
        playerSpinner.setEnabled(true);
        playerSpinner.setSelection(index);
    }

    @Override
    public void projectFilesRetrieved(List<String> list) {
        final List<String> projects = list;
        dispatchToUIThread(new Runnable() {
            @Override
            public void run() {
                List<CharSequence> elements = new ArrayList<CharSequence>();
                int index = 0;
                int i = 0;
                for (String project : projects) {
                    if (!project.endsWith(".apkg")) {
                        elements.add(project);
                        if (mProject.equals(project))
                            index = i;
                        ++i;
                    }
                }
                ArrayAdapter<CharSequence> adapter = new ArrayAdapter<CharSequence>(EditActivity.this, android.R.layout.simple_spinner_item, elements);
                adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                projectSpinner.setAdapter(adapter);
                projectSpinner.setEnabled(true);
                if (!projects.isEmpty()) {
                    projectSpinner.setSelection(index);
                }
            }
        });
    }

    @Override
    public void configurationChanged(Configuration configuration, String s) {
        final Configuration config = configuration;
        final String fileName = s;
        dispatchToUIThread(new Runnable() {
            @Override
            public void run() {
                mCurrentPlayerProject = fileName;
                Control.getInstance().setConfiguration(config, fileName);
                if (mProject.equals(mCurrentPlayerProject)) {
                    projectLoaded();
                }
            }
        });
    }

    @Override
    public void disconnect() {
    }

    @Override
    public void connectionFailed() {
        dispatchToUIThread(new Runnable() {
            @Override
            public void run() {
                projectSpinner.setEnabled(false);
                modeSpinner.setEnabled(false);
                elementSpinner.setEnabled(false);
            }
        });
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
    public void musicRepeatChanged(boolean b) {
    }

    @Override
    public void musicOnAllSpeakersChanged(boolean b) {
    }

    @Override
    public void importProgressChanged(int i, String s) {
    }

    @Override
    public void messageAdded(Message message) {
        if (message.getType() == Message.MessageType.Error)
        {
            Toast.makeText(this, message.getMessage(), Toast.LENGTH_LONG).show();
        }
        final String tag = "AresControllerLocalePlugin"; // $NON-NLS-1$

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

    private final Handler uiThreadCallback = new Handler();

    @Override
    public void dispatchToUIThread(Runnable runnable) {
        uiThreadCallback.post(runnable);
    }
}
