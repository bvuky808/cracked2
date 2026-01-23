using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 2f;        // Rychlost pohybu
    public int startPoint = 0;      // Kde zaèíná
    public Transform[] points;      // Pole bodù (Waypoints), kudy má jet

    private int i;                  // Index aktuálního bodu

    void Start()
    {
        // Nastavíme startovní pozici na první bod
        transform.position = points[startPoint].position;
        i = startPoint;
    }

    void Update()
    {
        // Zkontrolujeme vzdálenost k cílovému bodu
        // Vector2.Distance je pøesnìjší než prosté porovnání
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++; // Další bod

            // Pokud jsme na konci seznamu bodù, vrátíme se na zaèátek (cyklus)
            if (i == points.Length)
            {
                i = 0;
            }
        }

        // Pohyb smìrem k aktuálnímu bodu
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    // --- TRIK PROTI KLOUZÁNÍ ---
    // Když hráè naskoèí, pøilepíme ho k plošinì
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kontrola, jestli je hráè nad plošinou (aby se nepøilepil z boku nebo zespodu)
        if (collision.gameObject.CompareTag("Player") && collision.transform.position.y > transform.position.y)
        {
            collision.transform.SetParent(transform);
        }
    }

    // Když hráè vyskoèí/odejde, odlepíme ho
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
