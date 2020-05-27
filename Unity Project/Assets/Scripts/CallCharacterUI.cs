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

    public void ShowTextbox()
    {
        
        GameObject characterUIContainer = GameObject.Find("CharacterUI Container");
        characterUIContainer.transform.GetChild(0).gameObject.SetActive(true);

        GameObject.Find("CharacterUI").transform.GetChild(0).gameObject.SetActive(true);

        GameObject.Find("CharacterUI").transform.GetChild(1).gameObject.SetActive(true);

        GameObject.Find("CharacterUI").transform.GetChild(2).gameObject.SetActive(false);

        if (GameObject.Find("Master").GetComponent<Master>().guidedLearningOn)
        {
            characterUIContainer.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            characterUIContainer.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);
        }

        characterUIContainer.transform.GetChild(0).gameObject.GetComponent<CharacterUIScript>().startConversation();
    }


}
