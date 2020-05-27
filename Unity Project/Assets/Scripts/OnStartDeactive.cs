using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnStartDeactive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //immediately deactivates the gameobject, to show again later when prompted.
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
