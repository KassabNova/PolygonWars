using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMechanics : NetworkBehaviour
{
    [SyncVar(hook = "ChangeHealth")]
    public int health = 100;

    [SyncVar(hook = "ChangeName")]
    public string name = "initialName";

    public int kills = 0;
    public int deaths = 0;
    public bool death = false;
    public TMP_Text[] hudText;
    public TMP_Text ammoText;
    public TMP_Text killsText;
    public TMP_Text healthText;
    public int damage = 25;
    public float range = 40f;
    public Camera fpsCam;
    public bool multipleShots = false;
    private Animator[] animators;
    private Animator gunSlider;
    private AudioSource[] sources;
    private AudioSource gunSound;
    public ParticleSystem dieFx;
    public int ammo { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        hudText = GetComponentsInChildren<TMP_Text>();
        if (hudText.Length > 0)
        {
            killsText = hudText[0];
            ammoText = hudText[1];
            healthText = hudText[2];

        }
        dieFx = GetComponentInChildren<ParticleSystem>();
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
            healthText.text = $"Health:{health}";

        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (ammo > 0)
            {
                gunSlider.SetTrigger("Shoot");
                gunSound.PlayOneShot(gunSound.clip);
                Shoot();
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ammo = 8;
        }
        if (death)
        {
            Respawn();
        }
    }
    private void Respawn()
    {

    }

    public void TakeDamage(int amount)
    {
        if (isServer)
        {
            health -= amount;
            if (health <= 0)
                Die();
        }
        else
            CmdTakeDamage(amount);
    }
    [Command]
    void CmdTakeDamage(int value)
    {
        Debug.LogError($"Player health changed on server {this.name}, {this.health}");

        TakeDamage(value);
    }
    [Command]
    void CmdDealDamage(int value, string playerName)
    {
        Debug.LogError($"Player {this.name}, wants to hit {playerName}");
        PlayerMechanics player = GameObject.Find(playerName).GetComponent<PlayerMechanics>();
        Debug.LogError(player);
        player.TakeDamage(value);
    }
    void ChangeHealth(int oldValue, int newValue)
    {
        Debug.LogError($"Player {this.name} health changed on Hook  [Previous HP: {this.health}  New HP: {newValue}]");
        this.health = newValue;
    }

    [Command]
    void CmdChangeName()
    {
        string newName;
        newName = this.netId.ToString();
        name = newName;
        gameObject.name = newName;
    }
    void ChangeName(string oldName, string newName)
    {
        if (isServer)
        {
            name = newName;
            gameObject.name = newName;
        }
        else
        {
            CmdChangeName();
        }
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdChangeName();
    }
    public void Die()
    {
        Debug.Log($"Player {this.name} has {deaths} deaths");
        dieFx.Play();
        death = true;
        health = 100;
        deaths++;
    }
    void Shoot()
    {
        ammo--;
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            PlayerMechanics player = hit.transform.GetComponent<PlayerMechanics>();
            ShootableNPC npc = hit.transform.GetComponent<ShootableNPC>();
            if (player != null)
            {
                CmdDealDamage(damage, player.name);
                if (player.death)
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
