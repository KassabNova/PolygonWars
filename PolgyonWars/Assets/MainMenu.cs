using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayPolygonWars()
    {
        LoadSceneParameters sceneParameters = new LoadSceneParameters();
        SceneManager.LoadScene("WaitingLobby",sceneParameters);
    }

    public void ExitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
