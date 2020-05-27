using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseUI : MonoBehaviour
{

    public string UIObjectName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function is passed the name of a UI object and this function deactivates it. This function is used when the Close button is pressed.
    public void closeCanvas()
    {
        GameObject textbox = GameObject.Find(UIObjectName);
        textbox.SetActive(false);
        //gameObject.SetActive(false); // script will run on buttons"
    }
}
