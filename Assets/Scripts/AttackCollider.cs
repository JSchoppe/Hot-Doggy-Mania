﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            if(collision.gameObject.GetComponent<Enemy>().GetCanDie()==true)
            {
                collision.gameObject.GetComponent<Enemy>().Down();
            }
        }
    }
}
