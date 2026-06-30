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
    bool TrialActive = false;
    private Dictionary<GameObject, Vector3> targetPositions = new Dictionary<GameObject, Vector3>();
    int CorrectBottleIndex;
    bool gameRunning = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CorrectBottleIndex = Random.Range(0, GhostBottles.Count);

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
        if (gameRunning)
        {
            MoveBottles();
        }
        

        //If the correct bottle is no longer active, end the trial
        if (!GhostBottles[CorrectBottleIndex].activeInHierarchy)
        {
            EndBookTrial();
        }
    }

    public void StartTrial(bool TrialStart = false)
    {
        StartCoroutine(TrialStartSequence());
        Debug.Log("Trial started: " + TrialStart);
    }

    IEnumerator TrialStartSequence()
    {
        GhostAnimator.enabled = true; // Enable the animator when the trial starts
        yield return new WaitForSeconds(3f);
        GhostAnimator.SetBool("BottleReady", true); // Play the guessing animation
        yield return new WaitForSeconds(2.75f);
        GhostAnimator.enabled = false; // Disable the animator after the animation is finished

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
        gameRunning = true;
        ShuffleRounds = Mathf.Max(1, ShuffleRounds); // Ensure at least one round
        ShuffleRounds -= 1; // Decrement the rounds since we are starting the first shuffle immediately

        //for (int round = 0; round < shuffleRounds; round++)
        //{
        AssignRandomPositions();

            // Wait until everyone reaches their destination
            yield return new WaitUntil(AllObjectsReachedDestination);

            yield return new WaitForSeconds(RoundInterval);
        //}

        gameRunning = false;
        StartCoroutine(GameLoop());
    }

    
    void ActivateBookTrial()
    {
        TrialActive = true;
        this.GetComponent<Renderer>().material = GhostMat;

        CurrentDest = Random.Range(0, Destinations.Count);
        Vector3 dest = Destinations[CurrentDest];
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);
    }
    void AssignRandomPositions()
    {
        List<Vector3> availablePositions = new List<Vector3>(Destinations);

        foreach (GameObject obj in GhostBottles)
        {
            int index = Random.Range(0, availablePositions.Count);

            targetPositions[obj] = availablePositions[index];

            // Prevent duplicate positions
            availablePositions.RemoveAt(index);
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


    void EndBookTrial()
    {
        TrialActive = false;
        TrialManager.GetComponent<TrialManager>().TrialCompletion(GhostBottles[CorrectBottleIndex].transform.position, PlankToDestroy);
        Destroy(gameObject);
    }
}
