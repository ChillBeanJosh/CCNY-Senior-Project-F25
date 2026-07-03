using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpectralEntry : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] Vector3 TeleportLocation;
    [SerializeField] GameObject ButtonIndicator;
    [SerializeField] GameObject CurrentArea;
    [SerializeField] GameObject TargetArea;
    [SerializeField] ButtonIndicator indicator;

    [SerializeField] float ActivateDistance = 3f; // Distance within which the player can activate the teleportation
    [SerializeField] float TeleportDelay = 0.5f; // Delay before teleportation occurs
    [SerializeField] float FadeDuration = 0.5f; // Duration of the fade effect
    [SerializeField] float DelayAfterTeleport = 0.5f; // Delay after teleportation before the player can move again

    bool canTeleport = true; // Flag to prevent multiple teleportations in quick succession 
    bool Resetting = false; // Flag to indicate if the teleportation is resetting

    Material TeleportOn_Mat;
    [SerializeField] Material TeleportOff_Mat;

    SpectralEntry[] TeleportScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TeleportOn_Mat = GetComponent<Renderer>().material;
        TeleportScript = FindObjectsByType<SpectralEntry>(FindObjectsSortMode.None);
        foreach (SpectralEntry tpScript in TeleportScript)
        {
            Debug.Log(tpScript.gameObject.name);
        }
    }

    private void FixedUpdate()
    {
        //When player presses the "E" key and is within the trigger area, teleport them to the designated location
        if (Player != null && Vector3.Distance(Player.transform.position, transform.position) <= ActivateDistance)
        {
            if (canTeleport)
            {
                if (indicator != null)
                {
                    indicator.Appearance("Q");
                }
                    
                ButtonIndicator.GetComponent<Canvas>().enabled = true;

                if (Input.GetKey(KeyCode.Q))
                {
                    canTeleport = false;
                    StartCoroutine(TeleportPlayer());
                }

            }
            else
            {
                //Turn off canvas if the player is on cooldown and within the trigger area
                indicator.Exit();
                ButtonIndicator.GetComponent<Canvas>().enabled = false;
            }
        }
        else
        {
            indicator.Exit();
            ButtonIndicator.GetComponent<Canvas>().enabled = false;
        }


        if (Input.GetKey(KeyCode.P))
        {
            canTeleport = false;
            StartCoroutine(TeleportPlayer());
        }
    }

    IEnumerator TeleportPlayer()
    {
        indicator.Exit();
        ButtonIndicator.GetComponent<Canvas>().enabled = false;
        
        canTeleport = false;
        Debug.Log("Teleporting player to: " + TeleportLocation);
        //yield return new WaitForSeconds(TeleportDelay);

        //Tell the other teleport scripts to reset their cooldowns
        for (int i = 0; i < TeleportScript.Length; i++)
        {
            if (TeleportScript[i] != this)
            {
                TeleportScript[i].DisableTeleporters();
            }
        }
        //TargetArea.SetActive(true);
        yield return new WaitForSeconds(FadeDuration);
        Player.transform.position = TeleportLocation;
        yield return new WaitUntil(() => Player.transform.position == TeleportLocation);
        StartCoroutine(ResetTeleportCooldown());

        for (int i = 0; i < TeleportScript.Length; i++)
        {
            if (TeleportScript[i] != this)
            {
                TeleportScript[i].StartCoroutine(TeleportScript[i].ResetTeleportCooldown());
            }
        }
        //CurrentArea.SetActive(false);
    }

    public void DisableTeleporters()
    {
        canTeleport = false;
        gameObject.GetComponent<Renderer>().material = TeleportOff_Mat;
    }

    IEnumerator ResetTeleportCooldown()
    {
        yield return new WaitForSeconds(DelayAfterTeleport);
        gameObject.GetComponent<Renderer>().material = TeleportOn_Mat;
        canTeleport = true;
    }
}
