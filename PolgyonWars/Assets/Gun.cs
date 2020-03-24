using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public float damage = 10f;
    public float range = 100f;
    public Camera fpsCam;
    private Animator gunSlider;
    private void Start()
    {
        fpsCam = GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        gunSlider = GameObject.Find("SM_Wep_PistolSwat_01").GetComponent<Animator>();
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

            gunSlider.SetBool("isShooting", true);

            Debug.Log(gunSlider.GetBool("isShooting"));

            Shoot();

            while (gunSlider.GetCurrentAnimatorStateInfo(0).IsName("Pistol") &&
                    gunSlider.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
                WaitShoot();
            gunSlider.SetBool("isShooting", false);

            Debug.Log(gunSlider.GetBool("isShooting"));
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            PlayerMechanics player = hit.transform.GetComponent<PlayerMechanics>();
            if(player != null)
            {
                player.TakeDamage(damage);
            }
        }

    }
}
