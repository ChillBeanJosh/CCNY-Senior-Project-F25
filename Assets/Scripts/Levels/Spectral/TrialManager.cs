using NUnit.Framework.Internal;
using UnityEngine;

public class TrialManager : MonoBehaviour
{
    [Tooltip("The Wisp prefab that will be instantiated when the trial ends")]
    [SerializeField] GameObject Wisp;
    int TrialCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Trial Count: " + TrialCount);
    }

    public void TrialCompletion(Vector3 position, GameObject PlankTarget)
    {
        TrialCount++;
        var wispObject = Instantiate(Wisp, position, Quaternion.identity);
        ConfirmTrial confirmTrial = wispObject.GetComponent<ConfirmTrial>();
        // Set the GhostPlank reference in the ConfirmTrial script
        confirmTrial.SetGhostPlank(PlankTarget);
    }
}
