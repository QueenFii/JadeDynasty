﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class enemy : MonoBehaviour
{
    public GameObject player;
    public SpriteRenderer sprite;
    public Rigidbody2D rb;

    public float speed;
    public float speedOG;
    //public int hp;
    //public int OGhp;

    public hp hp;


    public int[] coords;
    public bool visable;

    public AudioClip hitSound;
    
    public AudioSource mySource;

    public bool dead;

    public Collider2D col;

    public GenerationManager genManage;

    public bool elite;

    public bool stasisFrozen;

    public Animator anim;

    //GameManager.dropsEmum thingToDrop;

    // Start is called before the first frame update
    public virtual void Start()
    {
        elite = false;
        stasisFrozen = false;
        hp = GetComponent<hp>();
    mySource = GetComponent<AudioSource>();

        //knockback = 15000f;
        //OGhp = hp;
        speedOG = speed;
        coords = new int[2];
        player = GameObject.Find("player");
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        genManage = GameObject.FindWithTag("GameController").GetComponent<GenerationManager>();

        if(genManage.floor > -1)//0)
        {
            int randomGoal = Random.Range(0, 101);
            if(randomGoal - (40 + (5*genManage.floor)) < 0)//5*genManage.floor < 0)
            {
                elite = true;
            }
        }

        hp.maxHP += (10 * genManage.floor);
        hp.currentHP += (10 * genManage.floor);
    }

    // Update is called once per frame
    public virtual void Update()
    {  
        if(!stasisFrozen)
        {
            facePlayer();
        }
        
    }
    
    public void playHitSound()
    {
        mySource.clip = hitSound;
        mySource.Play();
    }
    public void facePlayer()
    {
        if (player.transform.position.x >= transform.position.x)
        {
            sprite.flipX = false;
        }
        else if (player.transform.position.x <= transform.position.x)
        {
            
            sprite.flipX = true;
        }
    }
    
    
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("shot"))
        {
            playHitSound();

            switch (collision.GetComponent<shot>().effect)
            {
                case gunEnumScript.effect.EXPLOSION:


                    break;

                default:
                    //hp.takeDamage(collision.GetComponent<shot>().damage);// -= collision.GetComponent<shot>().damage;
                    break;
            }
            

            
            
            //Destroy(collision.gameObject);
        }
    }
    public virtual void OnBecameVisible()
    {
        visable = true;
    }
    public virtual void OnBecameInvisible()
    {
        visable = false;
    }

    //IEnumerator Die()
    //{

    public abstract void die();
    public IEnumerator dieKnockback()
    {
        rb.angularDrag = 0.05f;
        rb.drag = 0f;

        rb.velocity = hp.knockbackDir * 4;
        
        yield return new WaitForSeconds(1.5f/3);
        //Debug.Break();
        rb.velocity = hp.knockbackDir * 2;
        yield return new WaitForSeconds(1.5f/3);
        //Debug.Break();
        rb.velocity = hp.knockbackDir * 1;
        yield return new WaitForSeconds(1.5f/3);
        //Debug.Break();


        rb.angularDrag = 200f;
        rb.drag = 200f;
        col.enabled = false;
        yield return null;
    }
    public void death() //anim
    {
        //drop stuff
        GameManager.GM.enemiesKilled++;

        int random = Random.Range(0, 101);

        if (random >= 50) // 50%
        {
            GameManager.dropsEmum thingToDrop = GameManager.RollDrops();
            if (thingToDrop == GameManager.dropsEmum.gun)
            {
                GameObject omega = Instantiate(GameManager.GM.randomGunToDrop, transform.position, Quaternion.identity);
                omega.GetComponent<randomGun>().theGun.floor = genManage.floor;
                GameManager.GM.rGunsToReset.Add(omega);
            }
            else if (thingToDrop == GameManager.dropsEmum.ammo)
            {
                GameObject delta = Instantiate(GameManager.GM.ammoToDrop, transform.position, Quaternion.identity);
                GameManager.GM.droppedAmmoToReset.Add(delta);
            }
            else if (thingToDrop == GameManager.dropsEmum.health)
            {
                GameObject bravo = Instantiate(GameManager.GM.healthToDrop, transform.position, Quaternion.identity);
                GameManager.GM.droppedAmmoToReset.Add(bravo);
            }
        }



        
    }


    public IEnumerator deathCoroutine(float colorDuration)//(float aTime, float dur)
    {
        float t = 0;    
        while (t < colorDuration)
        {
            t += Time.deltaTime;
            sprite.color = Color.Lerp(sprite.color, new Color(150/255f,150/255f,150/255f), t / colorDuration);
            yield return null;
        }
        yield return null;
    }




    //    yield return null;
    //}
    //public virtual void die2222()
    //{
    //    //GameManager.GM.enemiesToReset.Remove(gameObject);
    //    //Destroy(gameObject);
    //}
    //public abstract void Die();
}
