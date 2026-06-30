using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Telescope_GhostTrial : MonoBehaviour
{
    [Header("Trial Settings")]
    [SerializeField] float Speed = 0.2f;
    [SerializeField] float TimeIdle = 2.55f;
    float IdleTimer = 0.0f;


    [SerializeField] List<Vector3> Destinations;
    private int CurrentDest;
    [SerializeField] Quaternion[] rotations;
    [SerializeField] GameObject Wisp;
    [SerializeField] GameObject PlankToDestroy;

    Quaternion originalRotation;
    Vector3 originalPosition;

    bool ActivatingTrial = true;
    bool TrialActive = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation;
        IdleTimer = TimeIdle;
    }

    public void StartTrial(bool TrialStart = false)
    {
        TrialActive = TrialStart;
        Debug.Log("Trial started: " + TrialStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (ActivatingTrial)
        {
            ActivateBookTrial();
            
        }

        if (TrialActive)
        {
            BookTrial();
        }
        else
        {
            EndBookTrial();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !TrialActive)
        {
            ActivatingTrial = true;
        }
    }


    void ActivateBookTrial()
    {
        TrialActive = true;

        Vector3 dest = Destinations[0];
        transform.position = Vector3.MoveTowards(transform.position, dest, 0.05f);
        transform.rotation = rotations[Random.Range(0, rotations.Length)];
        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            ActivatingTrial = false;
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
        EndBookTrial();
    }
    void EndBookTrial()
    {
        //this.transform.position = originalPosition;
        //this.transform.rotation = originalRotation;
        TrialActive = false;
        var wispObject = Instantiate(Wisp, transform.position, Quaternion.identity);
        ConfirmTrial confirmTrial = wispObject.GetComponent<ConfirmTrial>();
        // Set the GhostPlank reference in the ConfirmTrial script
        confirmTrial.SetGhostPlank(PlankToDestroy);
    }
}
