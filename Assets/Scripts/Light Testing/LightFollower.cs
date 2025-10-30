using System.Collections.Generic;
using UnityEngine;

public class LightFollower : MonoBehaviour
{
    [Header("References")]
    public LightReflection lightReflection;
    public GameObject followerObject;
    public PlayerMovement player;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public KeyCode startKey = KeyCode.Q;
    public bool loop = false;

    private int currentIndex = 0;
    private List<Vector3> pathPoints = new List<Vector3>();
    private bool isMoving = false;


    void Update()
    {
        if (Input.GetKeyDown(startKey) && !isMoving)
        {
            //Null Checks:
            if (lightReflection == null || followerObject == null) return;

            //Checks if a Lens Hit is Possible:
            if (!lightReflection.lensHit) return;

            //New List To Ensure Tranform Doesnt Get Changed:
            pathPoints = new List<Vector3>(lightReflection.laserPoints);
            if (pathPoints == null || pathPoints.Count == 0) return;


            //Initial Position in List:
            currentIndex = 0;
            Vector3 startPos = pathPoints[currentIndex];
            startPos.z = followerObject.transform.position.z;
            followerObject.transform.position = startPos;

            if (player != null)
                player.enabled = false;

            isMoving = true;
        }

        if (isMoving && pathPoints.Count > 1)
        {
            //All Other Positions, Until Last:
            if (currentIndex < pathPoints.Count - 1)
            {
                //Get the Position of the Next Position:
                Vector3 target = pathPoints[currentIndex + 1];
                //target.z = followerObject.transform.position.z;

                //Movement Logic:
                followerObject.transform.position = Vector3.MoveTowards(
                    followerObject.transform.position,  //current position.
                    target,                             //target position.
                    moveSpeed * Time.deltaTime          //speed.
                );

                //Iterate to the Next, Next Postion:
                if (Vector3.Distance(followerObject.transform.position, target) < 0.01f)
                {
                    currentIndex++;
                }
            }
            //In the case to Reset Back to Default:
            else
            {
                if (player != null)
                    player.enabled = true;

                if (loop) currentIndex = 0;
                else isMoving = false;
            }
        }
    }
}
