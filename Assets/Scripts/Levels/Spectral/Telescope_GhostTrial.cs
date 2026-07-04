using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Telescope_GhostTrial : MonoBehaviour
{
    [Header("Trial Settings")]
    [SerializeField] float Speed = 0.2f;

    [Header("Trial References")]
    [Tooltip("First position object will fly to before starting trialsa")]
    [SerializeField] Vector3 TrialStartPosition;
    [Tooltip("Where to send barnacles when trial starts")]
    [SerializeField] Vector3 BarnacleParentPosition;
    [Tooltip("The Door Plank the Wisp will fly to and destroy")]
    [SerializeField] GameObject PlankToDestroy;
    [Tooltip("Trial Manager script that tracks the completion of trials and spawning of wisps")]
    [SerializeField] GameObject TrialManager;
    [SerializeField] Material GhostMaterial;

    [Header("Trial Objects")]
    [Tooltip("Barnacles used in the trial")]
    [SerializeField] GameObject[] Barnacles;
    [Tooltip("Barnacle Parent Holder")]
    [SerializeField] GameObject BarnacleParent;
    [Tooltip("Gives telescope to player when picked up")]
    [SerializeField] GameObject TelescopePickup;
    [Tooltip("Used to take away telescope when trial is over")]
    [SerializeField] SunWheelController SunwheelUI;

    Vector3 whereToSpawnWisp;
    Vector3 DefaultBarnaclePosition;
    Quaternion originalRotation;
    Vector3 originalPosition;
    Material originalMaterial;
    int CurrentDest;

    bool PreparingTrial = false; // True when trial animations and preparations are active, false when trial is ready to start
    bool TrialActive = false; // True when trial sequence is active, false when the trial sequence is not active
    bool TrialCompleted = false; // True when the trial is completed, false when the trial is not completed
    bool BookTrialRunning = false; // True when trial preparation or trial is active, false when trial is not active and not preparing


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation;
        DefaultBarnaclePosition = BarnacleParent.transform.position;
        originalMaterial = this.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (PreparingTrial) StartTelescopeTrialSequence();

        if (TrialActive) TelescopeTrial();

        if (TrialActive || PreparingTrial) BookTrialRunning = true;
        Debug.Log(whereToSpawnWisp);
    }

    public void StartTrial(bool TrialStart = false)
    {
        PreparingTrial = TrialStart;
        this.GetComponent<Renderer>().material = GhostMaterial;
        this.GetComponent<BoxCollider>().enabled = false;
    }

    void StartTelescopeTrialSequence()
    {
        Vector3 dest = TrialStartPosition;
        transform.position = Vector3.MoveTowards(transform.position, dest, 0.1f);

        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            this.GetComponent<Renderer>().enabled = false;

            BarnacleParent.transform.position = BarnacleParentPosition;

            TelescopePickup.SetActive(true);

            PreparingTrial = false;
            TrialActive = true;
        }
    }

    void TelescopeTrial()
    {
        //Any barnacles that go inactive, get removed from the list of barnacles. When the list is empty, the trial is completed.
        for (int i = 0; i < Barnacles.Length; i++)
        {
            if (!Barnacles[i].activeSelf)
            {
                List<GameObject> barnacleList = Barnacles.ToList();
                barnacleList.Remove(Barnacles[i]);
                Barnacles = barnacleList.ToArray();
            }
        }

        if (Barnacles.Length == 1)
        {
            whereToSpawnWisp = Barnacles[0].transform.position;
        }
        if (Barnacles == null || Barnacles.Length <= 0)
        {
            Debug.Log("Telescope Trial");
            CompleteTelescopeTrial(whereToSpawnWisp);
        }
    }

    void CompleteTelescopeTrial(Vector3 WispSpawn)
    {
        TrialActive = false;
        TakeAwayTelescope();
        //TrialManager.GetComponent<TrialManager>().CleanupTrial();
        TrialManager.GetComponent<TrialManager>().UpdateTrialStatus(false);
        TrialManager.GetComponent<TrialManager>().TrialCompletion(WispSpawn, PlankToDestroy);
        Debug.Log("Telescope Trial Completed");
        this.gameObject.transform.position = Vector3.zero;
    }

    void TakeAwayTelescope()
    {
        GameManager.Instance.Player.item.SetActive(false);
        SunwheelUI.RemoveAbility(SunSpike.SunSpikeType.Telescope);
    }
}
