using UnityEngine;

public class SceneTransitionTester : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "YourSceneNameHere";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SwitchToScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("GameManager instance not found. Make sure GameManager is in the scene.");
            }
        }
    }
}
