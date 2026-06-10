using UnityEngine;

public class LevelHazard : MonoBehaviour
{
    [SerializeField] bool addSplash;
    bool splashCooldown;
    [SerializeField] GameObject splashParticles;
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Player.checkpoint = true;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Player.checkpoint = true;
            //Debug.Log("YERRE");
        }

        if (col.gameObject.CompareTag("Pot"))
        {
            if (addSplash && !splashCooldown)
            {
                Vector3 pos = col.ClosestPoint(transform.position);
                Instantiate(splashParticles, pos, Quaternion.identity);
                splashCooldown = true;
                Invoke(nameof(ResetParticles), 0.3f);
            }
        }
    }

    void ResetParticles()
    {
        splashCooldown = false;
    }
}
