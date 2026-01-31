using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Interview : MonoBehaviour
{   
    [SerializeField] Slider timerSlider;

    [SerializeField] GameObject head;

    [SerializeField] DialogueReader dialogueReader;

    [SerializeField] Dialogue[] dialogueArray;

    [SerializeField] int currentDialogue = 0;

    [SerializeField] float speakingPause = 3;

    [SerializeField] FacialExpressionScanner facialExpressionScanner;

    Coroutine interviewTextCoroutine = null;
    Coroutine endTextCoroutine = null;

    float timer;
    float timerLength;
    bool timerStart;
    Dialogue.Emotion currentEmotion;

    private void Start()
    {
        interviewTextCoroutine = StartCoroutine(InterviewTextCoroutine());

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            dialogueReader.SkipText();

            if(timerStart == true )
            {
                timer = timerLength;
            }
        }

        if(timerStart)
        {
            // Calculate normalized value (starts at 1, goes to 0)
            timerSlider.value = (timerLength - timer) / timerLength;
            timer += Time.deltaTime;

            if (timer >= timerLength)
            {
                //check if emotion is done here
                timerStart = false;

                if (Enum.TryParse(facialExpressionScanner.currentEmotion, true, out Dialogue.Emotion emotion))
                {
                    EmotionCheck(emotion);
                }
                else
                {
                    EmotionCheck(Dialogue.Emotion.Crazy);
                }
            }
        }
 
    }

    private void EmotionCheck(Dialogue.Emotion emotion)
    {
        if (dialogueArray[currentDialogue].requiredEmotion == emotion)
        {
            if (endTextCoroutine == null) endTextCoroutine = StartCoroutine(SuccessTextCoroutine());
            return;
        }

        foreach (Dialogue.ReactDialogue react in dialogueArray[currentDialogue].reactDialogues)
        {
            if (react.emotion == emotion)
            {
                if (endTextCoroutine == null) endTextCoroutine = StartCoroutine(ReactTextCoroutine( react.emotion));
                
                return;
            }
        }
        if (endTextCoroutine == null) endTextCoroutine = StartCoroutine(FailTextCoroutine());

        //Check dialogueArray[] for required state and for other states
        //Fail - Failtextcoroutine
    }



    void StartTimer()
    {
        // Use 0 to 1 range for the slider
        timerSlider.minValue = 0;
        timerSlider.maxValue = 1;
        timerSlider.value = 1;
        
        head.SetActive(true);
        timer = 0;
        timerLength = dialogueArray[currentDialogue].interviewTimer;
        
        // Safety check to avoid division by zero
        if (timerLength <= 0) timerLength = 0.1f;
        
        timerStart = true;
    }

    IEnumerator InterviewTextCoroutine()
    {
        string toread = dialogueArray[currentDialogue].mainDialogue;
        dialogueReader.TypeText(toread);

        yield return new WaitUntil(() => dialogueReader.typingFinished);
        yield return new WaitForSeconds(speakingPause);
        if(dialogueArray[currentDialogue].isInterviewQuestion)
        {
            StartTimer();
        }
        else if(dialogueArray[currentDialogue].dialogueEnd == false)
        {
            //StartCoroutine(DialogueWaitCoroutine());
            NextDialogue();
        }
    }

    IEnumerator SuccessTextCoroutine()
    {
        string toread = dialogueArray[currentDialogue].successDialogue;
        dialogueReader.TypeText(toread);

        yield return new WaitUntil(() => dialogueReader.typingFinished);
        yield return new WaitForSeconds(speakingPause);
        InterviewSuccess();
        endTextCoroutine = null;
    }

    IEnumerator FailTextCoroutine()
    {
        string toread = dialogueArray[currentDialogue].failureDialogue;
        dialogueReader.TypeText(toread);

        yield return new WaitUntil(() => dialogueReader.typingFinished);
        yield return new WaitForSeconds(speakingPause);
        InterviewFailure();
        endTextCoroutine = null;
    }

    IEnumerator ReactTextCoroutine(Dialogue.Emotion emotion)
    {
        string toread = "";
        bool success = false;

        foreach (Dialogue.ReactDialogue react in dialogueArray[currentDialogue].reactDialogues)
        {
            if(react.emotion == emotion)
            {
                toread = react.dialogue;
                success = react.success;
                break;
            }
        }


        dialogueReader.TypeText(toread);

        yield return new WaitUntil(() => dialogueReader.typingFinished);
        yield return new WaitForSeconds(speakingPause);
        if(success)
        {
            InterviewSuccess();
        }
        else
        {
            InterviewFailure();
        }
        endTextCoroutine = null;
    }

    void InterviewSuccess()
    {
        NextDialogue();
        //points +
    }

    void InterviewFailure()
    {
        NextDialogue();
        //points -
    }

    void NextDialogue()
    {
        currentDialogue = Mathf.Clamp(currentDialogue + 1, 0, dialogueArray.Length - 1);
        interviewTextCoroutine = StartCoroutine(InterviewTextCoroutine());
    }

    IEnumerator DialogueWaitCoroutine()
    {
        yield return new WaitForSeconds(speakingPause);

        NextDialogue();
    }
}
