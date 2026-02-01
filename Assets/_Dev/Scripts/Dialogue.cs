using System.Collections;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "DialogueSO", menuName = "DialogueSO")]
public class Dialogue : ScriptableObject
{
    public enum Emotion
    {
        Neutral,
        Happy,
        Sad,
        Angry,
        Shocked,
        Bored,
        Crazy
    }

    [System.Serializable]
    public struct ReactDialogue
    {
        [SerializeField, TextArea(3, 8)] public string dialogue;
        public Emotion emotion;
        public bool success; 
    }

    [SerializeField, TextArea(3, 8)] public string mainDialogue;
    [SerializeField, TextArea(3, 8)] public string successDialogue;
    [SerializeField, TextArea(3, 8)] public string failureDialogue;

    [SerializeField] public bool isInterviewQuestion;
    [SerializeField] public float interviewTimer = 20f;
    [SerializeField] public bool dialogueEnd = false;

    [SerializeField] public ReactDialogue[] reactDialogues;

    public Emotion requiredEmotion;
    //Fail/win state requirement

    
    //
}
