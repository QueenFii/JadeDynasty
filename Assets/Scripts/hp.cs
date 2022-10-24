using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hp : MonoBehaviour
{

    public int maxHP;
    public int currentHP;

    public bool onfire;
    public float fireTimer;
    public Color colorChange;
    public bool earthed;
    public float earthTimer;
    public bool watered;
    public float waterTimer;
    public bool aired;
    public float airedTimer;

    public SpriteRenderer sprite;
    public AudioClip fireClip;

    public AudioSource mySource;

    public bool player;
    public void elementEffect()
    {
        if (earthed)
        {
            earthTimer -= Time.deltaTime;
        }
        else
        {
            //facePlayer();
        }
        if (earthTimer <= 0)
        {
            earthed = false;
        }
        if (watered)
        {
            waterTimer -= Time.deltaTime;
            //speed = speedOG / 2;
        }
        else
        {
            //speed = speedOG;
        }
        if (waterTimer <= 0)
        {
            watered = false;
        }
        if (aired)
        {
            //speed = -speedOG;
            airedTimer -= Time.deltaTime;
        }
        else
        {
          //  speed = speedOG;
        }
        if (airedTimer <= 0)
        {
            aired = false;
        }
    }
    public void colorNormalize()
    {
        if (!onfire)
        {
            if (colorChange.g < 240 && colorChange.b < 240)
            {
                colorChange.g += Time.deltaTime;
                colorChange.b += Time.deltaTime;
            }
            else if (colorChange.g >= 240 && colorChange.b >= 240)
            {

            }
        }
        if (!watered)
        {
            if (colorChange.g < 240 && colorChange.r < 240)
            {
                colorChange.g += Time.deltaTime;
                colorChange.r += Time.deltaTime;
            }
            else if (colorChange.g >= 240 && colorChange.r >= 240)
            {

            }
        }
        if (!earthed)
        {
            if (colorChange.b < 240 && colorChange.r < 240)
            {
                colorChange.b += Time.deltaTime;
                colorChange.r += Time.deltaTime;
            }
            else if (colorChange.b >= 240 && colorChange.r >= 240)
            {

            }
        }
        if (!aired)
        {
            if (colorChange.b < 240)
            {
                colorChange.b += Time.deltaTime;
            }
            else if (colorChange.b >= 240)
            {

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        colorChange = new Color(255, 255, 255);
        currentHP = maxHP;
        sprite = GetComponent<SpriteRenderer>();
        mySource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        elementEffect();
        if (!player)
        {
            sprite.color = colorChange;
            colorNormalize();
        }
        checkHealth();

    }
    public void takeDamage(int dmg)
    {
        currentHP -= dmg;
    }
    public void healDamage(int heal)
    {
        if ((currentHP + heal) > maxHP)
        {
            currentHP = maxHP;
        }
        else
        {
            currentHP += heal;
        }
        
    }


    public void fireTick()//fire dot method 
    { 

    }

    //ice freeze method

    //poison method

    //bleed method
    public void checkHealth()
    {
        if (onfire)
        {

            sprite.color = colorChange;
            currentHP -= 1;
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0)
            {
                onfire = false;
            }
        }
        if (currentHP <= 0 && !player)
        {
            print("die" + currentHP);
           // GameManager.GM.updateScore(GameManager.GM.maxScore / GameManager.GM.enemiesToReset.Count);
           // GameManager.GM.enemiesToReset.Remove(gameObject);
            Destroy(gameObject);
        }
    }
    public void fireSound()
    {
        mySource.clip = fireClip;
        mySource.Play();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<shot>() != null)
        {
            switch (collision.GetComponent<shot>().element)
            {
                case gunEnumScript.element.Nothing:
                    {
                        break;
                    }
                case gunEnumScript.element.Fire:
                    {
                        fireSound();
                        //print(fire);
                        colorChange.b = 0;
                        colorChange.g = 0;
                        fireTimer = 0.25f;
                        onfire = true;

                        break;
                    }
                case gunEnumScript.element.Water:
                    {
                        colorChange.r = 0;
                        colorChange.g = 0;
                        waterTimer = 0.25f;
                        watered = true;
                        break;
                    }
                case gunEnumScript.element.Earth:
                    {
                        colorChange.b = 0;
                        colorChange.r = 0;
                        earthTimer = 0.25f;
                        earthed = true;
                        break;
                    }
                case gunEnumScript.element.Air:
                    {
                        colorChange.b = 0;
                        airedTimer = 0.75f;
                        aired = true;
                        //print(hp);
                        //print(gun.gunScript.damage + "Damage");
                        //print(collision.GetComponent<shot>().lookDirection * knockback);
                        //rb.AddForce(collision.GetComponent<shot>().lookDirection.normalized * knockback);
                        //aired = false;
                        //rb.AddForce(-collision.GetComponent<shot>().lookDirection.normalized * knockback);
                        break;
                    }
            }
        }
        
    }
}
