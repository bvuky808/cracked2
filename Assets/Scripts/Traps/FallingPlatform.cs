using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Settings")]
    public float fallDelay = 0.5f;    // Jak dlouho se klepe, než zmizí
    public float respawnDelay = 2f;   // Za jak dlouho se vrátí
    public GameObject destroyEffect;  // Sem dej ten stejný efekt jako na zeï!

    [Header("Components")]
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private Vector3 originalPos;
    private bool isFalling = false;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reaguje jen na hráèe a pokud už nepadá
        // Poznámka: Hráè se musí dotknout shora (kontrolujeme pøes kolizi)
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            // Kontrola, že hráè je nad plošinou (aby nespadla, když do ní drcneš hlavou)
            if (collision.transform.position.y > transform.position.y)
            {
                StartCoroutine(FallSequence());
            }
        }
    }

    private IEnumerator FallSequence()
    {
        isFalling = true;
        float timer = 0f;

        // FÁZE 1: Klepání (Varování)
        while (timer < fallDelay)
        {
            float x = Random.Range(-0.05f, 0.05f); // Jemné klepání do stran
            transform.position = originalPos + new Vector3(x, 0, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // Vrátíme na støed, aby nebyla posunutá
        transform.position = originalPos;

        // FÁZE 2: Výbuch a Zmizení
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Místo znièení objektu ho jen "vypneme" (neviditelný + nehmotný)
        spriteRenderer.enabled = false;
        boxCollider.enabled = false;

        // FÁZE 3: Èekání na Respawn
        yield return new WaitForSeconds(respawnDelay);

        // FÁZE 4: Návrat
        spriteRenderer.enabled = true;
        boxCollider.enabled = true;
        isFalling = false;
    }
}
