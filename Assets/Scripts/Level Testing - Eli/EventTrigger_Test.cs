using NUnit.Framework.Internal;
using UnityEngine;

public class EventTrigger_Test : MonoBehaviour
{
    [SerializeField] Animator[] animator;
    [SerializeField] GameObject[] Objects;
    [SerializeField] MovePlatform_Test[] PlatformEvents;

    [Header("If_TimedEvent")]
    public bool TimedEvent = false;
    public float EventTime = 3f;
    float EventTimer;

    [Header("If_Animation")]
    public bool AnimationEvent = false;

    private void Start()
    {
        if (animator != null)
        {
            //Disable all animators at start
            for (int i = 0; i < animator.Length; i++)
                animator[i].enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //If is not timed event, do action immediately
        if (!TimedEvent)
        {
            EventActions();
        }
        else
        {
            //Restart Timer when entering trigger
            EventTimer = EventTime;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //If is timed event, do action after countdown
        if (TimedEvent)
        {
            EventTimer -= Time.deltaTime;
            if (EventTimer < 0)
            {
                EventActions();
            }
        }
    }

    void EventActions()
    {
        // Deactivate all objects in the Objects array
        for (int i = 0; i < Objects.Length; i++)
            Objects[i].SetActive(false);

        //If Event uses Animations
        if (AnimationEvent)
        {
            //Play animations
            if (animator != null)
            {
                for (int i = 0; i < animator.Length; i++)
                    animator[i].enabled = true;
            }
        }
        else
        {
            //Move Platforms
            if (PlatformEvents != null)
                for (int i = 0; i < PlatformEvents.Length; i++)
                PlatformEvents[i].MovePlatform = true;
        }
    }
}
