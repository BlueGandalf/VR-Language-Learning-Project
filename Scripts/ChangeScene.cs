using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public int roomID;
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function changes the Scene from the MenuScene to the selected Room.
    public void SceneChange()
    {
        LoadDataFunction();

        //this logs the scene interaction using the Shared function.
        new Shared().logScene(roomID, 1);

        //This loads the scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    //This function loads all the necessary data to the loacl database from the web database
    private void LoadDataFunction()
    {
        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/getRoomData.php?RoomNumber=" + roomID.ToString();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(objects_url))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError)
            {
                Debug.Log("There was an error:" + webRequest.error);
            }
            else //if there was no error:
            {
                while (!webRequest.downloadHandler.isDone)
                {

                }//wait until the download is complete
                Debug.Log(webRequest.downloadHandler.text); //the returned string can be broken down into the usable bits of data
                try
                {
                    //split data into arrays
                    string[] databaseString = webRequest.downloadHandler.text.Split(';');//0 -> conversations, 1 -> items, 2 -> words, 3 -> answers
                    List<string[]> conversations = new List<string[]>();
                    string[] tempArray = databaseString[0].Split('~');
                    for (int i = 0; i < tempArray.Length; i++) //split the conversations string down further into its own array
                    {
                        conversations.Add(tempArray[i].Split(','));
                    }
                    List<string[]> items = new List<string[]>();
                    tempArray = databaseString[1].Split('~');
                    for (int i = 0; i < tempArray.Length; i++) //split the items string down further into its own array
                    {
                        items.Add(tempArray[i].Split(','));
                    }
                    List<string[]> words = new List<string[]>();
                    tempArray = databaseString[2].Split('~');
                    for (int i = 0; i < tempArray.Length; i++) //split the words string down further into its own array
                    {
                        words.Add(tempArray[i].Split(','));
                    }
                    List<string[]> answers = new List<string[]>();
                    tempArray = databaseString[3].Split('~');
                    for (int i = 0; i < tempArray.Length; i++) //split the answers string down further into its own array
                    {
                        answers.Add(tempArray[i].Split(','));
                    }

                    //insert data into sqlite if it doesn't already exist
                    string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
                    IDbConnection dbCon = new SqliteConnection(connection);
                    dbCon.Open();
                    Debug.Log("12");
                    IDbCommand dbcmd;
                    IDataReader reader;

                    //create the tables and indexes if they don't already exist
                    dbcmd = dbCon.CreateCommand();
                    string q_createTables = 
                        @" 
CREATE TABLE IF NOT EXISTS my_table (id INTEGER PRIMARY KEY, val INTEGER );
CREATE TABLE IF NOT EXISTS TblConversation (ConversationID INTEGER, WordLevel INTEGER, TextID INTEGER, CharacterID INTEGER, ItemID INTEGER, EasinessFactor REAL DEFAULT 2.5, Interval REAL, DateLastAnswered DATETIME );
CREATE TABLE IF NOT EXISTS TblAnswer (AnswerID INTEGER, ConversationID INTEGER, TextID INTEGER, CorrectAnswer INTEGER );
CREATE TABLE IF NOT EXISTS TblItem (ItemID INTEGER, ItemName VARCHAR(50), WordID INTEGER, RoomID INTEGER );
CREATE TABLE IF NOT EXISTS TblWord (WordID INTEGER, L1Text VARCHAR(255), L2Text VARCHAR(255), L2Audio VARCHAR(255), RoomID INTEGER );
CREATE TABLE IF NOT EXISTS TblInteraction (InteractionID INTEGER PRIMARY KEY, InteractionTypeID INTEGER, ItemID INTEGER, ConversationID INTEGER, RoomID INTEGER, DateTime DATETIME DEFAULT CURRENT_TIMESTAMP, UserID INTEGER, Correct BOOLEAN, SettingName VARCHAR(50), SettingValue VARCHAR(50), isEntering INTEGER );
CREATE TABLE IF NOT EXISTS TblConfiguration (ConfigurationID INTEGER PRIMARY KEY, ValueName VARCHAR(50), Value VARCHAR(255));
CREATE UNIQUE INDEX IF NOT EXISTS TblConvo_Unique on TblConversation (ConversationID, WordLevel, TextID, CharacterID, ItemID);
CREATE UNIQUE INDEX IF NOT EXISTS TblItem_Unique on TblItem (ItemID, ItemName, WordID, RoomID);
CREATE UNIQUE INDEX IF NOT EXISTS TblAnswer_Unique on TblAnswer (AnswerID, ConversationID, TextID, CorrectAnswer);
CREATE UNIQUE INDEX IF NOT EXISTS TblWord_Unique on TblWord (WordID, L1Text, L2Text, L2Audio, RoomID);";

                    dbcmd.CommandText = q_createTables;
                    Debug.Log("199");
                    Debug.Log(q_createTables);
                    reader = dbcmd.ExecuteReader();
                    Debug.Log("11");

                    string q_insertData = "";
                    for(int i = 0; i < conversations.Count; i++) //insert each conversation into the database
                    {
                        Debug.Log("conversations count: " + conversations.Count);
                        Debug.Log("conversation count: " + conversations[0].Length);
                        Debug.Log(conversations[0][0]);
                        q_insertData += "INSERT OR IGNORE INTO TblConversation ( ConversationID, WordLevel, TextID, CharacterID, ItemID ) VALUES (" + conversations[i][0] + "," + conversations[i][1] + "," + conversations[i][2] + "," + conversations[i][3] + "," + conversations[i][4] + ");";
                        Debug.Log("ConversationID: " + conversations[i][0] + "; WordLevel: " + conversations[i][1] + "; TextID: " + conversations[i][2] + "; CharacterID: " + conversations[i][3] + "; ItemID:" + conversations[i][4]);
                    }
                    for (int i = 0; i < items.Count; i++) //insert each item into the database
                    {
                        Debug.Log("2");
                        q_insertData += "INSERT OR IGNORE INTO TblItem ( ItemID, ItemName, WordID, RoomID ) VALUES (" + items[i][0] + ",'" + items[i][1] + "'," + items[i][2] + "," + items[i][3] + ");";
                    }
                    for (int i = 0; i < words.Count; i++) //insert each word into the database
                    {
                        Debug.Log("3");
                        q_insertData += "INSERT OR IGNORE INTO TblWord ( WordID, L1Text, L2Text, L2Audio, RoomID ) VALUES (" + words[i][0] + ",'" + words[i][1] + "','" + words[i][2] + "','" + words[i][3] + "'," + words[i][4] + ");";
                    }
                    for (int i = 0; i < answers.Count; i++) //insert each answer into the database
                    {
                        Debug.Log("4");
                        q_insertData += "INSERT OR IGNORE INTO TblAnswer ( AnswerID, ConversationID, TextID, CorrectAnswer ) VALUES (" + answers[i][0] + "," + answers[i][1] + "," + answers[i][2] + "," + answers[i][3] + ");";
                    }
                    for (int i = 0; i < answers.Count; i++)
                    {
                        Debug.Log("5");
                        Debug.Log(answers[i][0] + "," + answers[i][1] + "," + answers[i][2] + "," + answers[i][3]);
                    }
                    q_insertData += "INSERT INTO my_table (val) VALUES (9);";
                    //insert into the local database the MAC address, which is used as the unique Username.
                    q_insertData += "INSERT INTO TblConfiguration (ValueName, Value) VALUES (\"DeviceName\", \"" + SystemInfo.deviceUniqueIdentifier + "\");";

                    Debug.Log(SystemInfo.deviceUniqueIdentifier);

                    Debug.Log(q_insertData);
                    IDbCommand cmnd = dbCon.CreateCommand();
                    cmnd.CommandText = q_insertData;
                    cmnd.ExecuteNonQuery();
                    Debug.Log("10");



                    //Read the databases and log it for debug purposes.
                    IDbCommand cmnd_read_word = dbCon.CreateCommand();
                    string query = "SELECT * FROM TblWord";
                    cmnd_read_word.CommandText = query;
                    reader = cmnd_read_word.ExecuteReader();
                    Debug.Log("7");
                    while (reader.Read())
                    {
                        Debug.Log("WordID: " + reader[0].ToString() + "; L1Text: " + reader[1].ToString() + "; L2Text: " + reader[2].ToString() + "; L2Audio: " + reader[3].ToString() + "; RoomID:" + reader[4].ToString());
                    }
                    query = "SELECT * FROM TblItem";
                    IDbCommand cmnd_read_item = dbCon.CreateCommand();
                    cmnd_read_item.CommandText = query;
                    reader = cmnd_read_item.ExecuteReader();
                    Debug.Log("7");
                    while (reader.Read())
                    {
                        Debug.Log("ItemID: " + reader[0].ToString() + "; ItemName: " + reader[1].ToString() + "; WordID: " + reader[2].ToString() + "; RoomID: " + reader[3].ToString());
                    }
                    query = "SELECT * FROM TblConversation";
                    IDbCommand cmnd_read_convo = dbCon.CreateCommand();
                    cmnd_read_convo.CommandText = query;
                    reader = cmnd_read_convo.ExecuteReader();
                    Debug.Log("7");
                    while (reader.Read())
                    {
                        Debug.Log("ConversationID: " + reader[0].ToString() + "; WordLevel: " + reader[1].ToString() + "; TextID: " + reader[2].ToString() + "; CharacterID: " + reader[3].ToString() + "; ItemID:" + reader[4].ToString());
                    }



                    dbCon.Close();
                }
                catch (System.Exception e)
                {
                    print(e.Message);
                }
            }
        }
    }
}