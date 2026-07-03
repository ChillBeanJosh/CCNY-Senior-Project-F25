 using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Book_GhostTrial : MonoBehaviour
{
    [Header("Trial Settings")]
    [SerializeField] float Speed = 0.2f;
    [SerializeField] float TimeIdle = 2.55f;
    float IdleTimer = 0.0f;

    [Header("Trial References")]
    [Tooltip("Positions the object will fly between")]
    [SerializeField] List<Vector3> Destinations;
    [Tooltip("First position object will fly to before starting trial")]
    [SerializeField] Vector3 TrialStartPosition;
    [Tooltip("Rotates the object when the trial begins")]
    [SerializeField] Quaternion[] rotations;
    [Tooltip("The Door Plank the Wisp will fly to and destroy")]
    [SerializeField] GameObject PlankToDestroy;
    [Tooltip("Trial Manager script that tracks the completion of trials and spawning of wisps")]
    [SerializeField] GameObject TrialManager;
    [SerializeField] Material GhostMaterial;

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
        originalMaterial = this.GetComponent<Renderer>().material;
        IdleTimer = TimeIdle;
    }

    // Update is called once per frame
    void Update()
    {
        if (PreparingTrial) StartBookTrialSequence();

        if (TrialActive) BookTrial();
        
        if (TrialActive || PreparingTrial) BookTrialRunning = true;
    }

    public void StartTrial(bool TrialStart = false)
    {
        PreparingTrial = TrialStart;
        this.GetComponent<Renderer>().material = GhostMaterial;
    }

    void StartBookTrialSequence()
    {
        Vector3 dest = TrialStartPosition;
        transform.position = Vector3.MoveTowards(transform.position, dest, 0.1f);
        
        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            this.GetComponent<BoxCollider>().enabled = true;
            transform.rotation = rotations[Random.Range(0, rotations.Length)];
            PreparingTrial = false;
            TrialActive = true;
        }
    }

    void BookTrial()
    {
        if (Destinations.Count == 0) return;
        Vector3 dest = Destinations[CurrentDest];
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);

        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            IdleTimer -= Time.deltaTime;

            if (IdleTimer <= 0)
            {
                CurrentDest = Random.Range(0, Destinations.Count);
                //Make sure same number isn't chosen twice in a row
                while (CurrentDest == Destinations.IndexOf(transform.position))
                {
                    //Quaternion startRotation = transform.rotation;
                    //transform.rotation = Quaternion.Lerp(startRotation, rotations[Random.Range(0, rotations.Length)], 1f);
                    CurrentDest = Random.Range(0, Destinations.Count);
                }

                IdleTimer = TimeIdle;
            }
        }
    }

    private void OnDestroy()
    {
        if (BookTrialRunning && this.gameObject.scene.isLoaded) EndBookTrial();
    }
    void EndBookTrial()
    {
        TrialCompleted = false;
        BookTrialRunning = false;
        TrialManager.GetComponent<TrialManager>().TrialCompletion(this.transform.position, PlankToDestroy);
    }
}
