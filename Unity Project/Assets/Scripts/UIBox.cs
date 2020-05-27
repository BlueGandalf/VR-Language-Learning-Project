using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBox : MonoBehaviour
{

    public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowText(string textToShow)
    {
        go.SetActive(true);
        TextMesh textObject = GameObject.Find("Text").GetComponent<TextMesh>();
        textObject.text = textToShow;
    }
}
