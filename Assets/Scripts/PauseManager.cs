using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Feedbacks")]
    public MMF_Player PauseEntrance;
    public MMF_Player PauseExit;

    [Header("TMP & Image Animation")]
    public List<TextMeshProUGUI> TargetTexts;
    public Image TargetImage;
    public List<GameObject> LeftGameObjects;
    public List<GameObject> RightGameObjects;
    public Vector2 MovementVector;
    public float StartSpacing = 0f;
    public float EndSpacing = 10f;
    public Color StartColor = Color.white;
    public Color EndColor = Color.white;
    public float AnimationDuration = 2f;
    public AnimationCurve AnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool _isPaused = false;
    private Coroutine _animationCoroutine;
    private Dictionary<RectTransform, Vector2> _leftInitialPositions = new Dictionary<RectTransform, Vector2>();
    private Dictionary<RectTransform, Vector2> _rightInitialPositions = new Dictionary<RectTransform, Vector2>();
    public bool IsPaused => _isPaused;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PausePlayerControl();
        }

        if (PauseEntrance != null)
        {
            PauseEntrance.PlayerTimescaleMode = TimescaleModes.Unscaled;
            PauseEntrance.PlayFeedbacks();
        }

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        // Store initial positions of LeftGameObjects and RightGameObjects before starting animation
        _leftInitialPositions.Clear();
        foreach (var go in LeftGameObjects)
        {
            if (go != null)
            {
                RectTransform rt = go.GetComponent<RectTransform>();
                if (rt != null)
                {
                    _leftInitialPositions[rt] = rt.anchoredPosition;
                }
            }
        }

        _rightInitialPositions.Clear();
        foreach (var go in RightGameObjects)
        {
            if (go != null)
            {
                RectTransform rt = go.GetComponent<RectTransform>();
                if (rt != null)
                {
                    _rightInitialPositions[rt] = rt.anchoredPosition;
                }
            }
        }

        _animationCoroutine = StartCoroutine(AnimatePauseUI());
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumePlayerControl();
        }

        if (PauseExit != null)
        {
            PauseExit.PlayerTimescaleMode = TimescaleModes.Unscaled;
            PauseExit.PlayFeedbacks();
        }

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }

        // Reset spacing and color to start or a default value if needed when unpausing
        foreach (var text in TargetTexts)
        {
            if (text != null)
            {
                text.characterSpacing = StartSpacing;
            }
        }

        if (TargetImage != null)
        {
            TargetImage.color = StartColor;
        }

        foreach (var kvp in _leftInitialPositions)
        {
            if (kvp.Key != null)
            {
                kvp.Key.anchoredPosition = kvp.Value;
            }
        }

        foreach (var kvp in _rightInitialPositions)
        {
            if (kvp.Key != null)
            {
                kvp.Key.anchoredPosition = kvp.Value;
            }
        }
    }

    private IEnumerator AnimatePauseUI()
    {
        while (true)
        {
            float elapsed = 0f;
            while (elapsed < AnimationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float normalizedTime = Mathf.Clamp01(elapsed / AnimationDuration);
                float curveValue = AnimationCurve.Evaluate(normalizedTime);
                float currentSpacing = Mathf.LerpUnclamped(StartSpacing, EndSpacing, curveValue);
                Color currentColor = Color.LerpUnclamped(StartColor, EndColor, curveValue);

                foreach (var text in TargetTexts)
                {
                    if (text != null)
                    {
                        text.characterSpacing = currentSpacing;
                    }
                }

                if (TargetImage != null)
                {
                    TargetImage.color = currentColor;
                }

                foreach (var kvp in _leftInitialPositions)
                {
                    if (kvp.Key != null)
                    {
                        kvp.Key.anchoredPosition = kvp.Value - (MovementVector * curveValue);
                    }
                }

                foreach (var kvp in _rightInitialPositions)
                {
                    if (kvp.Key != null)
                    {
                        kvp.Key.anchoredPosition = kvp.Value + (MovementVector * curveValue);
                    }
                }

                yield return null;
            }
        }
    }
}
