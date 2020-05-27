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

    public void buttonClicked()
    {
        characterUIScript = gameObject.GetComponentInParent<CharacterUIScript>();
        characterUIScript.giveAnswer(answersIndex);
        //SynthesisToAudioFileAsync("help").Wait();
    }

    public  async Task SynthesisToAudioFileAsync(string temp)
    {
        var config = SpeechConfig.FromSubscription("c5ab91b760b24599b3667791c08aa7d9", "uksouth");

        var fileName = @"Assets/Resources/Audio/Conv" + temp + "Audio.wav";
        using (var fileOutput = AudioConfig.FromWavFileOutput(fileName))
        {
            using (var synthesizer = new SpeechSynthesizer(config, fileOutput))
            {
                StringBuilder sb = new StringBuilder();
                foreach (char c in temp)
                {
                    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.')
                    {
                        sb.Append(c);
                    }
                    if (c == '-')
                    {
                        sb.Append(',');
                    }
                }
                temp = sb.ToString();

                var text = "Hello world!";
                string ssml = @"<speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xml:lang='en-US'><voice name='ja-JP-Ayumi-Apollo'>" + temp + "</voice></speak>";
                var result = await synthesizer.SpeakSsmlAsync(text);

                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    //Console.WriteLine($"Speech synthesized to [{fileName}] for text [{text}]");
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    //Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        //Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        //Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                        //Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                }
            }
        }
    }

}
