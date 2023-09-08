using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerEffect : MonoBehaviour
{
    Weapon weapon;
    public GameObject skinEffect;

    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.CompareTag("EnemyWeapon") || other.CompareTag("EnemyBullet"))
    //     {
    //         Buff buff;
    //         if (buff = GameManager.Instance.Buff_Playing.Find(x => x.buffName == "Guarding Barrier"))
    //         {
    //             buff.donPlayEffect = true;
    //             Instantiate(buff.buffEffects[2], buff.buffEffects[0].transform.position,Quaternion.identity);
    //             buff.DeActivation();
    //         }
    //     }
    // }

    public IEnumerator MatColorLerp(Material mat, string paraName, Color target, float speed, bool isRetun)
    {
        Debug.Log("MatColorLerp");
        Color start = mat.GetColor(paraName);
        float lerp = 0;

        while (lerp <= 1)
        {
            mat.SetColor(paraName, Color.Lerp(start, target, lerp));
            lerp += Time.deltaTime * speed;
            yield return null;
        }

        if (isRetun)
        {
            lerp = 0;
            while (lerp <= 1)
            {
                mat.SetColor(paraName, Color.Lerp(target, start, lerp));
                lerp += Time.deltaTime * speed;
                yield return null;
            }
        }
    }

}
