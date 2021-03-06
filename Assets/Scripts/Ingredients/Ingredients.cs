﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType : byte
{
    CreamCheese,
    Onion,
    GrilledOnion,
    Pepper,
    Pickle,
    Tomato,
    Ketchup,
    Mustard,
    PeanutButter,
    Jelly,
    Salt
}

public abstract class Interactable : MonoBehaviour
{
    public abstract event Action InteractedWith;
}


public class Ingredients : Interactable
{

    [SerializeField]
    [Tooltip("Name of the ingredient, used by Orders")]
    private string name;
    [SerializeField]
    private float distanceBetweenConveyers = 4;
    [SerializeField]
    private bool isSalt=false;
    [SerializeField]
    private float fallingSpeed;

    private GameObject storedPlayer;
    private float conveyerSpeed = 2;
    private float gravityScale;
    private Vector2 nextPosition;
    private bool isFalling = false;
    private bool canFall = false;
    private bool isMovingLeft;

    private Conveyor currentConveyor;

    private BoxCollider2D notTriggerCollider;

    public override event Action InteractedWith;

    public string GetName()
    {
        return name;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        BoxCollider2D[] colliders = this.GetComponents<BoxCollider2D>();
        foreach (BoxCollider2D collider in colliders)
        {
            if (collider.isTrigger == false)
            {
                notTriggerCollider = collider;
            }
        }

        gravityScale = fallingSpeed;
        isFalling = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentConveyor!=null)
        {
            isFalling = false;
            MoveOnConveyer();
        }
        //If object is moving below map, then remove it 
        else if (transform.position.y < -7)
        {
            AudioSingleton.PlaySFX(SoundEffect.GarbageBin);
            Destroy(this.gameObject);
        }
    }

    private void MoveOnConveyer()
    {
        if(isFalling==false&&currentConveyor)
        {
            //transform.Translate(new Vector2(1, 0) * Time.deltaTime * currentConveyor.speed);
            transform.position = new Vector2(transform.position.x + currentConveyor.speed * Time.deltaTime, transform.position.y);
            float x = transform.position.x;
            x.Wrap(currentConveyor.start, currentConveyor.end);
            if(x!=transform.position.x)
            {
                transform.position = new Vector2(x, transform.position.y);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerPathMover>())
        {
            storedPlayer = collision.gameObject.transform.GetChild(0).gameObject;
            canFall = true;
            if (isSalt==true)
            {
                collision.gameObject.GetComponent<PlayerAttack>().AddUse(this.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerPathMover>())
        {
            canFall = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isFalling = false;
        if(collision.gameObject.GetComponent<Conveyor>())
        {
            currentConveyor = collision.gameObject.GetComponent<Conveyor>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Conveyor>())
        {
            currentConveyor = null;
        }
    }

    public void Fall()
    {
        if (storedPlayer != null)
        {
            InteractedWith?.Invoke();

            storedPlayer.gameObject.GetComponent<Animator>().SetTrigger("Push");
        }
        StartCoroutine(ToggleCollider(notTriggerCollider));
        currentConveyor = null;
        isFalling = true;
        this.GetComponent<Rigidbody2D>().gravityScale=gravityScale;
        nextPosition = new Vector2(0, this.transform.position.y - distanceBetweenConveyers);

        AudioSingleton.PlaySFX(SoundEffect.DropIngredient);
    }

    public bool GetFalling()
    {
        return isFalling;
    }

    public bool GetIsSalt()
    {
        return isSalt;
    }

    public IEnumerator ToggleCollider(BoxCollider2D collider)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        //If player pushes on a ladder, will reset the trigger after a set time so that the animation doesn't play when exiting the ladder
        if(storedPlayer!=null)
        {
            storedPlayer.gameObject.GetComponent<Animator>().ResetTrigger("Push");
        }
        collider.enabled = true;
    }
}
