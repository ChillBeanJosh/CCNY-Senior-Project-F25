using UnityEngine;
using Unity.Cinemachine;

public class ShelfCam : MonoBehaviour
{
    public bool start, end;
    CinemachineCamera shelfCam;

    void Start()
    {
        shelfCam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        if (start && !end && GameManager.Instance.Player.playerControl)
        {
            GameManager.Instance.Player.playerControl = false;
            shelfCam.Priority = 20;
        }

        if (end && !GameManager.Instance.Player.playerControl)
        {
            GameManager.Instance.Player.playerControl = true;
            shelfCam.Priority = 0;
        }
    }
}
