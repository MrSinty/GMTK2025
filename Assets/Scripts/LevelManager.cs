using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuObject;

    public void ChangeLevel(string levelName)
    {
        if (levelName != string.Empty && levelName != null)
        {
            SceneManager.LoadSceneAsync(levelName);
        }
        else
            Debug.Log($"Couln't load level \"{levelName}\"");
    }

    public void Unpause()
    { 
        pauseMenuObject.SetActive(false);
        CharacterController2D.isPaused = false;
        Time.timeScale = 1;
    }
}
