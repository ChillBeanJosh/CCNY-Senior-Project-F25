using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;

public class SpeechBubble : MonoBehaviour, MMEventListener<MMGameEvent> {
    [Header("Speech Bubble Settings")]
    [SerializeField, TextArea(3, 10)] private string speechText = "Hello!";
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
    }

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player spawnFeedback;
    [SerializeField] private MMF_Player despawnFeedback;

    [Header("Billboard Settings")]
    [SerializeField] private Transform canvasTransform; // Assign the Canvas child transform
    
    [Header("References")]
    [SerializeField] private TextMeshProUGUI tmpText; // Assign your TMP text component
    
    [Header("Spring Scale Settings")]
    [SerializeField] private MMSpringScale smallSpringScale;
    [SerializeField] private MMSpringScale mediumSpringScale;
    [SerializeField] private MMSpringScale largeSpringScale;


    private Camera mainCamera;
    private Transform playerTransform;
    private bool playerInRange = false;
    private bool isShowing = false;
    private float lastExitTime;
    private Vector3 anchorPosition;
    private bool cameraTriggeredThisEnter = false;
    private bool cameraAlreadyTriggeredInScene = false;
    private Coroutine cameraTimerCoroutine;
    private bool playerControlDisabled = false;

    void Awake() {
        // Store the initial world position as anchor
        anchorPosition = transform.position;

        // Find and set the TMP Text Reveal feedback's text
        if (spawnFeedback != null) {
            MMF_TMPTextReveal textReveal = spawnFeedback.GetFeedbackOfType<MMF_TMPTextReveal>();
            if (textReveal != null) {
                textReveal.NewText = speechText;
                textReveal.ReplaceText = true;
            }
        }
        
        // Find TMP text if not assigned
        if (tmpText == null) {
            tmpText = GetComponentInChildren<TextMeshProUGUI>();
        }
        
        // Set spring scale dampening and frequency
        SetSpringScaleProperties(smallSpringScale);
        SetSpringScaleProperties(mediumSpringScale);
        SetSpringScaleProperties(largeSpringScale);
    }
    
    void SetSpringScaleProperties(MMSpringScale springScale) {
        if (springScale != null) {
            springScale.SpringVector3.UnifiedSpring.Damping = bubbleSpringDampening;
            springScale.SpringVector3.UnifiedSpring.Frequency = bubbleSpringFrequency;
        }
    }

    void Start() {
        mainCamera = Camera.main;

        // If canvas transform not assigned, use this transform
        if (canvasTransform == null) {
            canvasTransform = transform;
        }
    }

    void OnEnable() {
        this.MMEventStartListening<MMGameEvent>();
    }

    void OnDisable() {
        this.MMEventStopListening<MMGameEvent>();
    }

    public void OnMMEvent(MMGameEvent gameEvent) {
        if (eventUpdates == null) return;

        foreach (var update in eventUpdates) {
            if (!string.IsNullOrEmpty(update.eventNameToListen) && gameEvent.EventName == update.eventNameToListen) {
                UpdateSpeechText(update.newSpeechText);
                break;
            }
        }
    }

    public void UpdateSpeechText(string newText) {
        if (speechText == newText) return;

        speechText = newText;
        cameraAlreadyTriggeredInScene = false;

        // Update the TMP Text Reveal feedback's text if assigned
        if (spawnFeedback != null) {
            MMF_TMPTextReveal textReveal = spawnFeedback.GetFeedbackOfType<MMF_TMPTextReveal>();
            if (textReveal != null) {
                textReveal.NewText = speechText;
                textReveal.ReplaceText = true;
            }
        }

        // If currently showing, update the actual TMP text immediately
        if (isShowing && tmpText != null) {
            if (spawnFeedback != null) {
                spawnFeedback.PlayFeedbacks();
            }
        }
    }

    void Update() {
        CheckPlayerProximity();
    }

    void LateUpdate() {
        // Billboard effect - only rotate the canvas to face camera
        if (mainCamera != null && canvasTransform != null) {
            canvasTransform.rotation = mainCamera.transform.rotation;
        }

        // Keep the root object anchored at its original position
        transform.position = anchorPosition;
    }

    void CheckPlayerProximity() {
        // Find player if not already found
        if (playerTransform == null) {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null) {
                playerTransform = player.transform;
            }
            else {
                return; // No player found yet
            }
        }

        // Calculate distance from anchor position to player
        float distance = Vector3.Distance(anchorPosition, playerTransform.position);

        // Player entered range
        if (distance <= detectionRadius) {
            if (!playerInRange) {
                playerInRange = true;
                cameraTriggeredThisEnter = false;
                ShowSpeechBubble();
            }
        }
        // Player exited range
        else if (distance > detectionRadius) {
            if (playerInRange) {
                playerInRange = false;
                lastExitTime = Time.time;
            }

            // Buffer logic: if enough time has passed since exit, hide
            if (isShowing && (Time.time - lastExitTime >= hideDelay)) {
                HideSpeechBubble();
            }
        }
    }

    void ShowSpeechBubble() {
        SetSpringScaleProperties(smallSpringScale);
        SetSpringScaleProperties(mediumSpringScale);
        SetSpringScaleProperties(largeSpringScale);

        bool canTriggerCamera = cinemachineCamera != null && !cameraTriggeredThisEnter;
        if (triggerCameraOnlyOncePerScene && cameraAlreadyTriggeredInScene) {
            canTriggerCamera = false;
        }

        if (canTriggerCamera) {
            cinemachineCamera.Priority = activePriority;
            cameraTriggeredThisEnter = true;
            cameraAlreadyTriggeredInScene = true;

            if (disablePlayerControlOnSwitch && GameManager.Instance != null) {
                GameManager.Instance.PausePlayerControl();
                playerControlDisabled = true;
            }

            if (useCameraTimer) {
                if (cameraTimerCoroutine != null) {
                    StopCoroutine(cameraTimerCoroutine);
                }
                cameraTimerCoroutine = StartCoroutine(CameraTimer());
            }
        }

        if (!isShowing && spawnFeedback != null) {
            // Clear the TMP text before playing the reveal feedback
            if (tmpText != null) {
                tmpText.text = "";
            }
            
            spawnFeedback.PlayFeedbacks();
            isShowing = true;
        }
    }

    void HideSpeechBubble() {
        SetSpringScaleProperties(smallSpringScale);
        SetSpringScaleProperties(mediumSpringScale);
        SetSpringScaleProperties(largeSpringScale);

        if (!useCameraTimer) {
            if (cinemachineCamera != null) {
                cinemachineCamera.Priority = inactivePriority;
            }
            RestorePlayerControl();
        }

        if (isShowing && despawnFeedback != null) {
            despawnFeedback.PlayFeedbacks();
            isShowing = false;
        }
    }

    private System.Collections.IEnumerator CameraTimer() {
        yield return new WaitForSeconds(cameraSwitchDuration);
        if (cinemachineCamera != null) {
            cinemachineCamera.Priority = inactivePriority;
        }
        RestorePlayerControl();
        cameraTimerCoroutine = null;
    }

    private void RestorePlayerControl() {
        if (playerControlDisabled && GameManager.Instance != null) {
            GameManager.Instance.ResumePlayerControl();
            playerControlDisabled = false;
        }
    }

}