using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour, MMEventListener<MMGameEvent> {
    [Header("Speech Bubble Settings")]
    [SerializeField, TextArea(3, 10)] private string speechText = "Hello!";
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float hideDelay = 2f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float bubbleSpringDampening = 0.5f;
    [SerializeField] private float bubbleSpringFrequency = 5f;

    [Header("Cinemachine Settings")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int inactivePriority = 0;
    [SerializeField] private float cameraSwitchDuration = 5f;
    [SerializeField] private bool useCameraTimer = true;
    [SerializeField] private bool disablePlayerControlOnSwitch = true;
    [SerializeField] private bool triggerCameraOnlyOncePerScene = false;

    [Header("Event Settings")]
    [SerializeField] private System.Collections.Generic.List<SpeechBubbleEventUpdate> eventUpdates;

    [System.Serializable]
    public class SpeechBubbleEventUpdate {
        public string eventNameToListen;
        public string newSpeechText;
        public Sprite newSprite;
    }

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player spawnFeedback;
    [SerializeField] private MMF_Player despawnFeedback;

    [Header("Screen Space Feedbacks")]
    [SerializeField] private MMF_Player screenSpaceSpawnFeedback;
    [SerializeField] private MMF_Player screenSpaceDespawnFeedback;
    [SerializeField] private TextMeshProUGUI screenSpaceTmpText;

    [Header("Billboard Settings")]
    [SerializeField] private Transform canvasTransform; // Assign the Canvas child transform
    
    [Header("References")]
    [SerializeField] private TextMeshProUGUI tmpText; // Assign your TMP text component
    [SerializeField] private Image bubbleImage; // Assign your world space image component
    [SerializeField] private Image screenSpaceBubbleImage; // Assign your screen space image component


    private Camera mainCamera;
    private Transform playerTransform;
    private bool playerInRange = false;
    private bool isWorldSpaceShowing = false;
    private bool isScreenSpaceShowing = false;
    private float lastExitTime;
    private Vector3 anchorPosition;
    private bool cameraAlreadyTriggeredInScene = false;
    private Coroutine cameraTimerCoroutine;
    private bool playerControlDisabled = false;

    void Awake() {
        anchorPosition = transform.position;

        InitializeFeedback(spawnFeedback);
        InitializeFeedback(screenSpaceSpawnFeedback);
        
        if (tmpText == null) {
            tmpText = GetComponentInChildren<TextMeshProUGUI>();
        }

        UpdateImageSprite(bubbleImage, defaultSprite);
        UpdateImageSprite(screenSpaceBubbleImage, defaultSprite);
    }

    private void InitializeFeedback(MMF_Player player) {
        if (player == null) return;
        
        MMF_TMPTextReveal textReveal = player.GetFeedbackOfType<MMF_TMPTextReveal>();
        if (textReveal != null) {
            textReveal.NewText = speechText;
            textReveal.ReplaceText = true;
        }
    }

    void Start() {
        mainCamera = Camera.main;
        if (canvasTransform == null) canvasTransform = transform;
    }

    void OnEnable() => this.MMEventStartListening<MMGameEvent>();
    void OnDisable() => this.MMEventStopListening<MMGameEvent>();

    public void OnMMEvent(MMGameEvent gameEvent) {
        if (eventUpdates == null) return;

        foreach (var update in eventUpdates) {
            if (!string.IsNullOrEmpty(update.eventNameToListen) && gameEvent.EventName == update.eventNameToListen) {
                UpdateSpeechBubbleContent(update.newSpeechText, update.newSprite);
                break;
            }
        }
    }

    public void UpdateSpeechBubbleContent(string newText, Sprite newSprite = null) {
        if (speechText == newText) return;
        
        speechText = newText;
        cameraAlreadyTriggeredInScene = false; // Reset if text changes? Keeping original behavior

        UpdateFeedbackText(spawnFeedback);
        UpdateFeedbackText(screenSpaceSpawnFeedback);

        if (isWorldSpaceShowing && spawnFeedback != null) spawnFeedback.PlayFeedbacks();
        if (isScreenSpaceShowing && screenSpaceSpawnFeedback != null) screenSpaceSpawnFeedback.PlayFeedbacks();

        UpdateImageSprite(bubbleImage, newSprite);
        UpdateImageSprite(screenSpaceBubbleImage, newSprite);
    }

    private void UpdateImageSprite(Image image, Sprite sprite) {
        if (image == null) return;
        image.sprite = sprite;
        image.enabled = sprite != null;
    }

    private void UpdateFeedbackText(MMF_Player player) {
        if (player == null) return;
        MMF_TMPTextReveal textReveal = player.GetFeedbackOfType<MMF_TMPTextReveal>();
        if (textReveal != null) {
            textReveal.NewText = speechText;
            textReveal.ReplaceText = true;
        }
    }

    void Update() => CheckPlayerProximity();

    void LateUpdate() {
        if (mainCamera != null && canvasTransform != null) {
            canvasTransform.rotation = mainCamera.transform.rotation;
        }
        transform.position = anchorPosition;
    }

    void CheckPlayerProximity() {
        if (playerTransform == null) {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null) playerTransform = player.transform;
            else return;
        }

        float distance = Vector3.Distance(anchorPosition, playerTransform.position);

        if (distance <= detectionRadius) {
            if (!playerInRange) {
                playerInRange = true;
                ShowSpeechBubble();
            }
        }
        else {
            if (playerInRange) {
                playerInRange = false;
                lastExitTime = Time.time;
            }

            bool isCameraActive = cinemachineCamera != null && cinemachineCamera.Priority == activePriority;
            if ((isWorldSpaceShowing || isScreenSpaceShowing) && !isCameraActive && (Time.time - lastExitTime >= hideDelay)) {
                HideSpeechBubble();
            }
        }
    }

    public void ShowSpeechBubble() {
        bool canTriggerCamera = cinemachineCamera != null && (!triggerCameraOnlyOncePerScene || !cameraAlreadyTriggeredInScene);

        if (canTriggerCamera) {
            TriggerWorldSpaceWithCamera();
        }
        else if (playerInRange) {
            TriggerScreenSpace();
        }
    }

    private void TriggerWorldSpaceWithCamera() {
        if (isScreenSpaceShowing) HideScreenSpace();

        cinemachineCamera.Priority = activePriority;
        cameraAlreadyTriggeredInScene = true;

        if (disablePlayerControlOnSwitch && GameManager.Instance != null) {
            GameManager.Instance.PausePlayerControl();
            playerControlDisabled = true;
        }

        if (useCameraTimer) {
            if (cameraTimerCoroutine != null) StopCoroutine(cameraTimerCoroutine);
            cameraTimerCoroutine = StartCoroutine(CameraTimer());
        }

        if (!isWorldSpaceShowing && spawnFeedback != null) {
            if (tmpText != null) tmpText.text = "";
            spawnFeedback.PlayFeedbacks();
            isWorldSpaceShowing = true;
        }
    }

    private void TriggerScreenSpace() {
        if (isWorldSpaceShowing) HideWorldSpace();

        if (!isScreenSpaceShowing && screenSpaceSpawnFeedback != null) {
            if (screenSpaceTmpText != null) screenSpaceTmpText.text = "";
            screenSpaceSpawnFeedback.PlayFeedbacks();
            isScreenSpaceShowing = true;
        }
    }

    public void HideSpeechBubble() {
        if (cinemachineCamera != null && !useCameraTimer) {
            cinemachineCamera.Priority = inactivePriority;
            RestorePlayerControl();
        }

        HideWorldSpace();
        HideScreenSpace();
    }

    private void HideWorldSpace() {
        if (isWorldSpaceShowing && despawnFeedback != null) {
            despawnFeedback.PlayFeedbacks();
        }
        isWorldSpaceShowing = false;
    }

    private void HideScreenSpace() {
        if (isScreenSpaceShowing && screenSpaceDespawnFeedback != null) {
            screenSpaceDespawnFeedback.PlayFeedbacks();
        }
        isScreenSpaceShowing = false;
    }

    private System.Collections.IEnumerator CameraTimer() {
        yield return new WaitForSeconds(cameraSwitchDuration);
        
        if (cinemachineCamera != null) cinemachineCamera.Priority = inactivePriority;
        RestorePlayerControl();
        cameraTimerCoroutine = null;

        HideWorldSpace();
        
        // After camera timer, if player is still in range, show screen space
        if (playerInRange) {
            TriggerScreenSpace();
        }
    }

    private void RestorePlayerControl() {
        if (playerControlDisabled && GameManager.Instance != null) {
            GameManager.Instance.ResumePlayerControl();
            playerControlDisabled = false;
        }
    }

}