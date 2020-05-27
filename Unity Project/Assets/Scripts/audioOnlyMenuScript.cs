using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioOnlyMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //bool master = GameObject.Find("Master").GetComponent<Master>().audioOnlyOn;
        //Debug.Log("audio only check in " + master);
        //gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn = master;
        //if (master == true)
        //{
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio Only: ON";
        //}
        //else
        //{
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Audio Only: OFF";
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleAudio()
    {
        GameObject.Find("Master").GetComponent<Master>().audioOnlyOn = gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn;

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
