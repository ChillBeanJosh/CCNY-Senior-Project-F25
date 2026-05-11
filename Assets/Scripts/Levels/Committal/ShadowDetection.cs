using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowDetection : MonoBehaviour
{
    public bool requiresTwoPlayers = false;
    Collider detectionCol, shadowCol; // Colliders for puzzle detection 
    [SerializeField] List<Collider> playerShadows = new List<Collider>(); // Colliders for puzzle requiring both players
    [Header("Two Player Check")]
    [Tooltip("Leave empty if puzzle does not require two players.")]
    [SerializeField] Collider leftDetectionCol;
    [SerializeField] Collider rightDetectionCol;
    [Space(15)]
    [SerializeField] Collider leftSizeCheckCol;
    [SerializeField] Collider rightSizeCheckCol;
    [Space(15)]
    [SerializeField] Collider leftCol;
    [SerializeField] Collider rightCol;
    [Header("One Player Check")]
    [SerializeField] Collider sizeCheckCol; // Additional collider to use as max size for shadow sprite
    [Space(15)]
    bool shadowDetected;
    public bool shadowIsInside;
    [SerializeField] ShadowPuzzleTrigger shadowTrigger;
    [SerializeField] ShadowCaster shadowCaster;
    public bool completed;
    bool turnOffPlayerCheck;
    //Vector3 testCorner = Vector3.zero;
    [SerializeField] Outline finalCheckOutline;
    Outline outline;

    void Start()
    {
        detectionCol = GetComponent<Collider>();
        outline = requiresTwoPlayers ? finalCheckOutline : sizeCheckCol.transform.gameObject.GetComponent<Outline>();
        outline.OutlineWidth = 10f;
        outline.OutlineColor = Color.white;
        outline.enabled = false;
    }

    void Update()
    {
        if (completed)
        {
            PuzzleComplete();
            return;
        }

        if (requiresTwoPlayers && playerShadows.Count == 2)
        {
            // Debug.Log(ContainsCollider(detectionCol, shadowCol) + "   " +
            //       NoCornersDetected(sizeCheckCol, shadowCol));
            //Collider leftCol, rightCol;

            // Check to see which player is on left side of puzzle
            if (playerShadows[0] != null && playerShadows[1] != null)
            {
                if (playerShadows[0].transform.position.x <= playerShadows[1].transform.position.x)
                {
                    leftCol = playerShadows[0];
                    rightCol = playerShadows[1];
                }
                else
                {
                    leftCol = playerShadows[1];
                    rightCol = playerShadows[0];
                }

                bool check1 = ContainsCollider(leftDetectionCol, leftCol) &&
                              NoCornersDetected(leftSizeCheckCol, leftCol);

                bool check2 = ContainsCollider(rightDetectionCol, rightCol) &&
                              NoCornersDetected(rightSizeCheckCol, rightCol);

                Debug.Log(check1 + "   " + check2);

                shadowIsInside = check1 && check2;

                OutlineHandler();
            }
        }
        else if (shadowCol != null && shadowDetected && shadowCaster.isChecking)
        {
            shadowIsInside = ContainsCollider(detectionCol, shadowCol) &&
                         NoCornersDetected(sizeCheckCol, shadowCol);

            OutlineHandler();
        }
        else
        {
            if (shadowIsInside) shadowIsInside = false;
            if (outline.enabled) outline.enabled = false;
        }

        if (requiresTwoPlayers)
        {
            for (int i = 0; i < playerShadows.Count; i++)
            {
                if (playerShadows[i] == null)
                    playerShadows.Remove(playerShadows[i]);
            }
        }
    }

    bool ContainsCollider(Collider colA, Collider colB)
    {
        // Check whether shadow collider is completely within the outer box collider
        return colA.bounds.Contains(colB.bounds.min) &&
               colA.bounds.Contains(colB.bounds.max);
    }

    bool NoCornersDetected(Collider colA, Collider colB)
    {
        // Check whether shadow collider is not within the inner box collider
        Vector3 shadowEdges = colB.bounds.size / 2f;
        Vector3 shadowCenter = colB.bounds.center;

        for (int i = 0; i < 4; i++)
        {
            Vector3 corner;
            // Get all corners of the box collider using its center and size as reference
            corner.x = i % 2 == 0 ? shadowEdges.x : -shadowEdges.x;
            corner.y = i % 2 == 0 ? shadowEdges.y : -shadowEdges.y;
            corner.z = 0f;

            Vector3 point = shadowCenter + corner;

            // If any of the corners are within the inner box collider, return false
            if (colA.bounds.Contains(point))
            {
                return false;
            }
        }

        // If no corners are within the inner box collider, return true
        return true;
    }

    void PuzzleComplete()
    {
        if (outline.OutlineColor != Color.black) outline.OutlineColor = Color.black;
        if (shadowDetected) shadowDetected = false;
        if (shadowCol != null) shadowCol = null;
        if (shadowTrigger.detectPlayer) shadowTrigger.detectPlayer = false;
        if (!turnOffPlayerCheck)
        {
            turnOffPlayerCheck = true;
            GameManager.Instance.Player.gameObject.GetComponent<DrawShadows>().shadowPuzzleActive = false;
        }
    }

    void OutlineHandler()
    {
        // Turn outline on 
        if (!outline.enabled) outline.enabled = true;

        // Set outline to cyan if at correct position
        if (shadowIsInside && outline.OutlineColor != Color.cyan)
            outline.OutlineColor = Color.cyan;
        // Set outline to white if at wrong position
        else if (!shadowIsInside && outline.OutlineColor != Color.white)
            outline.OutlineColor = Color.white;
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(test, 0.01f);
    }

    public void RemoveShadowFromList(Collider col)
    {
        if (requiresTwoPlayers && playerShadows.Count > 0)
        {
            if (playerShadows.Contains(col))
                playerShadows.Remove(col);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Shadow"))
        {
            if (requiresTwoPlayers)
            {
                if (!playerShadows.Contains(col))
                    playerShadows.Add(col);
            }
            else
            {
                shadowCol = col;
                shadowDetected = true;
            }

        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Shadow"))
        {
            if (requiresTwoPlayers)
            {
                if (playerShadows.Contains(col))
                    playerShadows.Remove(col);
            }
            else
            {
                shadowCol = null;
                shadowDetected = false;
            }

        }
    }
}
