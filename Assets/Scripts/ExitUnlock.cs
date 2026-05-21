using UnityEngine;
using MoreMountains.Tools;

public class ExitUnlock : MonoBehaviour
{
    public bool shelfIngredientAdded = false;
    public bool panIngredientAdded = false;

    [SerializeField] private GameObject exitDoor;

    public void AddShelfIngredient()
    {
        if (shelfIngredientAdded) return;
        
        bool firstIngredient = !shelfIngredientAdded && !panIngredientAdded;
        
        shelfIngredientAdded = true;
        TriggerIngredientEvent(firstIngredient);
        CheckIngredients();
    }

    public void AddPanIngredient()
    {
        if (panIngredientAdded) return;

        bool firstIngredient = !shelfIngredientAdded && !panIngredientAdded;

        panIngredientAdded = true;
        TriggerIngredientEvent(firstIngredient);
        CheckIngredients();
    }

    private void TriggerIngredientEvent(bool isFirst)
    {
        if (isFirst)
        {
            MMGameEvent.Trigger("IngredientAdded");
            Debug.Log("First ingredient added");
        }
        else
        {
            MMGameEvent.Trigger("AnotherIngredientAdded");
            Debug.Log("Second ingredient added");
        }
    }

    private void CheckIngredients()
    {
        if (shelfIngredientAdded && panIngredientAdded)
        {
            UnlockExit();
        }
    }

    private void UnlockExit()
    {
        exitDoor.SetActive(false);
    }
}
