using UnityEngine;

public class LensPrompt : MonoBehaviour
{
    ProjectorTraversal projector;
    public ButtonIndicator indicator;
    Outline outline;
    private bool _isHiddenByProjector;

    void Start()
    {
        outline = GetComponentInChildren<Outline>();
        if (outline != null)
            outline.enabled = false;
        
        if (indicator != null)
            indicator.Exit();
    }

    void Update()
    {
        if (indicator == null) return;

        if (projector != null)
        {
            if (projector.isInsideProjector)
            {
                if (!_isHiddenByProjector)
                {
                    indicator.Exit();
                    if (outline != null) outline.enabled = false;
                    _isHiddenByProjector = true;
                }
            }
            else
            {
                if (_isHiddenByProjector)
                {
                    indicator.Appearance("Q");
                    if (outline != null) outline.enabled = true;
                    _isHiddenByProjector = false;
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (indicator == null) return;

            indicator.Appearance("Q");
            if (outline != null) outline.enabled = true;
            projector = col.gameObject.GetComponent<ProjectorTraversal>();
            _isHiddenByProjector = false;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (indicator == null) return;

            indicator.Exit();
            if (outline != null) outline.enabled = false;
            projector = null;
        }
    }
}
