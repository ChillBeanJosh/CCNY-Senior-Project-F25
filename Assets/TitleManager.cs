using UnityEngine;

public class TitleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void StartGame(string sceneName)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SwitchToScene(sceneName);
        }
    }

    public void QuitGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }
}
