using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform bossTransform;
    [SerializeField] private float lerpSpeed = 5f;
    public bool lerpToBoss = false;
    Vector3 zoomedOutPosition;
    Vector3 zoomedInPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        zoomedOutPosition = new Vector3(-1.8f, 0f, -9.28f);
        zoomedInPosition = new Vector3(-1.74f, 1.48f, -2.94f);
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
    }

    private void LerpToBoss()
    {
        if (bossTransform == null || cameraTransform == null) return;

        float threshold = 0.05f;
        if (Mathf.Abs(cameraTransform.position.x - bossTransform.position.x) < threshold)
        {
            cameraTransform.position = new Vector3(zoomedInPosition.x, zoomedInPosition.y, zoomedInPosition.z);
            lerpToBoss = false;
            return;
        }

        float targetX = Mathf.Lerp(cameraTransform.position.x, bossTransform.position.x, lerpSpeed * Time.deltaTime);
        cameraTransform.position = new Vector3(targetX, zoomedInPosition.y, zoomedInPosition.z);
    }   

    private void LerpToZoomedOut()
    {
        if (bossTransform == null || cameraTransform == null) return;

        float threshold = 0.05f;
        if (Mathf.Abs(cameraTransform.position.x - bossTransform.position.x) < threshold)
        {
            cameraTransform.position = new Vector3(zoomedOutPosition.x, zoomedOutPosition.y, zoomedOutPosition.z);
            lerpToBoss = false;
            return;
        }

        float targetX = Mathf.Lerp(cameraTransform.position.x, bossTransform.position.x, lerpSpeed * Time.deltaTime);
        cameraTransform.position = new Vector3(targetX, zoomedOutPosition.y, zoomedOutPosition.z);
    }
}
