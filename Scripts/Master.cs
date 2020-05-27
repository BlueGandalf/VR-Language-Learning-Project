using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Networking;
using testingFYP;
using System.IO;
using System;
using System.Text;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class Master : MonoBehaviour
{

    public string voice;
    public int roomID;
    public int userID;

    public bool translationsOn;
    public bool audioOn;
    public bool guidedLearningOn;
    public bool audioOnlyOn;
    public int audioVolume;

    // Start is called before the first frame update
    void Start()
    {
        //Load User ID
        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/getUserID.php?UserName=" + SystemInfo.deviceUniqueIdentifier.ToString();

        

        using (UnityWebRequest webRequest = UnityWebRequest.Get(objects_url))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SendWebRequest();
            //yield return 
            if (webRequest.isNetworkError)
            {
                Debug.Log("There was an error:" + webRequest.error);
            }
            else
            {
                while (!webRequest.downloadHandler.isDone)
                {

                }
                Debug.Log("UserID: " + webRequest.downloadHandler.text);
                //when download is done, write the userID.
                userID = int.Parse(webRequest.downloadHandler.text);
            }
        }

        //set default values of settings.
        translationsOn = true;
        audioOn = true;
        guidedLearningOn = false;
        audioOnlyOn = false;
        audioVolume = 35;

        //get pre-set settings
        getSettings();
    }

    //this function gets the last settings that the user set, and then sets the settings to that.
    private void getSettings()
    {
        Debug.Log("setting update start");
        //gets the last known settings of the current user and room from a php script.
        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/getSettings.php?RoomID=" + roomID + "&UserID=" + userID;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(objects_url))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("There was an error:" + webRequest.error);
            }
            else
            {
                while (!webRequest.downloadHandler.isDone)
                {

                }
                Debug.Log(webRequest.downloadHandler.text);
                try
                {
                    //split output into array, which inform the settings.
                    string[] settings = webRequest.downloadHandler.text.Split(',');//translations,audio,audioonly,guidedlearning,volume
                    
                    translationsOn = Boolean.Parse(settings[1]);
                    audioOn = Boolean.Parse(settings[2]);
                    audioOnlyOn = Boolean.Parse(settings[3]);
                    guidedLearningOn = Boolean.Parse(settings[4]);
                    audioVolume = int.Parse(settings[5]);


                }
                catch
                {

                }
            }
        }
    }

}

//This class is a central repository of repeated functions
public class Shared
{
    GameObject master;
    Master masterScript;

    //each Shared function gets reference to the Master script. It uses this to reference the Master script's global parameters. 
    public Shared()
    {
        master = GameObject.Find("Master");
        masterScript = master.GetComponent<Master>();
    }

    //this function runs a given query to the local database that returns one value, like an ID or a string of text. 
    public string getValueFromDatabase(string query)
    {
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        string text = "";

        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();
        
        IDbCommand cmnd = dbCon.CreateCommand();
        cmnd.CommandText = query;
        IDataReader reader = cmnd.ExecuteReader();
        while (reader.Read())
        {
            Debug.Log(reader[0].ToString());
            text = reader[0].ToString();
        }
        dbCon.Close();

        return text;
    }

    //this function takes text and removes characters that are unwanted.
    public string cleanText(string text)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in text)
        {
            if (c == '-')
            {
                sb.Append(',');
            }
            else if (c == '#')
            {
                sb.Append('-');
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    //this function takes a conversation and plays that audio. conversationID is needed to save the audio down in an identifiable way.
    public void playConversationAudio(int conversationID, GameObject gameObject, string text)
    {
        string path = @"Audio/conv" + conversationID.ToString() + "Audio" + masterScript.roomID.ToString();
        string filePath = @"Assets/Resources/" + path + ".wav";
        if (File.Exists(filePath)){
            Debug.Log("File exists: " + filePath);
            playExistingAudio(path, gameObject);
        }
        else
        {
            Debug.Log("File doesn't exist: " + filePath);
            getAudio(gameObject, text, filePath);
        }
    }

    //this function takes an item and plays that audio. itemID is needed to save the audio down in an identifiable way.
    public void playItemAudio(int itemID, string text, GameObject gameObject)
    {
        string path = @"Audio/Item" + itemID.ToString() + "Audio" + masterScript.roomID;
        string filePath = @"Assets/Resources/" + path + ".wav";
        if (File.Exists(filePath))
        {
            Debug.Log("File exists: " + filePath);
            playExistingAudio(path, gameObject);
        }
        else
        {
            Debug.Log("File doesn't exist: " + filePath);
            getAudio(gameObject, text, filePath);
        }
    }

    //this function takes a filepath and if the audio already exists, it plays this audio. 
    private void playExistingAudio(string path, GameObject gameObject)
    {
        if (gameObject.GetComponent<AudioSource>() == null)
        {
            AudioSource newAudioSource = new AudioSource();
            gameObject.AddComponent(typeof(AudioSource));
        }
        AudioClip audioClip;
        try
        {
            audioClip = (AudioClip)Resources.Load(path);
            audioClip.LoadAudioData();
            gameObject.GetComponent<AudioSource>().clip = audioClip;
            gameObject.GetComponent<AudioSource>().volume = (float)masterScript.audioVolume / 100;


        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        if (gameObject.GetComponent<AudioSource>().isPlaying)
        {
            gameObject.GetComponent<AudioSource>().Pause();
        }
        else
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    //if there is no pre-existing audio, then this function streams the audio, and then downloads it for future use.
    private void getAudio(GameObject gameObject, string text, string filePath)
    {
        streamAudio(gameObject, text);
        downloadAudio(text, filePath);
    }

    //this function streams the audio
    private void streamAudio(GameObject gameObject, string text)
    {
        SpeechConfig speechConfig;
        SpeechSynthesizer synthesizer;

        // Creates an instance of a speech config with specified subscription key and service region.
        speechConfig = SpeechConfig.FromSubscription("c5ab91b760b24599b3667791c08aa7d9", "uksouth");

        // The default format is Riff16Khz16BitMonoPcm.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to Raw16Khz16BitMonoPcm.
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoPcm);

        // Creates a speech synthesizer.
        using (synthesizer = new SpeechSynthesizer(speechConfig, null))
        {
            text = cleanText(text);

            //this string defines the voice and text of what will be spoken.
            string ssml = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'><voice name='" + masterScript.voice + "'>" + text + "</voice></speak>";

            // Starts speech synthesis, and returns after a single utterance is synthesized.
            using (var result = synthesizer.SpeakSsmlAsync(ssml).Result)//synthesizer.SpeakTextAsync(getTextboxText()).Result
            {
                // Checks result
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Debug.Log("Streaming Audio");

                    var sampleCount = result.AudioData.Length / 2;
                    var audioData = new float[sampleCount];
                    for (var i = 0; i < sampleCount; ++i)
                    {
                        audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
                    }

                    // The output audio format is 16K 16bit mono
                    var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
                    audioClip.SetData(audioData, 0);

                    if (gameObject.GetComponent<AudioSource>() == null)
                    {
                        AudioSource newAudioSource = new AudioSource();
                        gameObject.AddComponent(typeof(AudioSource));
                    }

                    gameObject.GetComponent<AudioSource>().clip = audioClip;
                    gameObject.GetComponent<AudioSource>().volume = (float) masterScript.audioVolume / 100;

                    if (gameObject.GetComponent<AudioSource>().isPlaying)
                    {
                        gameObject.GetComponent<AudioSource>().Pause();
                    }
                    else
                    {
                        gameObject.GetComponent<AudioSource>().Play();
                    }
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Debug.Log(cancellation.Reason);
                }
            }
        }
    }

    //this function downlaods the audio to a specified filepath.
    private void downloadAudio(string text, string filePath)
    {
        SpeechConfig speechConfig;
        SpeechSynthesizer synthesizer;

        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        speechConfig = SpeechConfig.FromSubscription("c5ab91b760b24599b3667791c08aa7d9", "uksouth");

        AudioConfig audioConfig = AudioConfig.FromWavFileOutput(filePath);

        //string temp = getQuestionText(conversationID);

        // Creates a speech synthesizer.
        // Make sure to dispose the synthesizer after use!
        using (synthesizer = new SpeechSynthesizer(speechConfig, audioConfig))
        {

            text = cleanText(text);

            // Starts speech synthesis, and returns after a single utterance is synthesized.
            string ssml = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'><voice name='" + masterScript.voice + "'>" + text + "</voice></speak>";

            using (var result = synthesizer.SpeakSsmlAsync(ssml).Result)
            {
                // Checks result.
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Debug.Log("Downloaded Audio");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Debug.Log(cancellation.Reason);
                }
            }

        }
    }

    //this file takes an interaction and logs it. 
    public void log(int interactionTypeID, int itemID, int conversationID, int isCorrect)
    {
        Debug.Log("Shared log start");
        //log to the local database
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        string interactionID = "";
        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();
        
        IDbCommand cmnd_insert = dbCon.CreateCommand();
        //insert new log into database
        string query = "INSERT INTO TblInteraction(InteractionTypeID, ConversationID, ItemID, RoomID, UserID, Correct) VALUES(" + interactionTypeID + "," + conversationID + "," + itemID + "," + masterScript.roomID + "," + masterScript.userID + "," + isCorrect + "); ";
        cmnd_insert.CommandText = query;
        cmnd_insert.ExecuteNonQuery();

        IDbCommand cmnd_getID = dbCon.CreateCommand();
        //select that log to test that it worked.
        query = "SELECT * FROM TblInteraction ORDER BY InteractionID DESC LIMIT 1;";
        cmnd_getID.CommandText = query;
            
        IDataReader reader = cmnd_getID.ExecuteReader();
        while (reader.Read())
        {
            interactionID = reader[0].ToString();
            Debug.Log(reader[0].ToString() + ',' + reader[1].ToString() + ',' + reader[2].ToString() + ',' + reader[3].ToString() + ',' + reader[4].ToString() + ',' + reader[5].ToString() + ',' + reader[6].ToString() + ',' + reader[7].ToString());
        }
        dbCon.Close();

        Debug.Log("online log starts");
        //log to the online database
        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/insertInteraction.php?";
        objects_url += "interactionTypeID=" + interactionTypeID + "&";
        objects_url += "itemID=" + itemID + "&";
        objects_url += "conversationID=" + conversationID + "&";
        objects_url += "roomID=" + masterScript.roomID + "&";
        objects_url += "userID=" + masterScript.userID + "&";
        objects_url += "correct=" + isCorrect;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(objects_url))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("There was an error:" + webRequest.error);
            }
            else
            {
                while (!webRequest.downloadHandler.isDone)
                {

                }
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
        Debug.Log("Log finished");
    }

    //this function is used to log any changes to the settings. this is a separate function because it inserts a log for all the settings. (for convenience)
    public void logSettings()
    {
        //local log
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        string interactionID = "";
        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();

        IDbCommand cmnd_insert = dbCon.CreateCommand();
        string query = "";
        //insert 5 records for each setting.
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'Translations','" + masterScript.translationsOn.ToString() + "'); ";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'Audio','" + masterScript.audioOn.ToString() + "'); ";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'AudioOnly','" + masterScript.audioOnlyOn.ToString() + "'); ";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'GuidedLearning','" + masterScript.guidedLearningOn.ToString() + "'); ";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'Volume','" + masterScript.audioVolume.ToString() + "'); ";

        cmnd_insert.CommandText = query;
        cmnd_insert.ExecuteNonQuery();

        //select them back to check that it worked.
        IDbCommand cmnd_getID = dbCon.CreateCommand();
        query = "SELECT * FROM TblInteraction ORDER BY InteractionID DESC LIMIT 5;";
        cmnd_getID.CommandText = query;

        IDataReader reader = cmnd_getID.ExecuteReader();
        while (reader.Read())
        {
            interactionID = reader[0].ToString();
            Debug.Log(reader[0].ToString() + ',' + reader[1].ToString() + ',' + reader[2].ToString() + ',' + reader[3].ToString() + ',' + reader[4].ToString() + ',' + reader[5].ToString() + ',' + reader[6].ToString() + ',' + reader[7].ToString() + ',' + reader[8].ToString() + ',' + reader[9].ToString() + ',' + reader[10].ToString());
        }
        dbCon.Close();

        //log to web database using php script
        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/insertInteractionSettings.php?";
        objects_url += "interactionTypeID=" + 4 + "&";
        objects_url += "roomID=" + masterScript.roomID + "&";
        objects_url += "userID=" + masterScript.userID + "&";
        objects_url += "settingName1=" + "Translations" + "&";
        objects_url += "settingValue1=" + masterScript.translationsOn + "&";
        objects_url += "settingName2=" + "Audio" + "&";
        objects_url += "settingValue2=" + masterScript.audioOn + "&";
        objects_url += "settingName3=" + "AudioOnly" + "&";
        objects_url += "settingValue3=" + masterScript.audioOnlyOn + "&";
        objects_url += "settingName4=" + "GuidedLearning" + "&";
        objects_url += "settingValue4=" + masterScript.guidedLearningOn + "&";
        objects_url += "settingName5=" + "Volume" + "&";
        objects_url += "settingValue5=" + masterScript.audioVolume;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(objects_url))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("There was an error:" + webRequest.error);
            }
            else
            {
                while (!webRequest.downloadHandler.isDone)
                {

                }
                Debug.Log(webRequest.downloadHandler.text);
            }
        }

    }

    //this function is used to log any scene interactions.
    public void logScene(int roomID, int isEntering)
    {
        //local log
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        string interactionID = "";
        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();

        IDbCommand cmnd_insert = dbCon.CreateCommand();
        string query = "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, isEntering) VALUES ( 3," + roomID + "," + masterScript.userID + ","+ isEntering + "); ";
        
        cmnd_insert.CommandText = query;
        cmnd_insert.ExecuteNonQuery();

        IDbCommand cmnd_getID = dbCon.CreateCommand();
        query = "SELECT * FROM TblInteraction ORDER BY InteractionID DESC LIMIT 5;";
        cmnd_getID.CommandText = query;

        IDataReader reader = cmnd_getID.ExecuteReader();
        while (reader.Read())
        {
            interactionID = reader[0].ToString();
            Debug.Log(reader[0].ToString() + ',' + reader[1].ToString() + ',' + reader[2].ToString() + ',' + reader[3].ToString() + ',' + reader[4].ToString() + ',' + reader[5].ToString() + ',' + reader[6].ToString() + ',' + reader[7].ToString());
        }
        dbCon.Close();

        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/insertInteractionScene.php?";
        objects_url += "interactionTypeID=" + 3 + "&";
        objects_url += "roomID=" + masterScript.roomID + "&";
        objects_url += "userID=" + masterScript.userID + "&";
        objects_url += "isEntering=" + isEntering;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(objects_url))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log("There was an error:" + webRequest.error);
            }
            else
            {
                while (!webRequest.downloadHandler.isDone)
                {

                }
                Debug.Log(webRequest.downloadHandler.text);
            }
        }

    }








}
