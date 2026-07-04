using UnityEngine;

public class ConfirmTrial : MonoBehaviour
{
    [SerializeField] float WispSpeed = 0.05f;
    [SerializeField] float MoveDelayTime = 1.5f;
    float MoveDelayTimer = 0f;

    GameObject GhostPlank;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        MoveDelayTimer = MoveDelayTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveDelayTimer > 0f) MoveDelayTimer -= Time.deltaTime;

        if (MoveDelayTimer <= 0f) 
        {
            transform.position = Vector3.MoveTowards(transform.position, GhostPlank.transform.position, WispSpeed);
        }

        if (Vector3.Distance(transform.position, GhostPlank.transform.position) < 0.1f)
        {
            Destroy(GhostPlank);
            Destroy(gameObject);
        }

    }

    public void SetGhostPlank(GameObject plank)
    {
        GhostPlank = plank;
    }
}
