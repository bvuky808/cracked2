using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Settings")]
    public float fallDelay = 0.5f;      // Jak dlouho èekat, ne spadne
    public float destroyDelay = 2f;     // Za jak dlouho po pádu zmizí úplnì (úklid)

    private bool falling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // DÙLEITÉ: Plošina musí zaèít jako Kinematic, aby nepadala hned!
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kontrolujeme, jestli do nás narazil Hráè a jestli u nepadáme
        // Je nutné, aby tvùj hráè mìl v Unity nastavenı Tag "Player"!
        if (collision.gameObject.CompareTag("Player") && !falling)
        {
            // Kontrola, zda hráè dopadl SHORA (nechceme aby spadla, kdy do ní drcneš hlavou zespodu)
            // Pokud je hráèova Y pozice vyšší ne plošiny, tak stojí na ní.
            if (collision.transform.position.y > transform.position.y)
            {
                StartCoroutine(Fall());
            }
        }
    }

    private IEnumerator Fall()
    {
        falling = true;

        // Tady by se mohla plošina chvilku tøepat (animace), zatím jen èekáme
        yield return new WaitForSeconds(fallDelay);

        // Zmìníme typ na Dynamic -> Unity zapne gravitaci a plošina spadne
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f; // Mùeš zvıšit, aby padala rychleji

        // Volitelné: Vypneme kolize s ostatními vìcmi, aby propadla podlahou pryè
        // GetComponent<BoxCollider2D>().isTrigger = true; 

        // Úklid - znièení objektu po chvíli, a nezatìuje hru, kdy padá do nekoneèna
        Destroy(gameObject, destroyDelay);
    }
}
