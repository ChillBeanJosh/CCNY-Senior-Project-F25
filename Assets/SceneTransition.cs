using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneTransition : MonoBehaviour
{
    public Image FaderImage;
    public AnimationCurve FadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float FadeTime = 1f;
    public float WaitTime = 0.5f;

    public static event Action<string> SceneChange;

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

        if (FaderImage != null)
        {
            Color c = FaderImage.color;
            c.a = 0;
            FaderImage.color = c;
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
        if (FaderImage != null)
        {
            Color c = FaderImage.color;
            c.a = alpha;
            FaderImage.color = c;
        }
    }
}
