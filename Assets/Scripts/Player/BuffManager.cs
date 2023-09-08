using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public GameObject buffPrefab;
    [SerializeField] PlayerStateManager playerStateManager;

    public void CreateBuff(string buffName, string type, float du, GameObject[] effects, Sprite icon, float var = 0)
    {
        float duration = du * playerStateManager.stat_BuffDuration;

        GameObject buff = Instantiate(buffPrefab, transform);
        buff.GetComponent<Buff>().Init(buffName, type, duration, effects, var);
        buff.GetComponent<UnityEngine.UI.Image>().sprite = icon;
    }
}
