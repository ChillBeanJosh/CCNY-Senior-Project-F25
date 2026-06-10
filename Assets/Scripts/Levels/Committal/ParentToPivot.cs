using UnityEngine;

public class ParentToPivot : MonoBehaviour
{
    [SerializeField] string pivot;
    [SerializeField] Sprite unflipped, flipped;
    void Start()
    {
        Transform p = GameObject.Find(pivot).transform;
        if (p.localEulerAngles.y > 0)
        {
            p.GetComponent<FlipWhaleSprite>().reverse = true;
        }
        else
        {
            p.GetComponent<FlipWhaleSprite>().reverse = false;
        }
        transform.parent = p;
    }
}
