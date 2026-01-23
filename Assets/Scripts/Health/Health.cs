using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("iFrames Settings")]
    public float iFramesDuration = 1f;
    public float numberOfFlashes = 3f;

    [Header("Respawn & Checkpoints")]
    // HLAVNÕ CHECKPOINT (LaviËka/Start) - sem jdeö, kdyû um¯eö ˙plnÏ
    public Transform mainCheckpoint;

    // POSLEDNÕ BEZPE»N¡ POZICE - sem jdeö, kdyû spadneö do hrot˘ (Soft Respawn)
    private Vector3 lastSafePos;
    private float safeTimeCooldown; // »asovaË, abychom neukl·dali pozici na kraji propasti

    void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        movementScript = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        // Na zaË·tku je bezpeËn· pozice tam, kde zaËÌn·ö
        lastSafePos = transform.position;

        // Pojistka: Pokud jsi zapomnÏl nastavit Main Checkpoint, vytvo¯Ìme ho na startu
        if (mainCheckpoint == null)
        {
            GameObject startPoint = new GameObject("StartPoint");
            startPoint.transform.position = transform.position;
            mainCheckpoint = startPoint.transform;
        }
    }

    void Update()
    {
        // LOGIKA PRO UKL¡D¡NÕ BEZPE»N… POZICE
        // Pokud ûijeme a m·me script pro pohyb
        if (movementScript != null && !dead)
        {
            // Kontrola: StojÌme pevnÏ na zemi?
            if (IsGroundedCheck())
            {
                // P¯iËÌt·me Ëas, jak dlouho uû stojÌme
                safeTimeCooldown += Time.deltaTime;

                // Teprve kdyû stojÌme na zemi dÈle neû 0.2 vte¯iny, uloûÌme pozici
                // To zabr·nÌ uloûenÌ pozice, kdyû jen ökrtneö o kraj p¯i p·du
                if (safeTimeCooldown > 0.1f)
                {
                    lastSafePos = transform.position;
                }
            }
            else
            {
                // Jsme ve vzduchu -> resetujeme ËasovaË
                safeTimeCooldown = 0f;
            }
        }
    }

    private bool IsGroundedCheck()
    {
        if (movementScript == null) return false;

        // TOTO JE NOV…: VykreslÌ Ëervenou Ë·ru ve Scene oknÏ
        // VidÌö tak p¯esnÏ, kam aû paprsek sah·
        Debug.DrawRay(transform.position, Vector2.down * 2.5f, Color.red);

        // ZvÏtöil jsem dosah z 1.5 na 2.5, aby to s jistotou dos·hlo na zem
        return Physics2D.Raycast(transform.position, Vector2.down, 2.5f, movementScript.groundLayer);
    }

    // --- HLAVNÕ FUNKCE PRO ZRANÃNÕ ---
    public void TakeDamage(float _damage)
    {
        // OdeËteme ûivoty, ale nejdeme pod nulu
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            // ZranÏnÌ, ale ûijeme
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
        // 1. Ubere ûivot (spustÌ blik·nÌ atd.)
        TakeDamage(_damage);

        // 2. Pokud st·le ûijeme, vr·tÌme se na POSLEDNÕ PEVNOU ZEM
        if (!dead)
        {
            transform.position = lastSafePos;
            rb.velocity = Vector2.zero; // ZastavÌme setrvaËnost, aù nevylÈtneö
        }
        // Pokud jsme um¯eli (HP=0), funkce TakeDamage uû zavolala Die()
    }

    private void Die()
    {
        dead = true;
        anim.SetTrigger("die");

        // Vypneme ovl·d·nÌ a fyziku
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
        // DŸLEéIT…: Zkontroluj ËÌsla vrstev (10=Player, 11=Enemy/Hazard)
        Physics2D.IgnoreLayerCollision(10, 11, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f); // »erven· polopr˘hledn·
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
            spriteRend.color = Color.white; // Norm·lnÌ
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes * 2));
        }

        Physics2D.IgnoreLayerCollision(10, 11, false);
    }

    // respqn na konci animace die
    public void Respawn()
    {
        dead = false;
        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");

        // Zapneme zp·tky fyziku a pohyb
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (movementScript != null) movementScript.enabled = true;

        // --- HARD RESPAWN: N·vrat k LAVI»CE ---
        if (mainCheckpoint != null)
        {
            transform.position = mainCheckpoint.position;
            lastSafePos = mainCheckpoint.position; // Resetujeme i bezpeËnou pozici
        }

        StartCoroutine(Invulnerability());
    }

    // Pro LaviËky (Save System)
    public void SetCheckpoint(Transform newPoint)
    {
        mainCheckpoint = newPoint;
        AddHealth(startingHealth); // UloûenÌ doplnÌ ûivoty
    }
}
