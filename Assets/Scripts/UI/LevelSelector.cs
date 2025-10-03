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




    public void Level1()
    {
        SceneManager.LoadScene("Level1");
    }
    public void Level2()
    {
        SceneManager.LoadScene("Level2");
    }
    public void Level3()
    {
        SceneManager.LoadScene("Level3");
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
