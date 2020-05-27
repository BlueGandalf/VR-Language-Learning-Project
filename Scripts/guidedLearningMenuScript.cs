using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guidedLearningMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function is used to toggle the value in the Master script that represents the Guided Learning setting
    public void toggleGuidedLearning()
    {
        GameObject.Find("Master").GetComponent<Master>().guidedLearningOn = gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn;

        //This code changes the text in the menu
        if (gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn == true)
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Guided Learning: ON";
        }
        else
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Guided Learning: OFF";
        }

        //update conversations
        GameObject.Find("UI Container").GetComponent<CharacterUIScript>().getConversations();

        //update db
        new Shared().logSettings();
    }

}
