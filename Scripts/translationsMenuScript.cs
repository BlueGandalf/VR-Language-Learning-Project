using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class translationsMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function is used to toggle the value in the Master script that represents the Translations On/Off setting
    public void toggleTranslations()
    {
        GameObject.Find("Master").GetComponent<Master>().translationsOn = gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn;

        //This code changes the text in the menu
        if (gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn == true)
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Translations: ON";
        }
        else
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Translations: OFF";
        }

        //update db
        new Shared().logSettings();
    }

}
