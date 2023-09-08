using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }
    public KeyHint keyHint;
    public bool interactableObjectInRange;

    private void Awake() {
        if (Instance == null) Instance = this;
    }
    
}
