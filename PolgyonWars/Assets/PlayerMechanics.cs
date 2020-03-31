using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMechanics : NetworkBehaviour
{


	//Enseñando a usar git
    [SyncVar]
    public float health = 100f;

    public int kills = 0;
    public int deaths = 0;
    public bool death = false;
    public Canvas[] playerHUD;
    public TMP_Text[] hudText;
    public TMP_Text ammoText;
    public TMP_Text killsText;
    public float damage = 50f;
    public float range = 100f;
    public Camera fpsCam;
    public bool multipleShots = false;
    private Animator[] animators;
    private Animator gunSlider;
    private AudioSource[] sources;
    private AudioSource gunSound;
    private AudioSource gunShell;
    public PlayerMechanics player;
    public int ammo { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        hudText = GetComponentsInChildren<TMP_Text>();
        if(hudText.Length > 0)
        {
            ammoText = hudText[1];
            killsText = hudText[0];
        }

        fpsCam = GetComponentInChildren<Camera>();
        animators = GetComponentsInChildren<Animator>();
        foreach (Animator animator in animators)
        {
            if (animator.name == "SM_Wep_PistolSwat_01")
                gunSlider = animator;
        }
        sources = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in sources)
        {
            if (source.clip.name == "pistolGunshot")
                gunSound = source;
        }
        playerHUD = GetComponentsInChildren<Canvas>();
        player = GetComponent<PlayerMechanics>();
        ammo = 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;
        if (hudText.Length > 0)
        {
            ammoText.text = $"Ammo:{ammo}";
            killsText.text = $"Kills:{kills}";
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (ammo > 0)
            {
                gunSlider.SetTrigger("Shoot");
                gunSound.PlayOneShot(gunSound.clip);
                Shoot();
                ammo -= 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ammo = 8;
        }
    }
	
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
            Die();
    }
    public void Die()
    {
        death = true;
        deaths++;

        Destroy(this.gameObject);
    }


    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            PlayerMechanics enemy = hit.transform.GetComponent<PlayerMechanics>();
            ShootableNPC npc = hit.transform.GetComponent<ShootableNPC>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                if (enemy.death)
                {
                    player.kills += 1;
                    Debug.Log("The player killed another player!");
                }
            }
            if (npc != null)
            {
                npc.TakeDamage(damage);
                if (npc.death)
                {
                    player.kills += 1;
                    Debug.Log("The player killed an NPC!");
                }
            }
        }

    }
}
