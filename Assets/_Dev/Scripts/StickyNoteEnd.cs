using TMPro;
using UnityEngine;

public class StickyNoteEnd : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mText;
    [SerializeField] Interview interview;


    [SerializeField, TextArea(3, 5)] string goodNote;
    [SerializeField, TextArea(3, 5)] string midNote;
    [SerializeField, TextArea(3, 5)] string badNote;

    private void OnEnable()
    {
        if (interview.points > 5) mText.text = goodNote + "\n" + interview.points + " Points";
        else if (interview.points >= 0) mText.text = midNote + "\n" + interview.points + " Points";
        else if (interview.points < 0) mText.text = badNote + "\n" + interview.points + " Points";
    }

}
