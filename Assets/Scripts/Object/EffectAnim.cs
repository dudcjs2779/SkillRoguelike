using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EffectAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator VfxLerpFloat(VisualEffect vfx, string paraName, float target, float speed)
    {
        float start = vfx.GetFloat(paraName);
        float lerp = 0;

        while (lerp <= 1)
        {
            vfx.SetFloat(paraName, Mathf.Lerp(start, target, lerp));
            lerp += Time.deltaTime * speed;
            yield return null;
        }
    }

    public IEnumerator MatLerpFloat(Material mat, string paraName, float target, float speed)
    {
        //Debug.Log("MatLerpStart");
        float start = mat.GetFloat(paraName);
        float lerp = 0;

        while (lerp <= 1)
        {
            mat.SetFloat(paraName, Mathf.Lerp(start, target, lerp));
            lerp += Time.deltaTime * speed;
            yield return null;
        }
    }
}
