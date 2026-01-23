using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    // --- NOVÉ: Vrstva pro prùchozí plošinky ---
    public LayerMask oneWayPlatformLayer;

    [Header("References")]
    public Rigidbody2D body;
    public Animator animator;
    public BoxCollider2D boxCollider;
    public TrailRenderer tr;

    [Header("Movement Stats")]
    public float moveSpeed = 10f;
    public float jumpPower = 15f;

    [Header("Double Jump Settings")]
    public bool doubleJumpUnlocked = true;
    public int extraJumpsValue = 1;
    private int extraJumps;

    [Header("Dash Settings")]
    public bool dashUnlocked = true;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    private float wallJumpCooldown;
    private float horizontalInput;
    private float verticalInput;

    private bool canDash = true;
    private bool isDashing;

    // --- NOVÉ: Promìnná pro plošinku, na které stojíme ---
    private GameObject currentOneWayPlatform;

    void Update()
    {
        if (isDashing) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // otáèení spritù
        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-0.8f, 0.8f, 0.8f);

        // ANIMACE
        animator.SetBool("Run", horizontalInput != 0);
        animator.SetBool("grounded", isGrounded());

        // reset dostupných jumpù
        if (isGrounded())
        {
            extraJumps = extraJumpsValue;

            if (tr != null) tr.emitting = false;
        }

        // input na dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && dashUnlocked)
        {
            StartCoroutine(Dash());
            return;
        }

        // --- NOVÉ: PROPADNUTÍ DOLÙ (Terraria Style) ---
        // Pokud držíš šipku dolù a stojíš na prùchozí plošince
        if (Input.GetAxis("Vertical") < -0.5f && currentOneWayPlatform != null)
        {
            StartCoroutine(DisableCollision());
        }

        // pohyb a jump
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * moveSpeed, body.velocity.y);

            // wall slide
            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 5;
            }

            // jump input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (onWall() && !isGrounded())
                {
                    WallJump();
                }
                else if (isGrounded())
                {
                    PerformJump();
                }
                else if (doubleJumpUnlocked && extraJumps > 0)
                {
                    PerformJump();
                    extraJumps--;
                }
            }
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void PerformJump()
    {
        body.velocity = new Vector2(body.velocity.x, 0); //reset Y, plynulost d jumpu
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        animator.SetTrigger("jump");

        if (tr != null) tr.emitting = true;
    }

    private void WallJump()
    {
        if (horizontalInput == 0)
        {
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
            transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x) * 0.8f, transform.localScale.y, transform.localScale.z);
        }
        else
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);

        wallJumpCooldown = 0;

        if (tr != null) tr.emitting = true;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = body.gravityScale;
        body.gravityScale = 0f;

        // ZMÌNA: Naèítáme pouze horizontální osu. Y nastavíme natvrdo na 0.
        float dashDirectionX = Input.GetAxisRaw("Horizontal");

        // pokud nedržím šipku tak dashne tam kde se dívá
        if (dashDirectionX == 0)
        {
            dashDirectionX = Mathf.Sign(transform.localScale.x);
        }

        // dash nahoru nejde
        Vector2 dashDir = new Vector2(dashDirectionX, 0);

        body.velocity = dashDir.normalized * dashingPower;

        // trail on
        if (tr != null) tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        // trail off
        if (tr != null) tr.emitting = false;

        body.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }


    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }

    // --- NOVÉ FUNKCE PRO ONE WAY PLATFORM ---

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kontrola, jestli jsme stoupli na vrstvu OneWayPlatform
        if (isInLayerMask(collision.gameObject.layer, oneWayPlatformLayer))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Kontrola, jestli jsme odešli z vrstvy OneWayPlatform
        if (isInLayerMask(collision.gameObject.layer, oneWayPlatformLayer))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();

        // Vypneme kolizi mezi hráèem a plošinou
        Physics2D.IgnoreCollision(boxCollider, platformCollider);

        // Èekáme, než propadne
        yield return new WaitForSeconds(0.5f);

        // Zapneme kolizi zpátky
        Physics2D.IgnoreCollision(boxCollider, platformCollider, false);
    }

    // Pomocná funkce pro zjištìní, jestli je layer souèástí LayerMasky
    private bool isInLayerMask(int layer, LayerMask mask)
    {
        return (mask == (mask | (1 << layer)));
    }
}