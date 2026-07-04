using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonSays : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] Material highlightMaterial;

    [Header("Sequence")]
    [SerializeField] int sequenceLength = 4;
    [SerializeField] float highlightTime = 0.5f;
    [SerializeField] float delayBetweenHighlights = 0.25f;

    [Header("Game Settings")]
    [SerializeField] private int startingSequenceLength = 2;

    private int currentSequenceLength;
    private bool gameRunning;
    bool acceptingInput;

    GameObject[] choices;
    Renderer[] renderers;

    // Stores each object's original material
    Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();

    // Tracks whether each object was active last frame
    Dictionary<GameObject, bool> previousActiveStates = new Dictionary<GameObject, bool>();

    // Current sequence
    List<GameObject> sequence = new List<GameObject>();

    // Objects disabled by the player
    List<GameObject> disabledObjects = new List<GameObject>();

    int playerIndex;
    bool showingSequence;

    private void Start()
    {
        CacheObjects();

        currentSequenceLength = startingSequenceLength;
        gameRunning = true;

        StartCoroutine(BeginRound());
    }

    private void Update()
    {
        if (!gameRunning || !acceptingInput)
            return;

        foreach (GameObject obj in choices)
        {
            bool wasActive = previousActiveStates[obj];
            bool isActive = obj.activeSelf;

            if (wasActive && !isActive)
            {
                previousActiveStates[obj] = isActive;

                HandleObjectDisabled(obj);

                // Prevent processing any additional objects this frame.
                return;
            }

            previousActiveStates[obj] = isActive;
        }
    }

    private void CacheObjects()
    {
        int childCount = transform.childCount;

        choices = new GameObject[childCount];
        renderers = new Renderer[childCount];

        for (int i = 0; i < childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;

            choices[i] = obj;
            renderers[i] = obj.GetComponentInChildren<Renderer>();

            if (renderers[i] != null)
                originalMaterials[obj] = renderers[i].material;

            previousActiveStates[obj] = obj.activeSelf;
        }
    }

    private void ReactivateAllObjects()
    {
        foreach (GameObject obj in choices)
        {
            obj.SetActive(true);
            previousActiveStates[obj] = true;
        }

        disabledObjects.Clear();
    }

    IEnumerator BeginRound()
    {
        ReactivateAllObjects();

        GenerateSequence();

        yield return ShowSequence();

        playerIndex = 0;

        showingSequence = false;

        playerIndex = 0;
        acceptingInput = true;

        Debug.Log($"Round Started - Sequence Length: {currentSequenceLength}");
    }

    private void GenerateSequence()
    {
        sequence.Clear();

        List<GameObject> available = new List<GameObject>();

        foreach (GameObject obj in choices)
        {
            if (obj.activeSelf)
                available.Add(obj);
        }

        int count = Mathf.Min(currentSequenceLength, available.Count);

        for (int i = 0; i < count; i++)
        {
            int random = Random.Range(0, available.Count);

            sequence.Add(available[random]);

            // Remove so it cannot be picked twice
            available.RemoveAt(random);
        }
    }

    private IEnumerator ShowSequence()
    {
        acceptingInput = false;
        showingSequence = true;

        foreach (GameObject obj in sequence)
        {
            Renderer rend = obj.GetComponentInChildren<Renderer>();

            rend.material = highlightMaterial;

            yield return new WaitForSeconds(highlightTime);

            rend.material = originalMaterials[obj];

            yield return new WaitForSeconds(delayBetweenHighlights);
        }

        showingSequence = false;
    }

    /// <summary>
    /// Called when the player selects an object.
    /// </summary>
    private void HandleObjectDisabled(GameObject selected)
    {
        disabledObjects.Add(selected);

        if (selected == sequence[playerIndex])
        {
            playerIndex++;

            if (playerIndex >= sequence.Count)
            {
                currentSequenceLength++;
                StartCoroutine(StartNextRound());
            }
            else
            {
                acceptingInput = true;
            }
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator StartNextRound()
    {
        yield return new WaitForSeconds(1f);

        // Prevent the sequence from growing beyond the available objects.
        currentSequenceLength = Mathf.Min(currentSequenceLength, choices.Length);

        yield return BeginRound();
    }
    private void GameOver()
    {
        gameRunning = false;
        acceptingInput = false;

        StopAllCoroutines();

        ReactivateAllObjects();

        Debug.Log("Game Over");
    }

    public void RestartGame()
    {
        StopAllCoroutines();

        ReactivateAllObjects();

        currentSequenceLength = startingSequenceLength;
        playerIndex = 0;
        gameRunning = true;

        StartCoroutine(BeginRound());
    }
}