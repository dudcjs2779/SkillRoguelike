using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickDamage : MonoBehaviour
{
    public float damage;
    public float stgDamage;
    public float duration;
    public float period;

    public bool isMagic;
    [SerializeField] bool OnlyDeactivate;

    public MeshCollider meshCol;

    private void OnEnable()
    {
        StartCoroutine(LifeTime());
    }

    IEnumerator LifeTime()
    {
        float time = 0;
        while (time < duration)
        {
            time = time + Time.deltaTime;

            yield return null;
        }

        if (OnlyDeactivate)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(false);
            Destroy(gameObject, period);
        }
    }
}
