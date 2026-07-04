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
    int numberOfShuffles = 0;

    [Header("Trial References")]
    [Tooltip("Positions the object will switch between")]
    [SerializeField] List<Vector3> Destinations = new List<Vector3>();
    [Tooltip("The bottles used in the guessing mini game - child of this object")]
    [SerializeField] List<GameObject> GhostBottles = new List<GameObject>();
    [Tooltip("The Door Plank the Wisp will fly to and destroy")]
    [SerializeField] GameObject PlankToDestroy;
    [Tooltip("Trial Manager script that tracks the completion of trials and spawning of wisps")]
    [SerializeField] GameObject TrialManager;
    [Tooltip("Trigger box that starts the trial when player enters it")]
    [SerializeField] GameObject TrialTrigger;
    [SerializeField] Material GhostMat;

    int CurrentDest;
    int CorrectBottleIndex;
    Material DefaultMat;
    Animator GhostAnimator;
    Quaternion originalRotation;
    Vector3 originalPosition;
    
    private Dictionary<GameObject, Vector3> targetPositions = new Dictionary<GameObject, Vector3>();

    bool PreparingTrial = false; // True when trial animations and preparations are active, false when trial is ready to start
    bool TrialActive = false; // True when trial sequence is active, false when the trial sequence is not active
    bool TrialResetting = false; // True when the trial is resetting after an incorrect choice, false when the trial is not resetting
    bool TrialCompleted = false; // True when the trial is completed, false when the trial is not completed
    bool BottleTrialRunning = false; // True when trial preparation or trial is active, false when trial is not active and not preparing
    bool CorrectBottleChosen = false; // True when the correct bottle is chosen, false when the correct bottle is not chosen

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GhostBottles.Clear();
        //Add bottle gameobjects to the GhostBottles list if they are not already assigned in the inspector
        foreach (Transform child in transform)
        {
            GhostBottles.Add(child.gameObject);
        }

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

        numberOfShuffles = ShuffleRounds;
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
    }

    public void StartTrial(bool TrialStart = false)
    {
        TrialManager.GetComponent<TrialManager>().InitiateBottleTrial();
        StartCoroutine(TrialStartSequence());
        Debug.Log("Trial started: " + TrialStart);
    }

    IEnumerator TrialStartSequence()
    {
        CorrectBottleIndex = Random.Range(0, GhostBottles.Count);
        //DefaultLensPosition = GhostLens.transform.position; // Store the default position of the lens at the start of the trial

        foreach (GameObject obj in GhostBottles)
        {
            obj.SetActive(true); // Set all bottles to active at the start of the trial
            //obj.GetComponent<GhostBottle>().SetIsCorrectBottle(false); // Reset all bottles to not correct
        }

        //Turn off this mesh renderer so the ghost can be seen
        this.GetComponent<Renderer>().enabled = false;

        //for loop for each bottle to reset the isCorrectBottle variable to false
        for (int i = 0; i < GhostBottles.Count; i++)
        {
            GhostBottle bottleScript = GhostBottles[i].GetComponent<GhostBottle>();
            if (bottleScript != null)
            {
                bottleScript.SetBottles(false);
                bottleScript.SetTrialActive(true); // Set the trial to active for each bottle
            }
        }

        //Set the correct bottle to be the correct one
        GhostBottle correctBottleScript = GhostBottles[CorrectBottleIndex].GetComponent<GhostBottle>();
        if (correctBottleScript != null) {
            correctBottleScript.SetBottles(true);
        } else {
            Debug.LogError("GhostBottle component not found on the correct bottle.");
        }

        //Starting Animations for the trial
        GhostAnimator.enabled = true; // Enable the animator when the trial starts
        GhostAnimator.Play("StartBottleTrial", 0, 0f); // Places bottle into position
        //GhostLens.transform.position = LensPosition; // Move the lens to the specified position
        yield return new WaitForSeconds(2.5f);
        GhostAnimator.Play("SeparateBottles", 0, 0f); // Plays the bottle splitting animatio
        yield return new WaitForSeconds(2.25f);
        GhostAnimator.enabled = false; // Disable the animator after the animation is finished
        
        yield return new WaitForSeconds(TimeBeforeTrialStart);

        //Highlighting correct bottle
        GhostBottles[CorrectBottleIndex].GetComponent<Renderer>().material = GhostMat; // Highlight the correct bottle

        yield return new WaitForSeconds(1f);

        GhostBottles[CorrectBottleIndex].GetComponent<Renderer>().material = DefaultMat; // Reset the material after highlighting

        yield return new WaitForSeconds(1f);

        //Start shuffing the bottles
        StartCoroutine(GameLoop());
    }
    IEnumerator GameLoop()
    {
        if (numberOfShuffles <= 0)
        {
            Debug.LogWarning("shuffleRounds is set to 0 or less. No shuffling will occur.");

            for (int i = 0; i < GhostBottles.Count; i++)
            {
                GhostBottles[i].GetComponent<Collider>().enabled = true; // Disable the collider to prevent further interaction
            }

            yield break; // Exit the coroutine if there are no rounds to shuffle
        }

        TrialActive = true;
        numberOfShuffles = Mathf.Max(1, numberOfShuffles); // Ensure at least one round
        numberOfShuffles -= 1; // Decrement the rounds since we are starting the first shuffle immediately

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
        Debug.Log("ReceiveChoiceResults called with result: " + BottleResult);
        CorrectBottleChosen = BottleResult;
        DetermineResult();
    }

    void DetermineResult()
    {
        Debug.Log("Determining result. CorrectBottleChosen: " + CorrectBottleChosen + ", TrialActive: " + TrialActive + ", SceneLoaded: " + this.gameObject.scene.isLoaded);
        if (CorrectBottleChosen && this.gameObject.scene.isLoaded)
        {
            CompleteBottleTrial();
        }
        else if (!CorrectBottleChosen && this.gameObject.scene.isLoaded)
        {
            StartCoroutine(ResetTrial());
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

    void MoveBottles()
    {
        foreach (GameObject obj in GhostBottles)
        {
            if (obj == null) continue;

            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPositions[obj], Speed);
        }
    }

    void CompleteBottleTrial()
    {
        TrialActive = false;
        TrialManager.GetComponent<TrialManager>().CleanupTrial();
        TrialManager.GetComponent<TrialManager>().UpdateTrialStatus(false);
        TrialManager.GetComponent<TrialManager>().TrialCompletion(GhostBottles[CorrectBottleIndex].transform.position, PlankToDestroy);
        //yield return new WaitForSeconds(2f);
        gameObject.transform.position = Vector3.zero; // Reset position to zero
        //gameObject.SetActive(false); // Disable the game object instead of destroying it to prevent issues with the trial manager
    }

    IEnumerator ResetTrial()
    {
        Debug.Log("Resetting trial due to incorrect choice.");
        TrialActive = false;
        TrialResetting = true;
        TrialManager.GetComponent<TrialManager>().CleanupTrial();
        
        //Set bottle scripts boolean to false so they don't call ReceiveChoiceResults again when they are destroyed
        for (int i = 0; i < GhostBottles.Count; i++)
        {
            Debug.Log("Resetting bottle: " + GhostBottles[i].name);
            GhostBottle bottleScript = GhostBottles[i].GetComponent<GhostBottle>();
            if (bottleScript != null)
            {
                bottleScript.SetBottles(false);
                bottleScript.SetTrialActive(false); // Set the trial to active for each bottle
            }
            //Turn off the bottles
            GhostBottles[i].SetActive(false);
            GhostBottles[i].GetComponent<Collider>().enabled = false; // Disable the collider to prevent further interaction
        }
        yield return new WaitForSeconds(.25f);

        this.GetComponent<Renderer>().enabled = true;
        GhostAnimator.enabled = true; // Enable the animator when the trial starts
        GhostAnimator.Play("RestartBottleTrial", 0, 0f); // Places bottle into position

        yield return new WaitForSeconds(2.5f);

        numberOfShuffles = ShuffleRounds; // Reset the number of shuffles for the next trial
        GhostAnimator.enabled = false; // Disable the animator after the animation is finished
        TrialTrigger.SetActive(true);
        TrialManager.GetComponent<TrialManager>().UpdateTrialStatus(false);
        TrialResetting = false;
    }
}
