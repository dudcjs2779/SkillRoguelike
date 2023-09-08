using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class PlayerBullet : MonoBehaviour
{
    public float debugFloat1;
    public float debugFloat2;
    public float debugFloat3;

    public enum BulletPattern { None, Fireball, DefanceWall, FireBurst, FireTornado, Icicle, IceSpear, IceStorm, LightingSpear, LightingSplit, LightingThunder, GravityHole, PowerShieldWave };
    public enum EffectType { None, FireTornado, IceStorm, GravityHole };

    [Header("BASIC PARA")]
    public ActiveSkill usingSkill;
    public string attackName;
    public float damage;
    public float stgDamage;
    public float stgPower;
    public float fireVal;
    public float iceVal;
    public float lightingVal;
    public float duration;
    public Collider bulletCol;
    public SoundManager.GameSFXType hitSound;

    [Header("GUDIANCE PARA")]
    public float detect_Distance;
    public float curveVal;
    public float moveSpeed;
    public float rotateSpeed;
    public int chainCount;
    public bool doChain;

    [Header("TICK DAMAGE PARA")]
    public float inDelay;
    public float outDelay;
    public float frequency;

    [Header("BULLET TYPE")]
    [SerializeField] BulletPattern bulletPattern;
    [SerializeField] EffectType effectType;
    public bool isNotBullet;
    public bool isPierceBullet;
    public bool isDurationBullet;
    public bool isChainBullet;

    [Header("EFFECT")]
    public GameObject hitEffect;
    VisualEffect vfx;

    LayerMask enemy_layerMask = 1 << 9;

    public Rigidbody rigid;
    public Transform t_Target;
    public List<Collider> bulletColGroup;
    public List<PlayerWeaponCol> playerWeaponColList;

    [SerializeField] EffectAnim effectAnim;

    public UnityAction<Vector3, float> bulletSoundAction;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        vfx = GetComponent<VisualEffect>();
    }

    void Start()
    {
        StartCoroutine(BulletEffectControl());
        BulletControl();
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // 일반탄이 아닌경우
        if (isNotBullet)
        {

            if (other.CompareTag("EnemyCollision"))
            {
                Instantiate(hitEffect, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

                if (hitSound != SoundManager.GameSFXType.None)
                {
                    //Debug.Log("BulletHitSound");
                    SoundManager.Instance.PlayGameSound(hitSound, transform.position);
                }
            }
        }
        // 일반탄 + 관통탄
        else
        {
            //관통탄
            if (isPierceBullet)
            {

                if (other.CompareTag("Wall") || other.CompareTag("Ground"))
                {
                    DestroyBullet(other.ClosestPointOnBounds(transform.position));

                }
                else if (other.CompareTag("EnemyCollision"))
                {
                    Instantiate(hitEffect, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                    if (hitSound != SoundManager.GameSFXType.None)
                    {
                        //Debug.Log("BulletHitSound");
                        SoundManager.Instance.PlayGameSound(hitSound, transform.position);
                    }
                }
            }
            //일반탄
            else
            {
                if (other.CompareTag("Wall") || other.CompareTag("Ground") || other.CompareTag("EnemyCollision"))
                {
                    DestroyBullet(other.ClosestPointOnBounds(transform.position));
                }
            }

        }

        if (isChainBullet && other.CompareTag("EnemyCollision") && chainCount > 0)
        {
            Collider[] nearEnemyCols = Physics.OverlapSphere(transform.position, detect_Distance, LayerMask.GetMask("EnemyCollision"));

            List<Collider> nearEnemyColsList = nearEnemyCols.ToList<Collider>();
            nearEnemyColsList.Remove(other);

            if (nearEnemyColsList.Count > 0)
            {
                int ran = Random.Range(0, nearEnemyColsList.Count);
                t_Target = nearEnemyColsList[ran].transform;
            }

            if (t_Target == null)
            {
                rigid.velocity = transform.forward * 35;
                Debug.Log("noChain");
            }
            else
            {
                Vector3 targetDir = t_Target.position - transform.position;
                targetDir.y = 0;
                rigid.velocity = targetDir.normalized * 35;
                rigid.rotation = Quaternion.LookRotation(targetDir);
                Debug.Log(t_Target.name);
            }
            chainCount--;
            //doChain = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    void BulletControl()
    {
        switch (bulletPattern)
        {
            case BulletPattern.None:
                //Debug.Log("None Gudiance");
                break;

            case BulletPattern.Fireball:
                hitSound = SoundManager.GameSFXType.FireHit01_1;
                StartCoroutine(Pattern_Fireball());
                break;

            case BulletPattern.DefanceWall:
                StartCoroutine(Pattern_DefanceWall());
                break;

            case BulletPattern.FireBurst:
                hitSound = SoundManager.GameSFXType.FireHit01_1;
                StartCoroutine(Pattern_FireBurst());
                break;

            case BulletPattern.FireTornado:
                hitSound = SoundManager.GameSFXType.FireHit01_1;
                StartCoroutine(Pattern_FireTornado());
                StartCoroutine(DurationDamage(4.5f));
                break;

            case BulletPattern.Icicle:
                hitSound = SoundManager.GameSFXType.IceHit01;
                StartCoroutine(Pattern_Icicle());
                break;

            case BulletPattern.IceSpear:
                hitSound = SoundManager.GameSFXType.IceHit01;
                StartCoroutine(Pattern_IceSpear());
                break;

            case BulletPattern.IceStorm:
                StartCoroutine(DurationDamage(8f));
                break;

            case BulletPattern.LightingSpear:
                hitSound = SoundManager.GameSFXType.LightingHit01_1;
                StartCoroutine(Pattern_LightingSpear());
                break;

            case BulletPattern.LightingSplit:
                hitSound = SoundManager.GameSFXType.LightingHit01_1;
                StartCoroutine(Pattern_LightingSplit());
                break;

            case BulletPattern.LightingThunder:
                hitSound = SoundManager.GameSFXType.LightingHit01_1;
                StartCoroutine(Pattern_LightingThunder());
                break;

            case BulletPattern.GravityHole:
                StartCoroutine(Pattern_GravityHole());
                break;

            case BulletPattern.PowerShieldWave:
                StartCoroutine(Pattern_PowerShieldWave());
                break;
        }
    }

    IEnumerator Pattern_Fireball()
    {
        Vector3 targetVec = Vector3.zero;
        float angle = 0;

        while (true)
        {
            Collider[] detectArea = Physics.OverlapSphere(transform.position, detect_Distance, enemy_layerMask);

            if (t_Target == null)
            {
                // 가장 가까운 적을 목표로 설정
                if (detectArea.Length > 0)
                {
                    float minDist = detect_Distance;
                    foreach (var col in detectArea)
                    {
                        float dist = Vector3.Distance(col.transform.position, transform.position);
                        angle = Vector3.Angle(transform.forward, col.transform.position - transform.position);

                        if (dist < minDist && angle < 90)
                        {
                            t_Target = col.transform;
                            minDist = dist;
                        }
                    }
                }
            }
            else
            {
                targetVec = t_Target.position - transform.position;
                targetVec.y = 0;
                angle = Vector3.Angle(transform.forward, targetVec);

                if (angle < 100f)
                {
                    rigid.velocity = transform.forward * 25f;
                    var targetRot = Quaternion.LookRotation(targetVec);
                    rigid.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRot, curveVal * Time.deltaTime));
                }
                else
                {
                    t_Target = null;
                }
            }
            yield return null;
        }
    }

    IEnumerator Pattern_DefanceWall()
    {
        Collider wallCol = transform.GetChild(0).GetComponent<Collider>();

        yield return new WaitForSeconds(0.1f);
        bulletCol.enabled = true;
        bulletSoundAction.Invoke(transform.position, 5f);

        yield return new WaitForSeconds(0.1f);
        bulletCol.enabled = false;
        wallCol.enabled = true;

        yield return new WaitForSeconds(13.8f);
        wallCol.enabled = false;
    }

    IEnumerator Pattern_FireBurst()
    {
        Vector3 shootDir = transform.forward + Vector3.up * 0.15f;
        rigid.AddForce(shootDir.normalized * 20, ForceMode.Impulse);
        bulletSoundAction.Invoke(transform.position, 4f);

        float t = 0;

        StartCoroutine(DetectNearEnemy());

        Vector3 velocity = Vector3.zero;
        while (true)
        {
            Collider[] detectArea = Physics.OverlapSphere(transform.position, 0.3f, 1 << 6 | 1 << 7 | 1 << 9);
            if (detectArea.Length > 0)
            {
                //Debug.Log("act");

                VisualEffect[] vfxs = GetComponentsInChildren<VisualEffect>(true);
                ParticleSystem p = GetComponentInChildren<ParticleSystem>();
                vfxs[1].gameObject.SetActive(true);
                CinemachineShake.Instance.ShakeCamera(8, 1, 0.3f);
                vfxs[0].Stop();
                p.gameObject.SetActive(false);

                rigid.isKinematic = true;
                bulletCol.enabled = true;
                SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.FireExplosion01, transform.position);
                bulletSoundAction.Invoke(transform.position, 6f);
                GetComponentInChildren<AudioSource>().Stop();
                yield return new WaitForSeconds(0.1f);
                bulletCol.enabled = false;
                Destroy(gameObject, 1f);
                yield break;
            }

            if (t_Target != null)
            {
                t += Time.deltaTime * 4f;
                float angle = 0;

                Vector3 targetDir = t_Target.position - transform.position;
                Vector3 nowDir = rigid.velocity;
                nowDir.y = 0;

                Vector3 targetDDir = Vector3.Lerp(nowDir, targetDir, t);
                Quaternion turnRot = Quaternion.LookRotation(targetDDir);

                angle = Vector3.SignedAngle(nowDir, targetDir, Vector3.up);
                angle = Mathf.Clamp(angle, -2.2f, 2.2f);
                Quaternion turnDir = Quaternion.AngleAxis(angle, Vector3.up);

                velocity = turnDir * rigid.velocity;
                rigid.velocity = velocity;

                //Debug.DrawRay(transform.position, velocity, Color.blue);
            }

            Quaternion rot = Quaternion.LookRotation(rigid.velocity);
            rigid.MoveRotation(Quaternion.Slerp(transform.rotation, rot, rotateSpeed * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Pattern_FireTornado()
    {
        Vector3 velocity = Vector3.zero;
        SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.FireBurning02, gameObject, 1f, true);
        bulletSoundAction.Invoke(transform.position, 6f);

        while (true)
        {
            if (t_Target == null)
            {
                Collider[] detectArea = Physics.OverlapSphere(transform.position, detect_Distance, enemy_layerMask);

                if (detectArea.Length > 0)
                {
                    float minDist = detect_Distance;
                    foreach (var col in detectArea)
                    {
                        float dist = Vector3.Distance(col.transform.position, transform.position);
                        if (dist < minDist)
                        {
                            t_Target = col.transform;
                            minDist = dist;
                        }
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                Vector3 targetDir = t_Target.position - transform.position;
                float angle;

                velocity += targetDir.normalized * moveSpeed * Time.deltaTime;

                angle = Vector3.SignedAngle(velocity, targetDir, Vector3.up);
                angle = Mathf.Clamp(angle, -1, 1);
                Quaternion turnDir = Quaternion.AngleAxis(angle, Vector3.up);

                velocity = Vector3.ClampMagnitude(turnDir * velocity, 2);

                rigid.velocity = velocity;
                yield return null;
            }
        }
    }

    IEnumerator Pattern_Icicle()
    {
        float waitTime = 1.5f;
        float duration = 6f;

        while (t_Target == null)
        {
            if (duration < 0)
            {
                DestroyBullet(transform.position);
                yield break;
            }

            Collider[] detectArea = Physics.OverlapSphere(transform.position, detect_Distance, enemy_layerMask);

            // 가장 가까운 적을 목표로 설정
            if (detectArea.Length > 0)
            {

                float minDist = detect_Distance;
                foreach (var col in detectArea)
                {
                    float dist = Vector3.Distance(col.transform.position, transform.position);
                    if (dist < minDist)
                    {
                        t_Target = col.transform;
                        minDist = dist;
                    }
                }
            }

            waitTime = waitTime - Time.deltaTime;
            duration -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);

        rigid.isKinematic = false;
        transform.parent = null;
        Vector3 enemyCenter = Vector3.zero;
        if (t_Target != null) enemyCenter = t_Target.GetComponent<CapsuleCollider>().center;
        Vector3 velocity = Vector3.zero;


        while (duration > 0 && t_Target != null)
        {
            Vector3 targetDir = t_Target.position - transform.position + enemyCenter;
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

        DestroyBullet(transform.position);
    }

    IEnumerator Pattern_IceSpear()
    {
        yield return new WaitForSeconds(0.2f);
        bulletColGroup[0].enabled = true;
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact02, bulletColGroup[0].transform.position);
        bulletSoundAction.Invoke(bulletColGroup[0].transform.position, 4f);

        yield return new WaitForSeconds(0.1f);
        bulletColGroup[0].enabled = false;
        bulletColGroup[1].enabled = true;
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact02, bulletColGroup[1].transform.position);
        bulletSoundAction.Invoke(bulletColGroup[1].transform.position, 4f);

        yield return new WaitForSeconds(0.1f);
        bulletColGroup[1].enabled = false;
        bulletColGroup[2].enabled = true;
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.IceImpact02, bulletColGroup[2].transform.position);
        bulletSoundAction.Invoke(bulletColGroup[2].transform.position, 4f);

        yield return new WaitForSeconds(0.1f);
        bulletColGroup[2].enabled = false;
    }

    IEnumerator Pattern_LightingSpear()
    {
        yield return new WaitForSeconds(0.1f);
        bulletCol.enabled = true;

        if (t_Target == null)
        {
            rigid.velocity = transform.forward * 35;
            Debug.Log("noChain");
        }
        else
        {
            Vector3 targetDir = t_Target.position - transform.position;
            targetDir.y = 0;
            rigid.velocity = targetDir.normalized * 35;
            rigid.rotation = Quaternion.LookRotation(targetDir);
            Debug.Log(t_Target.name);
        }

        for (int i = 0; i < chainCount; i++)
        {
            // if (i > 0)
            // {
            //     Collider[] nearEnemyCols = Physics.OverlapSphere(transform.position, detect_Distance, enemy_layerMask);

            //     List<Collider> nearEnemyColsList = nearEnemyCols.ToList<Collider>();
            //     if(t_Target != null) nearEnemyColsList.Remove(t_Target.GetComponent<Collider>());

            //     if(nearEnemyColsList.Count > 0)
            //     {
            //         int ran = Random.Range(0, nearEnemyColsList.Count);
            //         t_Target = nearEnemyColsList[ran].transform;
            //     }
            // }

            // if (t_Target == null)
            // {
            //     rigid.velocity = transform.forward * 35;
            //     Debug.Log("noChain");
            // }
            // else
            // {
            //     Vector3 targetDir = t_Target.position - transform.position;
            //     targetDir.y = 0;
            //     rigid.velocity = targetDir.normalized * 35;
            //     rigid.rotation = Quaternion.LookRotation(targetDir);
            //     Debug.Log(t_Target.name);
            // }

            // yield return new WaitUntil(() => doChain);
            // doChain = false;
        }

    }

    IEnumerator Pattern_LightingSplit()
    {
        float lerp = 0;
        float targetZ;
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(Pattern_LightingSplit_ExplosionSound(bulletColGroup[0].transform));

        foreach (Collider col in bulletColGroup)
        {
            col.enabled = true;
        }

        while (lerp < 1)
        {
            foreach (Collider col in bulletColGroup)
            {
                targetZ = col.transform.localPosition.z;
                targetZ = Mathf.Lerp(0.5f, 5, lerp);
                col.transform.localPosition = new Vector3(col.transform.localPosition.x, col.transform.localPosition.y, targetZ);

                //Debug.Log(col.name + ": " + bulletColGroup[0].transform.localPosition + " || " + lerp);
            }

            lerp += Time.deltaTime * 5f;
            yield return null;
        }


        foreach (Collider col in bulletColGroup)
        {
            col.enabled = false;
        }
    }

    IEnumerator Pattern_LightingSplit_ExplosionSound(Transform targetTf)
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.05f);
            SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingExplosion01, targetTf.position);
            bulletSoundAction.Invoke(targetTf.position, 3.5f);
        }
    }

    IEnumerator Pattern_LightingThunder()
    {
        SphereCollider col = bulletCol.GetComponent<SphereCollider>();
        yield return new WaitForSeconds(inDelay);
        duration = duration - inDelay - 0.1f;

        bulletSoundAction.Invoke(transform.position, 5f);

        while (0 < duration)
        {
            col.enabled = true;
            yield return new WaitForSeconds(frequency);
            col.enabled = false;
            yield return new WaitForSeconds(0.1f);
            duration = duration - frequency - 0.1f;
        }

        PlayerWeaponCol playerWeaponCol = playerWeaponColList.Find(x => x.colList.Contains(bulletCol));

        playerWeaponCol.damage = damage * 5;
        playerWeaponCol.stgDamage = stgDamage * 5;
        playerWeaponCol.stgPower = usingSkill.stgPower[0];
        playerWeaponCol.lightingVal = lightingVal * 4;

        col.radius = 4f;
        col.enabled = true;

        CinemachineShake.Instance.ShakeCamera(debugFloat1, debugFloat2, debugFloat3);
        SoundManager.Instance.PlayGameSound(SoundManager.GameSFXType.LightingExplosion02, transform.position);
        bulletSoundAction.Invoke(transform.position, 6f);

        yield return new WaitForSeconds(0.1f);
        col.enabled = false;
        col.radius = 1.6f;

    }

    IEnumerator Pattern_GravityHole()
    {
        if (usingSkill.skillLv == 2)
        {
            transform.localScale = Vector3.one * 1.4f;
            detect_Distance = detect_Distance * 1.4f;
            moveSpeed = moveSpeed * 1.3f;
            bulletSoundAction.Invoke(transform.position, 5.5f);
            StartCoroutine(DurationDamage(6f));
        }
        else
        {
            StartCoroutine(DurationDamage(4.5f));
        }
        SoundManager.Instance.AttachGameSound(SoundManager.GameSFXType.BlackHole01, gameObject);
        yield return new WaitForSeconds(inDelay);

        while (0 < duration)
        {
            Collider[] detectArea = Physics.OverlapSphere(transform.position, detect_Distance, enemy_layerMask);

            if (detectArea.Length > 0)
            {
                foreach (var col in detectArea)
                {
                    NavMeshAgent nav = col.GetComponentInParent<NavMeshAgent>();
                    Vector3 velDir = transform.position - col.transform.position;
                    float disByPower = 1 - (velDir.magnitude / detect_Distance);
                    disByPower = Mathf.Clamp(disByPower, 0.2f, 1);

                    float disByPowerRemap = GameManager.Instance.Remap(disByPower, 0.2f, 0.9f, 0, 90);
                    float sinePower = Mathf.Sin(disByPowerRemap * Mathf.Deg2Rad);
                    sinePower = Mathf.Clamp(sinePower, 0.2f, 0.9f);

                    nav.nextPosition = nav.nextPosition + velDir.normalized * moveSpeed * sinePower * Time.deltaTime;
                }
            }

            yield return null;
        }
    }

    IEnumerator Pattern_PowerShieldWave()
    {
        bulletCol.enabled = true;
        yield return new WaitForSeconds(0.066f);

        bulletCol.enabled = false;

    }

    IEnumerator DurationDamage(float soundRange)
    {
        if (!isDurationBullet)
            yield break;

        Collider col = GetComponentInChildren<Collider>();
        yield return new WaitForSeconds(inDelay);

        duration = duration - outDelay;

        while (0 < duration)
        {
            col.enabled = true;
            bulletSoundAction(transform.position, soundRange);
            yield return new WaitForSeconds(frequency);
            col.enabled = false;
            yield return new WaitForSeconds(0.1f);
            duration = duration - frequency - 0.1f;
        }

    }

    IEnumerator DetectNearEnemy()
    {
        while (t_Target == null)
        {
            Collider[] detectArea = Physics.OverlapSphere(transform.position, detect_Distance, enemy_layerMask);

            // 가장 가까운 적을 목표로 설정
            if (detectArea.Length > 0)
            {
                float minDist = detect_Distance;
                foreach (var col in detectArea)
                {
                    float dist = Vector3.Distance(col.transform.position, transform.position);
                    if (dist < minDist)
                    {
                        t_Target = col.transform;
                        minDist = dist;
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator BulletEffectControl()
    {
        switch (effectType)
        {
            case EffectType.None:
                //Debug.Log("None Effect");
                break;

            case EffectType.FireTornado:
                Debug.Log(EffectType.FireTornado);
                vfx.SetFloat("Duration", duration + 3);
                effectAnim.StartCoroutine(effectAnim.VfxLerpFloat(vfx, "Dissolve", 1, 1f));
                yield return new WaitForSeconds(duration);
                effectAnim.StartCoroutine(effectAnim.VfxLerpFloat(vfx, "Dissolve", 0, 1f));
                SoundManager.Instance.SoundVolumeLerp(GetComponentInChildren<AudioSource>(), 0f, 1f);
                break;

            case EffectType.IceStorm:
                Debug.Log(EffectType.IceStorm);
                vfx.SetFloat("Duration", duration + 3);
                effectAnim.StartCoroutine(effectAnim.VfxLerpFloat(vfx, "Dissolve", 1, 0.6f));
                yield return new WaitForSeconds(duration);
                effectAnim.StartCoroutine(effectAnim.VfxLerpFloat(vfx, "Dissolve", 0, 0.6f));
                SoundManager.Instance.SoundVolumeLerp(GetComponentInChildren<AudioSource>(), 0f, 1f);
                break;

            case EffectType.GravityHole:
                Debug.Log(EffectType.GravityHole);
                vfx.SetFloat("Duration", duration);
                yield return new WaitForSeconds(duration - 0.5f);
                effectAnim.StartCoroutine(effectAnim.VfxLerpFloat(vfx, "SphereSize", 0, 5f));
                effectAnim.StartCoroutine(effectAnim.VfxLerpFloat(vfx, "DistortionAlpha", 0, 5f));
                SoundManager.Instance.SoundVolumeLerp(GetComponentInChildren<AudioSource>(), 0f, 1f);
                break;
        }
    }

    public void DestroyBullet(Vector3 postion)
    {
        Instantiate(hitEffect, postion, Quaternion.identity);
        if (hitSound != SoundManager.GameSFXType.None)
        {
            //Debug.Log("BulletHitSound");
            SoundManager.Instance.PlayGameSound(hitSound, transform.position);
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (bulletColGroup.Count == 0)
        {
            playerWeaponColList.Remove(playerWeaponColList.Find(x => x.colList.Contains(bulletCol)));
        }
        else
        {
            Debug.Log("bulletColGroup");
            playerWeaponColList.Remove(playerWeaponColList.Find(x => x.colList.Contains(bulletColGroup[0])));
        }

        bulletColGroup.Clear();
        if (bulletCol != null) bulletCol.enabled = false;
    }
}
