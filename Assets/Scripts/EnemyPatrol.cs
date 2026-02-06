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

    // Tady vyber "Ground" (a pøípadnì "Hazard", pokud se má otáèet i pøed hroty)
    public LayerMask terrainLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1. POHYB: Posíláme ho dopøedu aktuálním smìrem
        // Používáme transform.right, který se otáèí s objektem
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // 2. DETEKCE: Kdy se otoèit?
        // Raycast dolù (je tam podlaha?)
        bool groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, detectionDistance, terrainLayer);

        // Raycast dopøedu (je tam zeï?)
        // Používáme 'transform.right' aby paprsek šel vždy tam, kam kouká nepøítel
        bool wallInfo = Physics2D.Raycast(wallCheck.position, transform.right, detectionDistance, terrainLayer);

        // LOGIKA: Pokud NENÍ zemì (díra) NEBO JE zeï -> Otoè se
        if (groundInfo == false || wallInfo == true)
        {
            Flip();
        }
    }

    void Flip()
    {
        // Otoèíme logický smìr (jen pro jistotu)
        movingRight = !movingRight;

        // Otoèíme celý objekt o 180 stupòù
        // Tím se otoèí i grafika, i 'transform.right', i detektory
        transform.Rotate(0f, 180f, 0f);
    }

    // Pomocné èáry v editoru
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
            // Kreslíme èáru smìrem, kam kouká objekt
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + transform.right * detectionDistance);
        }
    }
}
