# VR_Research_Validation
A Unity VR project using the Pico Neo 2 Eye for running research studies with Tobii's eye tracking tech.

To get the project to open, you need to download 2 local dependencies to your machine.
1) https://developer.pico-interactive.com/sdk/index?id=8 
2) https://vr.tobii.com/sdk/downloads/

Then, place them at the as siblings in the directory where you cloned this repo, looking like this:
![Screenshot (7)](https://user-images.githubusercontent.com/5590748/123523496-cbe2f380-d678-11eb-80d8-dc1e091b4ab9.png)

Add the project into Unity, and once it loads, drag the **Underwater_FX** scene into your main scene. Then enable it in build settings as the scene to run.

This app only deploys to Android or the unity editor currently.

If you run into the issue with a mistmatched gradle version:
You will need to check the box "Export project" in build settings for android, and run the app from Android Studio. 
Once the project is opened in Android Studio, you need to update your gradle wrapper to 6.7.1 for the project to run.
