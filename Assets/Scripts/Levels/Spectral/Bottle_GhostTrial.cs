using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Bottle_GhostTrial : MonoBehaviour
{
    [Header("Trial Settings")]
    [SerializeField] float Speed = 1.0f;
    [SerializeField] float RoundInterval = 2.25f;
    [SerializeField] float TimeBeforeTrialStart = 2.25f;
    [SerializeField] int ShuffleRounds = 3; 

    [Header("Trial References")]
    [Tooltip("Positions the object will switch between")]
    [SerializeField] List<Vector3> Destinations = new List<Vector3>();
    [Tooltip("The bottles used in the guessing mini game - child of this object")]
    [SerializeField] List<GameObject> GhostBottles = new List<GameObject>();
    [Tooltip("The Door Plank the Wisp will fly to and destroy")]
    [SerializeField] GameObject PlankToDestroy;
    [Tooltip("Trial Manager script that tracks the completion of trials and spawning of wisps")]
    [SerializeField] GameObject TrialManager;
    [SerializeField] Material GhostMat;

    int CurrentDest;
    Material DefaultMat;
    Animator GhostAnimator;
    Quaternion originalRotation;
    Vector3 originalPosition;
    
    private Dictionary<GameObject, Vector3> targetPositions = new Dictionary<GameObject, Vector3>();

    int CorrectBottleIndex;
    bool PreparingTrial = false; // True when trial animations and preparations are active, false when trial is ready to start
    bool TrialActive = false; // True when trial sequence is active, false when the trial sequence is not active
    bool TrialCompleted = false; // True when the trial is completed, false when the trial is not completed
    bool BottleTrialRunning = false; // True when trial preparation or trial is active, false when trial is not active and not preparing
    bool CorrectBottleChosen = false; // True when the correct bottle is chosen, false when the correct bottle is not chosen

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Destinations.Count < GhostBottles.Count)
        {
            Debug.LogError("Must be at least as many destinations as bottles. Please assign destinations in the inspector.");
            enabled = false; // Disable the script to prevent further errors
            return;
        }

        // Initialize target positions
        foreach (GameObject obj in GhostBottles)
        {
            targetPositions.Add(obj, obj.transform.position);
        }

        GhostAnimator = gameObject.GetComponent<Animator>();
        GhostAnimator.enabled = false; // Disable the animator at the start
        DefaultMat = this.GetComponent<Renderer>().material;
        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (TrialActive) MoveBottles();
        if (TrialActive || PreparingTrial) BottleTrialRunning = true;

        //If the correct bottle is no longer active, end the trial
        if (!GhostBottles[CorrectBottleIndex].activeInHierarchy && BottleTrialRunning)
        {
            
        }
    }

    public void StartTrial(bool TrialStart = false)
    {
        StartCoroutine(TrialStartSequence());
        Debug.Log("Trial started: " + TrialStart);
    }

    IEnumerator TrialStartSequence()
    {
        CorrectBottleIndex = Random.Range(0, GhostBottles.Count);
        foreach (GameObject obj in GhostBottles)
        {
            obj.SetActive(true); // Set all bottles to active at the start of the trial
            obj.GetComponent<GhostBottle>().SetIsCorrectBottle(false); // Reset all bottles to not correct
        }
        //Set the correct bottle to be the correct one
        GhostBottle correctBottleScript = GhostBottles[CorrectBottleIndex].GetComponent<GhostBottle>();
        if (correctBottleScript != null) {
            correctBottleScript.SetIsCorrectBottle(true);
        } else {
            Debug.LogError("GhostBottle component not found on the correct bottle.");
        }


        GhostAnimator.enabled = true; // Enable the animator when the trial starts
        yield return new WaitForSeconds(2.5f);
        GhostAnimator.SetBool("BottleReady", true); // Plays the bottle splitting animation
        yield return new WaitForSeconds(2.25f);
        GhostAnimator.enabled = false; // Disable the animator after the animation is finished
        GhostAnimator.SetBool("BottleReady", false); // Returns to transition state for next time the trial starts

        yield return new WaitForSeconds(TimeBeforeTrialStart);

        GhostBottles[CorrectBottleIndex].GetComponent<Renderer>().material = GhostMat; // Highlight the correct bottle

        yield return new WaitForSeconds(1f);

        GhostBottles[CorrectBottleIndex].GetComponent<Renderer>().material = DefaultMat; // Reset the material after highlighting

        yield return new WaitForSeconds(1f);

        StartCoroutine(GameLoop());
    }
    IEnumerator GameLoop()
    {
        if (ShuffleRounds <= 0)
        {
            Debug.LogWarning("shuffleRounds is set to 0 or less. No shuffling will occur.");
            yield break; // Exit the coroutine if there are no rounds to shuffle
        }

        TrialActive = true;
        ShuffleRounds = Mathf.Max(1, ShuffleRounds); // Ensure at least one round
        ShuffleRounds -= 1; // Decrement the rounds since we are starting the first shuffle immediately

        AssignRandomPositions();

        yield return new WaitUntil(AllObjectsReachedDestination);

        yield return new WaitForSeconds(RoundInterval);

        //TrialActive = false;
        StartCoroutine(GameLoop());
    }

    void AssignRandomPositions()
    {
        if (Destinations.Count < GhostBottles.Count)
        {
            Debug.LogError("There must be at least as many positions as objects.");
            return;
        }

        bool success = false;
        int attempts = 0;

        while (!success && attempts < 100)
        {
            attempts++;

            success = true;

            List<Vector3> availablePositions = new List<Vector3>(Destinations);
            Dictionary<GameObject, Vector3> newTargets = new Dictionary<GameObject, Vector3>();

            foreach (GameObject obj in GhostBottles)
            {
                // Find all valid positions for this object
                List<Vector3> validPositions = new List<Vector3>();

                foreach (Vector3 pos in availablePositions)
                {
                    // Don't allow the object to remain in the same position
                    if (!targetPositions.ContainsKey(obj) || targetPositions[obj] != pos)
                    {
                        validPositions.Add(pos);
                    }
                }

                // No valid positions? Retry the entire assignment.
                if (validPositions.Count == 0)
                {
                    success = false;
                    break;
                }

                Vector3 chosen = validPositions[Random.Range(0, validPositions.Count)];

                newTargets.Add(obj, chosen);
                availablePositions.Remove(chosen);
            }

            if (success)
            {
                foreach (var pair in newTargets)
                {
                    targetPositions[pair.Key] = pair.Value;
                }
            }
        }

        if (!success)
        {
            Debug.LogWarning("Couldn't find a valid shuffle after 100 attempts.");
        }
    }

    public void ReceiveChoiceResults(bool BottleResult)
    {
        CorrectBottleChosen = BottleResult;
        DetermineResult();
    }

    void DetermineResult()
    {
        if (CorrectBottleChosen)
        {
            EndBottleTrial();
        }
    }
    bool AllObjectsReachedDestination()
    {
        foreach (GameObject obj in GhostBottles)
        {
            if (obj == null) continue;

            if (Vector3.Distance(obj.transform.position, targetPositions[obj]) > 0.01f)
                return false;
        }

        return true;
    }

    private void MoveBottles()
    {
        foreach (GameObject obj in GhostBottles)
        {
            if (obj == null) continue;

            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPositions[obj], Speed);
        }
    }


    void EndBottleTrial()
    {
        TrialActive = false;
        TrialManager.GetComponent<TrialManager>().TrialCompletion(GhostBottles[CorrectBottleIndex].transform.position, PlankToDestroy);
        Destroy(gameObject);
    }
}
