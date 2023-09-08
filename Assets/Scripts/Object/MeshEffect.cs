using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEffect : MonoBehaviour
{
    Animation anim;
    MeshFilter mf;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Material mat;
    public float meshDestroyDelay;


    private void Awake()
    {
        anim = GetComponent<Animation>();
        mf = GetComponent<MeshFilter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(mesh);

        mf.mesh = mesh;
        anim.Play("SkinEffect");

        Destroy(gameObject, meshDestroyDelay);
    }
}
