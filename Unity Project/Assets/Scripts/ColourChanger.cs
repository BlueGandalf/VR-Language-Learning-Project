using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourChanger : MonoBehaviour
{
    private int cycleCount;
    // Start is called before the first frame update
    void Start()
    {
        cycleCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CycleColor()
    {
        Color[] colorArray = { Color.red, Color.green, Color.blue, Color.cyan, Color.white, Color.black, Color.yellow, Color.magenta };
        List<Color> colors = new List<Color>(colorArray);
        colorArray = null;

        GetComponent<Renderer>().material.color = colors[cycleCount % colors.Count];
        cycleCount++;
    }



}
