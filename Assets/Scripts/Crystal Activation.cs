using UnityEngine;
using System.Collections.Generic;

public class CrystalActivation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterSwitcher characterSwitcher;


    [Header("Settings")]
    [SerializeField] private float activationTime = 3f;

    // tracks which light reflect beams are hitting crystal at current frame
    private List<LightReflection> beamsHittingThisFrame = new List<LightReflection>();
    private float timer = 0f;
    [SerializeField] MeshRenderer crystal;
    [SerializeField] Material unlit, lit;

    private void LateUpdate()
    {
        bool splitUnlocked = characterSwitcher.isSplitModeUnlocked;

        // split is off: any single beam hitting counts (re-enable)
        // split is on: both beams must be hitting simultaneously (disable)

        bool conditionMet = splitUnlocked
            ? (beamsHittingThisFrame.Count >= 2)
            : (beamsHittingThisFrame.Count >= 1);

        if (conditionMet)
        {
            timer += Time.deltaTime;
            if (crystal.material != lit) crystal.material = lit;
            if (timer >= activationTime)
            {

                timer = 0f;

                if (!splitUnlocked)
                    characterSwitcher.EnableSplitMode();
            }
        }
        else
        {
            if (crystal.material != unlit) crystal.material = unlit;
            timer = 0f;
        }

        beamsHittingThisFrame.Clear();
    }

    // called by light reflection every frame beam hits crystal
    public void RegisterBeamHit(LightReflection beam)
    {
        if (!beamsHittingThisFrame.Contains(beam))
            beamsHittingThisFrame.Add(beam);
    }
}
