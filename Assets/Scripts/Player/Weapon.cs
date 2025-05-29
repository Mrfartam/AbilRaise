using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float dirX, dirY, speed;
    public float offset;
    public Joystick joystick;
    public Animator animator;
    private float rotateWeapon;
    private int swordDirection;

    void Start()
    {
        offset = -45;
        swordDirection = 1;
    }

    void Update()
    {
        speed = 5 * (float)(Math.Sqrt(joystick.Horizontal * joystick.Horizontal + joystick.Vertical * joystick.Vertical));
        dirX = joystick.Horizontal * speed;
        dirY = joystick.Vertical * speed;
        if (dirX != 0 && dirY != 0)
        {
            rotateWeapon = Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotateWeapon + offset);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, rotateWeapon + offset);
        }
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") || animator.GetCurrentAnimatorStateInfo(0).IsName("InverseAttack")) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetTrigger("EndAttack");
        }
    }
    public void ClearColor(Color color)
    {
        SpriteRenderer bladeColor = transform.Find("Sword").Find("Blade").GetComponent<SpriteRenderer>();
        bladeColor.color = new Color(color.r, color.g, color.b, 1);
    }
    public void StopParticles()
    {
        ParticleSystem bladeParticle = transform.Find("Sword").Find("Blade").GetComponent<ParticleSystem>();
        bladeParticle.Stop();
    }
    public void PlayParticles()
    {
        ParticleSystem bladeParticle = transform.Find("Sword").Find("Blade").GetComponent<ParticleSystem>();
        bladeParticle.Play();
    }
    public void SetColorOfTrail(Color[] color)
    {
        TrailRenderer bladeTrail = transform.Find("Sword").Find("Trail").GetComponent<TrailRenderer>();
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[color.Length];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[color.Length];
        for(int i = 0; i < color.Length - 1; i++)
        {
            colorKey[i].color = color[i];
            colorKey[i].time = (float)i / color.Length;
            alphaKey[i].alpha = 1.0f;
            alphaKey[i].time = (float)i / color.Length;
        }
        colorKey[color.Length - 1].color = color[color.Length - 1];
        colorKey[color.Length - 1].time = 1;
        alphaKey[color.Length - 1].alpha = 1.0f;
        alphaKey[color.Length - 1].time = 1;
        gradient.SetKeys(colorKey, alphaKey);
        bladeTrail.colorGradient = gradient;
    }
    public void ClickButtonAttack()
    {
        animator.SetFloat("curRotation", rotateWeapon);

        /*Transform attack = transform.Find("Sword").Find("AttackPos");
        var script = attack.GetComponent<PlayerAttack>();
        script.enabled = true;*/

        if (swordDirection == -1)
        {
            animator.SetTrigger("InverseAttack");
            swordDirection = 1;
        }
        else
        {
            animator.SetTrigger("Attack");
            swordDirection = -1;
        }
    }
}
