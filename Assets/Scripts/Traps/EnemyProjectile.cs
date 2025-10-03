using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : EnemyDamage //will damage the player when collisi
{
    [SerializeField] private float speed;
    public float resetTime;
    private float lifetime;
    public void ActivateProjectile()
    {
        lifetime = 0;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > resetTime)
            gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision); //pouzije to co se stane pri kolizi z enemy damage scriptu
        gameObject.SetActive(false); //deactivuje se kdyz narazi
    }
}
