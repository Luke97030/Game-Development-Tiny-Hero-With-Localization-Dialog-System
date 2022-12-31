using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script is used for start port. 
// It has the information about the portal type, and the information about destination portal 
public class PortalStart : MonoBehaviour
{
    // adding the audio for teleporting
    public AudioSource teleportAudio;

    public enum PortalType
    { 
        SameScene, 
        DifferentScene
    }
    [Header("Portal Info")]
    public string scene;
    public PortalType portalType;
    public PortalDestination.DestinationTag destinationTag;
    // bool variable to indicate if player can portal or not
    private bool canPortal;

    private void Update()
    {
        // when player press E, teleport  
        if (Input.GetKeyDown(KeyCode.E) && canPortal)
        {
            // play the teleport audio
            teleportAudio.Play();
            // Transport
            SceneController.singletonInstance.teleportToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canPortal = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canPortal = false;
    }
}
