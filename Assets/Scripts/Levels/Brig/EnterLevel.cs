using System;
using UnityEngine;

public class EnterLevel : MonoBehaviour
{
    [SerializeField] String nextLevel, previousLevel;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.X) && !string.IsNullOrEmpty(nextLevel))
        {
            GameManager.Instance.SwitchToScene(nextLevel);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Z) && !string.IsNullOrEmpty(previousLevel))
        {
            GameManager.Instance.SwitchToScene(previousLevel);
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameManager.Instance.SwitchToScene(nextLevel);
        }
    }
}
