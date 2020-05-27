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
        //Load Data
        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/getUserID.php?UserName=" + SystemInfo.deviceUniqueIdentifier.ToString();

        //WWW hs_post = new WWW(objects_url);
        //Debug.Log("18");
        //yield return hs_post;

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
                userID = int.Parse(webRequest.downloadHandler.text);
            }
        }


        translationsOn = true;
        audioOn = true;
        guidedLearningOn = false;
        audioOnlyOn = false;
        audioVolume = 35;

        getSettings();
    }

    private void getSettings()
    {
        Debug.Log("setting update start");
        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/getSettings.php?RoomID=" + roomID + "&UserID=" + userID;
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
                Debug.Log(webRequest.downloadHandler.text);
                try
                {
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

public class Shared
{
    GameObject master;
    Master masterScript;

    public Shared()
    {
        master = GameObject.Find("Master");
        masterScript = master.GetComponent<Master>();
    }

    public int getNumber()
    {
        return 5;
    }

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

    private void getAudio(GameObject gameObject, string text, string filePath)
    {
        streamAudio(gameObject, text);
        downloadAudio(text, filePath);
    }

    private void streamAudio(GameObject gameObject, string text)
    {
        SpeechConfig speechConfig;
        SpeechSynthesizer synthesizer;

        // Creates an instance of a speech config with specified subscription key and service region.
        // Replace with your own subscription key and service region (e.g., "westus").
        speechConfig = SpeechConfig.FromSubscription("c5ab91b760b24599b3667791c08aa7d9", "uksouth");

        // The default format is Riff16Khz16BitMonoPcm.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to Raw16Khz16BitMonoPcm.
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoPcm);

        // Creates a speech synthesizer.
        // Make sure to dispose the synthesizer after use!
        using (synthesizer = new SpeechSynthesizer(speechConfig, null))
        {
            text = cleanText(text);

            // Starts speech synthesis, and returns after a single utterance is synthesized.
            string ssml = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'><voice name='" + masterScript.voice + "'>" + text + "</voice></speak>";

            using (var result = synthesizer.SpeakSsmlAsync(ssml).Result)//synthesizer.SpeakTextAsync(getTextboxText()).Result
            {
                // Checks result
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Debug.Log("Streaming Audio");

                    // Native playback is not supported on Unity yet (currently only supported on Windows/Linux Desktop).
                    // Use the Unity API to play audio here as a short term solution.
                    // Native playback support will be added in the future release.

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

    public void log(int interactionTypeID, int itemID, int conversationID, int isCorrect)
    {
        Debug.Log("Shared log start");
        //local log
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        string interactionID = "";
        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();
        
        IDbCommand cmnd_insert = dbCon.CreateCommand();
        string query = "INSERT INTO TblInteraction(InteractionTypeID, ConversationID, ItemID, RoomID, UserID, Correct) VALUES(" + interactionTypeID + "," + conversationID + "," + itemID + "," + masterScript.roomID + "," + masterScript.userID + "," + isCorrect + "); ";
        cmnd_insert.CommandText = query;
        cmnd_insert.ExecuteNonQuery();

        IDbCommand cmnd_getID = dbCon.CreateCommand();
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
        //online log
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
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
        Debug.Log("Log finished");
    }

    public void logSettings()
    {

        //public bool translationsOn;
        //public bool audioOn;
        //public bool guidedLearningOn;
        //public bool audioOnlyOn;
        //public int audioVolume;



        //local log
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        string interactionID = "";
        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();

        IDbCommand cmnd_insert = dbCon.CreateCommand();
        string query = "";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'Translations','" + masterScript.translationsOn.ToString() + "'); ";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'Audio','" + masterScript.audioOn.ToString() + "'); ";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'AudioOnly','" + masterScript.audioOnlyOn.ToString() + "'); ";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'GuidedLearning','" + masterScript.guidedLearningOn.ToString() + "'); ";
        query += "INSERT INTO TblInteraction(InteractionTypeID, RoomID, UserID, SettingName, SettingValue ) VALUES ( 4," + masterScript.roomID + "," + masterScript.userID + ",'Volume','" + masterScript.audioVolume.ToString() + "'); ";

        cmnd_insert.CommandText = query;
        cmnd_insert.ExecuteNonQuery();

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
