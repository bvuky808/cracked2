using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Settings")]
    public float bounceForce = 20f; // Jak silnì to vystøelí

    [Header("References")]
    public Animator animator; // Pokud máš animaci pro trampolínu (zmáèknutí)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Zkontrolujeme, jestli je to hráè
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. Zkontrolujeme, jestli na trampolínu dopadl SHORA
            // (Aby tì nevystøelila, když do ní narazíš z boku)
            if (collision.transform.position.y > transform.position.y)
            {
                // Získáme Rigidbody hráèe
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

                if (playerRb != null)
                {
                    // 3. Aplikujeme sílu
                    // Dùležité: Nejdøív vynulujeme aktuální Y rychlost (pád), 
                    // aby byl odraz vždy stejnì silný.
                    playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);

                    // 4. Pøehrajeme animaci (pokud existuje)
                    if (animator != null)
                    {
                        animator.SetTrigger("jump");
                    }
                }
            }
        }
    }
}