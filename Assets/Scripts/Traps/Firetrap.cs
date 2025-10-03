using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firetrap : MonoBehaviour
{
    public float firetrapDamage;
    [Header("Firetrap Timers")]
    public float activationDelay;
    public float activeTime;
    public Animator anim;
    public SpriteRenderer spriteRend;

    private bool triggered;
    private bool active;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!triggered)
            {
                StartCoroutine(ActivateFiretrap());
            }
            if (active)
            {
                collision.GetComponent<Health>().TakeDamage(firetrapDamage);
            }
        }
    }
    private IEnumerator ActivateFiretrap()
    {
        //je triggered hrac do ni narazil
        triggered = true;
        spriteRend.color = Color.red;

        //delay aby mel cas na escape
        yield return new WaitForSeconds(activationDelay);
        spriteRend.color = Color.white;

        //trapka se zapne
        active = true;
        anim.SetBool("activated", true);

        //delay a vypne se
        yield return new WaitForSeconds(activeTime);
        active = false;
        triggered = false;
        anim.SetBool("activated", false);
    }
    
}
