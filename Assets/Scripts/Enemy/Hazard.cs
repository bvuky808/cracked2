using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 1; // Kolik to ubere životù


    // 2. Varianta: Pokud jsou hroty pevné (odrazíš se od nich)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeHazardDamage(damage);
            }
        }
    }
}
