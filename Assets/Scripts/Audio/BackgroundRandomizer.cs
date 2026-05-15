using System.Collections.Generic;
using UnityEngine;

public class BackgroundRandomizer : MonoBehaviour
{
    [Header("Audio Settings")]
    public List<string> soundPool;

    [Header("Interval Settings:")]
    public float minWaitTime = 5f;
    public float maxWaitTime = 15f;

    [Header("Snippet Settings:")]
    public float minSnippetDuration = 0.5f;
    public float maxSnippetDuration = 2.0f;
    public float playbackSpeed = 1f;

    private float _nextEventTime;
    private float _timer;

    void Start()
    {
        SetRandomTimer();
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _nextEventTime)
        {
            TriggerRandomSound();
            SetRandomTimer();
        }
    }
    private void SetRandomTimer()
    {
        //Randomize the timer for the next event:
        _timer = 0f;
        _nextEventTime = Random.Range(minWaitTime, maxWaitTime);
    }

    private void TriggerRandomSound()
    {
        //Check If Sound Pool Is Empty:
        if (soundPool == null || soundPool.Count == 0) return;

        //Select A Random Sound From The Pool:
        string randomSoundName = soundPool[Random.Range(0, soundPool.Count)];

        //Play A Random Snippet Of The Selected Sound:
        AudioController.Instance.RandomizedOneShot(
            randomSoundName,
            minSnippetDuration,
            maxSnippetDuration,
            playbackSpeed,
            useFade: true
        );
    }
}
