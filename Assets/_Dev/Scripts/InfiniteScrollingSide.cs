using UnityEngine;

[DefaultExecutionOrder(100)]
public class InfiniteScrollingSide : MonoBehaviour
{
    [Header("Infinite Setup")]
    public Transform targetCamera;

    private float _worldSpriteWidth;
    private bool _initialized;
    private bool _isChildOnLeft;

    void Start()
    {
        if (targetCamera == null && Camera.main != null)
            targetCamera = Camera.main.transform;

        // AUTOMATIC DETECTION
        if (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            
            // Calculate world space difference between parent and child
            // This is crucial to support scaled backgrounds
            float localDiff = child.localPosition.x;
            _worldSpriteWidth = Mathf.Abs(localDiff * transform.lossyScale.x);
            _isChildOnLeft = localDiff < 0;
            
            if (_worldSpriteWidth > 0.001f) _initialized = true;
        }
        else
        {
            Debug.LogError("InfiniteScrollingSide: No child found! To work, this script needs the second background to be a child of this object.");
        }
    }

    void LateUpdate()
    {
        if (!_initialized || targetCamera == null) return;

        float camX = targetCamera.position.x;
        
        // We handle movement in a loop to support fast-moving cameras (multiple jumps in one frame)
        // and to ensure we always stay within the safe window.
        
        int safetyNet = 0;
        bool moved = true;
        
        while (moved && safetyNet < 10)
        {
            moved = false;
            safetyNet++;
            
            float myX = transform.position.x;
            float relativeX = myX - camX;
            
            // Hysteresis margin to prevent flickering at the threshold
            float margin = 0.1f * _worldSpriteWidth;

            if (_isChildOnLeft)
            {
                // [Child] [Parent]
                // If camera moves too far right of parent
                if (relativeX < -0.1f * _worldSpriteWidth)
                {
                    transform.position += new Vector3(_worldSpriteWidth, 0, 0);
                    moved = true;
                }
                // If camera moves too far left of parent
                else if (relativeX > 0.9f * _worldSpriteWidth + margin)
                {
                    transform.position -= new Vector3(_worldSpriteWidth, 0, 0);
                    moved = true;
                }
            }
            else
            {
                // [Parent] [Child]
                // If camera moves too far left of parent
                if (relativeX > 0.1f * _worldSpriteWidth)
                {
                    transform.position -= new Vector3(_worldSpriteWidth, 0, 0);
                    moved = true;
                }
                // If camera moves too far right of parent
                else if (relativeX < -0.9f * _worldSpriteWidth - margin)
                {
                    transform.position += new Vector3(_worldSpriteWidth, 0, 0);
                    moved = true;
                }
            }
        }
    }
}
