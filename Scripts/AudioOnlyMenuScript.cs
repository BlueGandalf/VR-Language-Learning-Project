using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioOnlyMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function is used to toggle the value in the Master script that represents the Audio Only setting
    public void toggleAudio()
    {
        GameObject.Find("Master").GetComponent<Master>().audioOnlyOn = gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn;

        //This is used to change the text in the menu
        if (gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn == true)
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio Only: ON";
        }
        else
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio Only: OFF";
        }

        //update db
        new Shared().logSettings();
    }

}
