using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [Tooltip("The speed at which the credits move up (x)")]
    [SerializeField] private float scrollSpeed = 50f;
    
    [Header("Transition Settings")]
    [Tooltip("Time in seconds before switching to the next scene (y)")]
    [SerializeField] private float duration = 30f;
    
    [Tooltip("The name of the scene to load after the duration")]
    [SerializeField] private string nextSceneName;

    private float timer = 0f;

    void Update()
    {
        // Move the credits up at a rate of x
        transform.Translate(Vector3.up * (scrollSpeed * Time.deltaTime));

        // Increment timer
        timer += Time.deltaTime;

        // After y seconds, go to a set scene using GameManager
        if (timer >= duration)
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SwitchToScene(nextSceneName);
        }
        else
        {
            Debug.LogError("[CreditsRoll] GameManager.Instance not found! Cannot switch scene.");
            // Fallback if GameManager is missing
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
        
        // Disable this script to prevent multiple loads
        enabled = false;
    }
}
