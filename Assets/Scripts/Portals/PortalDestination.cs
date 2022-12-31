using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDestination : MonoBehaviour
{
    public enum DestinationTag
    {
        ENTER,
        A,
        B,
        C
    }

    public DestinationTag destinationTag;
}
