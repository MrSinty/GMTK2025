using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void ChangeLevel(string levelName)
    { 
        if (levelName != string.Empty && levelName != null)
        { 
            SceneManager.LoadSceneAsync(levelName);
        }
        else
            Debug.Log($"Couln't load level \"{levelName}\"");
    }
}
