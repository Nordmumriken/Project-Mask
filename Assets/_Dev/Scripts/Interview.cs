using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class Interview : MonoBehaviour
{
    [SerializeField] Animator robAnimator;

    [SerializeField] DialogueReader dialogueReader;

    [SerializeField] Dialogue[] dialogueArray;

    [SerializeField] int currentDialogue = 0;

    [SerializeField] float speakingPause = 3;

    Coroutine interviewTextCoroutine = null;
    Coroutine endTextCoroutine = null;

    float timer;
    float timerLength;
    bool timerStart;
    Dialogue.Emotion currentEmotion;

    private void Start()
    {
        robAnimator.Play("Idle");
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
            timer += Time.deltaTime;

            // placeholder for testing
            // add input for speeding up
            if (Input.GetKeyDown(KeyCode.A))
            {
                currentEmotion = Dialogue.Emotion.Happy;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                currentEmotion = Dialogue.Emotion.Sad;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                currentEmotion = Dialogue.Emotion.Angry;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                currentEmotion = Dialogue.Emotion.Shocked;
            }

            //

            if (timer >= timerLength)
            {
                //check if emotion is done here
                timerStart = false;
                EmotionCheck(currentEmotion);

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
        timer = 0;
        timerLength = dialogueArray[currentDialogue].interviewTimer;
        timerStart = true;
    }

    IEnumerator InterviewTextCoroutine()
    {
        string toread = dialogueArray[currentDialogue].mainDialogue;
        dialogueReader.TypeText(toread);

        yield return new WaitUntil(() => dialogueReader.typingFinished);
        if(dialogueArray[currentDialogue].isInterviewQuestion)
        {
            StartTimer();
        }
        else if(dialogueArray[currentDialogue].dialogueEnd == false)
        {
            StartCoroutine(DialogueWaitCoroutine());
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
