using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public VRTeleporter teleporter;
    public Transform bodyTransform;

    private Boolean isDisplaying = false;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteTeleport()
    {
        teleporter.Teleport();
        float height = (float)1.7;
        bodyTransform.position = bodyTransform.position + new Vector3(0, height, 0);
    }

    public void TurnOnDisplay()
    {
            teleporter.ToggleDisplay(true);
    }

    public void TurnOffDisplay()
    {
        teleporter.ToggleDisplay(false);
    }
}
