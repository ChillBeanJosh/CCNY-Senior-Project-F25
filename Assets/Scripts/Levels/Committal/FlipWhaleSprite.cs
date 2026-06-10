using UnityEngine;

public class FlipWhaleSprite : MonoBehaviour
{
    [SerializeField] Transform anchor;
    [SerializeField] Sprite unflipped, flipped;
    public bool reverse;

    void Update()
    {
        //Debug.Log(Vector3.Dot(anchor.forward, -transform.right));
        if (transform.childCount > 1)
        {
            // Check to see if wall is rotated and flip sprite based on current rotation
            float dot = Vector3.Dot(-transform.right, anchor.forward);
            SpriteMask[] sm = GetComponentsInChildren<SpriteMask>();

            for (int i = 0; i < sm.Length; i++)
            {
                if (dot < 0.2f)
                {
                    if (!reverse && sm[i].sprite != flipped)
                        sm[i].sprite = flipped;
                    else if (reverse && sm[i].sprite != unflipped)
                        sm[i].sprite = unflipped;

                }
                else if (dot >= 0.2f)
                {
                    if (!reverse && sm[i].sprite != unflipped)
                        sm[i].sprite = unflipped;
                    else if (reverse && sm[i].sprite != flipped)
                        sm[i].sprite = flipped;
                }
            }
        }
    }
}
