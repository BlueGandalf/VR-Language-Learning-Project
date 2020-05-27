using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterUIScript : MonoBehaviour
{

    private string voice;
    private int roomID;

    public GameObject button;

    private object threadLocker = new object();
    private bool waitingForSpeak;
    private string message;

    private string[][] currentAnswers;
    private List<string[]> conversations;
    private string[] chosenConversation;
    private int chosenConversationIndex;

    private CharacterButtonScript characterButtonScript;

    private string currentQuestionText;
    private int conversationID;
    private int questionType;

    private DateTime timeQuestionStarted;
    private DateTime timeQuestionAnswered;

    private int correctAnswerIndex;
    // Start is called before the first frame update
    void Start()
    {
        //AsyncOperation ao = getAudio(17);
        voice = GameObject.Find("Master").GetComponent<Master>().voice;
        roomID = GameObject.Find("Master").GetComponent<Master>().roomID;

        //get all valid conversations
        getConversations();
    }

    public void getConversations()
    {
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        List<string[]> tempConversations = new List<string[]>();
        conversations = new List<string[]>();

        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();

        IDataReader reader;
        IDbCommand dbcmd = dbCon.CreateCommand();

        string query = "SELECT ConversationID, WordLevel, EasinessFactor, Interval, DateLastAnswered FROM TblConversation JOIN TblItem ON TblConversation.ItemID = TblItem.ItemID WHERE TblItem.RoomID = " + roomID.ToString();
        dbcmd.CommandText = query;

        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            Debug.Log("ConversationID: " + reader[0].ToString() + "; WordLevel: " + reader[1].ToString() + " EasinessFactor: " + reader[2].ToString() + " Interval: " + reader[3].ToString() + " DateLastAnswered: " + reader[4].ToString());
            string[] tempArray = { reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), "0" }; //[5] is no. of times answered incorrectly this session.
            tempConversations.Add(tempArray);
        }

        dbCon.Close();

        if (GameObject.Find("Master").GetComponent<Master>().guidedLearningOn)
        {
            for (int i = 0; i < tempConversations.Count; i++)
            {
                if (tempConversations[i][1] == "1")
                {
                    conversations.Add(tempConversations[i]);
                    Debug.Log("ConversationID: " + tempConversations[i][0].ToString() + "; WordLevel: " + tempConversations[i][1].ToString() + " EasinessFactor: " + tempConversations[i][2].ToString() + " Interval: " + tempConversations[i][3].ToString() + " DateLastAnswered: " + tempConversations[i][4].ToString());
                }
            }
        }
        for (int i = 0; i < tempConversations.Count; i++)
        {
            if (tempConversations[i][1] == "2")
            {
                conversations.Add(tempConversations[i]);
                Debug.Log("ConversationID: " + tempConversations[i][0].ToString() + "; WordLevel: " + tempConversations[i][1].ToString() + " EasinessFactor: " + tempConversations[i][2].ToString() + " Interval: " + tempConversations[i][3].ToString() + " DateLastAnswered: " + tempConversations[i][4].ToString());
            }
        }
        for (int i = 0; i < tempConversations.Count; i++)
        {
            if (tempConversations[i][1] == "3")
            {
                conversations.Add(tempConversations[i]);
                Debug.Log("ConversationID: " + tempConversations[i][0].ToString() + "; WordLevel: " + tempConversations[i][1].ToString() + " EasinessFactor: " + tempConversations[i][2].ToString() + " Interval: " + tempConversations[i][3].ToString() + " DateLastAnswered: " + tempConversations[i][4].ToString());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startConversation()
    {
        Debug.Log("Hello");
        //decide conversation
        conversationID = getConversationID();

        Debug.Log("ConversationID: " + conversationID);

        if(conversationID != -1)
        {
            //get question + translation
            setQuestion();

            //get answers
            setAnswers();

            if (GameObject.Find("Master").GetComponent<Master>().guidedLearningOn && questionType == 1)
            {
                setArrow();
            }
            else
            {
                setArrow(false);
            }

            //audio
            if (GameObject.Find("Master").GetComponent<Master>().audioOn)
            {
                playAudio(getQuestionText());
            }
        }
        else
        {
            setQuestion(false);
            setAnswers(false);
        }

        timeQuestionStarted = DateTime.Now;
    }

    private void setArrow()
    {
        GameObject arrow = GameObject.Find("Arrow");
        GameObject targetItem = GameObject.Find(new Shared().getValueFromDatabase("SELECT TblItem.ItemName FROM TblConversation JOIN TblItem ON TblConversation.ItemID = TblItem.ItemID WHERE TblConversation.ConversationID = " + conversationID)); ;
        arrow.transform.LookAt(targetItem.transform);
    }
    private void setArrow(bool isNeeded)
    {
        GameObject arrow = GameObject.Find("Arrow");
        if (arrow != null)
        {
            arrow.SetActive(false);
        }
    }

    private int getConversationID()
    {
        int convID = 0;
        bool conversationPicked = false;
        int counter = 0;
        DateTime dateLastAnswered;
        double interval;

        Debug.Log("jkl");
        Debug.Log(counter + "," + conversations.Count + "," + conversationPicked);

        while (!conversationPicked && counter < conversations.Count)
        {
            Debug.Log("ghj");
            if(conversations[counter][4] != "")
            {
                dateLastAnswered = DateTime.Parse(conversations[counter][4]);
            }
            else
            {
                dateLastAnswered = DateTime.Now;
            }
            
            if(conversations[counter][3] != "")
            {
                interval = double.Parse(conversations[counter][3]);
            }
            else
            {
                interval = 0;
                conversations[counter][3] = "0";
            }

            Debug.Log("Ho ho ho");
            Debug.Log(getDateForNextCheck(interval, dateLastAnswered));
            Debug.Log(hasDatePassed(getDateForNextCheck(interval, dateLastAnswered)));
            if (hasDatePassed(getDateForNextCheck(interval, dateLastAnswered))) // conversation is ready to be asked
            {
                Debug.Log("conversation can be picked" + counter);
                conversationPicked = true;
                convID = int.Parse(conversations[counter][0]);
                chosenConversation = conversations[counter];
                chosenConversationIndex = counter;
            }
            counter++;
        }

        if(counter == conversations.Count)
        {
            Debug.Log("all available conversations completed");
            convID = -1;
        }
        else
        {
            questionType = int.Parse(conversations[counter - 1][1]);
            Debug.Log("QT: " + questionType);
        
            if(conversations[counter-1][1] == "1")
            {
                StartCoroutine(correct(-1));
            }
        }

        return convID;// 28;
    }

    private bool hasDatePassed(DateTime dateForNextCheck)
    {
        bool hasDatePassed = false;
        DateTime now = DateTime.Now;
        if(DateTime.Compare(now, dateForNextCheck) < 0) //dateForNextCheck is after now
        {
            hasDatePassed = false;
        }
        else if (DateTime.Compare(now, dateForNextCheck) > 0) //dateForNextCheck is before now
        {
            hasDatePassed = true;
        }
        else //.Compare = 0, and dateForNextCheck is equal to now
        {
            hasDatePassed = true;
        }
        return hasDatePassed;
    }

    private DateTime getDateForNextCheck(double interval, DateTime dateLastAnswered)
    {
        DateTime dateForNextCheck = dateLastAnswered.AddDays(interval);
        return dateForNextCheck;
    }

    private void setQuestion()
    {
        if (!GameObject.Find("Master").GetComponent<Master>().audioOnlyOn)
        {
            //change text
            GameObject textboxText = GameObject.Find("QuestionText");
            textboxText.GetComponent<UnityEngine.UI.Text>().text = getQuestionText();
            GameObject translationTextbox = GameObject.Find("QuestionTranslationText");
            translationTextbox.GetComponent<UnityEngine.UI.Text>().text = getQuestionTranslationText();
            Color clear = new Color(255, 255, 255, 0);
            translationTextbox.GetComponent<UnityEngine.UI.Text>().color = clear;
            translationTextbox.GetComponent<UnityEngine.UI.Outline>().effectColor = clear;
        }
        else
        {
            GameObject textboxText = GameObject.Find("QuestionText");
            textboxText.GetComponent<UnityEngine.UI.Text>().text = "";
            GameObject translationTextbox = GameObject.Find("QuestionTranslationText");
            translationTextbox.GetComponent<UnityEngine.UI.Text>().text = "";
        }
    }
    private void setQuestion(bool isConversation)
    {
        //change text
        GameObject textboxText = GameObject.Find("QuestionText");
        textboxText.GetComponent<UnityEngine.UI.Text>().text = "There's no conversations left!";
        GameObject translationTextbox = GameObject.Find("QuestionTranslationText");
        translationTextbox.GetComponent<UnityEngine.UI.Text>().text = "There is no conversations left!";
        Color clear = new Color(255, 255, 255, 0);
        translationTextbox.GetComponent<UnityEngine.UI.Text>().color = clear;
        translationTextbox.GetComponent<UnityEngine.UI.Outline>().effectColor = clear;
    }

    public string getQuestionText()
    {
        string query = "SELECT word.L2Text FROM TblWord word JOIN TblConversation convo ON convo.TextID = word.WordID WHERE convo.ConversationID = " + conversationID.ToString();
        string text = new Shared().getValueFromDatabase(query);

        text = new Shared().cleanText(text);

        return text;
    }

    public string getQuestionTranslationText()
    {
        string query = "SELECT word.L1Text FROM TblWord word JOIN TblConversation convo ON convo.TextID = word.WordID WHERE convo.ConversationID = " + conversationID.ToString();
        string translationText = new Shared().getValueFromDatabase(query);

        translationText = new Shared().cleanText(translationText);

        return translationText;
    }

    private int random(int starting, int ending)
    {
        int rNumber;
        System.Random random = new System.Random();
        rNumber = random.Next(starting, ending);
        return rNumber;
    }

    private void setAnswers()
    {
        string connection = "URI=file:" + Application.persistentDataPath + "/FYP_Database";
        List<string[]> answers = new List<string[]>();

        IDbConnection dbCon = new SqliteConnection(connection);
        dbCon.Open();
        
        IDataReader reader;
        IDbCommand dbcmd = dbCon.CreateCommand();
            
        string query = "SELECT AnswerID, CorrectAnswer, word.L2Text FROM TblAnswer JOIN TblWord as word ON TblAnswer.TextID = word.WordID WHERE ConversationID = " + conversationID.ToString();
        dbcmd.CommandText = query;
            
        reader = dbcmd.ExecuteReader();
            
        int counter = 0;

        while (reader.Read())
        {
            Debug.Log("AnswerID: " + reader[0].ToString() + "; Correct Answer: " + reader[1].ToString() + " Text: " + reader[2].ToString());
            string[] tempArray = { reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), counter.ToString() };
            answers.Add(tempArray);
            counter++;
            if (questionType == 2 && reader[1].ToString() == "1")
            {
                correctAnswerIndex = counter - 1;
            }
        }

        dbCon.Close();

        currentAnswers = answers.ToArray();

        if (answers.Count == 4)
        {
            questionType = 2; // multiple choice

            //TOP LEFT ANSWER
            int rNumber = random(0, 3); //get random answer

            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[rNumber][2];
            Debug.Log(answers[rNumber][2]);

            GameObject temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(-100f, 100f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[rNumber][3]);
            answers.RemoveAt(rNumber);

            //TOP RIGHT ANSWER
            rNumber = random(0, 2);

            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[rNumber][2];
            Debug.Log(answers[rNumber][2]);

            temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(100f, 100f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[rNumber][3]);
            answers.RemoveAt(rNumber);

            //MIDDLE LEFT ANSWER
            rNumber = random(0, 1);

            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[rNumber][2];
            Debug.Log(answers[rNumber][2]);

            temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(-100f, 0f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[rNumber][3]);
            answers.RemoveAt(rNumber);

            //MIDDLE RIGHT ANSWER
            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[0][2];
            Debug.Log(answers[0][2]);

            temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(100f, 0f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[0][3]);
        }
        else if (answers.Count == 6)
        {
            questionType = 3; // answer blocks

            //TOP LEFT ANSWER
            int rNumber = random(0, 5);

            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[rNumber][2];
            Debug.Log(answers[rNumber][2]);

            GameObject temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(-100f, 100f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[rNumber][3]);
            answers.RemoveAt(rNumber);

            //TOP RIGHT ANSWER
            rNumber = random(0, 4);

            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[rNumber][2];
            Debug.Log(answers[rNumber][2]);

            temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(100f, 100f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[rNumber][3]);
            answers.RemoveAt(rNumber);

            //MIDDLE LEFT ANSWER
            rNumber = random(0, 3);

            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[rNumber][2];
            Debug.Log(answers[rNumber][2]);

            temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(-100f, 0f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[rNumber][3]);
            answers.RemoveAt(rNumber);

            //MIDDLE RIGHT ANSWER
            rNumber = random(0, 2);

            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[rNumber][2];
            Debug.Log(answers[rNumber][2]);

            temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(100f, 0f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[rNumber][3]);
            answers.RemoveAt(rNumber);

            //BOTTOM LEFT ANSWER
            rNumber = random(0, 1);

            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[rNumber][2];
            Debug.Log(answers[rNumber][2]);

            temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(-100f, -100f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[rNumber][3]);
            answers.RemoveAt(rNumber);

            //BOTTOM RIGHT ANSWER
            button.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = answers[0][2];
            Debug.Log(answers[0][2]);

            temp = Instantiate(button, gameObject.transform.GetChild(0).GetChild(1).GetChild(0).transform);
            temp.transform.localPosition = new Vector3(100f, -100f, 0f);
            characterButtonScript = temp.GetComponent<CharacterButtonScript>();
            characterButtonScript.answersIndex = int.Parse(answers[0][3]);
        }
        else if (answers.Count == 0 || answers.Count == 1)
        {
            questionType = 1;
            GameObject container = GameObject.Find("Building Block Container");
            foreach (Transform child in container.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
    private void setAnswers(bool isConversation)
    {
        GameObject container = GameObject.Find("Building Block Container");

        foreach (Transform child in container.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    
    public void playAudio(string text)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in text)
        {
            if (c == '_')
            {
                sb.Append(' ');
            }
            else
            {
                sb.Append(c);
            }
        }

        text = sb.ToString();

        if (GameObject.Find("Master").GetComponent<Master>().audioOn)
        {
            StartCoroutine(getAudioAsync(text));
        }
    }

    //for reference if needed
    private void streamAudio(string text)
    {

        //lock (threadLocker)
        //{
        //    waitingForSpeak = true;
        //}

        //lock (threadLocker)
        //{
        //    message = newMessage;
        //    waitingForSpeak = false;
        //}

    }

    IEnumerator getAudioAsync(string text)
    {
        yield return null;

        new Shared().playConversationAudio(conversationID, gameObject, text);
    }

    public void giveAnswer(int answersIndex)
    {
        timeQuestionAnswered = DateTime.Now;
        Debug.Log("Answer given: " + answersIndex.ToString());

        //give audio
        if(questionType == 2)
        {
            string text = currentAnswers[answersIndex][2];
            //StartCoroutine(getAudioAsync(text));
        }
        else if(questionType == 3)
        {
            //don't stream audio
        }
        
        //is correct?
        string isCorrectstr = currentAnswers[answersIndex][1];
        bool isCorrect;

        if(isCorrectstr == "1")
        {
            isCorrect = true;
        }
        else
        {
            isCorrect = false;
        }

        Debug.Log("Answer was " + isCorrect);

        if (isCorrect)
        {
            //if correct
            StartCoroutine(correct(answersIndex));
        }
        else
        {
            //if not correct
            StartCoroutine(incorrect());
        }
    }
    IEnumerator incorrect()
    {
        //log result
        logConversation(0);

        //display result
        GameObject textboxText = GameObject.Find("QuestionText");
        currentQuestionText = textboxText.GetComponent<UnityEngine.UI.Text>().text;
        textboxText.GetComponent<UnityEngine.UI.Text>().text = "That is incorrect.";

        Debug.Log("Waiting...");
        yield return new WaitForSeconds(1f);
        Debug.Log("Done.");

        if(questionType == 2)
        {
            textboxText.GetComponent<UnityEngine.UI.Text>().text = "The correct answer is: " + currentAnswers[correctAnswerIndex][2];
            Debug.Log("Waiting...");
            yield return new WaitForSeconds(2f);
            Debug.Log("Done.");

            //increase incorrect counter
            int q = int.Parse(chosenConversation[5]);
            q++;
            chosenConversation[5] = q.ToString();

            updateConversationIncorrect(2); // incorrect always has q=2

            string[] tempArray = { chosenConversation[0], chosenConversation[1] , chosenConversation[2] , chosenConversation[3] , chosenConversation[4] , chosenConversation[5] };

            Debug.Log(conversations.Count);
            conversations.RemoveAt(chosenConversationIndex);
            conversations.Add(tempArray);
            Debug.Log(conversations.Count);

            for(int i = 0; i < conversations.Count; i++)
            {
                Debug.Log(conversations[i][0]);
            }


            //close canvas
            gameObject.transform.GetChild(1).gameObject.GetComponent<CloseUI>().closeCanvas();
        }
        else if(questionType == 3)
        {
            //increase incorrect counter
            int q = int.Parse(chosenConversation[5]);
            q++;
            chosenConversation[5] = q.ToString();
        }

        
        
        //reset text
        textboxText.GetComponent<UnityEngine.UI.Text>().text = currentQuestionText;
        
    }
    IEnumerator correct(int answersIndex)
    {
        
        if(answersIndex == -1)
        {
            updateConversationCorrect(5); // incorrect always has q=2

            string[] tempArray = { chosenConversation[0], chosenConversation[1], chosenConversation[2], chosenConversation[3], chosenConversation[4], chosenConversation[5] };

            Debug.Log(conversations.Count);
            conversations.RemoveAt(chosenConversationIndex);
            conversations.Add(tempArray);
            Debug.Log(conversations.Count);
        }
        else if(questionType == 2)
        {
            int q = 5;
            q = q - int.Parse(chosenConversation[5]);

            if (q <= 2)
            {
                q = 3;
            }

            updateConversationCorrect(q);

            logConversation(1);

            //display result
            GameObject textboxText = GameObject.Find("QuestionText");
            currentQuestionText = textboxText.GetComponent<UnityEngine.UI.Text>().text;
            textboxText.GetComponent<UnityEngine.UI.Text>().text = "That is correct.";

            Debug.Log("Waiting...");
            yield return new WaitForSeconds(1f);
            Debug.Log("Done.");
            
            //close canvas
            gameObject.transform.GetChild(1).gameObject.GetComponent<CloseUI>().closeCanvas();

            //update conversation
            
        }
        else if(questionType == 3)
        {
            //reset text
            GameObject textboxText = GameObject.Find("QuestionText");
            currentQuestionText = textboxText.GetComponent<UnityEngine.UI.Text>().text;

            StringBuilder sb = new StringBuilder();
            Debug.Log(textboxText.GetComponent<UnityEngine.UI.Text>().text);
            string[] temps = currentQuestionText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None );
            string newAnswer = "";

            if(temps.Length > 1)
            {
                sb.Append(temps[1]);
                sb.Append(currentAnswers[answersIndex][2]);
                newAnswer = sb.ToString();
            }
            else
            {
                newAnswer = currentAnswers[answersIndex][2];
            }

            Debug.Log("New answer:" + newAnswer);

            //get answer
            string fullAnswer = new Shared().getValueFromDatabase("SELECT word.L2Text FROM TblConversation convo JOIN TblItem item ON convo.ItemID = item.ItemID JOIN TblWord word ON word.WordID = item.WordID WHERE convo.ConversationID = " + conversationID.ToString());
            Debug.Log("Full Answer: " + fullAnswer);
            fullAnswer = fullAnswer.ToUpper();

            if (fullAnswer.StartsWith(newAnswer))
            {
                logConversation(1);
                //display result
                textboxText.GetComponent<UnityEngine.UI.Text>().text = "That is correct.";

                Debug.Log("Waiting...");
                yield return new WaitForSeconds(1f);
                Debug.Log("Done.");

                //correct input
                textboxText.GetComponent<UnityEngine.UI.Text>().text = temps[0] + Environment.NewLine + newAnswer;
                

            }
            else
            {
                logConversation(0);
                //correct input, wrong order

                //display result
                textboxText.GetComponent<UnityEngine.UI.Text>().text = "That is incorrect.";

                Debug.Log("Waiting...");
                yield return new WaitForSeconds(1f);
                Debug.Log("Done.");

                //reset text
                textboxText.GetComponent<UnityEngine.UI.Text>().text = currentQuestionText;
                
            }

            if (newAnswer == fullAnswer)
            {
                //display result
                currentQuestionText = textboxText.GetComponent<UnityEngine.UI.Text>().text;
                textboxText.GetComponent<UnityEngine.UI.Text>().text = "That is the correct word.";

                Debug.Log("Waiting...");
                yield return new WaitForSeconds(2f);
                Debug.Log("Done.");

                //close canvas
                gameObject.transform.GetChild(1).gameObject.GetComponent<CloseUI>().closeCanvas();

                //update conversation
                int q = 5;
                q = q - int.Parse(chosenConversation[5]);

                if (q <= 2)
                {
                    q = 3;
                }

                updateConversationCorrect(q);
            }
        }
        else if(questionType == 1)
        {
            //update conversation
            logConversation(1);
        }
        else
        {

        }
        Debug.Log("correct person");
    }

    public void updateConversationIncorrect(int q)
    {
        Debug.Log("Before: " + chosenConversation[2] + "," + chosenConversation[3]);

        //conversations 2 EF 3 I(n)
        double oldEF = double.Parse(chosenConversation[2]);
        double oldInterval = double.Parse(chosenConversation[3]);

        double newEF = oldEF + (0.1 - (5 - q) * (0.08 + (5 - q) * 0.02));

        if(newEF > 2.5)
        {
            newEF = 2.5;
        }

        double newInterval;
        if (oldInterval == 0)
        {
            newInterval = 0;
        }
        else
        {
            newInterval = oldInterval * oldEF;
        }

        chosenConversation[2] = newEF.ToString();
        chosenConversation[3] = newInterval.ToString();

        Debug.Log("After: " + chosenConversation[2] + "," + chosenConversation[3]);
    }

    public void updateConversationCorrect(int q)
    {
        Debug.Log("Before: " + chosenConversation[2] + "," + chosenConversation[3]);

        //conversations 2 EF 3 I(n)
        double oldEF = double.Parse(chosenConversation[2]);
        double oldInterval = double.Parse(chosenConversation[3]);

        double newEF = oldEF + (0.1 - (5 - q) * (0.08 + (5 - q) * 0.02));

        if (newEF > 2.5)
        {
            newEF = 2.5;
        }

        double newInterval;
        if (oldInterval == 0)
        {
            newInterval = 1;
        }
        else
        {
            newInterval = oldInterval * oldEF;
        }

        chosenConversation[2] = newEF.ToString();
        chosenConversation[3] = newInterval.ToString();

        Debug.Log("After: " + chosenConversation[2] + "," + chosenConversation[3]);
    }

    private void logConversation(int isCorrect)
    {
        //interactionID -> auto assigned
        //interactionTypeID -> 2=character
        //itemID -> null
        //CharacterID -> 3
        //RoomID -> 1
        //DateTime -> auto assigned
        //UserID
        //Correct -> parameter

        Debug.Log("Starting log");
        StartCoroutine(logAsync(isCorrect));
    }
    IEnumerator logAsync(int isCorrect)
    {
        yield return null;
        Debug.Log("continuing log");
        new Shared().log(2, 0, conversationID, isCorrect);
    }


}

