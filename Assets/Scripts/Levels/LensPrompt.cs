using UnityEngine;

public class LensPrompt : MonoBehaviour
{
    ProjectorTraversal projector;
    public GameObject text;
    Outline outline;

    void Start()
    {
        outline = GetComponentInChildren<Outline>();
        if (outline != null)
            outline.enabled = false;
        if (text != null)
            text.SetActive(false);
    }

    void Update()
    {
        if (text == null || outline == null) return;

        if (projector != null)
        {
            if (!text.activeInHierarchy && !projector.isInsideProjector)
            {
                text.SetActive(true);
                outline.enabled = true;
            }
            if (text.activeInHierarchy && projector.isInsideProjector)
            {
                text.SetActive(false);
                outline.enabled = false;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (text == null || outline == null) return;

            text.SetActive(true);
            outline.enabled = true;
            projector = col.gameObject.GetComponent<ProjectorTraversal>();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (text == null || outline == null) return;

            text.SetActive(false);
            outline.enabled = false;
            projector = null;
        }
    }
}
