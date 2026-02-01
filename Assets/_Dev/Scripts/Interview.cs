using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Audio;

public class Interview : MonoBehaviour
{
    [SerializeField] Animator robAnimator;
    [SerializeField] GameObject gameEndScreen;

    [SerializeField] Slider timerSlider;
    [SerializeField] TextMeshProUGUI timerText;

    [SerializeField] PlayableDirector head;
    [SerializeField] PlayableDirector hideHeadTimeline;
    [SerializeField] DialogueReader dialogueReader;

    [SerializeField] Dialogue[] dialogueArray;

    [SerializeField] int currentDialogue = 0;

    [SerializeField] float speakingPause = 3;

    [SerializeField] float lookaroundSpeed;

    [SerializeField] FacialExpressionScanner facialExpressionScanner;

    [SerializeField] CameraScript cameraScript;
    Coroutine interviewTextCoroutine = null;
    Coroutine endTextCoroutine = null;
    Coroutine lookaroundCoroutine = null;

    [SerializeField] PlayableAsset hideheadTimeline;
    [SerializeField] PlayableAsset lookaroundTimeline;
    [SerializeField] PlayableAsset minigameTimeline;
    [SerializeField] PlayableAsset stopLookingTimeline;

    [SerializeField] AudioSource speakAudioSource;
    [SerializeField] AudioSource effectAudioSource;
    [SerializeField] AudioClip bossLike, bossDislike;

    public int points { get; private set; }

    Camera mainCam;

    float timer;
    float timerLength;
    bool timerStart;

    bool lookingAround;
    Dialogue.Emotion currentEmotion;

    private void Start()
    {
        mainCam = Camera.main;
        robAnimator.Play("Idle");
        interviewTextCoroutine = StartCoroutine(InterviewTextCoroutine());

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            dialogueReader.SkipText();

            if(timerStart)
            {
                timerText.text = "";
               
                // Calculate normalized value (starts at 1, goes to 0)
                timerSlider.value = (timerLength - timer) / timerLength;

                if (timerText != null) timerText.text = "";
                timer = timerLength;
            }
        }

        if(timerStart)
        {
            timer += Time.deltaTime;

            float remainingTime = Mathf.Max(0, timerLength - timer);
            if (timerText != null) timerText.text = remainingTime.ToString("F2");
            timerSlider.value = (timerLength - timer) / timerLength;

            // placeholder for testing
            // add input for speeding up
            


            //

            if (timer >= timerLength)
            {
                if(lookaroundCoroutine != null) StopCoroutine(lookaroundCoroutine);
                lookaroundCoroutine = null;
                head.playableAsset = minigameTimeline;
                hideHeadTimeline.playableAsset = hideheadTimeline;
                //check if emotion is done here
                timerStart = false;
                if (timerText != null) timerText.text = "";
                hideHeadTimeline.Play();

                if (Enum.TryParse(facialExpressionScanner.currentEmotion, true, out Dialogue.Emotion emotion))
                {
                    EmotionCheck(emotion);
                    cameraScript.LerpToBossActivate();
                }
                else
                {
                    EmotionCheck(Dialogue.Emotion.Crazy);
                    cameraScript.LerpToBossActivate();
                }

            }
            else
            {
                if (Input.GetKey(KeyCode.A))
                {
                    lookingAround = true;
                    if (lookaroundCoroutine == null) lookaroundCoroutine = StartCoroutine(LookaroundCoroutine());
                    mainCam.transform.position = new Vector3(mainCam.transform.position.x - (Time.deltaTime * lookaroundSpeed), mainCam.transform.position.y, mainCam.transform.position.z);
                }
                else if (Input.GetKey(KeyCode.D))
                {

                    lookingAround = true;
                    if (lookaroundCoroutine == null) lookaroundCoroutine = StartCoroutine(LookaroundCoroutine());
                    mainCam.transform.position = new Vector3(mainCam.transform.position.x + (Time.deltaTime * lookaroundSpeed), mainCam.transform.position.y, mainCam.transform.position.z);
                }
                else
                {
                    lookingAround = false;
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

        cameraScript.LerpToZoomedOutActivate();
        Invoke("ShowHead", 1.5f);
        timer = 0;
        timerLength = dialogueArray[currentDialogue].interviewTimer;

        // Safety check to avoid division by zero
        if (timerLength <= 0) timerLength = 0.1f;

        timerStart = true;
    }

    void ShowHead()
    {
        head.gameObject.SetActive(true);
        head.Play();
    }

    

    IEnumerator InterviewTextCoroutine()
    {
        string toread = dialogueArray[currentDialogue].mainDialogue;
        dialogueReader.TypeText(toread);

        robAnimator.Play("Normal");
        speakAudioSource.Play();

        yield return new WaitUntil(() => dialogueReader.typingFinished);
        robAnimator.Play("Idle");
        speakAudioSource.Stop();
        yield return new WaitForSeconds(speakingPause);
        if (dialogueArray[currentDialogue].isInterviewQuestion)
        {
            StartTimer();
        }
        else if(dialogueArray[currentDialogue].dialogueEnd == false)
        {
            //StartCoroutine(DialogueWaitCoroutine());
            NextDialogue();
        }
        else if(dialogueArray[currentDialogue].dialogueEnd)
        {
            Invoke("InterviewEnd", 6f);
        }
    }

    IEnumerator SuccessTextCoroutine()
    {
        string toread = dialogueArray[currentDialogue].successDialogue;
        dialogueReader.TypeText(toread);

        robAnimator.Play("Approve");
        effectAudioSource.PlayOneShot(bossLike);
        speakAudioSource.Play();

        yield return new WaitUntil(() => dialogueReader.typingFinished);
        robAnimator.Play("Idle");
        speakAudioSource.Stop();
        yield return new WaitForSeconds(speakingPause);
        InterviewSuccess();
        endTextCoroutine = null;
    }

    IEnumerator FailTextCoroutine()
    {
        string toread = dialogueArray[currentDialogue].failureDialogue;
        dialogueReader.TypeText(toread);

        robAnimator.Play("Confused");
        effectAudioSource.PlayOneShot(bossDislike);
        speakAudioSource.Play();

        yield return new WaitUntil(() => dialogueReader.typingFinished);
        robAnimator.Play("Idle");
        speakAudioSource.Stop();
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
        if (success)
        {
            robAnimator.Play("Approve");
            effectAudioSource.PlayOneShot(bossLike);
        }
        else
        {
            robAnimator.Play("Confused");
            effectAudioSource.PlayOneShot(bossDislike);
        }

        speakAudioSource.Play();



        yield return new WaitUntil(() => dialogueReader.typingFinished);
        robAnimator.Play("Idle");
        speakAudioSource.Stop();
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
        points += 1;
        //points +
    }

    void InterviewFailure()
    {
        NextDialogue();
        points -= 1;
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

    IEnumerator LookaroundCoroutine()
    {
        if (timer < timerLength) hideHeadTimeline.playableAsset = lookaroundTimeline;
        if (timer < timerLength) head.playableAsset = stopLookingTimeline;

        if (timer < timerLength) head.Stop();
        if (hideHeadTimeline.state != PlayState.Playing) hideHeadTimeline.Play();
        yield return new WaitUntil(() => lookingAround == false || timer >= timerLength);
        yield return new WaitUntil(() => hideHeadTimeline.state != PlayState.Playing);
        if (timer < timerLength) ShowHead();
        lookaroundCoroutine = null;
    }

    private void InterviewEnd()
    {
        gameEndScreen.SetActive(true);
    }
}
