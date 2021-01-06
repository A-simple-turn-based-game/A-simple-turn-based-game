using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 释放方式
/// </summary>
public enum ReleaseType{

    Missile,
    UserBottom,
    UserFront,
    TargetBottom,
    TargetFront
};


public class EffectsController : RecoverableObject
{ 
    public ReleaseType releaseType = ReleaseType.Missile;
    
    // 二级粒子特效
    public GameObject secondParticle;
    public float moveSpeed = 10;
    
    private ParticleSystem particle;
    private Vector3 user;
    private Vector3 target;
    private void Awake()
    {
        particle = GetComponentInChildren<ParticleSystem>();
        //ParticleSystem[] pList = GetComponentsInChildren<ParticleSystem>();
        //float time = 0;
        // 确认最长粒子特效
        //foreach (ParticleSystem p in pList)
        //{
        //    if (p.main.duration + p.main.startDelay.constant > time) {
        //        time = p.main.duration + p.main.startDelay.constant;
        //        particle = p;
        //    }
        //} 
    }

    //private void OnEnable()
    //{
        
    //    particle.Play();
    //} 
    public void OnInit(ICharacter _user,ICharacter _target)
    {
        this.user = _user.GetControllerSystem().effectPos.position;
        this.target = _target.GetControllerSystem().effectPos.position;

        switch (releaseType)
        {
            case ReleaseType.Missile:
            case ReleaseType.UserFront:
                transform.position = user;
                break;
            case ReleaseType.UserBottom:
                transform.position = _user.gameObject.transform.position;
                break;
            case ReleaseType.TargetBottom:
                transform.position = _target.gameObject.transform.position;
                break;
            case ReleaseType.TargetFront:
                transform.position = target;
                break; 
            default:
                break;
        }

    }

    private void Update()
    {
        if (particle.isStopped) {
            OnRecycle();
        }
        if (releaseType == ReleaseType.Missile) {

            float distance = Vector3.Distance(transform.position, target);
            if (distance <= 0.001f)
            {
                if (secondParticle != null) { 
                    GameObject expl = Instantiate(secondParticle, transform.position, Quaternion.identity) as GameObject;
                    Destroy(expl, 3);  
                }
            }
            else
            {
                Vector3 dir = target - transform.position;
                Vector3 moveDir = dir.normalized * moveSpeed * Time.deltaTime; 
                if (distance <= moveDir.magnitude) { transform.position = target; }
                else transform.Translate(moveDir, Space.World);
            }
        }
    }

     
    public override void OnGenerate()
    {
        this.gameObject.SetActive(true);
        particle.Play();
    }

    public override void OnRecycle()
    {
        this.gameObject.SetActive(false);
    }
}
 
