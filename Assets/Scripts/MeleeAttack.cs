using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public Transform attackPoint;

    // --- NOVÉ: Reference na pohyb, abychom vìdìli, jestli jsme kulièka ---
    private PlayerMovement playerMovement;

    [Header("Stats")]
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;

    [Header("Targets")]
    public LayerMask enemyLayers;

    void Start()
    {
        // Najdeme si skript s pohybem
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            // --- ZMÌNA: POKUD JSI KULIÈKA, ZRUŠ ÚTOK ---
            if (playerMovement != null && playerMovement.isRolling) return;

            // Tady už NEREŠÍME zásah, jen spustíme vizuál
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.X))
            {
                animator.SetTrigger("Attack"); // Spustí animaci
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    // TUTO funkci zavolá až samotná animace v pøesný moment!
    public void DealDamage()
    {
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D obj in hitObjects)
        {
            // Zdi (beze zmìny)
            DestructibleWall wall = obj.GetComponent<DestructibleWall>();
            if (wall != null)
            {
                wall.TakeDamage(attackDamage);
            }

            // Nepøátelé
            EnemyHealth enemy = obj.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage, transform);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}