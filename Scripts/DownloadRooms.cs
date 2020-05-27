using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Networking;

public class downloadRooms : MonoBehaviour
{

    public GameObject menuItem;
    private List<int> roomIDs;
    
    // Start is called before the first frame update
    void Start()
    {
        roomIDs = new List<int>();
        LoadDataFunction();
        DisplayRooms();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this function is used to download the room data on startup of the application.
    private void LoadDataFunction()
    {
        //Load Data from php script
        string objects_url = "http://cops.sci-project.lboro.ac.uk/FYP/getRooms.php";


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

                } // wait until download has finished.
                Debug.Log(webRequest.downloadHandler.text);
                try
                {
                    //split data into arrays
                    string[] databaseString = webRequest.downloadHandler.text.Split(';');//0 -> rooms, 1 -> languages
                    List<string[]> rooms = new List<string[]>();
                    List<string[]> languages = new List<string[]>();
                    string[] tempArray = databaseString[0].Split('~');
                    for (int i = 0; i < tempArray.Length; i++)//load them into arrays
                    {
                        rooms.Add(tempArray[i].Split(','));
                    }

                    tempArray = databaseString[1].Split('~');
                    for (int i = 0; i < tempArray.Length; i++)
                    {
                        languages.Add(tempArray[i].Split(','));
                    }

                    //insert data into sqlite if not exists
                    string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
                    IDbConnection dbCon = new SqliteConnection(connection);
                    dbCon.Open();
                    Debug.Log("12");
                    IDbCommand dbcmd;
                    IDataReader reader;

                    //create database tables and indexes if they don't already exist
                    dbcmd = dbCon.CreateCommand();
                    string q_createTables = 
                        @"
CREATE TABLE IF NOT EXISTS TblRoom (RoomID INTEGER PRIMARY KEY, RoomName VarChar(255), L1ID INTEGER, L2ID INTEGER);
CREATE TABLE IF NOT EXISTS TblLanguage (LanguageID INTEGER PRIMARY KEY, LanguageName VARCHAR(50), LanguageNameEN VARCHAR(50), L2ID INTEGER);
CREATE UNIQUE INDEX IF NOT EXISTS TblRoom_Unique on TblRoom (RoomID, RoomName, L1ID, L2ID);
CREATE UNIQUE INDEX IF NOT EXISTS TblLanguage_Unique on TblLanguage (LanguageID, LanguageName, LanguageNameEN);";

                    dbcmd.CommandText = q_createTables;
                    reader = dbcmd.ExecuteReader();
                    Debug.Log("11");

                    //insert the data if it isn't already in the database
                    string q_insertData = "";
                    for (int i = 0; i < rooms.Count; i++)
                    {
                        q_insertData += "INSERT OR IGNORE INTO TblRoom ( RoomID, RoomName, L1ID, L2ID ) VALUES (" + rooms[i][0] + ",'" + rooms[i][1] + "'," + rooms[i][2] + "," + rooms[i][3] + ");";
                    }
                    for (int i = 0; i < languages.Count; i++)
                    {
                        q_insertData += "INSERT OR IGNORE INTO TblLanguage ( LanguageID, LanguageName, LanguageNameEN ) VALUES (" + languages[i][0] + ",'" + languages[i][1] + "','" + languages[i][2] + "');";
                    }

                    IDbCommand cmnd = dbCon.CreateCommand();
                    cmnd.CommandText = q_insertData;
                    cmnd.ExecuteNonQuery();
                    Debug.Log("10");

                    IDbCommand cmnd_read_room = dbCon.CreateCommand();
                    string query = "SELECT * FROM TblRoom";
                    cmnd_read_room.CommandText = query;
                    reader = cmnd_read_room.ExecuteReader();
                    Debug.Log("7");
                    while (reader.Read())
                    {
                        Debug.Log("RoomID: " + reader[0].ToString() + "; RoomName: " + reader[1].ToString() + "; L1ID: " + reader[2].ToString() + "; L2ID: " + reader[3].ToString());
                    }
                    query = "SELECT * FROM TblLanguage";
                    IDbCommand cmnd_read_language = dbCon.CreateCommand();
                    cmnd_read_language.CommandText = query;
                    reader = cmnd_read_language.ExecuteReader();
                    Debug.Log("7");
                    while (reader.Read())
                    {
                        Debug.Log("LanguageID: " + reader[0].ToString() + "; LanguageName: " + reader[1].ToString() + "; LanguageNameEN: " + reader[2].ToString());
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

    //this functions is used to translate the rooms that are in the database into items on the available rooms list. 
    private void DisplayRooms()
    {
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();
        Debug.Log("12");
        IDataReader reader;
        IDbCommand dbcmd = dbCon.CreateCommand();
        //get data from the local database about the rooms.
        string query = "SELECT RoomName, L1.LanguageNameEN, L1.LanguageName, L2.LanguageNameEN, L2.LanguageName, RoomID FROM TblRoom JOIN TblLanguage L1 ON TblRoom.L1ID = L1.LanguageID JOIN TblLanguage L2 ON TblRoom.L2ID = L2.LanguageID;";
        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();
        Debug.Log("7");
        List<string[]> rooms = new List<string[]>();
        //load data into array
        while (reader.Read())
        {
            roomIDs.Add(int.Parse(reader[5].ToString()));
            Debug.Log("RoomName: " + reader[0].ToString() + "; From: " + reader[2].ToString() + " To: " + reader[4].ToString() + "; or From: " + reader[1].ToString() + " To: " + reader[3].ToString());
            string[] tempArray = { reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString() };
            rooms.Add(tempArray);
            //test
            //tempArray = new string[] { reader[0].ToString() + "1", reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString() };
            //rooms.Add(tempArray);
            //tempArray = new string[] { reader[0].ToString() + "2", reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString() };
            //rooms.Add(tempArray);
            //tempArray = new string[] { reader[0].ToString() + "3", reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString() };
            //rooms.Add(tempArray);
            //tempArray = new string[] { reader[0].ToString() + "4", reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString() };
            //rooms.Add(tempArray);

        }
        Debug.Log(menuItem.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text);
        menuItem.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Kitchen";
        menuItem.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "England " + '\u27A1' + "Japan";
        Debug.Log(menuItem.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text);
        menuItem.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "England " + '\u27A1' + "China";
        Debug.Log(menuItem.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text);

        //Instantiate(menuItem, gameObject.transform.GetChild(1).transform);

        menuItem.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Bedroom";
        //Instantiate(menuItem, gameObject.transform.GetChild(1).transform);

        //for each room, instantiate a menu item.
        if (rooms.Count <= 5)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                menuItem.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = rooms[i][0];
                menuItem.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = rooms[i][1] + " " + '\u27A1' + " " + rooms[i][3];// "England " + '\u27A1' + "Japan";
                GameObject temp = Instantiate(menuItem, gameObject.transform.GetChild(1).transform);
                temp.transform.localPosition = new Vector3(0f, 242f - (102f * i), 0f);
                temp.GetComponentInChildren<ChangeScene>().sceneName = "KitchenScene" + roomIDs[i].ToString();
                temp.GetComponentInChildren<ChangeScene>().roomID = roomIDs[i];

            }
        }
        else
        {

        }
        
    }

}
