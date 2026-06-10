using UnityEngine;

public class Splash : MonoBehaviour
{
    void Start()
    {
        Invoke(nameof(KillParticle), 2f);
    }

    void KillParticle()
    {
        Destroy(gameObject);
    }
}
