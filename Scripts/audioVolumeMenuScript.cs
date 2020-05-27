using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioVolumeMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function is used to change the value in the Master script that represents the Audio Volume setting
    public void changeVolume()
    {
        //This gets the percentage of the slider
        int percentage = (int)(Math.Round(gameObject.GetComponent<UnityEngine.UI.Slider>().value, 2) * 100);

        //this updates the master script variable
        GameObject.Find("Master").GetComponent<Master>().audioVolume = percentage;

        //This code changes the text in the menu
        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio Volume: " + percentage + "%";


        //update db
        new Shared().logSettings();
    }

}
