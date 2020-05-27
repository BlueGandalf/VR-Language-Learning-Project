using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class translationsMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //bool master = GameObject.Find("Master").GetComponent<Master>().translationsOn;
        //Debug.Log("translation check in " + master);
        //gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn = master;
        //if (master == true)
        //{
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Translations: ON";
        //}
        //else
        //{
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Translations: OFF";
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleTranslations()
    {
        GameObject.Find("Master").GetComponent<Master>().translationsOn = gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn;

        if(gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn == true)
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
