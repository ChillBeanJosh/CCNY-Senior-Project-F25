using UnityEngine;
using System.Collections.Generic;

public class CrystalActivation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterSwitcher characterSwitcher;


    [Header("Settings")]
    [SerializeField] private float activationTime = 3f;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem hitParticles;

    // tracks which light reflect beams are hitting crystal at current frame
    private List<LightReflection> beamsHittingThisFrame = new List<LightReflection>();
    private List<Vector3> hitPointsThisFrame = new List<Vector3>();
    private float timer = 0f;

    private void LateUpdate()
    {
        bool splitUnlocked = characterSwitcher.isSplitModeUnlocked;

        // particles fire whenever a beam is hitting
        bool anyBeamHitting = beamsHittingThisFrame.Count >= 1;

        // split is off: any single beam hitting counts (re-enable)
        // split is on: both beams must be hitting simultaneously (disable)

        bool conditionMet = splitUnlocked
            ? (beamsHittingThisFrame.Count >= 2)
            : (beamsHittingThisFrame.Count >= 1);

        if (anyBeamHitting)
        {
            // moves particles to hitpoint and plays if it is not already
            if (hitParticles != null)
            {
                Vector3 averageHitPoint = Vector3.zero;
                foreach (Vector3 point in hitPointsThisFrame)
                    averageHitPoint += point;
                averageHitPoint /= hitPointsThisFrame.Count;

                hitParticles.transform.position = averageHitPoint;

                if (!hitParticles.isEmitting)
                    hitParticles.Play();
            }
        }
        else
        {
            // stops particles when it's no longer hitting
            if (hitParticles != null && hitParticles.isPlaying)
                hitParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (conditionMet)
        {
            timer += Time.deltaTime;
            
            if (timer >= activationTime)
            {
                timer = 0f;

                if (splitUnlocked)
                    characterSwitcher.DisableSplitMode();
                else
                    characterSwitcher.EnableSplitMode();
            }
        }
        else
        {
            timer = 0f;
        }

        beamsHittingThisFrame.Clear();
        hitPointsThisFrame.Clear();
    }

    // called by light reflection every frame beam hits crystal
    public void RegisterBeamHit(LightReflection beam, Vector3 hitPoint)
    {
        if (!beamsHittingThisFrame.Contains(beam))
            beamsHittingThisFrame.Add(beam);

        hitPointsThisFrame.Add(hitPoint);
    }
}
