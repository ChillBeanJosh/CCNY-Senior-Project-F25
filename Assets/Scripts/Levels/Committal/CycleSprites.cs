using UnityEngine;

public class CycleSprites : MonoBehaviour
{
    [SerializeField] SpriteRenderer spr;
    [SerializeField] SpriteMask mask;
    public Sprite[] sprites;
    bool enteredScene;

    void Update()
    {
        //if (spr == null) spr = GetComponent<SpriteRenderer>();
        if (mask == null) mask = GetComponent<SpriteMask>();



        // if (!enteredScene)
        // {
        //     enteredScene = true;

        //     //spr.sprite = sprites[0];
        //     mask.sprite = sprites[0];
        // }
    }

    public void ChangeSprite(int s)
    {
        //spr.sprite = sprites[s];
        //mask.sprite = spr.sprite;
        mask.sprite = sprites[s];
    }
}
