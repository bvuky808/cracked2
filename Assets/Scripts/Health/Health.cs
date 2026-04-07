using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// P¯id·no pro jistotu, kdyby UI manager selhal, ale prim·rnÏ to ¯eöÌ tv˘j UIManager
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Health Stats")]
    public float startingHealth = 3;
    public float currentHealth { get; private set; }
    private bool dead;

    [Header("References")]
    public Animator anim;
    public SpriteRenderer spriteRend;
    private PlayerMovement movementScript;
    private Rigidbody2D rb;

    private UIManager uiManager;

    [Header("iFrames Settings")]
    public float iFramesDuration = 1f;
    public float numberOfFlashes = 3f;

    [Header("Respawn & Checkpoints")]
    public Transform mainCheckpoint;

    // POSLEDNÕ BEZPE»N¡ POZICE 
    private Vector3 lastSafePos;
    private float safeTimeCooldown;

    void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        lastSafePos = transform.position;

        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        // LOGIKA PRO UKL¡D¡NÕ BEZPE»N… POZICE
        if (movementScript != null && !dead)
        {
            //  pevnÏ na zemi?
            if (IsGroundedCheck())
            {
                // p¯iËÌt·me Ëas, jak dlouho uû stojÌme
                safeTimeCooldown += Time.deltaTime;

                // teprve kdyû stojÌme na zemi dÈle neû 0.1 vte¯iny uloûÌme pozici
                if (safeTimeCooldown > 0.1f)
                {
                    lastSafePos = transform.position;
                }
            }
            else
            {
                // resetujeme ËasovaË
                safeTimeCooldown = 0f;
            }
        }
    }

    private bool IsGroundedCheck()
    {
        if (movementScript == null) return false;

        return Physics2D.Raycast(transform.position, Vector2.down, 1.2f, movementScript.groundLayer);
    }

    // --- HLAVNÕ FUNKCE PRO ZRANÃNÕ ---
    public void TakeDamage(float _damage)
    {
        // odeËteme ûivoty, ale nejdeme pod nulu
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
        }
        else
        {
            // Smrt
            if (!dead)
            {
                Die();
            }
        }
    }

    // --- TOTO VOLAJÕ HROTY A PILY (Soft Respawn) ---
    public void TakeHazardDamage(float _damage)
    {
        // 1. ubere ûivot
        TakeDamage(_damage);

        // 2.pokud st·le ûijeme, vr·tÌme se na POSLEDNÕ PEVNOU ZEM
        if (!dead)
        {
            transform.position = lastSafePos;
            rb.velocity = Vector2.zero; // zastavÌme setrvaËnost, aù nevylÈtneö
        }
        // pokud jsme um¯eli (HP=0), funkce TakeDamage uû zavolala Die()
    }

    private void Die()
    {
        dead = true;
        anim.SetTrigger("die");

        // vypneme ovl·d·nÌ a fyziku
        if (movementScript != null) movementScript.enabled = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    // IEnumerator pro blik·ni p¯i respqnu, 
    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10, 11, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // Ëerven· polopr˘hledn·
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white; // norm·lnÌ
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
    }

    // tuto funkci vol· Animation Event na konci animace smrti
    public void Respawn()
    {
        // A: M¡ME AKTIVNÕ LAVI»KU oûivÌme hr·Ëe
        if (mainCheckpoint != null)
        {
            dead = false;
            AddHealth(startingHealth);
            anim.ResetTrigger("die");
            anim.Play("Idle");

            rb.bodyType = RigidbodyType2D.Dynamic;
            if (movementScript != null) movementScript.enabled = true;

            transform.position = mainCheckpoint.position;
            lastSafePos = mainCheckpoint.position; // resetujeme i bezpeËnou pozici

            StartCoroutine(Invulnerability());
        }
        // B: NEM¡ME LAVI»KU GAME OVER OBRAZOVKA
        else
        {
            Debug.Log("Game Over! Vol·m UI Manager.");
            if (uiManager != null)
            {
                uiManager.GameOver();
            }
            else
            {
                // kdyby UI manager nebyl nalezen
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    //(Save System)
    public void SetCheckpoint(Transform newPoint)
    {
        mainCheckpoint = newPoint;
        AddHealth(startingHealth); // UloûenÌ doplnÌ ûivoty
    }
}
