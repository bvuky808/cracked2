using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Settings")]
    [Range(0, 10)] public float aheadDistance = 1f;   // Pohled dopøedu
    public float lookAheadSpeed = 2f;

    [Header("Height Settings")]
    public float yOffset = 0.5f;      // Základní výška (trochu nad hráèem)
    public float fallOffset = -2f;    // O kolik sjede kamera dolù, když padáš
    public float fallThreshold = -5f; // Jak rychle musíš padat, aby se to aktivovalo

    [Header("Damping")]
    public float smoothTime = 0.2f;   // Celková plynulost

    private Vector3 currentVelocity;
    private float currentLookAheadX;
    private float currentYOffset;
    private Rigidbody2D playerRB;

    void Start()
    {
        if (player != null)
            playerRB = player.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 1. X OSA (Pohled do stran)
        float targetLookAhead = aheadDistance * player.localScale.x;
        currentLookAheadX = Mathf.Lerp(currentLookAheadX, targetLookAhead, Time.deltaTime * lookAheadSpeed);

        // 2. Y OSA (Chytrý pohled dolù pøi pádu)
        float targetYOffset = yOffset;

        // Pokud máme Rigidbody a padáme rychleji než je limit (fallThreshold)
        if (playerRB != null && playerRB.velocity.y < fallThreshold)
        {
            // Pøepneme cílový offset na "pádový"
            targetYOffset = fallOffset;
        }

        // Plynulý pøechod mezi normální výškou a pádovou výškou
        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset, Time.deltaTime * 2f);

        // 3. Finální pozice
        Vector3 targetPosition = new Vector3(
            player.position.x + currentLookAheadX,
            player.position.y + currentYOffset,
            transform.position.z
        );

        // 4. SmoothDamp pohyb
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}
