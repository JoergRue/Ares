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
package de.joerg_ruedenauer.ares.locale.controllerplugin.bundle;

import com.twofortyfouram.assertion.BundleAssertions;
import com.twofortyfouram.spackle.AppBuildInfo;

import net.jcip.annotations.ThreadSafe;

import android.content.Context;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.util.Log;

import de.joerg_ruedenauer.ares.locale.controllerplugin.CommandType;

import static com.twofortyfouram.assertion.Assertions.assertNotEmpty;
import static com.twofortyfouram.assertion.Assertions.assertNotNull;

/**
 * Manages the {@link com.twofortyfouram.locale.api.Intent#EXTRA_BUNDLE EXTRA_BUNDLE} for this
 * plug-in.
 */
@ThreadSafe
public final class PluginBundleValues {

    public static final String PLAYER_AUTO = "<auto>";

    /**
     * Type: {@code int}.
     * <p>
     * Player UDP port for search
     */
    @NonNull
    public static final String BUNDLE_EXTRA_INT_PLAYER_PORT
            = "de.joerg_ruedenauer.ares.locale.controllerplugin.extra.INT_PLAYER_PORT"; //$NON-NLS-1$

    /**
     * Type: {@code String}.
     * <p>
     * Player name, or "auto" to connect to the first player found.
     */
    @NonNull
    public static final String BUNDLE_EXTRA_STRING_PLAYER
            = "de.joerg_ruedenauer.ares.locale.controllerplugin.extra.STRING_PLAYER"; //$NON-NLS-1$

    /**
     * Type: {@code String}.
     * <p>
     * Project file name.
     */
    @NonNull
    public static final String BUNDLE_EXTRA_STRING_PROJECT_FILE
            = "de.joerg_ruedenauer.ares.locale.controllerplugin.extra.STRING_PROJECT_FILE"; //$NON-NLS-1$

    /**
     * Type: {@code int}.
     * <p>
     * Command type (enum value)
     */
    @NonNull
    public static final String BUNDLE_EXTRA_INT_COMMAND_TYPE
            = "de.joerg_ruedenauer.ares.locale.controllerplugin.extra.INT_COMMAND_TYPE"; //$NON-NLS-1$

    /**
     * Type: {@code int}.
     * <p>
     * ID of element to switch
     */
    @NonNull
    public static final String BUNDLE_EXTRA_INT_ELEMENT_ID
            = "de.joerg_ruedenauer.ares.locale.controllerplugin.extra.INT_ELEMENT_ID"; //$NON-NLS-1$

    /**
     * Type: {@code boolean}.
     * <p>
     * Whether the plugin shall disconnect after the command
     */
    @NonNull
    public static final String BUNDLE_EXTRA_BOOL_DISCONNECT
            = "de.joerg_ruedenauer.ares.locale.controllerplugin.extra.BOOL_DISCONNECT"; //$NON-NLS-1$

    /**
     * Type: {@code boolean}.
     * <p>
     * Whether the plugin shall execute the command synchronously
     */
    @NonNull
    public static final String BUNDLE_EXTRA_BOOL_SYNCHRONOUS
            = "de.joerg_ruedenauer.ares.locale.controllerplugin.extra.BOOL_SYNCHRONOUS"; //$NON-NLS-1$

    /**
     * Type: {@code int}.
     * <p>
     * versionCode of the plug-in that saved the Bundle.
     */
    /*
     * This extra is not strictly required, however it makes backward and forward compatibility
     * significantly easier. For example, suppose a bug is found in how some version of the plug-in
     * stored its Bundle. By having the version, the plug-in can better detect when such bugs occur.
     */
    @NonNull
    public static final String BUNDLE_EXTRA_INT_VERSION_CODE
            = "de.joerg_ruedenauer.ares.locale.controllerplugin.extra.INT_VERSION_CODE"; //$NON-NLS-1$

    /**
     * Method to verify the content of the bundle are correct.
     * <p>
     * This method will not mutate {@code bundle}.
     *
     * @param bundle bundle to verify. May be null, which will always return false.
     * @return true if the Bundle is valid, false if the bundle is invalid.
     */
    public static boolean isBundleValid(@Nullable final Bundle bundle) {
        if (null == bundle) {
            return false;
        }

        try {
            BundleAssertions.assertHasString(bundle, BUNDLE_EXTRA_STRING_PLAYER, false, false);
            BundleAssertions.assertHasString(bundle, BUNDLE_EXTRA_STRING_PROJECT_FILE, false, false);
            BundleAssertions.assertHasInt(bundle, BUNDLE_EXTRA_INT_COMMAND_TYPE);
            BundleAssertions.assertHasInt(bundle, BUNDLE_EXTRA_INT_ELEMENT_ID);
            BundleAssertions.assertHasInt(bundle, BUNDLE_EXTRA_INT_VERSION_CODE);
            BundleAssertions.assertHasInt(bundle, BUNDLE_EXTRA_INT_PLAYER_PORT);
        } catch (final AssertionError e) {
            Log.e("AresControllerPlugin", "Bundle failed verification: " + e.getMessage()); //$NON-NLS-1$
            return false;
        }

        return true;
    }

    /**
     * @param context Application context.
     * @param playerPort the UDP port to search for a player
     * @param player The player to be controlled.
     * @param projectFile The project to be used
     * @param commandType The type of command to be issued
     * @param elementId The id of the element to be switched
     * @return A plug-in bundle.
     */
    @NonNull
    public static Bundle generateBundle(@NonNull final Context context, final int playerPort,
            @NonNull final String player, final String projectFile, final CommandType commandType,
                                        final int elementId, final boolean disconnect,
                                        final boolean synchronous) {
        assertNotNull(context, "context"); //$NON-NLS-1$
        assertNotEmpty(player, "player"); //$NON-NLS-1$

        final Bundle result = new Bundle();
        result.putInt(BUNDLE_EXTRA_INT_VERSION_CODE, AppBuildInfo.getVersionCode(context));
        result.putInt(BUNDLE_EXTRA_INT_PLAYER_PORT, playerPort);
        result.putString(BUNDLE_EXTRA_STRING_PLAYER, player);
        result.putString(BUNDLE_EXTRA_STRING_PROJECT_FILE, projectFile);
        result.putInt(BUNDLE_EXTRA_INT_COMMAND_TYPE, commandType.ordinal());
        result.putInt(BUNDLE_EXTRA_INT_ELEMENT_ID, elementId);
        result.putBoolean(BUNDLE_EXTRA_BOOL_DISCONNECT, disconnect);
        result.putBoolean(BUNDLE_EXTRA_BOOL_SYNCHRONOUS, synchronous);

        return result;
    }

    /**
     * @param bundle A valid plug-in bundle.
     * @return The player inside the plug-in bundle.
     */
    @NonNull
    public static String getPlayer(@NonNull final Bundle bundle) {
        String val = bundle.getString(BUNDLE_EXTRA_STRING_PLAYER);
        return val != null ? val : PLAYER_AUTO;
    }

    /**
     * @param bundle A valid plug-in bundle.
     * @return The project inside the plug-in bundle.
     */
    @NonNull
    public static String getProject(@NonNull final Bundle bundle) {
        String val = bundle.getString(BUNDLE_EXTRA_STRING_PROJECT_FILE);
        return val != null ? val : "";
    }

    /**
     * @param bundle A valid plug-in bundle.
     * @return The command inside the plug-in bundle.
     */
    @NonNull
    public static CommandType getCommand(@NonNull final Bundle bundle) {
        int type = bundle.getInt(BUNDLE_EXTRA_INT_COMMAND_TYPE);
        if (type >= 0 && type < CommandType.values().length)
            return CommandType.values()[type];
        else
            return CommandType.None;
    }

    /**
     * @param bundle A valid plug-in bundle.
     * @return The element id inside the plug-in bundle.
     */
    @NonNull
    public static int getElement(@NonNull final Bundle bundle) {
        return bundle.getInt(BUNDLE_EXTRA_INT_ELEMENT_ID);
    }

    /**
     * @param bundle A valid plug-in bundle.
     * @return The player port inside the plug-in bundle.
     */
    @NonNull
    public static int getPlayerPort(@NonNull final Bundle bundle) {
        return bundle.getInt(BUNDLE_EXTRA_INT_PLAYER_PORT);
    }

    /**
     * @param bundle A valid plug-in bundle.
     * @return Whether the plug-in shall disconnect after the command
     */
    public static boolean getDisconnect(@NonNull final Bundle bundle) {
        return bundle.containsKey(BUNDLE_EXTRA_BOOL_DISCONNECT) ? bundle.getBoolean(BUNDLE_EXTRA_BOOL_DISCONNECT) : true;
    }

    /**
     * @param bundle A valid plug-in bundle.
     * @return Whether the command shall execute synchronously
     */
    public static boolean getSynchronous(@NonNull final Bundle bundle) {
        return bundle.containsKey(BUNDLE_EXTRA_BOOL_SYNCHRONOUS) ? bundle.getBoolean(BUNDLE_EXTRA_BOOL_SYNCHRONOUS) : false;
    }

    /**
     * Private constructor prevents instantiation
     *
     * @throws UnsupportedOperationException because this class cannot be instantiated.
     */
    private PluginBundleValues() {
        throw new UnsupportedOperationException("This class is non-instantiable"); //$NON-NLS-1$
    }
}
