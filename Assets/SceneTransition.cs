using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using MoreMountains.Feedbacks;

public class SceneTransition : MonoBehaviour
{
    public List<Image> FaderImages;
    public MMF_Player EntrancePlayer;
    public MMF_Player ExitPlayer;
    public AnimationCurve FadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float FadeTime = 1f;
    public float WaitTime = 0.5f;

    public static event Action<string> SceneChange;

    public static bool HasListeners()
    {
        return SceneChange != null;
    }

    public static void InvokeSceneChange(string sceneName)
    {
        SceneChange?.Invoke(sceneName);
    }

    private static SceneTransition instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (FaderImages != null)
        {
            foreach (var faderImage in FaderImages)
            {
                if (faderImage != null)
                {
                    Color c = faderImage.color;
                    c.a = 0;
                    faderImage.color = c;
                }
            }
        }
    }

    void OnEnable()
    {
        SceneChange += HandleSceneChange;
    }

    void OnDisable()
    {
        SceneChange -= HandleSceneChange;
    }

    private void HandleSceneChange(string sceneName)
    {
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        // Fade Out
        if (EntrancePlayer != null)
        {
            EntrancePlayer.PlayFeedbacks();
        }

        float elapsed = 0f;
        while (elapsed < FadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = FadeCurve.Evaluate(elapsed / FadeTime);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);

        // Stay black
        yield return new WaitForSeconds(WaitTime);

        // Change Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Fade In
        if (ExitPlayer != null)
        {
            ExitPlayer.PlayFeedbacks();
        }

        elapsed = 0f;
        while (elapsed < FadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = FadeCurve.Evaluate(1f - (elapsed / FadeTime));
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (FaderImages != null)
        {
            foreach (var faderImage in FaderImages)
            {
                if (faderImage != null)
                {
                    Color c = faderImage.color;
                    c.a = alpha;
                    faderImage.color = c;
                }
            }
        }
    }
}
