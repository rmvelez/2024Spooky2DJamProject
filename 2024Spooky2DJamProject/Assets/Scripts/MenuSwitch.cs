using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSwitch : MonoBehaviour
{
    public GameObject menu;
    public GameObject learn;
    public GameObject credit;

    // Start is called before the first frame update
    void Start()
    {
        ShowScreen(menu);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void HowToPlay()
    {
        ShowScreen(learn);
    }

    public void SpecialThanks()
    {
        ShowScreen(credit);
    }

    public void BackToMenu()
    {
        ShowScreen(menu);
    }

    private void ShowScreen(GameObject gameObjectToShow)
    {
        menu.SetActive(false);
        learn.SetActive(false);
        credit.SetActive(false);
        
        gameObjectToShow.SetActive(true);
    }
}
