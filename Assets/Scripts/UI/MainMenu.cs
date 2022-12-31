using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    private Button newGameBtn;
    private Button continueGameBtn;
    private Button quitGameBtn;

    // TimeLine animation
    PlayableDirector timeLineAnimationPlayableDirector;

    private void Awake()
    {
        // there are 4 children under the canvas component,
        // [0] Game title text 
        // [1] New Game Button 
        // [2] Continue Game Button 
        // [3] Quit Game Button 
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueGameBtn = transform.GetChild(2).GetComponent<Button>();
        quitGameBtn = transform.GetChild(3).GetComponent<Button>();

        // adding an event listener for new game button
        newGameBtn.onClick.AddListener(timeLinePlay);
        // adding an event listener for continue game button
        continueGameBtn.onClick.AddListener(continueGame);
        // adding an event listener for quit game button
        quitGameBtn.onClick.AddListener(quitGame);
        // getting the playable director component
        timeLineAnimationPlayableDirector = FindObjectOfType<PlayableDirector>();
        timeLineAnimationPlayableDirector.stopped += newGame;
    }

    private void timeLinePlay()
    {
        // play the confirm audio when button is clicked 
        //confirmAudioSource.Play();
        // play time line 
        timeLineAnimationPlayableDirector.Play();

    }
    private void newGame(PlayableDirector pd)
    {
        // delete all previous record
        PlayerPrefs.DeleteAll();
        // switching to the First scene(SampleScene)
        SceneController.singletonInstance.loadFirstScene();
    }
    private void continueGame()
    {
        // play the confirm audio when button is clicked 
        //confirmAudioSource.Play();
        // switching to the scene where you paused
        SceneController.singletonInstance.loadSavedSceneForContinue();

    }
    private void quitGame()
    {
        // play the confirm audio when button is clicked 
        //confirmAudioSource.Play();
        Application.Quit();
    }
}
