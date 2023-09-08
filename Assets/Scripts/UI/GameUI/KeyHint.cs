using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyHint : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keyText;
    [SerializeField] TextMeshProUGUI explainText;

    private void Awake() {

    }

    public void Activate(){
        gameObject.SetActive(true);
    }
    public void KeyHintUpdate(string key, string explain){
        keyText.text = key;
        explainText.text = explain;
    }

    public void Deactivate(){
        gameObject.SetActive(false);
    }

}
