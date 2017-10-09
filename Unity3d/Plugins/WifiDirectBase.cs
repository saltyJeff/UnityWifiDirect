using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System;
#if UNITY_ANDROID
/// <summary>
/// The base class of the library
/// </summary>
/// <remarks>
/// Either use this class, or a class that derives from this to have all the powers of the library
/// </remarks>
public class WifiDirectBase : MonoBehaviour {
	private static AndroidJavaObject wifiDirect = null;
	/// <summary>
	/// Intializes the library, should only be called once.
	/// </summary>
	/// <param name="gameObjectName">
	/// Name of the GameObject that will control the Wifi Direct and recieve all the events
	/// </param> 
	public void initialize (string gameObjectName) {
		if (wifiDirect == null) {
			wifiDirect = new AndroidJavaObject ("com.gmail.jeffersonlee2000.unitywifidirect.UnityWifiDirect");
			wifiDirect.CallStatic ("initialize", gameObjectName);
		}
	}
	/// <summary>
	/// Terminates the library (use to gracefully exit)
	/// </summary> 
	public void terminate () {
		if (wifiDirect != null) {
			wifiDirect.CallStatic ("terminate");
		}
	}
	/// <summary>
	/// Broadcasts a service for other devices to discover.
	/// </summary>
	/// <param name="service">
	/// The name that will be on the broadcasted service
	/// </param>
	/// <param name="record">
	/// The key value pairs to send along with the service
	/// </param>
	public void broadcastService(string service, Dictionary<string, string> record) {
		using(AndroidJavaObject hashMap = new AndroidJavaObject("java.util.HashMap"))
		{
			foreach(KeyValuePair<string, string> kvp in record)
			{
				hashMap.Call<string> ("put", kvp.Key, kvp.Value);
			}
			wifiDirect.CallStatic ("broadcastService", service, hashMap);
		}
	}
	/// <summary>
	/// Search for services (no timeout)
	/// </summary>
	public void discoverServices () {
		wifiDirect.CallStatic ("discoverServices");
	}
	/// <summary>
	/// Stops searching for services
	/// </summary>
	public void stopDiscovering () {
		wifiDirect.CallStatic ("stopDiscovering");
	}
	/// <summary>
	/// Connects to a service
	/// </summary>
	/// <param name="addr">
	/// The address to connect to
	/// </param>
	public void connectToService (string addr) {
		wifiDirect.CallStatic ("connectToService", addr);
	}
	/// <summary>
	/// Sends a message to the connected device
	/// </summary>
	/// <param name="msg">
	/// The message string to send
	/// </param>
	public void sendMessage (string msg) {
		wifiDirect.CallStatic ("sendMessage", msg);
	}

	/// <summary>
	/// Returns if the library is ready
	/// </summary>
	/// <remarks>
	/// set to true by onServiceConnected() and set to false by terminate()
	/// </remarks>
	/// <returns>
	/// a bool stating if the library is ready
	/// </returns>
	public bool isReady () {
		return wifiDirect.GetStatic<bool> ("wifiDirectHandlerBound");
	}
	//events
	/// <summary>
	/// Called when the library is ready
	/// </summary>
	public virtual void onServiceConnected () {
		Debug.Log ("service is legit");
	}
	/// <summary>
	/// Called when the library's backend is shutdown
	/// </summary>
	public virtual void onServiceDisconnected () {
		Debug.Log ("service failed");
	}
	/// <summary>
	/// Called when a service without text records has been found
	/// </summary>
	/// <param name="addr">
	/// the address of the service
	/// </param>
	public virtual void onServiceFound (string addr) {

	}
	/// <summary>
	/// Called when a service with text records has been found (includes deserializer)
	/// </summary>
	/// <remarks>
	/// Don't override this, override the onTxtRecord() method because the deserializer is necessary
	/// </remarks>
	/// <param name="uglyRecord">
	/// The deserialized text reocrds
	/// </param>
	public void onUglyTxtRecord (string uglyRecord) {
		int addrSplitAddress = uglyRecord.IndexOf ('_');
		string addrEncoded = uglyRecord.Substring (0, addrSplitAddress);
		string addr = Encoding.Unicode.GetString(Convert.FromBase64String(addrEncoded));
		string remaining = uglyRecord.Substring (addrSplitAddress+1);
		int splitIndex = remaining.IndexOf ('_');
		Dictionary<string, string> record = new Dictionary<string, string> ();
		while (splitIndex > 0 && remaining.Length > 0) {
			int eqIndex = remaining.IndexOf ('?');
			string key = remaining.Substring (0, eqIndex);
			splitIndex = remaining.IndexOf ('_');
			string value = remaining.Substring (eqIndex + 1, splitIndex-eqIndex-1);
			remaining = remaining.Substring (splitIndex + 1);
			record.Add (Encoding.Unicode.GetString(Convert.FromBase64String(key)), Encoding.Unicode.GetString(Convert.FromBase64String(value)));
		}
		Debug.Log("ugly record found");
		this.onTxtRecord (addr, record);
	}
	/// <summary>
	/// Called when a service with text record is found (deserialized already)
	/// </summary>
	/// <param name="addr">
	/// The address of the service
	/// </param>
	/// <param name="record">
	/// The key value pairs of the text record
	/// </param>
	public virtual void onTxtRecord(string addr, Dictionary<string, string> record) {
		
	}
	/// <summary>
	/// Called when connected to a client
	/// </summary>
	public virtual void onConnect () {
		
	}
	/// <summary>
	/// Called when the other device has sent a message.
	/// </summary>
	/// <param name="message">
	/// The message sent
	/// </param>
	public virtual void onMessage (string message) {

	}
}
#endif
