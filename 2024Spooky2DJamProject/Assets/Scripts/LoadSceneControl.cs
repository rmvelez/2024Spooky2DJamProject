using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneControl : MonoBehaviour
{
    public void BackToStart()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Scene1");
    }
}
