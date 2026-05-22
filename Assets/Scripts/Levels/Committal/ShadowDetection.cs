using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShadowDetection : MonoBehaviour
{
    public bool requiresTwoPlayers = false;
    Collider detectionCol, shadowCol; // Colliders for puzzle detection 
    [SerializeField] List<Collider> playerShadows = new List<Collider>(); // Colliders for puzzle requiring both players
    [SerializeField] GameObject[] players = new GameObject[2];
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
    [Space(15)]
    [SerializeField] Transform centerPt;
    [SerializeField] Transform approximatePos2;
    [SerializeField] Transform targetPos2;
    [Header("One Player Check")]
    [SerializeField] Collider sizeCheckCol; // Additional collider to use as max size for shadow sprite
    [Space(15)]
    bool shadowDetected;
    public bool shadowIsInside;
    [SerializeField] ShadowPuzzleTrigger shadowTrigger;
    [SerializeField] ShadowCaster shadowCaster;
    public bool completed;
    bool turnOffPlayerCheck;
    [SerializeField] float currentBurnTime = 0f;
    //Vector3 testCorner = Vector3.zero;
    [SerializeField] Outline shadowObjOutline;

    [SerializeField] Outline finalCheckOutline;
    [SerializeField] Outline leftFinalCheck, rightFinalCheck;
    Outline outline;
    [SerializeField] Transform approximatePos;
    [SerializeField] Transform targetPos;

    void Start()
    {
        detectionCol = GetComponent<Collider>();
        outline = requiresTwoPlayers ? finalCheckOutline : shadowObjOutline;
        outline.OutlineWidth = 3f;
        outline.OutlineColor = Color.white;
        outline.enabled = false;
    }

    void Update()
    {
        if (requiresTwoPlayers) Debug.Log(players[1].activeInHierarchy);
        if (completed)
        {
            PuzzleComplete();
            return;
        }

        //if (requiresTwoPlayers && playerShadows.Count == 2)
        if (requiresTwoPlayers && players[1].activeInHierarchy)
        {
            if (playerShadows.Count == 2 && playerShadows[0] != null && playerShadows[1] != null)
            {
                if (players[0].transform.position.x <= players[1].transform.position.x)
                {
                    leftCol = playerShadows[0];
                    if (players[0].GetComponent<DrawShadows>().approximatePos != approximatePos2 ||
                        players[0].GetComponent<DrawShadows>().targetPos != targetPos2)
                    {
                        players[0].GetComponent<DrawShadows>().approximatePos = approximatePos2;
                        players[0].GetComponent<DrawShadows>().targetPos = targetPos2;
                    }

                    rightCol = playerShadows[1];
                    if (players[1].GetComponent<DrawShadows>().approximatePos != approximatePos ||
                        players[1].GetComponent<DrawShadows>().targetPos != targetPos)
                    {
                        players[1].GetComponent<DrawShadows>().approximatePos = approximatePos;
                        players[1].GetComponent<DrawShadows>().targetPos = targetPos;
                    }
                }
                else
                {
                    leftCol = playerShadows[1];
                    if (players[1].GetComponent<DrawShadows>().approximatePos != approximatePos2 ||
                        players[1].GetComponent<DrawShadows>().targetPos != targetPos2)
                    {
                        players[1].GetComponent<DrawShadows>().approximatePos = approximatePos2;
                        players[1].GetComponent<DrawShadows>().targetPos = targetPos2;
                    }

                    rightCol = playerShadows[0];
                    if (players[0].GetComponent<DrawShadows>().approximatePos != approximatePos ||
                        players[0].GetComponent<DrawShadows>().targetPos != targetPos)
                    {
                        players[0].GetComponent<DrawShadows>().approximatePos = approximatePos;
                        players[0].GetComponent<DrawShadows>().targetPos = targetPos;
                    }
                }

                if (rightCol.transform.position == targetPos.position && !leftFinalCheck.enabled)
                {
                    leftFinalCheck.enabled = true;
                }
                else if (leftFinalCheck && rightCol.transform.position != targetPos.position)
                {
                    leftFinalCheck.enabled = false;
                }

                if (leftCol.transform.position == targetPos2.position && !rightFinalCheck.enabled)
                {
                    rightFinalCheck.enabled = true;
                }
                else if (rightFinalCheck && leftCol.transform.position != targetPos2.position)
                {
                    rightFinalCheck.enabled = false;
                }

                shadowIsInside = rightCol.transform.position == targetPos.position && leftCol.transform.position == targetPos2.position;

                Debug.Log((rightCol.transform.position == targetPos.position) + "  |  " + (leftCol.transform.position == targetPos2.position));

            }

            if (shadowCaster.isChecking)
            {
                OutlineHandler();
            }
            else
            {
                if (outline.enabled) outline.enabled = false;
            }
        }
        else if (shadowCol != null && shadowDetected && shadowCaster.isChecking)
        {
            //shadowIsInside = Vector3.Distance(GameManager.Instance.Player.transform.position, approximatePos.position) < 0.05f;
            shadowIsInside = shadowCol.transform.position == targetPos.position;

            Debug.Log(shadowIsInside);
            //Debug.Log(ContainsCollider(detectionCol, shadowCol) + "  |  " + NoCornersDetected(sizeCheckCol, shadowCol));

            if (shadowCaster.isChecking)
            {
                OutlineHandler();
            }
            else
            {
                if (outline.enabled) outline.enabled = false;
            }

        }
        else
        {
            if (shadowIsInside) shadowIsInside = false;

            if (outline.enabled)
            {
                outline.enabled = false;
                outline.OutlineColor = Color.white;
            }
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

        if (requiresTwoPlayers)
        {
            if (leftFinalCheck.enabled) leftFinalCheck.enabled = false;
            if (rightFinalCheck.enabled) rightFinalCheck.enabled = false;
        }

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

        // // Set outline to cyan if at correct position
        // if (shadowIsInside && outline.OutlineColor != Color.cyan)
        //     outline.OutlineColor = Color.cyan;
        // // Set outline to white if at wrong position
        // else if (!shadowIsInside && outline.OutlineColor != Color.white)
        //     outline.OutlineColor = Color.white;
        if (currentBurnTime != 1f)
        {
            ApplyBurn();
        }
        else if (!shadowCaster.castComplete)
        {
            shadowCaster.castComplete = true;
            outline.OutlineWidth = 5f;
        }
    }

    void ApplyBurn()
    {
        float time = Time.deltaTime / 3f;

        if (shadowIsInside)
        {
            currentBurnTime += time;
        }
        else
        {
            currentBurnTime -= time;
        }

        currentBurnTime = Mathf.Clamp01(currentBurnTime);

        Color c = shadowIsInside ? Color.cyan : Color.white;
        outline.OutlineColor = Color.Lerp(c, Color.black, currentBurnTime);
        outline.OutlineWidth = Mathf.Lerp(3f, 10f, currentBurnTime);

        //Debug.Log(currentBurnTime);
    }

    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(test, 0.01f);
    }

    public void RemoveShadowFromList(Collider col)
    {
        if (requiresTwoPlayers)
        {
            if (playerShadows.Count > 0)
            {
                if (playerShadows.Contains(col))
                {
                    int index = playerShadows.IndexOf(col);
                    //if (players.Count == index + 1 && players.Contains(players[index])) players.Remove(players[index]);
                    playerShadows.Remove(col);
                }
            }
        }
    }

    public void AddPlayer(GameObject p)
    {
        //if (!players.Contains(p)) players.Add(p);
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
