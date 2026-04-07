using UnityEngine;
using UnityEngine.SceneManagement; // povoluje nám to pøepínat scény!

public class DoorTransition : MonoBehaviour
{
    [Header("Kam ty dveøe vedou?")]
    public string nazevSceny;

    private bool isPlayerAtDoor = false; // hlídá jestli hráè stojí ve dveøích

    void Update()
    {
        // pokud hráè stojí ve dveøích a zmáèkne "E" nebo šipku nahoru
        // if (isPlayerAtDoor && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.UpArrow)))
        if (isPlayerAtDoor)
        {
            Debug.Log("Naèítám novou lokaci: " + nazevSceny);
            SceneManager.LoadScene(nazevSceny); 
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerAtDoor = true; // Hráè vstoupil do zóny dveøí
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerAtDoor = false; // Hráè odešel od dveøí
            // (Zde nápis vypneme)
        }
    }
}