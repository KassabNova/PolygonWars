using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour
{
	//Enseñando a usar git
    public float health = 100f;
    public int kills = 0;
    public int deaths = 0;
    public bool death = false;
    public Canvas[] playerHUD;
    public TMP_Text[] hudText;
    public TMP_Text ammoText;
    public TMP_Text killsText;
    public Gun playerGun;
    // Start is called before the first frame update
    void Start()
    {
        hudText = GetComponentsInChildren<TMP_Text>();
        playerGun = GetComponent<Gun>();
        if(hudText.Length > 0)
        {
            ammoText = hudText[1];
            killsText = hudText[0];
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(hudText.Length > 0)
        {
            ammoText.text = $"Ammo:{playerGun.ammo}";
            killsText.text = $"Kills:{kills}";
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
        Destroy(this.gameObject);
    }
}
