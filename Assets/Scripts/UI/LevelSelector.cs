using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{


    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }



    public void BaseNothing()
    {
        SceneManager.LoadScene("BASE_nothing");
    }
    public void Level1()
    {
        SceneManager.LoadScene("Level_1");
    }
    public void Level2()
    {
        SceneManager.LoadScene("Level_2");
    }
    public void Level3()
    {
        SceneManager.LoadScene("Level_3");
    }

    public void LevelFinal()
    {
        SceneManager.LoadScene("Level_Final");
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelector");
    }
    public void Quit()
    {
        Application.Quit();
    }
    //------------------------

    public void MainMenu()
    {
        SceneManager.LoadScene("_MainMenu");
    }
}
