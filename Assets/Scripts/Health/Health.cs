using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Pøidáno pro jistotu, kdyby UI manager selhal, ale primárnì to øeší tvùj UIManager
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

    // PØIDÁNO: Odkaz na tvùj UI Manager
    private UIManager uiManager;

    [Header("iFrames Settings")]
    public float iFramesDuration = 1f;
    public float numberOfFlashes = 3f;

    [Header("Respawn & Checkpoints")]
    // HLAVNÍ CHECKPOINT (Lavièka/Start) - sem jdeš, kdy umøeš úplnì
    public Transform mainCheckpoint;

    // POSLEDNÍ BEZPEÈNÁ POZICE - sem jdeš, kdy spadneš do hrotù (Soft Respawn)
    private Vector3 lastSafePos;
    private float safeTimeCooldown; // Èasovaè, abychom neukládali pozici na kraji propasti

    void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        // Na zaèátku je bezpeèná pozice tam, kde zaèínáš
        lastSafePos = transform.position;

        // PØIDÁNO: Najdeme tvùj UIManager ve scénì
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        // LOGIKA PRO UKLÁDÁNÍ BEZPEÈNÉ POZICE
        // Pokud ijeme a máme script pro pohyb
        if (movementScript != null && !dead)
        {
            // Kontrola: Stojíme pevnì na zemi?
            if (IsGroundedCheck())
            {
                // Pøièítáme èas, jak dlouho u stojíme
                safeTimeCooldown += Time.deltaTime;

                // Teprve kdy stojíme na zemi déle ne 0.1 vteøiny, uloíme pozici
                if (safeTimeCooldown > 0.1f)
                {
                    lastSafePos = transform.position;
                }
            }
            else
            {
                // Jsme ve vzduchu -> resetujeme èasovaè
                safeTimeCooldown = 0f;
            }
        }
    }

    private bool IsGroundedCheck()
    {
        if (movementScript == null) return false;

        // Zkrat délku z 2.5f na tøeba 1.2f nebo 1.5f
        return Physics2D.Raycast(transform.position, Vector2.down, 1.2f, movementScript.groundLayer);
    }

    // --- HLAVNÍ FUNKCE PRO ZRANÌNÍ ---
    public void TakeDamage(float _damage)
    {
        // Odeèteme ivoty, ale nejdeme pod nulu
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // Zranìní, ale ijeme
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

    // --- TOTO VOLAJÍ HROTY A PILY (Soft Respawn) ---
    public void TakeHazardDamage(float _damage)
    {
        // 1. Ubere ivot (spustí blikání atd.)
        TakeDamage(_damage);

        // 2. Pokud stále ijeme, vrátíme se na POSLEDNÍ PEVNOU ZEM
        if (!dead)
        {
            transform.position = lastSafePos;
            rb.velocity = Vector2.zero; // Zastavíme setrvaènost, a nevylétneš
        }
        // Pokud jsme umøeli (HP=0), funkce TakeDamage u zavolala Die()
    }

    private void Die()
    {
        dead = true;
        anim.SetTrigger("die");

        // Vypneme ovládání a fyziku
        if (movementScript != null) movementScript.enabled = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }

    // IEnumerator pro blikáni pøi respqnu, 
    private IEnumerator Invulnerability()
    {
        // DÙLEITÉ: Zkontroluj èísla vrstev (10=Player, 11=Enemy/Hazard)
        Physics2D.IgnoreLayerCollision(10, 11, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // Èervená poloprùhledná
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white; // Normální
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
    }

    // Tuto funkci volá Animation Event na konci animace smrti
    public void Respawn()
    {
        // SCÉNÁØ A: MÁME AKTIVNÍ LAVIÈKU -> Oivíme hráèe
        if (mainCheckpoint != null)
        {
            dead = false;
            AddHealth(startingHealth);
            anim.ResetTrigger("die");
            anim.Play("Idle");

            // Zapneme zpátky fyziku a pohyb
            rb.bodyType = RigidbodyType2D.Dynamic;
            if (movementScript != null) movementScript.enabled = true;

            // Teleport na lavièku
            transform.position = mainCheckpoint.position;
            lastSafePos = mainCheckpoint.position; // Resetujeme i bezpeènou pozici

            StartCoroutine(Invulnerability());
        }
        // SCÉNÁØ B: NEMÁME LAVIÈKU -> GAME OVER OBRAZOVKA
        else
        {
            Debug.Log("Game Over! Volám UI Manager.");
            if (uiManager != null)
            {
                uiManager.GameOver(); // <--- Tady se zapne tvoje Game Over obrazovka
            }
            else
            {
                // Pojistka, kdyby UI manager nebyl nalezen
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    // Pro Lavièky (Save System)
    public void SetCheckpoint(Transform newPoint)
    {
        mainCheckpoint = newPoint;
        AddHealth(startingHealth); // Uloení doplní ivoty
    }
}
