# UnityWifiDirect
Adds Wifi Direct capability to Android Unity3d builds (working on windows version)
## Using the library
Look at the Unity3d/Plugins/WifiDirectBase.cs file for available methods
## Build Instructions
### Building the android library:
1. Directory ./Android is already a Android Studio Gradle project, just import it into Android Studio
2. Use the build button in the Android Studio menu bar (the gradle file has been pre-configured)
3. Go into the app-release.aar file in the app/build/outputs/aar/ folder
4. Open the aar with 7zip and delete the "res" folder
5. Open "lib" folder and delete "unity.jar"
6. Copy the android-support-v4.jar file from the Android SDK install location into 'libs'

