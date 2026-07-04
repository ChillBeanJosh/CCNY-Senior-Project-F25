using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] SunWheelController sunWheel;
    [SerializeField] GameObject tutorial;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (!GameManager.Instance.Player.item.activeInHierarchy)
                GameManager.Instance.Player.item.SetActive(true);

            if (sunWheel != null)
                sunWheel.UnlockAbility(SunSpike.SunSpikeType.Telescope);

            if (tutorial != null) tutorial.SetActive(true);

            this.gameObject.SetActive(false);
        }
    }
}
