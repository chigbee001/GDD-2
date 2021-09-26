using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PumpkinPatch : MonoBehaviour
{
    //health variables
    private float health;
    private float maxHealth;
    private bool isAlive;

    //health bar
    private Image healthBar;
    private Image barBackground;
    private Text healthText;
    private float barFullSize;
    private float barHeight;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 10;
        health = 5;
        isAlive = true;

        //health bar setup
        Transform canvas = transform.GetChild(0);
        barBackground = canvas.GetChild(0).GetComponent<Image>();
        healthBar = canvas.GetChild(1).GetComponent<Image>();
        healthText = canvas.GetChild(2).GetComponent<Text>();
        barFullSize = barBackground.rectTransform.rect.width;
        barHeight = barBackground.rectTransform.rect.height;

        UpdateHealthBar(health, maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //handle decreasing health
    public void DecreaseHealth(float amount)
    {
        //update health, make health 1 decimal place since enemies will get stronger causing decimal attack values
        health -= amount;
        health = Mathf.Floor(health * 10) / 10;
        
        UpdateHealthBar(health, maxHealth);

        //check if patch was destroyed
        if (health <= 0)
        {
            isAlive = false;
        }
    }

    //update health bar
    private void UpdateHealthBar(float amount, float max)
    {
        //change bar
        healthBar.rectTransform.sizeDelta = new Vector2(amount / max * barFullSize, barHeight);

        //change text
        healthText.text = amount.ToString("F1");
    }

    //proprty for isAlive
    //to be used by the game manager to end the game
    //to be used by the spawn manager to stop spawning enemies that attack this patch
    public bool IsAlive
    {
        get { return isAlive; }
    }

    //check for enemy collision
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.transform.tag == "enemy") {
            //this is under the assumption that there will be some public function in Enemy that will return a float and destroy the enemy gameobject
            //DecreaseHealth(collision.gameObject.GetComponent<Enemy>().Attack());
        }
    }
}
