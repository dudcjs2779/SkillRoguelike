using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PkoemonNPC : MonoBehaviour
{
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color chamanderColor = Color.red;
    [SerializeField] private Color bulbasaurColor = Color.green;
    [SerializeField] private Color squirtleColor = Color.blue;

    private SkinnedMeshRenderer meshRenderer;

    private void Start() {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void Update(){
        string pokemonName = ((Ink.Runtime.StringValue) DialogueManager.Instance.GetVariableState("pokemon_name")).value;

        switch (pokemonName)
        {
            case "":
                meshRenderer.material.SetColor("Color_c18aea2e3ad54319abb53f299507b005", defaultColor);
                break;
            case "Chamander":
                meshRenderer.material.SetColor("Color_c18aea2e3ad54319abb53f299507b005", chamanderColor);
                break;
            case "Bulbasaur":
                meshRenderer.material.SetColor("Color_c18aea2e3ad54319abb53f299507b005", bulbasaurColor);
                break;
            case "Squirtle":
                meshRenderer.material.SetColor("Color_c18aea2e3ad54319abb53f299507b005", squirtleColor);
                break;

            default:
                //Debug.LogWarning("Pokemon name not handled by witch statement: " + pokemonName);
                break;
        }
    }
}
