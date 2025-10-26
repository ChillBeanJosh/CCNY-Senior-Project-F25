using UnityEngine;

public class LightModeToggle : MonoBehaviour
{
    [Header("Default Mode: ")]
    public LightAuraFollower lightAuraFollower;


    [Header("Aiming Mode: ")]
    public Transform lightAimingTool;


    void Update()
    {
        //Using M2 to match Aiming Key From Player Controller:
        if (Input.GetMouseButton(1))
        {
            if (lightAuraFollower != null) lightAuraFollower.SetAuraActive(false);
            if (lightAimingTool != null) lightAimingTool.gameObject.SetActive(true);
        }
        else
        {
            if (lightAuraFollower != null) lightAuraFollower.SetAuraActive(true);
            if (lightAimingTool != null) lightAimingTool.gameObject.SetActive(false);
        }
    }
}
