# VR-Language-Learning-Project
A VR app that can help users learn languages. This was a key part of my final year project

## To view scripts

Scripts that have been written by me can be found in the /Scripts/ folder. These are also in the Unity Project folder (under /Assets/Scripts/), but have been copied into this folder for convenience.

Summary of each script's function:

- Master / Shared. This script stores variables for use by other scripts. This script also holds the Shared class, which holds functions that are used by other scripts.
- Setting scripts. These are called when a user changes a setting. They change the setting values in the Master script. They are:
  - guidedLearningMenuScript
  - audioOnlyMenuScript
  - audioVolumeMenuScript
  - translationsMenuScript
  - audioMenuScript
- Call UI scripts. These scripts call their respective UIs. They are
  - callTextbox. This also gets the text that is needed.
  - callCharacterUI. This calls a function in characterUIScript that sets up the content of the UI.
- Translation scripts. These scripts are called when the user's cursor hovers over a textbox and displays the translations. They are:
  - textBoxScript
  - CharacterTextScript
- Glass. This makes the panels of 'glass' in the window transparent.
- Teleport. This is used to make the user teleport using the VRTeleporter library.
- openMenu. This is called when the menu button is pressed and this displays the menu panel.
- downloadRooms. This is called on startup of the application to download the necessary data about the rooms.
- CharacterUIScript. This handles selecting the conversation, displaying the question/answers, and reacting when an answer is pressed.
- CharacterButtonScript. This is called when an answer is selected. It relays this information back to CharacterUIScript.
- ChangeScene. This is called when the user selects a room to go to in the Menu Scene. This changes the scene to that scene.
- CloseUI. This is called when the user presses the close button and closes the specified UI.
- exitToMenuScript. This is called when the user presses the exit to menu button on the settings menu. This returns the user to the Menu Scene.
- OnStartDeactive. This is run when the item first starts up, and deactivates it until it is needed. This is done on the Character UI and the textbox objects.
- 'get' PHP scripts. These are used to download data from the web database to be put in the local database. They are
  - getSettings
  - getUserID
  - getRoomData
  - getRooms
- 'insert' PHP scripts. These are used to insert new log files into the web database. They are
  - insertInteraction
  - insertInteractionSettings
  - insertInteractionScene

## To open and run in Unity

Once Unity Hub is open, if you 'Add' a new project, selecting the /Unity Project/ folder will open my project. Some asset packs were uninstalled in order for this zip file to be compressed into a file under 200MB. Before the project can be run correctly, these asset packs must be reinstalled. These packs are:

- [Door Asset Pack](https://assetstore.unity.com/packages/3d/props/interior/door-free-pack-aferar-148411)
- [Wallpaper Asset Pack](https://assetstore.unity.com/packages/2d/textures-materials/brick/18-high-resolution-wall-textures-12567)
- [Floor Asset Pack](https://assetstore.unity.com/packages/2d/textures-materials/wood/wooden-floor-materials-150564)
- GoogleVR Unity Package - can be installed by following the documentation [here](https://developers.google.com/vr/develop/unity/get-started-android)
- Microsoft Azure Speech SDK - can be installed by following the documentation [here](https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/quickstarts/setup-platform?tabs=unity%2Cwindows%2Cjre&pivots=programming-language-csharp)

To run the app inside the editor, you'll need to navigate in the Project window to /Assets/Scenes/ and click on MenuScene. If you are connected to the University network (directly or via VPN), once you press play, it should load up. You may hold Alt while the Game window is in focus and this enables the user to move their mouse to control the player.

## To run app on an Android phone

The app can be installed through Unity - once the project is open as specified above, if you go to 'Build and Run' with the phone plugged into the computer via USB and connected to the University network, it will build the app again and run this automatically on the phone.

Alternatively, you may only 'Build' the app to create an .apk file, and then install it to a phone by navigating to the approriate folder and run the following script using command line:

> adb install <buildName>.apk

This will install it to the phone, which can then be opened.

A successful build file was initially planned to be included with this additional material but was too large to fit in the 200MB limit.
