using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MovingPieces : MonoBehaviour
{
     public List<Vector3> Destinations;
    private int CurrentDest;
    
    [Header("Speed Settings")]
    public float Speed = 0.1f;
    [SerializeField] bool Stationary = false;   

    [Header("Interval Settings")]
    [SerializeField] bool HasWaitTime = true;
    [SerializeField] float WaitTime = 1f;
    float waitTime;

    //Used to platform Y level equals acid level
    [Header("Equalize Y Settings")]
    [SerializeField] bool EqualizeY = false;
    [SerializeField] GameObject Acid;
    [SerializeField] float YOffset = 0.35f;

    [Header("Rider Settings")]
    [SerializeField] bool HasRiders = true;

    public enum platformMode
    {
        Forward,
        Reverse,
        Stationary
    }
    void Start()
    {
        waitTime = WaitTime;
        if (Stationary)
            platMode = platformMode.Stationary;
        else
            platMode = platformMode.Forward;
    }
    public platformMode platMode;

    void FixedUpdate()
    {
        //If enabled, will set platform Y level to acid level + offset, and will set all destinations Y level to acid level + offset, so platform will always be at the same height as the acid.
        if (EqualizeY && Acid != null)
        {
            transform.position = new Vector3(transform.position.x, Acid.transform.position.y - YOffset, transform.position.z);
            for (int i = 0; i < Destinations.Count; i++)
            {
                Destinations[i] = new Vector3(Destinations[i].x, Acid.transform.position.y - YOffset, Destinations[i].z);
            }
        }

        //Different directions the platform will switch through, will move between destinations in order, then reverse and move back through them in reverse order, then repeat.
        switch (platMode)
        {
            case platformMode.Forward:
                MovePlatformForward();
                break;
            case platformMode.Reverse:
                MovePlatformReverse();
                break;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (HasRiders && other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (HasRiders && other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    //Normal Move platform function, moves platform to destinations in order, and moves riders with it. 

    void MovePlatformForward()
    {
        if (Destinations.Count == 0) return;
        Vector3 dest = Destinations[CurrentDest];
        Vector3 old = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);
        Vector3 movement = transform.position - old;
        Vector3 lastStop = Destinations.Last();

        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            if (HasWaitTime)
            {
                waitTime -= Time.fixedDeltaTime;

                if (waitTime <= 0)
                {
                    if (Vector3.Distance(transform.position, lastStop) < 0.01f)
                    {
                        platMode = platformMode.Reverse;
                    }
                    else
                    {
                        CurrentDest++;
                    }
                    waitTime = WaitTime;
                }
            }
            else 
            {
                if (Vector3.Distance(transform.position, lastStop) < 0.01f)
                {
                    platMode = platformMode.Reverse;
                }
                else
                {
                    CurrentDest++;
                }
            }

        }
    }

    void MovePlatformReverse()
    {
        if (Destinations.Count == 0) return;
        Vector3 dest = Destinations[CurrentDest];
        Vector3 old = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);
        Vector3 movement = transform.position - old;

        if (Vector3.Distance(transform.position, dest) < 0.01f)
        {
            if (HasWaitTime)
            {
                waitTime -= Time.fixedDeltaTime;

                if (waitTime <= 0)
                {
                    CurrentDest--;
                    if (CurrentDest < 0)
                    {
                        CurrentDest = 0;
                        platMode = platformMode.Forward;

                    }
                    waitTime = WaitTime;
                }
            }
            else
            {
                CurrentDest--;
                if (CurrentDest < 0)
                {
                    CurrentDest = 0;
                    platMode = platformMode.Forward;

                }
            }
        }
    }
}
