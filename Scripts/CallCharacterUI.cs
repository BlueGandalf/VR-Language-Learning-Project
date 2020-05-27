using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Networking;

public class CallCharacterUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //This function finds the character container and activates the question and answer panels (leaving the menu panel deactivated) and also activates the close and menu buttons, as well as the guided learning arrow if needed.
    public void ShowTextbox()
    {
        
        GameObject characterUIContainer = GameObject.Find("CharacterUI Container");
        
        characterUIContainer.transform.GetChild(0).gameObject.SetActive(true); //This activates the UI canvas

        GameObject.Find("CharacterUI").transform.GetChild(0).gameObject.SetActive(true); //This activates the quesiton panel

        GameObject.Find("CharacterUI").transform.GetChild(1).gameObject.SetActive(true); //This activates the answer panel

        GameObject.Find("CharacterUI").transform.GetChild(2).gameObject.SetActive(false); //This activates the menu panel (set to false)

        if (GameObject.Find("Master").GetComponent<Master>().guidedLearningOn)
        {
            characterUIContainer.transform.GetChild(0).GetChild(3).gameObject.SetActive(true); //This activates the guided learning arrow if the setting is true
        }
        else //otherwise
        {
            characterUIContainer.transform.GetChild(0).GetChild(3).gameObject.SetActive(false); //This deactivates the arrow
        }

        characterUIContainer.transform.GetChild(0).gameObject.GetComponent<CharacterUIScript>().startConversation(); //this prompts the conversation to start.
    }


}
