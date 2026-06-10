using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerMovement Player;
    public LanternTravel LanternTravel;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            ResetScene();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.B))
        {
            QuitGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PausePlayerControl()
    {
        if (Player != null)
        {
            Player.playerControl = false;
            Player.moveDirection = Vector3.zero;
            Player.isAiming = false;

            Rigidbody rb = Player.GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void ResumePlayerControl()
    {
        if (Player != null)
        {
            Player.playerControl = true;
        }
    }

    public void SwitchToScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;

        if (SceneTransition.HasListeners())
        {
            SceneTransition.InvokeSceneChange(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
