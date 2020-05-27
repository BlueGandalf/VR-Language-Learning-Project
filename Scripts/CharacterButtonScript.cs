using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharacterButtonScript : MonoBehaviour
{
    public int answersIndex;

    public CharacterUIScript characterUIScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This function is used to communicate to the CharacterUIScript that an answer has been pressed, and in which position it was in.
    public void buttonClicked()
    {
        characterUIScript = gameObject.GetComponentInParent<CharacterUIScript>();
        characterUIScript.giveAnswer(answersIndex);
    }
}
