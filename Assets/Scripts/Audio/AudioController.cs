using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [System.Serializable]
    public struct Sound
    {
        public string name;
        public AudioClip clip;
    }

    [Header("Sound Clips:")]
    public Sound[] sounds;
    private AudioSource _defaultSource;
    private Dictionary<string, AudioSource> _activeLoops = new Dictionary<string, AudioSource>();

    public static AudioController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _defaultSource = GetComponent<AudioSource>();
    }

    // ------------------------------------------------------------------------------------------------------------

    public AudioClip GetSound(string name)
    {
        //Getter Function To Retrieve An AudioClip By Name From The Sounds Array:
        foreach (Sound sound in sounds)
        {
            if (sound.name == name)
            {
                return sound.clip;
            }
        }
        Debug.LogWarning("THE SOUND: '" + name + "' DOES NOT EXIST.");
        return null;
    }

    // ------------------------------------------------------------------------------------------------------------
    public void Play(string name, float speed = 1f, float volume = 1f)
    {
        //If Audio We Are Playing Is Already Looping, Don't Start Another Loop:
        if (_activeLoops.ContainsKey(name)) return;

        //Create A New AudioSource For This Looping Sound:
        AudioClip clip = GetSound(name);
        if (clip != null)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();

            newSource.clip = clip;
            newSource.loop = true;
            newSource.pitch = speed;
            newSource.volume = volume;
            newSource.Play();

            _activeLoops.Add(name, newSource);
        }
    }

    // ------------------------------------------------------------------------------------------------------------
    public void Stop(string name)
    {
        //If We Have An Active Loop With This Name, Stop It And Clean Up:
        if (_activeLoops.ContainsKey(name))
        {
            _activeLoops[name].Stop();

            Destroy(_activeLoops[name]); 

            _activeLoops.Remove(name);
        }
    }

    // ------------------------------------------------------------------------------------------------------------
    public void OneShot(string name, float speed = 1f, float volume = 1f)
    {
        //Play A One-Shot Sound Using The Default AudioSource:
        AudioClip clip = GetSound(name);
        if (clip != null)
        {
            _defaultSource.pitch = speed;
            _defaultSource.volume = volume;
            _defaultSource.PlayOneShot(clip);
        }
    }

    // ------------------------------------------------------------------------------------------------------------
    public void RandomizedOneShot(string name, float minDuration, float maxDuration, float speed = 1f, float volume = 1f, bool useFade = false)
    {
        //Play A Random Snippet Of The Sound With Optional Fade In/Out:
        AudioClip clip = GetSound(name);
        if (clip != null)
        {
            StartCoroutine(PlayRandomSnippet(clip, minDuration, maxDuration, speed, volume, useFade));
        }
    }

    private IEnumerator PlayRandomSnippet(AudioClip clip, float min, float max, float speed, float volume, bool useFade)
    {
        AudioSource tempSource = gameObject.AddComponent<AudioSource>();

        float startTime = Random.Range(0f, Mathf.Max(0, clip.length - max));
        float duration = Random.Range(min, max);
        float fadeTime = 0.5f;

        tempSource.clip = clip;
        tempSource.time = startTime;
        tempSource.pitch = speed;
        tempSource.volume = volume;

        // If using fade, start at 0 volume and fade in:
        if (useFade)
        {
            tempSource.volume = 0f;
            tempSource.Play();
            yield return StartCoroutine(FadeAudio(tempSource, 0f, 1f, fadeTime));
        }
        else
        {
            tempSource.Play();
        }

        float waitTime = (duration / speed);
        yield return new WaitForSeconds(useFade ? Mathf.Max(0, waitTime - fadeTime) : waitTime);

        //If using fade, fade out before stopping and destroying the source:
        if (useFade)
        {
            yield return StartCoroutine(FadeAudio(tempSource, 1f, 0f, fadeTime));
        }

        // Stop and clean up the temporary audio source:
        if (tempSource != null)
        {
            tempSource.Stop();
            Destroy(tempSource);
        }
    }

    private IEnumerator FadeAudio(AudioSource source, float startVol, float endVol, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            if (source == null) yield break;

            //Lerp volume based on timer:
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, endVol, timer / duration);

            yield return null;
        }
        source.volume = endVol;
    }

    // ------------------------------------------------------------------------------------------------------------
}
