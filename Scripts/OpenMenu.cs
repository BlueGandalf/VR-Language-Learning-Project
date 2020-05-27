using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openMenu : MonoBehaviour
{

    private bool menuOpenAlready = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function is used to display the menu Screen.
    public void displayMenu()
    {
        //if the menu isn't already open, this deactives the question/answer panels of the Character's UI and activates the menu panel. 
        if (!menuOpenAlready)
        {
            GameObject characterUI = GameObject.Find("CharacterUI");
            characterUI.transform.GetChild(0).gameObject.SetActive(false);
            characterUI.transform.GetChild(1).gameObject.SetActive(false);
            characterUI.transform.GetChild(2).gameObject.SetActive(true);
            menuOpenAlready = true;
        }
        else // if the menu is already open, this deactivates it and reactivates the question/answer panels
        {
            GameObject characterUI = GameObject.Find("CharacterUI");
            characterUI.transform.GetChild(0).gameObject.SetActive(true);
            characterUI.transform.GetChild(1).gameObject.SetActive(true);
            characterUI.transform.GetChild(2).gameObject.SetActive(false);
            menuOpenAlready = false;
        }

        updateMenu();
    }

    //This is run when the menu is opened/closed, updating the visual components with the settings saved in the Master script
    private void updateMenu()
    {
        Master master = GameObject.Find("Master").GetComponent<Master>();
        GameObject container = GameObject.Find("Building Block Container");


        GameObject translations = container.transform.GetChild(1).gameObject;
        translations.GetComponent<UnityEngine.UI.Toggle>().isOn = master.translationsOn;
        GameObject audio = container.transform.GetChild(3).gameObject;
        audio.GetComponent<UnityEngine.UI.Toggle>().isOn = master.audioOn;
        GameObject audioOnly= container.transform.GetChild(2).gameObject;
        audioOnly.GetComponent<UnityEngine.UI.Toggle>().isOn = master.audioOnlyOn;
        GameObject guidedLearning= container.transform.GetChild(0).gameObject;
        guidedLearning.GetComponent<UnityEngine.UI.Toggle>().isOn = master.guidedLearningOn;
        GameObject volume = container.transform.GetChild(4).gameObject;
        volume.GetComponent<UnityEngine.UI.Slider>().value = (float) master.audioVolume / 100;
    }
}
