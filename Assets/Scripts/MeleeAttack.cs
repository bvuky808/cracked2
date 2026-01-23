using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform attackPoint;

    [Header("Stats")]
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    [Header("Targets")]
    public LayerMask enemyLayers;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            // Tady už NEREŠÍME zásah, jen spustíme vizuál
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.X))
            {
                animator.SetTrigger("Attack"); // Spustí animaci
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    // TUTO funkci zavolá až samotná animace v pøesný moment!
    // Musí být PUBLIC, aby ji Event vidìl.
    public void DealDamage()
    {
        Debug.Log("POKUS O ÚTOK!");
        // Detekovat vše v dosahu
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Poškodit
        foreach (Collider2D obj in hitObjects)
        {
            DestructibleWall wall = obj.GetComponent<DestructibleWall>();
            if (wall != null)
            {
                wall.TakeDamage(attackDamage);
            }

            // Debug pro kontrolu
            Debug.Log("Zásah v pøesný moment: " + obj.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}