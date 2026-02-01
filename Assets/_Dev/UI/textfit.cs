using TMPro;
using UnityEngine;

public class TMPAutoFit : MonoBehaviour
{
    private TextMeshProUGUI tmp;

    [Header("Font Size Limits")]
    public float maxFontSize = 36f;
    public float minFontSize = 10f;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();

        tmp.enableAutoSizing = true;
        tmp.fontSizeMax = maxFontSize;
        tmp.fontSizeMin = minFontSize;
    }
}