using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioVolumeMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //int master = GameObject.Find("Master").GetComponent<Master>().audioVolume;
        //Debug.Log("volume check in " + master);
        //gameObject.GetComponent<UnityEngine.UI.Slider>().value = (float)master / 100;
        //gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio Volume: " + master + "%";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeVolume()
    {
        int percentage = (int)(Math.Round(gameObject.GetComponent<UnityEngine.UI.Slider>().value, 2) * 100);


        GameObject.Find("Master").GetComponent<Master>().audioVolume = percentage;


        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio Volume: " + percentage + "%";


        //update db
        new Shared().logSettings();
    }

}
