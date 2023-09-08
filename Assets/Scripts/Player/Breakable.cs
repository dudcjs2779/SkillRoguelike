using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;
    public bool existWall;

    public float durationTime;

    Animator anim;

    [SerializeField] ParticleSystem WallBreak;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        existWall = true;
        Invoke("FadeOut", durationTime - 1f);
        Destroy(transform.gameObject, durationTime);

    }
    
    void Update()
    {
        
    }

    void FadeOut()
    {
        anim.SetTrigger("FadeOut");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            float damage;

            EnemyBullet enemyBullet = other.GetComponent<EnemyBullet>();
            damage = enemyBullet.damage;

            curHealth = curHealth - damage;
        }

        if (other.CompareTag("EnemyWeapon"))
        {
            float damage;

            EnemyWeapon enemyWeapon = other.GetComponentInChildren<EnemyWeapon>();
            damage = enemyWeapon.damage;

            curHealth = curHealth - damage;
        }

        if (curHealth <= 0)
        {
            WallBreak.Play();
            transform.GetComponent<BoxCollider>().enabled = false;
            transform.GetComponent<Animator>().SetTrigger("Destroy");
            Destroy(transform.gameObject, 3f);
        }
    }

    IEnumerator OnHit()
    {

        yield return null;
    }

}
