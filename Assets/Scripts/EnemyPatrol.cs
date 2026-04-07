using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    private Rigidbody2D rb;
    private bool movingRight = true;

    private Animator anim;

    [Header("Detection")]
    public Transform groundCheck; // Bod pøed nohama (hlídá díru)
    public Transform wallCheck;   // Bod pøed oblièejem (hlídá zeï)
    public float detectionDistance = 0.5f;

    public LayerMask terrainLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        //POHYB Posíláme ho dopøedu aktuálním smìrem
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // kdy se otoèit?
        // raycast dolù (je tam podlaha?)
        bool groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, detectionDistance, terrainLayer);

        // raycast dopøedu (je tam zeï?)
        bool wallInfo = Physics2D.Raycast(wallCheck.position, transform.right, detectionDistance, terrainLayer);

        if (groundInfo == false || wallInfo == true)
        {
            Flip();
        }
    }

    void Flip()
    {
        // otoèíme logický smìr 
        movingRight = !movingRight;


        transform.Rotate(0f, 180f, 0f);
    }

    // pomocné èáry v editoru
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * detectionDistance);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.yellow;
            // kreslíme èáru smìrem, kam kouká objekt
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * detectionDistance);
        }
    }
}
