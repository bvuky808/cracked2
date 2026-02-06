using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Combat Feedback")]
    public float knockbackForce = 5f; // Síla odhození
    public float stunTime = 0.5f;     // Jak dlouho se nebude hýbat
    public Color damageColor = Color.red;

    [Header("Effects")]
    public GameObject deathEffect; // Tady je ten slot pro particle effect!

    private Animator anim;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private EnemyPatrol patrolScript;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        patrolScript = GetComponent<EnemyPatrol>();
        anim = GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        currentHealth -= damage;

        // 1. Zastavíme pohyb
        if (patrolScript != null)
        {
            StartCoroutine(DisableMovement());
        }

        // 2. Aplikujeme Knockback
        if (rb != null && attacker != null)
        {
            Vector2 direction = (transform.position - attacker.position).normalized;
            Vector2 force = new Vector2(direction.x * knockbackForce, 2f);

            rb.velocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);
        }

        // 3. Vizuál
        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Nezapomeò mít nahoøe v metodì Start: anim = GetComponentInChildren<Animator>();

    IEnumerator DisableMovement()
    {
        if (patrolScript != null)
        {
            // 1. Fyzicky ho zastavíme
            patrolScript.enabled = false;

            // 2. Vizuálnì ho pošleme do "Pádu" (Knockback animace)
            // Protože máme Animator na dítìti, musíme ho mít naètený pøes GetComponentInChildren
            Animator anim = GetComponentInChildren<Animator>();

            if (anim != null)
            {
                // Øekneme animátoru: "Dostal jsi ránu, pøehraj Fall/Stun!"
                anim.SetBool("isStunned", true);
            }

            // Èekáme (letí vzduchem / vzpamatovává se)
            yield return new WaitForSeconds(stunTime);

            // 3. Konec stunu - vracíme se do normálu
            if (anim != null)
            {
                // Øekneme animátoru: "Už jsi v pohodì, zaèni chodit."
                anim.SetBool("isStunned", false);
            }

            // 4. Zapneme fyzický pohyb
            patrolScript.enabled = true;
        }
    }

    void Die()
    {
        Debug.Log("Nepøítel znièen!");

        // ZNOVU PØIDÁNO: Vytvoøení efektu smrti
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator FlashEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }
}