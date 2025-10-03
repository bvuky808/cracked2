using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header ("Health")]
    public float startingHealth;
    public float currentHealth { get; private set; }
    public Animator anim;
    private bool dead;

    [Header("iFrames")]
    public float iFramesDuration;
    public float numberOfFlashes;
    public SpriteRenderer spriteRend;

    [Header("Components")]
    private Behaviour[] components;
    private bool invulnerable;

    void Awake()
    {
        currentHealth = startingHealth;


    }
    public void TakeDamage(float _damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - _damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            StartCoroutine(Invulnerability());
        }
        else
        {
            if (!dead)
            {
                anim.SetTrigger("die");
                GetComponent<PlayerMovement>().enabled = false;
                dead = true;
            }
        }
    }
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, startingHealth);
    }
    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(10,11,true);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes*2));
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(iFramesDuration / (numberOfFlashes*2));
        }
        Physics2D.IgnoreLayerCollision(10, 11, false);
    }
    public void Respawn()
    {
        dead = false;
        AddHealth(startingHealth);
        anim.ResetTrigger("die");
        anim.Play("Idle");
        //StartCoroutine(Invulnerability()); - nesmrtelnost po respawnu

        //aktivuje vsechny movement komponenty
        GetComponent<PlayerMovement>().enabled = true;
    }

}
