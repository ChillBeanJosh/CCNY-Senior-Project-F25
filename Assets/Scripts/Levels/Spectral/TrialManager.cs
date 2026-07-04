using NUnit.Framework.Internal;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
    Vector3 DefaultLensPosition;

    [Header("Book Trial")]
    [HideInInspector] public bool BookTrialActive = false;
    [SerializeField] Vector3 Book_LensPosition;

    [Header("Bottle Trial")]
    [HideInInspector] public bool BottleTrialActive = false;
    [SerializeField] Vector3 Bottle_LensPosition;

    [Header("Telescope Trial")]
    [HideInInspector] public bool TelescopeTrialActive = false;


    [Header("References")]
    [Tooltip("The Wisp prefab that will be instantiated when the trial ends")]
    [SerializeField] GameObject Wisp;
    [Tooltip("The Wisp prefab that will be instantiated when the trial ends")]
    [SerializeField] GameObject GhostLens; 
    [Tooltip("Script for taking player off")]
    [SerializeField] ProjectorTraversal PlayerLensScript;
    [SerializeField] GameObject[] TrialTriggers;

    [Header("Level Complete")]
    [SerializeField] GameObject[] SpawnOnLevelComplete;
    [SerializeField] Animator ExitDoorAnimator;


    int TrialCount = 0;
    bool TrialInProgress = false;
    bool AllTrialsCompleted = false;
    bool LevelSetupComplete = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultLensPosition = GhostLens.transform.position; // Store the default position of the lens at the start of the trial
        ExitDoorAnimator.enabled = false; // Enable the animator when the trial starts
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Trial Count: " + TrialCount);

        for (int i = 0; i < TrialTriggers.Length; i++)
        {
            if (TrialTriggers[i].activeSelf && TrialTriggers[i] != null)
            {
                if (TrialInProgress)
                {
                    TrialTriggers[i].GetComponent<StartTrial>().SetCanStartTrial(false);
                }
                else
                {
                    TrialTriggers[i].GetComponent<StartTrial>().SetCanStartTrial(true);
                }
            }
        }
    }

    public void UpdateTrialStatus(bool value)
    {
        TrialInProgress = value;
    }

    public void TrialCompletion(Vector3 position, GameObject PlankTarget)
    {
        TrialCount++;
        var wispObject = Instantiate(Wisp, position, Quaternion.identity);
        ConfirmTrial confirmTrial = wispObject.GetComponent<ConfirmTrial>();
        // Set the GhostPlank reference in the ConfirmTrial script
        confirmTrial.SetGhostPlank(PlankTarget);

        if (TrialCount >= 3) AllTrialsCompleted = true;
        

        if (AllTrialsCompleted)
        {
            //LevelSetupComplete = false;
            for (int i = 0; i < SpawnOnLevelComplete.Length; i++)
            {
                if (SpawnOnLevelComplete[i] != null)
                {
                    SpawnOnLevelComplete[i].SetActive(true);
                }
            }
            ExitDoorAnimator.Play("Open_GhostDoor", 0, 0f); // Places bottle into position
        }
    }

    public void InitiateBookTrial()
    {
        GhostLens.transform.position = Book_LensPosition;
    }
    public void InitiateBottleTrial()
    {
        GhostLens.transform.position = Bottle_LensPosition;
    }

    public void CleanupTrial()
    {
        Projector projector = GhostLens.GetComponent<Projector>();
        if (projector != null) projector.ClearDriver(); // Disable the projector when the trial is completed

        GameManager.Instance.Player.projector = null;
        if (gameObject != null) PlayerLensScript.ExitProjectorMode(); // Disable the projector when the trial is completed
        GhostLens.transform.position = DefaultLensPosition; // Reset the lens to its default position
    }
}
