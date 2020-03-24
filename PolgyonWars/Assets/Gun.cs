using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Gun : MonoBehaviour
{

    public float damage = 50f;
    public float range = 100f;
    public Camera fpsCam;
    public bool multipleShots = false;
    private Animator gunSlider;
    private Animation gunShoot;
    private AudioSource gunSound;
    private AudioSource gunShell;
    private Canvas[] playerHUD;
    private PlayerMechanics player;
    public int ammo { get; set; }

    private void Start()
    {
        fpsCam = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        gunSlider = GameObject.Find("SM_Wep_PistolSwat_01").GetComponent<Animator>();
        gunShoot = GameObject.Find("SM_Wep_PistolSwat_01").GetComponent<Animation>();
        gunSound = GameObject.Find("SM_Wep_PistolSwat_01").GetComponent<AudioSource>();
        playerHUD = GetComponentsInChildren<Canvas>();
        player = GetComponent<PlayerMechanics>();
        ammo = 8;
    }
    void LateUpdate()
    {
    }

    private System.Collections.IEnumerable WaitShoot()
    {
        yield return new WaitForSeconds(gunSlider.GetCurrentAnimatorStateInfo(0).length*2 + gunSlider.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if(ammo > 0)
            {
                gunSlider.SetTrigger("Shoot");
                gunSound.PlayOneShot(gunSound.clip);
                Shoot();
                ammo -= 1;
            }
            
            
            //else
            //{
            //    if (gunSlider.GetCurrentAnimatorStateInfo(0).IsName("IdlePistol"))
            //    {
            //        gunSlider.SetTrigger("Shoot");
            //        gunSound.PlayOneShot(gunSound.clip);
            //        Shoot();
            //    }
            //}
            
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            ammo = 8;
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            PlayerMechanics enemy = hit.transform.GetComponent<PlayerMechanics>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
                if (enemy.death)
                {
                    player.kills += 1;
                    Debug.Log("");
                }
            }
        }

    }
}
