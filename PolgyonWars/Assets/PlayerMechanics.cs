using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMechanics : NetworkBehaviour
{


    //Enseñando a usar git
    [SyncVar(hook = "ChangeQWER")]
    public int health = 100;

    [SyncVar(hook = "ChangeName")]
    public string name = "ASDF";

    public int kills = 0;
    public int deaths = 0;
    public bool death = false;
    public Canvas[] playerHUD;
    public TMP_Text[] hudText;
    public TMP_Text ammoText;
    public TMP_Text killsText;
    public TMP_Text healthText;
    public int damage = 50;
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
            killsText = hudText[0];
            ammoText = hudText[1];
            healthText = hudText[2];

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
            healthText.text = $"Health:{health}";

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
            if(health < 0)
            {
                Die();
            }
        }
        else
        {
            Debug.LogError($"{this.name} entering CMD with {this.health} hp");
            CmdTakeDamage(amount);
        }

    }
    [Command]
    void CmdTakeDamage(int value)
    {
        Debug.LogError($"Player health changed on server {this.name}, {this.health}");

        TakeDamage(value);
    }
    [ClientRpc]
    void RpcTakeDamage(int value)
    {
        Debug.LogError($"Player health changed on RPC  Name: {this.name}, Health: {this.health}");

        this.health = value;
    }
    void ChangeQWER(int oldValue, int newValue)
    {
        Debug.LogError($"Player health changed on Hook OldVal: {oldValue}, New: {newValue}, Name: {this.name}, Health: {this.health}");

        this.health = newValue;
    }
    
    [Command]
    void CmdChangeName(string newName)
    {
        ChangeName("oldName",newName);
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
            CmdChangeName(newName);
        }
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        System.Random rand = new System.Random();
        //char asdf = rand.Next(4).ToString();
        gameObject.name = $"Player{rand.Next(4)} ";
        ChangeName("oldName",gameObject.name);
    }
    public void Die()
    {
        Debug.LogError($"Player {this.name} has {deaths} deaths");

        //death = true;
        health = 100;
        deaths++;

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
                Debug.LogError($"Player health changed on raycast hit Enemy:[ name: {enemy.name}, health: {enemy.health}] This:[name: {this.name}, health: {this.health}]");

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
