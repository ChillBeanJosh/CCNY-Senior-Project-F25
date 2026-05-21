using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CutsceneController : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneFrame
    {
        public Image image;
        public bool fadeToBlack = false;
        public bool showTextAtStartOfFade = false;
        public bool useDefaults = true;
        public float fadeInTime = 1f;
        public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float fadeOutTime = 1f;
        public AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    [Header("Default Fade Settings")]
    [SerializeField] private float defaultFadeInTime = 1f;
    [SerializeField] private AnimationCurve defaultFadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float defaultFadeOutTime = 1f;
    [SerializeField] private AnimationCurve defaultFadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Cutscene Sequence")]
    [SerializeField] private List<CutsceneFrame> cutsceneFrames;
    [SerializeField] private string nextSceneName;
    [SerializeField] private Image blackImage;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    private int _currentIndex = 0;
    private Coroutine _fadeCoroutine;

    void Start()
    {
        if (cutsceneFrames == null || cutsceneFrames.Count == 0) return;

        // Initialize all images to 0 alpha except the first one
        for (int i = 0; i < cutsceneFrames.Count; i++)
        {
            if (cutsceneFrames[i].image != null)
            {
                Color c = cutsceneFrames[i].image.color;
                c.a = (i == 0) ? 1f : 0f;
                cutsceneFrames[i].image.color = c;
            }
        }

        // Initialize dialogue
        UpdateDialogueText(0);

        if (blackImage != null)
        {
            Color bc = blackImage.color;
            bc.a = 1f; // Start with black at 100%
            blackImage.color = bc;
            
            // Fade out black along frame one's fade in settings
            _fadeCoroutine = StartCoroutine(FadeFromBlack(0));
        }
    }

    private IEnumerator FadeFromBlack(int frameIndex)
    {
        CutsceneFrame frame = cutsceneFrames[frameIndex];
        float fadeInTime = frame.useDefaults ? defaultFadeInTime : frame.fadeInTime;
        AnimationCurve fadeInCurve = frame.useDefaults ? defaultFadeInCurve : frame.fadeInCurve;

        float elapsed = 0f;
        while (elapsed < fadeInTime)
        {
            elapsed += Time.deltaTime;
            float tIn = Mathf.Clamp01(elapsed / fadeInTime);
            float alphaBlack = 1f - fadeInCurve.Evaluate(tIn);
            Color c = blackImage.color;
            c.a = alphaBlack;
            blackImage.color = c;
            yield return null;
        }

        {
            Color c = blackImage.color;
            c.a = 0f;
            blackImage.color = c;
        }
        _fadeCoroutine = null;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextFrame();
        }
    }

    private void ShowNextFrame()
    {
        if (cutsceneFrames == null || cutsceneFrames.Count == 0) return;
        if (_currentIndex >= cutsceneFrames.Count - 1)
        {
            if (GameManager.Instance != null && !string.IsNullOrEmpty(nextSceneName))
            {
                GameManager.Instance.SwitchToScene(nextSceneName);
            }
            else
            {
                Debug.Log("Cutscene finished, but no next scene defined or GameManager missing.");
            }
            return;
        }

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        // Hide current dialogue upon click
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        _fadeCoroutine = StartCoroutine(TransitionFrames(_currentIndex, _currentIndex + 1));
        _currentIndex++;
    }

    private void UpdateDialogueText(int frameIndex)
    {
        if (dialogueData == null || dialogueText == null) return;
        
        // Assuming one line per frame across all sequences
        int globalLineCount = 0;
        foreach (var sequence in dialogueData.sequences)
        {
            if (sequence.lines == null) continue;
            for (int i = 0; i < sequence.lines.Length; i++)
            {
                if (globalLineCount == frameIndex)
                {
                    dialogueText.text = sequence.lines[i];
                    return;
                }
                globalLineCount++;
            }
        }
    }

    private IEnumerator TransitionFrames(int fromIndex, int toIndex)
    {
        CutsceneFrame prevFrame = cutsceneFrames[fromIndex];
        CutsceneFrame nextFrame = cutsceneFrames[toIndex];

        float fadeOutTime = prevFrame.useDefaults ? defaultFadeOutTime : prevFrame.fadeOutTime;
        AnimationCurve fadeOutCurve = prevFrame.useDefaults ? defaultFadeOutCurve : prevFrame.fadeOutCurve;

        float fadeInTime = nextFrame.useDefaults ? defaultFadeInTime : nextFrame.fadeInTime;
        AnimationCurve fadeInCurve = nextFrame.useDefaults ? defaultFadeInCurve : nextFrame.fadeInCurve;

        if (nextFrame.showTextAtStartOfFade && !prevFrame.fadeToBlack)
        {
            UpdateDialogueText(toIndex);
        }

        if (prevFrame.fadeToBlack && blackImage != null)
        {
            // Fade black image in using prevFrame's fade out curve
            float elapsed = 0f;
            while (elapsed < fadeOutTime)
            {
                elapsed += Time.deltaTime;
                float tOut = Mathf.Clamp01(elapsed / fadeOutTime);
                float alphaBlack = fadeOutCurve.Evaluate(tOut);
                Color c = blackImage.color;
                c.a = alphaBlack;
                blackImage.color = c;
                yield return null;
            }

            // Set final alpha for black and swap frames
            {
                Color c = blackImage.color;
                c.a = 1f;
                blackImage.color = c;
            }

            if (nextFrame.showTextAtStartOfFade)
            {
                UpdateDialogueText(toIndex);
            }

            if (prevFrame.image != null)
            {
                Color c = prevFrame.image.color;
                c.a = 0f;
                prevFrame.image.color = c;
            }

            if (nextFrame.image != null)
            {
                Color c = nextFrame.image.color;
                c.a = 1f;
                nextFrame.image.color = c;
            }

            // Fade black image out using nextFrame's fade in curve
            elapsed = 0f;
            while (elapsed < fadeInTime)
            {
                elapsed += Time.deltaTime;
                float tIn = Mathf.Clamp01(elapsed / fadeInTime);
                float alphaBlack = 1f - fadeInCurve.Evaluate(tIn);
                Color c = blackImage.color;
                c.a = alphaBlack;
                blackImage.color = c;
                yield return null;
            }

            // Ensure black is fully transparent
            {
                Color c = blackImage.color;
                c.a = 0f;
                blackImage.color = c;
            }
        }
        else
        {
            float elapsed = 0f;
            float maxDuration = Mathf.Max(fadeOutTime, fadeInTime);

            while (elapsed < maxDuration)
            {
                elapsed += Time.deltaTime;

                // Fade Out previous
                if (prevFrame.image != null)
                {
                    float tOut = Mathf.Clamp01(elapsed / fadeOutTime);
                    float alphaOut = 1f - fadeOutCurve.Evaluate(tOut);
                    Color c = prevFrame.image.color;
                    c.a = alphaOut;
                    prevFrame.image.color = c;
                }

                // Fade In next
                if (nextFrame.image != null)
                {
                    float tIn = Mathf.Clamp01(elapsed / fadeInTime);
                    float alphaIn = fadeInCurve.Evaluate(tIn);
                    Color c = nextFrame.image.color;
                    c.a = alphaIn;
                    nextFrame.image.color = c;
                }

                yield return null;
            }
        }

        // Ensure final values
        if (prevFrame.image != null)
        {
            Color c = prevFrame.image.color;
            c.a = 0f;
            prevFrame.image.color = c;
        }

        if (nextFrame.image != null)
        {
            Color c = nextFrame.image.color;
            c.a = 1f;
            nextFrame.image.color = c;
        }

        if (!nextFrame.showTextAtStartOfFade)
        {
            UpdateDialogueText(toIndex);
        }

        _fadeCoroutine = null;
    }
}
