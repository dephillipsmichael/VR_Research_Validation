# VR_Research_Validation
A Unity VR project using the Pico Neo 2 Eye for running research studies with Tobii's eye tracking tech.


To run the project in Unity, drag the UI_Trigger_Menu scene or the Underwater_FX scene into your main scene. Then enable it in build settings as the scene to run.

This app only deploys to Android or the unity editor currently.

If you run into the issue with a mistmatched gradle version:
You will need to check the box "Export project" in build settings for android, and run the app from Android Studio. 
Once the project is opened in Android Studio, you need to update your gradle wrapper to 6.7.1 for the project to run.
