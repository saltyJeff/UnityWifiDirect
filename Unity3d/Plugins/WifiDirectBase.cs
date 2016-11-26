using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
#if UNITY_ANDROID
public class WifiDirectBase : MonoBehaviour {
	public static AndroidJavaObject wifiDirect = null;
	// Use this for initialization
	public void initialize (string gameObjectName) {
		if (wifiDirect == null) {
			wifiDirect = new AndroidJavaObject ("com.gmail.jeffersonlee2000.unitywifidirect.UnityWifiDirect");
			wifiDirect.CallStatic ("initialize", gameObjectName);
		}
	}
	public void terminate () {
		if (wifiDirect != null) {
			wifiDirect.CallStatic ("terminate");
		}
	}
	public void broadcastService(string service, Dictionary<string, string> record) {
		using(AndroidJavaObject hashMap = new AndroidJavaObject("java.util.HashMap"))
		{
			foreach(KeyValuePair<string, string> kvp in record)
			{
				hashMap.Call ("put", kvp.Key, kvp.Value);
			}
			wifiDirect.CallStatic ("broadcastService", service, hashMap);
		}
	}
	public void discoverServices () {
		wifiDirect.CallStatic ("discoverServices");
	}
	public void stopDiscovering () {
		wifiDirect.CallStatic ("stopDiscovering");
	}
	public void connectToService (string addr) {
		wifiDirect.CallStatic ("connectToService", addr);
	}
	public void sendMessage (string msg) {
		wifiDirect.CallStatic ("sendMessage", msg);
	}
	public bool isReady () {
		return wifiDirect.GetStatic<bool> ("wifiDirectHandlerBound");
	}
	//events
	public virtual void onServiceConnected () {
		Debug.Log ("service is legit");
	}
	public virtual void onServiceDisconnected () {
		Debug.Log ("service failed");
	}
	public virtual void onServiceFound (string addr) {

	}
	public void onUglyTxtRecord (string uglyRecord) {
		int addrSplitAddress = uglyRecord.IndexOf ('&');
		string addr = uglyRecord.Substring (0, addrSplitAddress);
		string temp = uglyRecord.Substring (addrSplitAddress);
		int splitIndex = temp.IndexOf ('&');
		Dictionary<string, string> record = new Dictionary<string, string> ();
		while (splitIndex > 0) {
			int eqIndex = temp.IndexOf ('=');
			string key = temp.Substring (1, eqIndex);
			splitIndex = temp.IndexOf ('&');
			string value;
			if (splitIndex < 0) {
				value = temp.Substring (eqIndex + 1);

			} 
			else {
				value = temp.Substring (eqIndex + 1, splitIndex);
			}
			record.Add (key, value);
		}
		onTxtRecord (addr, record);
	}
	public virtual void onTxtRecord(string addr, Dictionary<string, string> record) {
		
	}
	public virtual void onConnect () {

	}
	public virtual void onMessage () {

	}
	void OnApplicationPause () {
		terminate ();
	}
}
#endif