using TMPro;
using UnityEngine;

public class TMPFitToBox : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    public float maxFontSize = 36f;
    public float minFontSize = 10f;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        FitText();
    }

    public void FitText()
    {
        tmp.enableAutoSizing = false;
        tmp.fontSize = maxFontSize;

        // Force TMP to update layout info
        tmp.ForceMeshUpdate();

        while (tmp.isTextOverflowing && tmp.fontSize > minFontSize)
        {
            tmp.fontSize -= 1f;
            tmp.ForceMeshUpdate();
        }
    }
}