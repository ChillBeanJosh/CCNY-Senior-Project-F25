using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ShadowRegen : MonoBehaviour
{
    [SerializeField] GameObject piecesPrefab;
    [SerializeField] List<GameObject> allPillars;
    [SerializeField] GameObject regenObj, regenAnchor;
    [SerializeField] CycleSprites sprites;
    DrawShadows drawShadows;
    [SerializeField] ShadowCaster shadowCaster;
    int currentSprite = 0;
    public enum PillarsDestroyed
    {
        None,
        Left,
        Right,
        Top,
    }
    public PillarsDestroyed state;
    bool regenInProgress;

    void Start()
    {
        state = PillarsDestroyed.None;
    }

    void Update()
    {
        // Update shadow script reference when player switches
        if (GameManager.Instance.Player.gameObject.GetComponent<DrawShadows>() != drawShadows) drawShadows = GameManager.Instance.Player.gameObject.GetComponent<DrawShadows>();

        if (drawShadows.shadow != null && sprites == null && drawShadows.shadow.TryGetComponent<CycleSprites>(out CycleSprites cycleSprites))
        {
            // Update shadow sprite based on which burnables are burned off
            sprites = cycleSprites; //drawShadows.shadow.GetComponent<CycleSprites>();
            sprites.ChangeSprite(currentSprite);
        }

        bool regen = false;

        for (int i = 2; i < allPillars.Count; i++)
        {
            // Check last three burnables
            if (!allPillars[i].activeInHierarchy) regen = true;
        }

        if (!regen && sprites != null) UpdateShadowSprite();

        // Regenerate object if all pillars have been removed
        if (regen && !regenInProgress) StartCoroutine(SpawnObject());
    }

    void UpdateShadowSprite()
    {

        if (allPillars[0].activeInHierarchy && allPillars[1].activeInHierarchy)
        {
            if (state is not PillarsDestroyed.None)
            {
                state = PillarsDestroyed.None;
                currentSprite = 0;
                sprites.ChangeSprite(currentSprite);
                shadowCaster.isChecking = false;
            }
        }
        else if (allPillars[0].activeInHierarchy && !allPillars[1].activeInHierarchy)
        {
            if (state is not PillarsDestroyed.Left)
            {
                state = PillarsDestroyed.Left;
                currentSprite = 1;
                sprites.ChangeSprite(currentSprite);
            }
        }
        else if (!allPillars[0].activeInHierarchy && allPillars[1].activeInHierarchy)
        {
            if (state is not PillarsDestroyed.Right)
            {
                state = PillarsDestroyed.Right;
                currentSprite = 2;
                sprites.ChangeSprite(currentSprite);
            }
        }
        else if (!allPillars[0].activeInHierarchy && !allPillars[1].activeInHierarchy)
        {
            if (state is not PillarsDestroyed.Top)
            {
                state = PillarsDestroyed.Top;
                currentSprite = 3;
                sprites.ChangeSprite(currentSprite);
                shadowCaster.isChecking = true;
            }
        }
    }
    IEnumerator SpawnObject()
    {
        regenInProgress = true;

        regenObj.SetActive(false);
        Destroy(sprites.gameObject);

        Vector3 target = regenAnchor.transform.position;
        Vector3 start = target + Vector3.up * 6f;

        regenAnchor.transform.position = start;

        yield return new WaitForSeconds(1);

        // Reset object and pillars
        regenObj.SetActive(true);

        foreach (GameObject p in allPillars)
        {
            p.SetActive(true);
        }

        float elapsed = 0f;
        float duration = 3f;

        while (elapsed < duration)
        {
            float time = elapsed / duration;
            regenAnchor.transform.position = Vector3.Lerp(start, target, time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        regenAnchor.transform.position = target;
        regenInProgress = false;
    }
}
