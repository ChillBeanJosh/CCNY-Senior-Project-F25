using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Book_GhostTrial : MonoBehaviour
{
    [Header("Trial Settings")]
    [SerializeField] float Speed = 1.0f;
    [SerializeField] float TimeIdle = 2.25f;
    float IdleTimer = 0.0f;
    public List<Vector3> Destinations;
    private int CurrentDest;

    Material DefaultMat;
    [SerializeField] Material GhostMat;
    
    Quaternion originalRotation;
    Vector3 originalPosition;

    bool TrialActive = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultMat = this.GetComponent<Renderer>().material;
        originalPosition = this.transform.position;
        originalRotation = this.transform.rotation;
        IdleTimer = TimeIdle;
    }

    // Update is called once per frame
    void Update()
    {
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
        if (other.CompareTag("Player"))
        {
            ActivateBookTrial();
        }
    }


    void ActivateBookTrial()
    {
        TrialActive = true;
        this.GetComponent<Renderer>().material = GhostMat;

        CurrentDest = Random.Range(0, Destinations.Count);
        Vector3 dest = Destinations[CurrentDest];
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);
    }

    void BookTrial()
    {

        if (Destinations.Count == 0) return;
        Vector3 dest = Destinations[CurrentDest];
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);


        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            IdleTimer -= Time.fixedDeltaTime;

            if (IdleTimer <= 0)
            {
                CurrentDest = Random.Range(0, Destinations.Count);
                //Make sure same number isn't chosen twice in a row
                while (CurrentDest == Destinations.IndexOf(transform.position))
                {
                    CurrentDest = Random.Range(0, Destinations.Count);
                }
                IdleTimer = TimeIdle;
            }
        }
        
    }

    void EndBookTrial()
    {
        this.GetComponent<Renderer>().material = DefaultMat;
        this.transform.position = originalPosition;
        this.transform.rotation = originalRotation;
    }
}
