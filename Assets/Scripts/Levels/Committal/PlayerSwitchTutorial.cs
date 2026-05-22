using UnityEngine;

public class PlayerSwitchTutorial : MonoBehaviour
{
    [SerializeField] GameObject p2;
    [SerializeField] GameObject tutorial;
    int numOfSwitches = 0;
    bool fin;
    void Update()
    {
        if (!fin && p2.activeInHierarchy)
        {
            if (!tutorial.activeInHierarchy)
            {
                tutorial.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.C)) numOfSwitches++;

            if (numOfSwitches > 1)
            {
                fin = true;
                tutorial.SetActive(false);
            }
        }
    }
}
