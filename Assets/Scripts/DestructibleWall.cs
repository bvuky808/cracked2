using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    [Header("Settings")]
    public int health = 3;          // Kolik ran vydrží
    public float shakeAmount = 0.1f; // Jak moc se klepe pøi zásahu
    public GameObject destroyEffect; // Sem pak dáme efekt (èástice), pokud budeš chtít

    private Vector3 originalPos;
    private bool isShaking = false;

    void Start()
    {
        originalPos = transform.position;
    }

    // Tuto funkci zavoláme z tvého skriptu pro útok (PlayerAttack)
    public void TakeDamage(int damage)
    {
        health -= damage;

        // Zahuèení (Shake effect)
        if (!isShaking && health > 0)
        {
            StartCoroutine(Shake());
        }

        // Znièení
        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsed = 0.0f;
        float duration = 0.15f; // Krátký, rychlý otøes

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            transform.position = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos; // Vrátíme pøesnì na místo
        isShaking = false;
    }

    private void Die()
    {
        // Tady pak vytvoøíme efekt výbuchu (Particle System), jestli nìjaký bude
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Znièení zdi
        Destroy(gameObject);
    }
}
