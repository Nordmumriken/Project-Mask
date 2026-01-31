using UnityEngine;
using UnityEngine.Playables;

public class TimelineAnimate : MonoBehaviour
{
    [SerializeField] PlayableDirector mPlayableDirector;
    [SerializeField] float middleTime;
    [SerializeField] float maxTime;

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)) {
            mPlayableDirector.Play();
            SetCurrentTime(Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.DownArrow)) {
            mPlayableDirector.Play();
            SetCurrentTime(-Time.deltaTime);
        }
        else
        {
            mPlayableDirector.Pause();
        }

    }

    private void SetCurrentTime(float time) 
    {
        mPlayableDirector.time = Mathf.Clamp((float)mPlayableDirector.time + time, 0, float.MaxValue);  
    }
}
