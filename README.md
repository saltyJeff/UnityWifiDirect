# UnityWifiDirect
Adds Wifi Direct capability to Android Unity3d builds (working on Windows version)

Provides a C# wrapper around the [Wifi-Buddy project](https://github.com/Crash-Test-Buddies/WiFi-Buddy)

## Using the library
Copy the Unity3d/Plugins folder into your Assets folder, and create a class that inherits from WifiDirectBase.cs

Look at the Unity3d/Documentation/html for Doxygen files with descriptions on what to inherit

Will put up example later

## Build Instructions
### Building the Android library:
1. Directory ./Android is already a Android Studio Gradle project, just import it into Android Studio
2. Use the build button in the Android Studio menu bar (the gradle file has been pre-configured)
3. Go into the "app-debug.aar" file in the app/build/outputs/aar/ folder
4. Open the aar with 7zip and delete the "res" folder
5. Open "lib" folder in the aar and delete "unity.jar"
6. Copy the android-support-v4.jar file from the Android SDK install location into "libs" inside the aar
7. Copy the .aar into (your Unity3d project)/Assets/Plugins/Android and rename it "UnityWifiDirect.aar", and make sure WifiDirectBase.cs is inside Assets/Plugins
8. Folow the instructions in the "Using the Library" section above

