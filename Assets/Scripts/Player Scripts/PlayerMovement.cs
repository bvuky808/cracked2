using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask oneWayPlatformLayer;

    [Header("References")]
    public Rigidbody2D body;
    public Animator animator;
    public BoxCollider2D boxCollider;
    public TrailRenderer tr;

    [Header("Roll (Kulièka)")]
    public SpriteRenderer spriteRenderer; 
    public Sprite rollSprite; 
    private Sprite normalSprite; 
    private Vector3 normalVisualScale = new Vector3(0.8f, 0.8f, 0.8f);
    public float rollVisualScaleMultiplier = 0.5f;

    
    public Vector2 rollColliderSize = new Vector2(0.5f, 0.5f);
    public Vector2 rollColliderOffset = new Vector2(0f, -0.25f);

   
    private Vector2 normalColliderSize;
    private Vector2 normalColliderOffset;

    public bool isRolling { get; private set; } = false; // veøejné, aby to vidìl PlayerCombat
    // ------------------------------------

    [Header("Movement Stats")]
    public float moveSpeed = 10f;
    public float jumpPower = 15f;

    [Header("Double Jump Settings")]
    public int extraJumpsValue = 1;
    private int extraJumps;

    [Header("Dash Settings")]
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    [Header("Abilities unlock")]
    public bool dashUnlocked = false;
    public bool doubleJumpUnlocked = false;
    public bool rollUnlocked = false;

    private float wallJumpCooldown;
    private float horizontalInput;
    private float verticalInput;

    private bool canDash = true;
    private bool isDashing;

    private GameObject currentOneWayPlatform;

    void Start()
    {
       
        if (spriteRenderer != null) normalSprite = spriteRenderer.sprite;
        normalColliderSize = boxCollider.size;
        normalColliderOffset = boxCollider.offset;
    }

    void Update()
    {
        if (isDashing) return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

      
        Vector3 currentScale = isRolling ? (normalVisualScale * rollVisualScaleMultiplier) : normalVisualScale;

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(currentScale.x, currentScale.y, currentScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-currentScale.x, currentScale.y, currentScale.z);
        // -------------------------------------------------

        // 
        if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl)) && rollUnlocked)
        {
            if (isRolling)
            {
                // musíme zkontrolovat strop
                if (IsCeilingClear())
                {
                    StandUp();
                }
                else
                {
                    Debug.Log("Nemùžu se postavit, nad hlavou mám zeï!");
                }
            }
            else
            {
                GoToBall();
            }
        }
        // ---------------------------------------------------------

        // ANIMACE
        if (!isRolling) animator.SetBool("Run", horizontalInput != 0);
        animator.SetBool("grounded", isGrounded());

        // reset dostupných jumpù
        if (isGrounded())
        {
            extraJumps = extraJumpsValue;
            if (tr != null) tr.emitting = false;
        }

        // input na dash (Zakázáno, když jsi kulièka)
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && dashUnlocked && !isRolling)
        {
            StartCoroutine(Dash());
            return;
        }

        if (Input.GetAxis("Vertical") < -0.5f && currentOneWayPlatform != null)
        {
            StartCoroutine(DisableCollision());
        }

        // pohyb a jump
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * moveSpeed, body.velocity.y);

            // wall slide (Zakázáno, když jsi kulièka)
            if (onWall() && !isGrounded() && !isRolling)
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
                if (onWall() && !isGrounded() && !isRolling)
                {
                    WallJump();
                }
                else if (isGrounded())
                {
                    PerformJump();
                }
                else if (doubleJumpUnlocked && extraJumps > 0 && !isRolling)
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

    private void GoToBall()
    {
        isRolling = true;
        animator.enabled = false;
        if (spriteRenderer != null && rollSprite != null) spriteRenderer.sprite = rollSprite;

        boxCollider.size = rollColliderSize;
        boxCollider.offset = rollColliderOffset;

        // zjistím kam se hráè kouká
        float currentFlip = Mathf.Sign(transform.localScale.x);

        // vypocitam nový, menší scale
        Vector3 newBallScale = normalVisualScale * rollVisualScaleMultiplier;

        // dáme menší scale, zachováme otáèení (flipped)
        transform.localScale = new Vector3(newBallScale.x * currentFlip, newBallScale.y, newBallScale.z);
    }

    private void StandUp()
    {
        isRolling = false;
        animator.enabled = true;
        if (spriteRenderer != null) spriteRenderer.sprite = normalSprite;

        boxCollider.size = normalColliderSize;
        boxCollider.offset = normalColliderOffset;

        transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

        // zjistím kam je hráè otoèen
        float currentFlip = Mathf.Sign(transform.localScale.x);

        // vrátíme pùvodní scale
        transform.localScale = new Vector3(normalVisualScale.x * currentFlip, normalVisualScale.y, normalVisualScale.z);
        // --------------------------------------------------
    }

    private bool IsCeilingClear()
    {
        // vystøelí paprsek z aktuální pozice kulièky nahoru do výšky normálního hráèe
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, normalColliderSize.y, groundLayer | wallLayer);
        // pokud paprsek prošel mùže se hráè postavit
        return hit.collider == null;
    }


    private void PerformJump()
    {
        body.velocity = new Vector2(body.velocity.x, 0);
        body.velocity = new Vector2(body.velocity.x, jumpPower);

        // animace skoku se pøehraje jen, když nejsme kulièka
        if (!isRolling) animator.SetTrigger("jump");

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

        float dashDirectionX = Input.GetAxisRaw("Horizontal");
        if (dashDirectionX == 0) dashDirectionX = Mathf.Sign(transform.localScale.x);

        Vector2 dashDir = new Vector2(dashDirectionX, 0);
        body.velocity = dashDir.normalized * dashingPower;

        if (tr != null) tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
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
        // zákaz útoku v kulièce
        return horizontalInput == 0 && isGrounded() && !onWall() && !isRolling;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInLayerMask(collision.gameObject.layer, oneWayPlatformLayer))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isInLayerMask(collision.gameObject.layer, oneWayPlatformLayer))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(boxCollider, platformCollider);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(boxCollider, platformCollider, false);
    }

    private bool isInLayerMask(int layer, LayerMask mask)
    {
        return (mask == (mask | (1 << layer)));
    }
}