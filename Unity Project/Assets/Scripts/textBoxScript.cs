using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class textBoxScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showTranslation()
    {
        if (GameObject.Find("Master").GetComponent<Master>().translationsOn)
        {
            GameObject textBoxText = GameObject.Find("TextBoxText");//gameObject.transform.GetChild(0).GetChild(0).gameObject;
            GameObject translationText = GameObject.Find("TranslationText");//gameObject.transform.GetChild(0).GetChild(1).gameObject;
            textBoxText.GetComponent<Transform>().position = textBoxText.GetComponent<Transform>().position + new Vector3(0f, 0.07f, 0f);
            translationText.GetComponent<UnityEngine.UI.Text>().color = Color.yellow;
            translationText.GetComponent<UnityEngine.UI.Outline>().effectColor = Color.black;
            translationText.GetComponent<Transform>().position = translationText.GetComponent<Transform>().position + new Vector3(0f, -0.07f, 0f);
        }
    }
    public void hideTranslation()
    {
        if (GameObject.Find("Master").GetComponent<Master>().translationsOn)
        {
            GameObject textBoxText = GameObject.Find("TextBoxText");
            GameObject translationText = GameObject.Find("TranslationText");
            textBoxText.GetComponent<Transform>().position = textBoxText.GetComponent<Transform>().position + new Vector3(0f, -0.07f, 0f);
            translationText.GetComponent<Transform>().position = translationText.GetComponent<Transform>().position + new Vector3(0f, 0.07f, 0f);
            Color clear = new Color(255, 255, 255, 0);
            translationText.GetComponent<UnityEngine.UI.Text>().color = clear;
            translationText.GetComponent<UnityEngine.UI.Outline>().effectColor = clear;
        }
    }

}
