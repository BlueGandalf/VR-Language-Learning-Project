using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exitToMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void exitMenu()
    {
        //This script logs the scene interaction, and then changes the scene to the MenuScene.
        new Shared().logScene(GameObject.Find("Master").GetComponent<Master>().roomID, 0);
        SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);


    }
}
