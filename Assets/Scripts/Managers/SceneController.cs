using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

// Cannot name is SceneManager
public class SceneController : Singleton<SceneController>, IEndgameObserver
{
    private GameObject player;
    public GameObject playerPrefab;

    // we need to stop agent movement after player teleport 
    private NavMeshAgent playerAI;

    // get the fade scene prefab
    public FadeScene fadeScenePrefab;
    bool fadeEffectFinished;

    // add the sceneController as an observer
    private void Start()
    {
        // add the
        GameManager.singletonInstance.addToObserver(this);
        fadeEffectFinished = true;
    }

    protected override void Awake()
    {
        // keep whatever base.Awake has 
        base.Awake();
        // do not destory the sceneController when load a new scene
        DontDestroyOnLoad(this);
    }
    public void teleportToDestination(PortalStart portalStart)
    {
        if (portalStart.portalType == PortalStart.PortalType.SameScene)
        {
            StartCoroutine(teleport(SceneManager.GetActiveScene().name, portalStart.destinationTag));
        }
        else if (portalStart.portalType == PortalStart.PortalType.DifferentScene)
        {
            // passing the scene name and the destinationTag
            StartCoroutine(teleport(portalStart.scene, portalStart.destinationTag));
        }
    }

    IEnumerator teleport(string scene, PortalDestination.DestinationTag destinationTag)
    {
        // save data when using portal
        DataManager.singletonInstance.savePlayerData();

        // create fadeScenePrefab
        var templatefadeScenePrefab = Instantiate(fadeScenePrefab);
        // transit to different scene
        if (SceneManager.GetActiveScene().name != scene)
        {
          
            // adding fade effect
            yield return StartCoroutine(templatefadeScenePrefab.alphaControlFadeOut(1f));
            // Load the target transition scene async 
            yield return SceneManager.LoadSceneAsync(scene);

            // generate player prefab 
            yield return Instantiate(playerPrefab, getDestination(destinationTag).transform.position, getDestination(destinationTag).transform.rotation);
            // Load the playerData after the player prefab be created, not before
            DataManager.singletonInstance.loadPlayerData();
            yield return StartCoroutine(templatefadeScenePrefab.alphaControlFadeIn(1f));
            // end Coroutine
            yield break;
        }
        else // transition in same scene
        {
            // adding fade effect
            yield return StartCoroutine(templatefadeScenePrefab.alphaControlFadeOut(1f));
            // get the transform of the player
            player = GameManager.singletonInstance.playerData.gameObject;
            // when transition, disable the Nav mesh agent
            playerAI = player.GetComponent<NavMeshAgent>();
            playerAI.enabled = false;
            player.transform.SetPositionAndRotation(getDestination(destinationTag).transform.position, getDestination(destinationTag).transform.rotation);
            // After transition, enable the Nav mesh agent
            playerAI.enabled = true;
            yield return StartCoroutine(templatefadeScenePrefab.alphaControlFadeIn(1f));
            yield return null;
        }
    }

    private PortalDestination getDestination(PortalDestination.DestinationTag destinationTag)
    {
        var ports = FindObjectsOfType<PortalDestination>();
        foreach (var port in ports)
        {
            if(port.destinationTag == destinationTag)
            {
                return port;
            }
        }
        return null;
    }
    

    public void loadFirstScene()
    {
        // load the SampleScene because it is the first scene
        StartCoroutine(loadScene("SampleScene"));
    }



    // load the scene where we save the game, when continue load that scene
    public void loadSavedSceneForContinue()
    {
        // load the SampleScene because it is the first scene
        StartCoroutine(loadScene(DataManager.singletonInstance.Scene));
    }

    IEnumerator loadScene(string scene)
    {
        // create fadeScenePrefab
        var templatefadeScenePrefab = Instantiate(fadeScenePrefab);
        if (scene != "")
        {
            // calling the alphaControlFadeOut
            yield return StartCoroutine(templatefadeScenePrefab.alphaControlFadeOut(2f));
            yield return SceneManager.LoadSceneAsync(scene);
            // load the scene and teleport the player by portal
            yield return player = Instantiate(playerPrefab, GameManager.singletonInstance.getEnterPortTransform().position, GameManager.singletonInstance.getEnterPortTransform().rotation);

            // save data 
            DataManager.singletonInstance.savePlayerData();
            // calling the alphaControlFadeIn
            yield return StartCoroutine(templatefadeScenePrefab.alphaControlFadeIn(2f));
            // end Coroutine
            yield break;
        }
    }

    public IEnumerator loadMainMenuScene()
    {
        // create fadeScenePrefab
        var templatefadeScenePrefab = Instantiate(fadeScenePrefab);
      
        // calling the alphaControlFadeOut
        yield return StartCoroutine(templatefadeScenePrefab.alphaControlFadeOut(2f));
        // load the main menu scene 
        yield return SceneManager.LoadSceneAsync("MainMenu");
        // play fadein
        yield return StartCoroutine(templatefadeScenePrefab.alphaControlFadeIn(2f));
        yield break;
       
    }


    // we want to get notified when the player die
    // then we will play fade in/out effect
    public void endGameNotify()
    {
        if (fadeEffectFinished)
        {
            fadeEffectFinished = false;
            StartCoroutine(loadMainMenuScene());
        }
   
    }
}
