using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CallTextbox : MonoBehaviour
{
    public int ItemID;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private string message;

    private AudioClip audioclip1;

    private string voice;
    private int roomID;

    // Start is called before the first frame update
    void Start()
    {
        string filePath = @"Assets/Resources/Audio/Item" + ItemID.ToString() + "Audio" + roomID.ToString() + ".wav";
        if (File.Exists(filePath))
        {
            audioclip1 = (AudioClip)Resources.Load("Audio/Item" + ItemID.ToString() + "Audio" + roomID.ToString());
        }

        voice = GameObject.Find("Master").GetComponent<Master>().voice;
        roomID = GameObject.Find("Master").GetComponent<Master>().roomID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowTextbox()
    {
        if (!GameObject.Find("Master").GetComponent<Master>().audioOnlyOn)
        {
            //get textbox container. Not textbox directly. As it is deactive, it cannot be found.
            GameObject textboxContainer = GameObject.Find("TextboxContainer");

            //change location
            float height = gameObject.GetComponent<MeshRenderer>().bounds.extents.y;
            textboxContainer.GetComponent<Transform>().position = (gameObject.GetComponent<Transform>().position + new Vector3(0f, 0.5f + (2 * height), 0f));

            //activate textbox container
            GameObject textboxInnerContainer = textboxContainer.transform.GetChild(0).gameObject;
            textboxInnerContainer.SetActive(true);

            //change text
            GameObject textboxText = GameObject.Find("TextBoxText");
            textboxText.GetComponent<UnityEngine.UI.Text>().text = getTextboxText();
            GameObject translationTextbox = GameObject.Find("TranslationText");
            translationTextbox.GetComponent<UnityEngine.UI.Text>().text = getTranslationText();
            Color clear = new Color(255, 255, 255, 0);
            translationTextbox.GetComponent<UnityEngine.UI.Text>().color = clear;
            translationTextbox.GetComponent<UnityEngine.UI.Outline>().effectColor = clear;

            //change rotation
            GameObject camera = GameObject.Find("Main Camera");
            textboxContainer.transform.LookAt(camera.transform);
        }

        //log interaction
        logInteraction(ItemID);

        //sound audio
        if (GameObject.Find("Master").GetComponent<Master>().audioOn)
        {
            playAudio();
            Debug.Log("Audio Done");
        }
    }

    private void logInteraction(int itemID)
    {
        StartCoroutine(logAsync());
    }
    IEnumerator logAsync()
    {
        yield return null;

        new Shared().log(1, ItemID, 0, 0);
    }

    public string getTextboxText()
    {
        string text = new Shared().getValueFromDatabase("SELECT word.L2Text FROM TblWord word JOIN TblItem item ON item.WordID = word.WordID WHERE item.ItemID = " + ItemID.ToString());
        
        return text;
    }

    public string getTranslationText()
    {
        string translationText = new Shared().getValueFromDatabase("SELECT word.L1Text FROM TblWord word JOIN TblItem item ON item.WordID = word.WordID WHERE item.ItemID = " + ItemID.ToString());
        
        return translationText;
    }

    public void playAudio()
    {
        if(audioclip1 != null)
        {
            if (gameObject.GetComponent<AudioSource>() == null)
            {
                AudioSource newAudioSource = new AudioSource();
                gameObject.AddComponent(typeof(AudioSource));
            }
            
            gameObject.GetComponent<AudioSource>().clip = audioclip1;
            gameObject.GetComponent<AudioSource>().volume = (float) GameObject.Find("Master").GetComponent<Master>().audioVolume / 100;
            if (gameObject.GetComponent<AudioSource>().isPlaying)
            {
                gameObject.GetComponent<AudioSource>().Pause();
            }
            else
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            StartCoroutine(getAudioAsync());
        }
    }
    IEnumerator getAudioAsync()
    {
        yield return null;

        new Shared().playItemAudio(ItemID, getTextboxText(), gameObject);
    }
}
