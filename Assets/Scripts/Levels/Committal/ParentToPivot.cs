using UnityEngine;

public class ParentToPivot : MonoBehaviour
{
    [SerializeField] string pivot;
    void Start()
    {
        transform.parent = GameObject.Find(pivot).transform;
    }
}
