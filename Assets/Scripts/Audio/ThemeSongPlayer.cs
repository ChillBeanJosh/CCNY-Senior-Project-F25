using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ThemeSongPlayer : MonoBehaviour
{
    void Start()
    {
        AudioController.Instance.Play("Theme Song");
    }
}
