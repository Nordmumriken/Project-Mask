using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueReader : MonoBehaviour
{
    [SerializeField] float typingSpeed;

    [SerializeField] TextMeshProUGUI dialougeUIText;

    [SerializeField] AudioSource speakingSource;



    public bool typingFinished { get; private set; }
    private float startTypingSpeed;

    private string currentText;
    

    IEnumerator TypeTextCoroutine(string sentence)
    {
        //dialogueVoice.Pronounce(sentence);
        dialougeUIText.text = "";
        string voiceText = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialougeUIText.text += letter;
            if (letter == ' ' || letter == '.' || letter == '?' || letter == '!' || letter == ';' || letter == ':')
            {

                voiceText = "";

            }
            else
            {

                voiceText += letter;

            }

            yield return new WaitForSeconds(typingSpeed);
        }
        typingFinished = true;
        //TypingFinished();
    }

    public void TypeText(string sentence)
    {
        typingFinished = false;
        currentText = sentence;
        StartCoroutine("TypeTextCoroutine", sentence);
    }

    public void SkipText( )
    {
        if (typingSpeed == startTypingSpeed)
            typingSpeed *= 0.01f;
        else
        {
            StopAllCoroutines();
            typingFinished = true;
            dialougeUIText.text = currentText;
        }
    }

}
