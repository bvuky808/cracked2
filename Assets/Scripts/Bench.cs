using UnityEngine;

public class Bench : MonoBehaviour
{
    [Header("Settings")]
    public Transform spawnPoint; // Místo respawnu
    public KeyCode interactKey = KeyCode.E;

    [Header("Visuals")]
    public GameObject interactPrompt; // Bublina "E"
    private Animator anim; // Odkaz na animátor

    private bool isPlayerInRange;
    private bool isActivated = false; // Abychom vìdìli, jestli už svítí
    private Health playerHealth;

    void Start()
    {
        // Najdeme animátor na stejném objektu
        anim = GetComponent<Animator>();

        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Update()
    {
        // Interakce
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            SitDown();
        }
    }

    void SitDown()
    {
        // Pokud ještì není aktivovaná, rozsvítíme ji
        if (!isActivated)
        {
            isActivated = true;
            if (anim != null)
            {
                anim.SetBool("isActivated", true); // Tady øekneme animátoru: ROZSVÍTIT!
            }
        }

        // Doplnìní života a uložení
        if (playerHealth != null)
        {
            playerHealth.SetCheckpoint(spawnPoint);
            playerHealth.AddHealth(100);
            Debug.Log("Hra uložena! Lavièka aktivována.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerHealth = collision.GetComponent<Health>();
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerHealth = null;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}