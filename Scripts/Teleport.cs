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

    //This function teleports the user to the target and gives them a height of 1.7.
    public void ExecuteTeleport()
    {
        teleporter.Teleport();
        float height = (float)1.7;
        bodyTransform.position = bodyTransform.position + new Vector3(0, height, 0);
    }

    //When the user looks at the floor, this function is called to display the teleportation target.
    public void TurnOnDisplay()
    {
            teleporter.ToggleDisplay(true);
    }

    //When the user stops looking at the floor, this function is called to hide the target.
    public void TurnOffDisplay()
    {
        teleporter.ToggleDisplay(false);
    }
}
