using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterData playerData;

    List<IEndgameObserver> endgameObservers = new List<IEndgameObserver>();

    // getting the Cinemachine back when load to a different scene 
    private CinemachineFreeLook cinemachine;
    protected override void Awake()
    {
        // keep whatever base.Awake has 
        base.Awake();
        // do not destory the sceneController when load a new scene
        DontDestroyOnLoad(this);
    }



    public void rigisterPlayer(CharacterData player)
    {
        playerData = player;
        // get Free look camera 
        cinemachine = FindObjectOfType<CinemachineFreeLook>();
        if (cinemachine != null)
        {
            cinemachine.Follow = playerData.transform;
            cinemachine.LookAt = playerData.transform;
        }

    }

    // when enemy generated, add them into the endgameObservers list
    // when enemy die, remove them from the endgameObservers list 
    public void addToObserver(IEndgameObserver observer)
    {
        endgameObservers.Add(observer);
    }

    public void removeFromObserver(IEndgameObserver observer)
    {
        endgameObservers.Remove(observer);
    }

    // let all the observers know if one of the enemy die, or player dies
    public void notifyObservers()
    {
        foreach (var observer in endgameObservers)
        {
            observer.endGameNotify();
        }
    }

    public Transform getEnterPortTransform()
    {
        var ports = FindObjectsOfType<PortalDestination>();
        foreach (var port in ports)
        {
            if (port.destinationTag == PortalDestination.DestinationTag.C)
            {
                return port.transform;
            }          
        }
        return null;
    }
}
