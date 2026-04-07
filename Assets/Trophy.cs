using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    [Header("UI nápis")]
    public GameObject victoryText; 

    private void Start()
    {

        if (victoryText != null)
        {
            victoryText.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (victoryText != null)
            {
                // zapneme
                victoryText.SetActive(true);

                // Volitelný bonus: Zastaví èas ve høe, aby hráè už nemohl bìhat dál
                // Time.timeScale = 0f; 
            }
        }
    }
}