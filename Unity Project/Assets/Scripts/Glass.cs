using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.clear;
        Color a = Color.white;
        a.a = 0.2f;
        gameObject.GetComponent<Renderer>().material.color = a;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
