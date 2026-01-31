using UnityEngine;
using UnityEngine.U2D;

public class SpriteshapeAnim : MonoBehaviour
{
    [SerializeField] SpriteShapeController mSpriteShape;
    Spline mSpline;
    [SerializeField] Transform[] splinePoints;

    private void Start()
    {
        mSpriteShape.spline.SetPosition(0, splinePoints[0].localPosition);
        mSpriteShape.spline.SetPosition(1, splinePoints[1].localPosition);
        mSpriteShape.spline.SetPosition(2, splinePoints[2].localPosition);

        Debug.Log(mSpriteShape.spline.GetPosition(0) + "  point: " + splinePoints[0].position);
        Debug.Log(mSpriteShape.spline.GetPosition(1) + "  point: " + splinePoints[1].position);
        Debug.Log(mSpriteShape.spline.GetPosition(2) + "  point: " + splinePoints[2].position);
    }

    private void Update()
    {
        mSpriteShape.spline.SetPosition(0, splinePoints[0].localPosition);
        mSpriteShape.spline.SetPosition(1, splinePoints[1].localPosition);
        mSpriteShape.spline.SetPosition(2, splinePoints[2].localPosition);
    }
}
