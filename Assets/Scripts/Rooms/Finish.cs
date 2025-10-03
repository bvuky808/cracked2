using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // když je objekt hrace nebo jakykoli jiny pozadovany
        if (collision.CompareTag("Player")) // ujisti se, ze objekt ma spravny tag
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            // Zkontroluje, zda existuje další scéna
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("Žádná další scéna v Build Settings!");
            }
        }
    }
}

