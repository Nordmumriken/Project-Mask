using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)] // Run before other updates to ensure targets are set
public class SpriteshapeAnim : MonoBehaviour
{
    [SerializeField] SpriteShapeController mSpriteShape;
    [SerializeField] GameObject movePoint;
    [SerializeField] Transform[] splinePoints;
    
    [Header("Sliders")]
    [SerializeField] Slider rightMungipaSlider;
    [SerializeField] Slider leftMungipaSlider;
    [SerializeField] Slider upperLipSlider;
    [SerializeField] Slider lowerLipSlider;
    [SerializeField] Slider rightEyebrowSlider;
    [SerializeField] Slider leftEyebrowSlider;
    [SerializeField] Slider leftEyeSlider;
    [SerializeField] Slider rightEyeSlider;
    
    [Header("Start Positions (Local)")]
    [SerializeField] Vector3[] startLocalPositions;
    
    [SerializeField] Slider[] sliders;
    int sliderCount;
    int pointCount;
    [SerializeField] bool isMouth, isEyebrow, isEye;
    [SerializeField] float movementRange = 2f;

    private void Start() 
    { 
        if (mSpriteShape == null) mSpriteShape = GetComponent<SpriteShapeController>();
        
        pointCount = mSpriteShape.spline.GetPointCount();
        splinePoints = new Transform[pointCount];
        startLocalPositions = new Vector3[pointCount];
        
        // Instantiate move points at each spline point position
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 localPos = mSpriteShape.spline.GetPosition(i);
            GameObject pointObj = Instantiate(movePoint, transform);
            pointObj.transform.localPosition = localPos;
            splinePoints[i] = pointObj.transform;
            startLocalPositions[i] = localPos;
        }
        
        // Sync initial state
        UpdateSplinePoints();
    }

    public void RandomizeFace()
    {
        sliderCount = sliders.Length;
        for (int i = 0; i < sliderCount; i++)
        {
            if (sliders[i] != null)
            {
                sliders[i].value = Random.Range(0.4f, 0.7f);
            }
        }
    }

    private void DragPoints()
    {
        int random = Random.Range(0, 20);
        if(random >= 5)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                if (sliders[i] != null)
                {
                    var random1 = Random.Range(0, 100);
                    if(random1 <= 50)
                        sliders[i].value += Random.Range(-0.0005f, 0.0005f);
                    else
                        sliders[i].value -= Random.Range(-0.0005f, 0.0005f);
                }
            }  
        }
    }

    private void Update()
    {
        DragPoints();
        
        if(isMouth)
        {
            UpdateMouth();
        }
        if(isEyebrow)
        {
            UpdateEyebrows();
        }
        if(isEye)
        {
            UpdateEyes();
        }

        UpdateSplinePoints();
    }

    private void UpdateSplinePoints()
    {
        for (int i = 0; i < splinePoints.Length; i++)
        {
            if (splinePoints[i] != null)
            {
                // ALWAYS use localPosition to prevent warping when the parent moves
                mSpriteShape.spline.SetPosition(i, splinePoints[i].localPosition);
            }
        }
    }

    private void UpdateMouth()
    {
        // 0: Left Mouth Corner, 1: Upper Lip, 2: Right Mouth Corner, 3: Lower Lip
        if (leftMungipaSlider) SetPointY(0, leftMungipaSlider.value, -movementRange, movementRange);
        if (rightMungipaSlider) SetPointY(2, rightMungipaSlider.value, -movementRange, movementRange);

        // Advanced Lip Clamping (Local Space)
        // We calculate world-like collisions using local coordinates
        float lowerLipCurrentLocalY = splinePoints[3].localPosition.y;
        float upperLipCurrentLocalY = splinePoints[1].localPosition.y;

        if (upperLipSlider) 
        {
            float minLocalY = lowerLipCurrentLocalY + 0.1f;
            SetPointY(1, upperLipSlider.value, -movementRange, movementRange, minLocalY, startLocalPositions[1].y + movementRange);
        }

        if (lowerLipSlider) 
        {
            float maxLocalY = upperLipCurrentLocalY - 0.1f;
            SetPointY(3, lowerLipSlider.value, -movementRange, movementRange, startLocalPositions[3].y - movementRange, maxLocalY);
        }
    }
    private void UpdateEyebrows()
    {
        // 0: Right, 2: Left
        if (rightEyebrowSlider) SetPointY(0, rightEyebrowSlider.value, -movementRange, movementRange);
        if (leftEyebrowSlider) SetPointY(2, leftEyebrowSlider.value, -movementRange, movementRange);
    }

    private void UpdateEyes()
    {
        // 1: Left, 3: Right
        if (leftEyeSlider) SetPointY(1, leftEyeSlider.value, -movementRange, movementRange);
        if (rightEyeSlider) SetPointY(3, rightEyeSlider.value, -movementRange, movementRange);
    }

    private void SetPointY(int index, float sliderValue, float minOffset, float maxOffset, float absoluteMin = float.MinValue, float absoluteMax = float.MaxValue)
    {
        if (index >= splinePoints.Length) return;
        
        Vector3 pos = splinePoints[index].localPosition;
        float baseY = startLocalPositions[index].y;
        
        // Calculate target Y
        float targetY = Mathf.Lerp(baseY + minOffset, baseY + maxOffset, sliderValue);
        
        // Apply clamping (useful for lips)
        pos.y = Mathf.Clamp(targetY, absoluteMin, absoluteMax);
        
        splinePoints[index].localPosition = pos;
    }
}
