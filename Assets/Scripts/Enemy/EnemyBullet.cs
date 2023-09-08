using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerBullet;

public class EnemyBullet : MonoBehaviour
{
    public enum BulletPattern { None, Icicle, RisingWall };
    public enum EffectType { None };

    public string enemyType;
    public string attackName;
    public float damage;

    public float moveSpeed;
    public float rotateSpeed;

    public bool isPierceBullet;

    public Rigidbody rigid;
    public Transform t_Target;

    [SerializeField] BulletPattern bulletPattern;
    [SerializeField] EffectType effectType;

    public bool isBigAttack;

    public List<EnemyWeaponCol> enemyWeaponColList;

    public Collider bulletCol;

    [Header("EFFECT")]
    public GameObject hitEffect;

    public SoundManager.GameSFXType hitSound;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        BulletControl();
    }

    void BulletControl()
    {
        switch (bulletPattern)
        {
            case BulletPattern.None:
                //Debug.Log("None Gudiance");
                break;

            case BulletPattern.Icicle:
                StartCoroutine(Pattern_Icicle());
                break;

            case BulletPattern.RisingWall:
                StartCoroutine(Pattern_RisingWall());
                break;
        }
    }

    IEnumerator Pattern_RisingWall()
    {
        Collider wallCol = transform.GetChild(0).GetComponent<Collider>();

        yield return new WaitForSeconds(0.8f);
        bulletCol.enabled = true;
        yield return new WaitForSeconds(0.1f);
        bulletCol.enabled = false;
        wallCol.enabled = true;

        yield return new WaitForSeconds(2f);
        wallCol.enabled = false;
    }


    IEnumerator Pattern_Icicle()
    {
        float waitTime = 1.5f;

        yield return new WaitForSeconds(waitTime);
        Vector3 targetCenter = t_Target.GetComponent<CapsuleCollider>().center;
        Vector3 velocity = Vector3.zero;
        rigid.isKinematic = false;
        transform.parent = null;

        float duration = 7;
        while (duration > 0)
        {
            Vector3 targetDir = t_Target.position - transform.position + targetCenter;
            float angle = 0;

            velocity += targetDir.normalized * moveSpeed * Time.deltaTime;

            angle = Vector3.SignedAngle(velocity, targetDir, Vector3.up);
            angle = Mathf.Clamp(angle, -1, 1);
            Quaternion turnDir = Quaternion.AngleAxis(angle, Vector3.up);

            velocity = Vector3.ClampMagnitude(turnDir * velocity, 12);

            rigid.velocity = velocity;

            //Debug.DrawRay(transform.position, velocity, Color.blue);

            Quaternion rot = Quaternion.LookRotation(velocity);
            rigid.MoveRotation(Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime));

            duration -= Time.deltaTime;
            yield return null;
        }

        DestroyBullet();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if ((!isPierceBullet) && (other.CompareTag("Player") && !other.GetComponent<Player>().isIframes) || other.CompareTag("Wall") || other.CompareTag("Ground"))
        // {
        //     if(hitSound != SoundManager.GameSFXType.None) SoundManager.Instance.PlayGameSound(hitSound, transform.position);
        //     Instantiate(hitEffect, transform.position, Quaternion.identity);
        //     Destroy(gameObject);
        // }

        if(!isPierceBullet){
            if(other.CompareTag("Wall") || other.CompareTag("Ground"))
            {
                if (hitSound != SoundManager.GameSFXType.None) SoundManager.Instance.PlayGameSound(hitSound, transform.position);
                Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        else{
            
        }
    }

    public void DestroyBullet(){
        if (hitSound != SoundManager.GameSFXType.None) SoundManager.Instance.PlayGameSound(hitSound, transform.position);
        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        enemyWeaponColList.Remove(enemyWeaponColList.Find(x => x.col == bulletCol));
    }

}
