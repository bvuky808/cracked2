using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHead : EnemyDamage
{
    [Header ("Spikehead Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;
    private float checkTimer;
    private bool attacking;
    private Vector3 destination;
    private Vector3[] directions = new Vector3[4];

    private void OnEnable()
    {
        Stop();

    }
    private void Update()
    {
        if (attacking)//move to destination only if attacking
        {
            transform.Translate(destination * Time.deltaTime * speed);
        }
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
            {
                CheckForPlayer();
            }
        }
    }
    private void CheckForPlayer()
    {
        CalculateDirections();

        //Check if spikehead sees player in all 4 directions
        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

            if (hit.collider != null && !attacking)
            {
                attacking = true;
                destination = directions[i];
                checkTimer = 0;
            }
        }
    }
    private void CalculateDirections()
    {
        directions[0] = transform.right * range; //right direc
        directions[1] = -transform.right * range; //left direc
        directions[2] = transform.up * range; //up direc
        directions[3] = -transform.up * range; //down direc
    }
    private void Stop()
    {
        destination = transform.position; // dam destinaci na current pozici takze se zastavi
        attacking = false;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        base.OnTriggerEnter2D(_collision);
        //Stopnout to kdyz neco hitne
        Stop();
    }
}
