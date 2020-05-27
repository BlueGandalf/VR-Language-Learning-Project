using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class guidedLearningMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //bool master = GameObject.Find("Master").GetComponent<Master>().guidedLearningOn;
        //Debug.Log("guided check in " + master);
        //gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn = master;
        //if (master == true)
        //{
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Guided Learning: ON";
        //}
        //else
        //{
        //    gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Guided Learning: OFF";
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleGuidedLearning()
    {
        GameObject.Find("Master").GetComponent<Master>().guidedLearningOn = gameObject.GetComponent<UnityEngine.UI.Toggle>().isOn;

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
