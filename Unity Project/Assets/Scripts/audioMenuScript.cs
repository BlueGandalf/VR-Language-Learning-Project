using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //bool master = GameObject.Find("Master").GetComponent<Master>().audioOn;
        //Debug.Log("audio check in " + master);
        //gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn = master;
        //if (master == true)
        //{
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio: ON";
        //}
        //else
        //{
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio: OFF";
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleAudio()
    {
        GameObject.Find("Master").GetComponent<Master>().audioOn = gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn;

        if (gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn == true)
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio: ON";
        }
        else
        {
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio: OFF";
        }

        //update db
        new Shared().logSettings();
    }

}
