using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform bossTransform;
    [SerializeField] private float lerpSpeed = 3f;
    public bool lerpToBoss = false;
    public bool lerpToZoomedOut = false;
    Vector3 zoomedOutPosition;
    Vector3 zoomedInPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        zoomedOutPosition = new Vector3(-1.8f, 0f, -9.28f);
        zoomedInPosition = new Vector3(-1.74f, 1.48f, -2.94f);

        // SAFETY: Non-uniform scale on the camera (e.g. 1.1, 1, 1) is the #1 cause of 
        // child objects "warping" or "shearing" when the camera moves.
        if (cameraTransform != null)
        {
            cameraTransform.localScale = Vector3.one;
        }
    }
  //zoomed out position boss: -1.8, 0, -9.28
  //zoomed in position boss: -1.74, 1.48, -2.94
    // Update is called once per frame
    void Update()
    {
        if (lerpToBoss)
        {
            LerpToBoss();
        }
        if (lerpToZoomedOut)
        {
            LerpToZoomedOut();
        }
    }

    public void LerpToBossActivate()
    {
        lerpToBoss = true;
        lerpToZoomedOut = false;
    }

    public void LerpToZoomedOutActivate()
    {
        lerpToZoomedOut = true;
        lerpToBoss = false;
    }

    private void LerpToBoss()
    {
        if (cameraTransform == null) return;

        // Use Vector3.Distance for a more accurate check across all axes
        if (Vector3.Distance(cameraTransform.position, zoomedInPosition) < 0.01f)
        {
            cameraTransform.position = zoomedInPosition;
            lerpToBoss = false;
            return;
        }

        // Lerp the entire position vector for smooth movement in all directions
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, zoomedInPosition, lerpSpeed * Time.deltaTime);
    }   

    private void LerpToZoomedOut()
    {
        if (cameraTransform == null) return;

        if (Vector3.Distance(cameraTransform.position, zoomedOutPosition) < 0.01f)
        {
            cameraTransform.position = zoomedOutPosition;
            lerpToZoomedOut = false;
            return;
        }

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, zoomedOutPosition, lerpSpeed * Time.deltaTime);
    }
}
