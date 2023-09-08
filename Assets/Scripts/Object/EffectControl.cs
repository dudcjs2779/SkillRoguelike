using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EffectControl : MonoBehaviour
{
    public bool isVfx;
    public bool onlyDeactivate;
    public bool isDuration;

    ParticleSystem ps;
    VisualEffect vfx;
    VisualEffect[] vfxs;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        vfx = GetComponent<VisualEffect>();
        vfxs = GetComponentsInChildren<VisualEffect>();
    }

    void OnEnable()
    {
        if (!isVfx)
            StartCoroutine(PsCheckIfAlive());
        else
        {
            if (!isDuration){
                if(vfxs.Length == 1){
                    StartCoroutine(VfxCheckIfAlive(vfxs[0]));
                }
                else{
                    StartCoroutine(VfxsCheckIfAlive());
                }
            }
        }
    }

    IEnumerator PsCheckIfAlive()
    {
        while (true && ps != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (!ps.IsAlive(true))
            {
                if (onlyDeactivate)
                {
                    this.gameObject.SetActive(false);
                }
                else
                    GameObject.Destroy(this.gameObject);

                break;
            }
        }
    }

    IEnumerator VfxCheckIfAlive(VisualEffect visualEffect)
    {
        while (true && visualEffect != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (visualEffect.aliveParticleCount <= 0)
            {
                if (onlyDeactivate)
                {
                    this.gameObject.SetActive(false);
                }
                else
                    GameObject.Destroy(this.gameObject);

                break;
            }
        }
    }

    IEnumerator VfxsCheckIfAlive()
    {
        int aliveCount = vfxs.Length;
        while (true && vfxs.Length > 0)
        {
            yield return new WaitForSeconds(0.5f);

            foreach (var vfx in vfxs)
            {
                if (vfx.aliveParticleCount <= 0)
                {
                    aliveCount--;
                    if (aliveCount == 0)
                    {
                        if (onlyDeactivate)
                        {
                            this.gameObject.SetActive(false);
                        }
                        else
                        {
                            GameObject.Destroy(this.gameObject);
                        }

                    }
                }
            }
        }
    }

    public IEnumerator VfxDuration(float _delay, float _duration)
    {
        yield return new WaitForSeconds(_delay);
        vfx.Play();

        yield return new WaitForSeconds(_duration);
        if (onlyDeactivate)
        {
            vfx.Stop();
            yield return new WaitUntil(() => vfx.aliveParticleCount <= 0);
            this.gameObject.SetActive(false);
        }
        else
        {
            vfx.Stop();
            yield return new WaitUntil(() => vfx.aliveParticleCount <= 0);
            GameObject.Destroy(this.gameObject);
        }

    }
}
