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

    [SyncVar(hook = "ChangeSpawn")]
    public bool death = false;

    public int kills = 0;
    public int deaths = 0;
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

    /*
     on death you're probably already sending a TargetRpc to the client of the deceased...
     include a line in there that sets client authority off for the NetTransform. 
     Then a few seconds later when you decide to respawn on the server, 
     you can just move it on the server and it should teleport correctly. 
     Then re-enable client auth on the resurrected client

        Alternative: actually re-instantiate the player object, call NetworkServer.ReplacePlayerForConnection, then destroy the corpse.
         */
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
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            if (ammo > 0)
            {
                gunSlider.SetTrigger("Shoot");
                gunSound.PlayOneShot(gunSound.clip);
                Shoot();
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Respawn();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ammo = 8;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    [Command]
    private void CmdRespawn()
    {
        this.death = false;
    }

    [ClientRpc]
    private void RpcRespawn()
    {

        //Respawn();
        Debug.LogError($"Target Respawn {this.hasAuthority}");
        NetworkTransform networkTransform = GetComponent<NetworkTransform>();
        networkTransform.clientAuthority = false;
        Debug.LogError($"Target Respawn {this.hasAuthority}");

        //GameObject SpawnsHolder = GameObject.Find("Spawns");
        //NetworkStartPosition[] Spawns = SpawnsHolder.GetComponentsInChildren<NetworkStartPosition>();
        //System.Random rand = new System.Random();
        ////this.transform = Spawns[rand.Next(0, 3)].gameObject.transform;
        //NetworkTransform networkTransform = GetComponent<NetworkTransform>();
        //networkTransform.transform.position = Spawns[rand.Next(0, 3)].gameObject.transform.position;
    }
    [TargetRpc]
    private void TargetRespawn(NetworkConnection conn)
    {

        //Respawn();

        Debug.LogError($"Target Respawn {this.hasAuthority}");
        NetworkTransform networkTransform = GetComponent<NetworkTransform>();
        NetworkIdentity identity = GetComponent<NetworkIdentity>();
        networkTransform.clientAuthority = false;
        
        Debug.LogError($"Target Respawn this: {this.hasAuthority}, trans {networkTransform.hasAuthority} , ident {identity.hasAuthority}");



    }

    private void Respawn()
    {
        Debug.LogError(hasAuthority);
        GameObject SpawnsHolder = GameObject.Find("Spawns");
        NetworkStartPosition[] Spawns = SpawnsHolder.GetComponentsInChildren<NetworkStartPosition>();
        System.Random rand = new System.Random();
        //this.transform = Spawns[rand.Next(0, 3)].gameObject.transform;
        NetworkTransform networkTransform = GetComponent<NetworkTransform>();
        Debug.LogError($"Client Respawn {this.hasAuthority}");
        Transform spawn = Spawns[rand.Next(0, 3)].gameObject.transform;
        while(networkTransform.transform.position == spawn.position)
        {
            spawn = Spawns[rand.Next(0, 3)].gameObject.transform;
        }
        networkTransform.transform.position = spawn.position;
        networkTransform.transform.rotation = spawn.rotation;
        this.transform.position = spawn.position;
        this.transform.rotation = spawn.rotation;
        //this.transform.position = spawn;
    }
    private IEnumerator RespawnTwo()
    {
        yield return new WaitForSeconds(3f);

        GameObject SpawnsHolder = GameObject.Find("Spawns");
        NetworkStartPosition[] Spawns = SpawnsHolder.GetComponentsInChildren<NetworkStartPosition>();
        System.Random rand = new System.Random();

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        //SetupPlayer();

        Debug.Log(transform.name + " respawned.");
    }
    public void TakeDamage(PlayerMechanics whoShooted,int amount)
    {
        if (isServer)
        {
            health -= amount;
            if (health <= 0)
            {
                health = 100;
                Debug.LogError($"Server before die {this.hasAuthority}");
                RpcDie();
                death = true;
                NetworkIdentity identity = GetComponent<NetworkIdentity>();
                //TargetRespawn(identity.connectionToClient);
                Debug.LogError($"Server after die {this.hasAuthority}");

            }
        }
    }

    [Command]
    void CmdDealDamage(int damage, string playerName)
    {
        Debug.LogError($"Player {this.name}, wants to hit {playerName}");
        PlayerMechanics player = GameObject.Find(playerName).GetComponent<PlayerMechanics>();
        Debug.LogError(player);
        player.TakeDamage(this,damage);
    }
    void ChangeHealth(int oldValue, int newValue)
    {
        Debug.LogError($"Player {this.name} health changed on Hook  [Previous HP: {this.health}  New HP: {newValue}]");
        this.health = newValue;
    }
    void ChangeSpawn(bool oldValue, bool newValue)
    {
        Debug.LogError($"Player {this.name} life changed on Hook  [Previous status: {this.death}  New status: {newValue}]");
        this.death = newValue;
    }

    [Command]
    void CmdChangeName()
    {
        string newName;
        newName = this.netId.ToString();
        name = newName;
        gameObject.name = newName;
    }
    [Command]
    void CmdShoot()
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
        Debug.LogError($"Player {this.name} has {deaths} deaths");
        dieFx.Play();
        health = 100;
        deaths++;
        if (isLocalPlayer)
        {
            Debug.LogError($"Player is respawning");

            Respawn();
        }
    }
    [ClientRpc]
    public void RpcDie()
    {
        Die();
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
                if (player.name == this.name)
                    return;
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
                    kills += 1;
                    Debug.Log("The player killed an NPC!");
                }
            }
        }
    }
}
