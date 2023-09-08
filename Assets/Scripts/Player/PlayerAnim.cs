using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnim : MonoBehaviour
{
    Animator anim;
    Player player;
    Weapon weapon;
    public float basicAnimMoveSpeed;
    public float animMoveSpeed;
    public float amountOfPower;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
        weapon = GetComponentInChildren<Weapon>();
        basicAnimMoveSpeed = 1.1f;
        animMoveSpeed = basicAnimMoveSpeed;
    }

    private void Update()
    {
        
    }

    public void PlayAnimation(string targetAnim, bool isRootMotion, float tDuration, int layerIndex = 0, float normalizedTime = 0)
    {
        anim.applyRootMotion = isRootMotion;
        anim.SetBool("isRootMotion", isRootMotion);
        anim.CrossFade(targetAnim, tDuration, layerIndex, normalizedTime);
        //Debug.Log("PlayAnimation: " + Time.frameCount);

    }

    private void OnAnimatorMove()
    {
        if (player.isRootMotion == false)
        {
            //if(amountOfPower != 0) Debug.Log(amountOfPower);
            //amountOfPower = 0;
            return;
        }

        float delta = Time.deltaTime;
        //player.rigid.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity;
        velocity = deltaPosition / delta * animMoveSpeed;
        //Debug.Log(velocity);

        Vector3 slideVec = player.SlideWallVec(player.wallNormal, velocity);

        if (slideVec != Vector3.zero)
        {
            //Debug.Log("SlideWallVec");
            velocity = slideVec;
        }

        if (!velocity.IsNaN()) player.rigid.velocity = velocity;
        //player.rigid.position += velocity * Time.deltaTime;
        //Debug.Log(velocity.magnitude);
    }

    public IEnumerator SetAnimMoveSpeed(float speed, float dur)
    {
        animMoveSpeed = animMoveSpeed * speed;
        yield return new WaitForSeconds(dur);
        animMoveSpeed = basicAnimMoveSpeed;

    }
}
