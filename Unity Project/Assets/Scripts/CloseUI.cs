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

    public void closeCanvas()
    {
        GameObject textbox = GameObject.Find(UIObjectName);
        textbox.SetActive(false);
        //gameObject.SetActive(false); // script will run on buttons"
    }
}
