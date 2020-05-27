using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBoxTimer : MonoBehaviour
{


    private int timeCounter;
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        timeCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            timeCounter++;
            if (timeCounter >= 300)
            {
                gameObject.SetActive(false);
                timeCounter = 0;
            }
        }
    }
}
