using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteshapeAnim : MonoBehaviour
{
    [SerializeField] SpriteShapeController mSpriteShape;
    Spline mSpline;
    [SerializeField] GameObject movePoint;
    [SerializeField] Transform[] splinePoints;
    [SerializeField] Slider rightMungipaSlider;
    [SerializeField] Slider leftMungipaSlider;
    [SerializeField] Slider upperLipSlider;
    [SerializeField] Slider lowerLipSlider;
    [SerializeField] Slider rightEyebrowSlider;
    [SerializeField] Slider leftEyebrowSlider;
    [SerializeField] Slider leftEyeSlider;
    [SerializeField] Slider rightEyeSlider;
    [SerializeField] Vector3 rightMungipaStartPosition;
    [SerializeField] Vector3 leftMungipaStartPosition;
    [SerializeField] Vector3 upperLipStartPosition;
    [SerializeField] Vector3 lowerLipStartPosition;
    [SerializeField] Vector3 rightEyebrowStartPosition;
    [SerializeField] Vector3 leftEyebrowStartPosition;
    [SerializeField] Vector3 leftEyeStartPosition;
    [SerializeField] Vector3 rightEyeStartPosition;
    [SerializeField] Slider[] sliders;
    int sliderCount;
    float currentUpperLipSliderValue;
    float currentLowerLipSliderValue;
    float currentRightEyebrowSliderValue;
    float currentLeftEyebrowSliderValue;
    float currentLeftEyeSliderValue;
    float currentRightEyeSliderValue;
    int pointCount;
    [SerializeField] bool isMouth, isEyebrow, isEye;
    private void Start() 
    { 
        sliderCount = sliders.Length;
        pointCount = mSpriteShape.spline.GetPointCount();
        
        // Instantiate move points at each spline point position
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 position = mSpriteShape.spline.GetPosition(i);
            GameObject pointObj = Instantiate(movePoint, transform);
            pointObj.transform.localPosition = position;
            splinePoints[i] = pointObj.transform;
        }
        if(isMouth)
        {
            rightMungipaStartPosition = splinePoints[2].transform.position;
            leftMungipaStartPosition = splinePoints[0].transform.position;
            upperLipStartPosition = splinePoints[1].transform.position;
            lowerLipStartPosition = splinePoints[3].transform.position;
        }
        if(isEyebrow)
        {
            rightEyebrowStartPosition = splinePoints[0].transform.position;
            leftEyebrowStartPosition = splinePoints[2].transform.position;
        }
        if(isEye)
        {
            leftEyeStartPosition = splinePoints[1].transform.position;
            rightEyeStartPosition = splinePoints[3].transform.position;
        }
        
        // Set initial positions
        for (int i = 0; i < Mathf.Min(pointCount, splinePoints.Length); i++)
        {
            if (splinePoints[i] != null)
            {
                mSpriteShape.spline.SetPosition(i, splinePoints[i].localPosition);
            }
        }

        for (int i = 0; i < sliderCount; i++)
        {
            if (sliders[i] != null)
            {
                sliders[i].value = Random.Range(0.2f, 0.8f);
            }
        }
    }
   private void FixedUpdate()
    {
                int random = Random.Range(0, 20);
        if(random >= 15)
        {
                  for (int i = 0; i < sliderCount; i++)
        {
            if (sliders[i] != null)
            {
                var random1 = Random.Range(0, 50);
                
                if(random1 <= 50)
                    sliders[i].value = sliders[i].value + Random.Range(-0.01f, 0.01f);
                else
                    sliders[i].value = sliders[i].value - Random.Range(-0.01f, 0.01f);
                
            }
        }  
        }
    }
    private void Update()
    {
        if(isMouth)
        {
            UpdateRightMungipa();
            UpdateLeftMungipa();
            UpdateUpperLip();
            UpdateLowerLip();
        }
        if(isEyebrow)
        {
            UpdateRightEyebrow();
            UpdateLeftEyebrow();
        }
        if(isEye)
        {
            UpdateLeftEye();
            UpdateRightEye();
        }

        int pointCount = mSpriteShape.spline.GetPointCount();
        int updateCount = Mathf.Min(pointCount, splinePoints.Length);
        
        for (int i = 0; i < updateCount; i++)
        {
            if (splinePoints[i] != null)
            {
                mSpriteShape.spline.SetPosition(i, splinePoints[i].localPosition);
            }
        }
    }

    public void UpdateRightMungipa()
    {
        float sliderValue = rightMungipaSlider.value;
        splinePoints[2].transform.position = new Vector3(
            splinePoints[2].transform.position.x,
            Mathf.Lerp(rightMungipaStartPosition.y - 2f, rightMungipaStartPosition.y + 2f, sliderValue),
            splinePoints[2].transform.position.z
        );
    }

    public void UpdateLeftMungipa()
    {
        float sliderValue = leftMungipaSlider.value;
        splinePoints[0].transform.position = new Vector3(
            splinePoints[0].transform.position.x,
            Mathf.Lerp(leftMungipaStartPosition.y - 2f, leftMungipaStartPosition.y + 2f, sliderValue),
            splinePoints[0].transform.position.z
        );
    }

    public void UpdateUpperLip()
    {
        float sliderValue = upperLipSlider.value;
        float minY = lowerLipStartPosition.y + 0.1f; // Keep upper lip slightly above lower lip start
        float maxY = upperLipStartPosition.y + 2f;
        
        float targetY = Mathf.Lerp(upperLipStartPosition.y - 2f, upperLipStartPosition.y + 2f, sliderValue);
        targetY = Mathf.Clamp(targetY, minY, maxY);
        
        splinePoints[1].transform.position = new Vector3(
            splinePoints[1].transform.position.x,
            targetY,
            splinePoints[1].transform.position.z
        );
        
        currentUpperLipSliderValue = sliderValue;
    }

    public void UpdateLowerLip()
    {
        float sliderValue = lowerLipSlider.value;
        float maxY = upperLipStartPosition.y - 0.1f; // Keep lower lip slightly below upper lip start
        float minY = lowerLipStartPosition.y - 2f;
        
        float targetY = Mathf.Lerp(lowerLipStartPosition.y - 2f, lowerLipStartPosition.y + 2f, sliderValue);
        targetY = Mathf.Clamp(targetY, minY, maxY);
        
        splinePoints[3].transform.position = new Vector3(
            splinePoints[3].transform.position.x,
            targetY,
            splinePoints[3].transform.position.z
        );
        
        currentLowerLipSliderValue = sliderValue;
    }

    public void UpdateRightEyebrow()
    {
        float sliderValue = rightEyebrowSlider.value;
        splinePoints[0].transform.position = new Vector3(
            splinePoints[0].transform.position.x,
            Mathf.Lerp(rightEyebrowStartPosition.y - 2f, rightEyebrowStartPosition.y + 2f, sliderValue),
            splinePoints[0].transform.position.z
        );
    }

    public void UpdateLeftEyebrow()
    {
        float sliderValue = leftEyebrowSlider.value;
        splinePoints[2].transform.position = new Vector3(
            splinePoints[2].transform.position.x,
            Mathf.Lerp(leftEyebrowStartPosition.y - 2f, leftEyebrowStartPosition.y + 2f, sliderValue),
            splinePoints[2].transform.position.z
        );
    }

    public void UpdateLeftEye()
    {
        float sliderValue = leftEyeSlider.value;
        splinePoints[1].transform.position = new Vector3(
            splinePoints[1].transform.position.x,
            Mathf.Lerp(leftEyeStartPosition.y - 2f, leftEyeStartPosition.y + 2f, sliderValue),
            splinePoints[1].transform.position.z
        );
    }

    public void UpdateRightEye()
    {
        float sliderValue = rightEyeSlider.value;
        splinePoints[3].transform.position = new Vector3(
            splinePoints[3].transform.position.x,
            Mathf.Lerp(rightEyeStartPosition.y - 2f, rightEyeStartPosition.y + 2f, sliderValue),
            splinePoints[3].transform.position.z
        );
    }
}
