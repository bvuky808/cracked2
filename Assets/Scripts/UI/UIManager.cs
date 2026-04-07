using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header ("Game over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenu;

    [Header("Unlock Notifications")]
    public GameObject unlockPopup; // Celý objekt, který se zapíná/vypíná
    public Image unlockImageComponent;

    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //kdyz bude uz aktivni tak unpause
            if (pauseScreen.activeInHierarchy)
                PauseGame(false);
            else
                PauseGame(true);
        }
    }
    #region Game Over
    //Activate game over screen
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        //SoundManager.Instance.PlaySound(gameOverSound);
    }

    //-----------------------
    //Game over buttony (funkce)
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        else
            Time.timeScale = Time.timeScale;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();
    }
    //------------------------
    #endregion



    // Upravili jsme funkci tak, aby pøijímala (string novyText)
    public void ShowUnlockSprite(Sprite novySprite)
    {
        if (unlockPopup != null && unlockImageComponent != null)
        {
            // Pøepíšeme obrázek v UI na ten, co nám poslal Item
            unlockImageComponent.sprite = novySprite;

            // Nastavíme originální velikost obrázku (aby nebyl deformovaný)
            //unlockImageComponent.SetNativeSize();

            StartCoroutine(UnlockRoutine());
        }
    }

    private System.Collections.IEnumerator UnlockRoutine()
    {
        unlockPopup.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        unlockPopup.SetActive(false);
    }
    #region Pause

    public void PauseGame(bool status)
    {
        //kdyz je starus true, pause. kdyz ne unpause
        pauseScreen.SetActive(status);

        if (status)
            Time.timeScale = 0; //zastaví èas
        else
            Time.timeScale = 1;
        
    }
    #endregion
    #region MainMenu
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    #endregion
}
