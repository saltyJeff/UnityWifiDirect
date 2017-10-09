package com.gmail.jeffersonlee2000.unitywifidirect;

/**
 * Created by Jefferson on 10/22/2016.
 */
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.ServiceConnection;
import android.os.IBinder;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;
import android.util.Base64;

import com.unity3d.player.*;

import java.io.UnsupportedEncodingException;
import java.nio.charset.Charset;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;

import edu.rit.se.wifibuddy.DnsSdService;
import edu.rit.se.wifibuddy.DnsSdTxtRecord;
import edu.rit.se.wifibuddy.WifiDirectHandler;

public class UnityWifiDirect {
    public static Activity unityActivity;
    public static String TAG = "UnityWifiDirect";
    public static boolean wifiDirectHandlerBound = false;
    public static WifiDirectHandler wifiDirectHandler;
    public static String gameObject = "UnityWifiDirect";
    private UnityWifiDirect () {
        //its static
    }
    public static void initialize () {
        Log.i(TAG, "initializing");
        unityActivity = UnityPlayer.currentActivity;
        Log.i(TAG, unityActivity.getLocalClassName());
        //register broadcast receiver
        IntentFilter filter = new IntentFilter();
        filter.addAction(WifiDirectHandler.Action.SERVICE_CONNECTED);
        filter.addAction(WifiDirectHandler.Action.MESSAGE_RECEIVED);
        filter.addAction(WifiDirectHandler.Action.DEVICE_CHANGED);
        filter.addAction(WifiDirectHandler.Action.WIFI_STATE_CHANGED);
        filter.addAction(WifiDirectHandler.Action.DNS_SD_TXT_RECORD_AVAILABLE);
        filter.addAction(WifiDirectHandler.Action.DNS_SD_SERVICE_AVAILABLE);
        LocalBroadcastManager.getInstance(unityActivity).registerReceiver(broadcastReceiver, filter);
        Log.i(TAG, "Broadcast receiver registered");
        //bind service
        Intent intent = new Intent(unityActivity, WifiDirectHandler.class);
        boolean bound = unityActivity.bindService(intent, wifiServiceConnection, Context.BIND_AUTO_CREATE);
        Log.i(TAG, "bound: "+bound);
    }
    public static void initialize(String go) {
        initialize();
        gameObject = go;
    }
    public static void terminate () {
        if(wifiDirectHandlerBound) {
            unityActivity.unbindService(wifiServiceConnection);
            wifiDirectHandlerBound = false;
            Log.i(TAG, "unbound");
        }
    }
    public static void broadcastService(String serviceName, HashMap<String, String> records) {
        Log.i(TAG, "broadcasting service: "+serviceName);
        wifiDirectHandler.addLocalService(serviceName, records);
    }
    public static void discoverServices () {
        wifiDirectHandler.continuouslyDiscoverServices();
        Log.i(TAG, "discovering services");
    }
    public static void stopDiscovering () {
        wifiDirectHandler.stopServiceDiscovery();
        Log.i(TAG, "discovery stopped");
    }
    public static void connectToService (String address) {
        wifiDirectHandler.initiateConnectToService(wifiDirectHandler.getDnsSdServiceMap().get(address));
        Log.i(TAG, "initiating connection to "+address);
    }
    public static void sendMessage (String msg) {
        try {
            wifiDirectHandler.getCommunicationManager().write(msg.getBytes("UTF-16"));
        }
        catch (Exception e) { //because there's gonna be that one fool who doesn't support UTF-16

        }
    }
    //anonymous classes
    private static ServiceConnection wifiServiceConnection = new ServiceConnection() {

        @Override
        public void onServiceConnected(ComponentName name, IBinder service) {
            Log.i(TAG, "Binding WifiDirectHandler service");
            Log.i(TAG, "ComponentName: " + name);
            Log.i(TAG, "Service: " + service);
            WifiDirectHandler.WifiTesterBinder binder = (WifiDirectHandler.WifiTesterBinder) service;

            wifiDirectHandler = binder.getService();
            wifiDirectHandlerBound = true;
            Log.i(TAG, "WifiDirectHandler service bound");

            UnityPlayer.UnitySendMessage(gameObject, "onServiceConnected","");
        }

        @Override
        public void onServiceDisconnected(ComponentName name) {
            wifiDirectHandlerBound = false;
            Log.i(TAG, "WifiDirectHandler service unbound");
            UnityPlayer.UnitySendMessage(gameObject, "onServiceDisconnected","");
        }
    };
    private static BroadcastReceiver broadcastReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive (Context context, Intent intent) {
            switch(intent.getAction()) {
                case WifiDirectHandler.Action.DNS_SD_SERVICE_AVAILABLE:
                    String serviceAddress = intent.getStringExtra(WifiDirectHandler.SERVICE_MAP_KEY);
                    Log.i(TAG, "device found @ address "+serviceAddress);
                    UnityPlayer.UnitySendMessage(gameObject, "onServiceFound", serviceAddress);
                    break;
                case WifiDirectHandler.Action.DNS_SD_TXT_RECORD_AVAILABLE:
                    String txtAddress = intent.getStringExtra(WifiDirectHandler.TXT_MAP_KEY);
                    Map<String, String> recordMap = wifiDirectHandler.getDnsSdTxtRecordMap().get(txtAddress).getRecord();
                    //encode into base64
                    /*
                    addr_key1?val1_key2?val2_
                    addr, keys, and values are all base64 so you can put any string through
                     */
                    StringBuilder encoded = new StringBuilder();
                    try {
                        encoded.append(Base64.encodeToString(txtAddress.getBytes("UTF-16"), Base64.DEFAULT) + "_");
                        for(Map.Entry<String, String> entry : recordMap.entrySet()) {
                            encoded.append(Base64.encodeToString(entry.getKey().getBytes("UTF-16"), Base64.DEFAULT) + "?");
                            encoded.append(Base64.encodeToString(entry.getValue().getBytes("UTF-16"), Base64.DEFAULT) + "_");
                        }
                    } catch (UnsupportedEncodingException e) {
                        e.printStackTrace();
                    }
                    String result = encoded.toString();
                    Log.i(TAG, "device found with text record, formatted string: "+result);
                    UnityPlayer.UnitySendMessage(gameObject, "onUglyTxtRecord", result);
                    break;
                case WifiDirectHandler.Action.SERVICE_CONNECTED:
                    Log.i(TAG, "Connection made!");
                    UnityPlayer.UnitySendMessage(gameObject, "onConnect", "");
                    break;
                case WifiDirectHandler.Action.MESSAGE_RECEIVED:
                    try {
                        String msg = new String(intent.getByteArrayExtra(WifiDirectHandler.MESSAGE_KEY), "UTF-16");
                        Log.i(TAG, "Message received: "+msg);
                        UnityPlayer.UnitySendMessage(gameObject, "onMessage", msg);
                    } catch (Exception e) {}
                    break;
                default:
                    break;
            }
        }
    };
}
