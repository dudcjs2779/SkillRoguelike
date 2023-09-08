using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canvas7 : MonoBehaviour
{
    public static Canvas7 Instance { get; private set; }

    private void Awake()
        {
            if (Instance != null)
            {
                Debug.Log("Found more than one Data Persistence Manager in the Scene, Destroying the newest one.");
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
}
