using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static void TakeDamage(EntityStats entityStats, int damage, Vector3 knockbackDirection)
    {
        entityStats.health -= damage;

        if(entityStats.health <= 0)
        {
            entityStats.health = 0;
            Debug.Log(entityStats.name + " Dead");
        }
        else
        {
            //apply knockback through transform translate
            entityStats.GetComponent<Transform>().Translate(knockbackDirection * entityStats.knockbackRate);

            //apply red flash
            Color color = entityStats.GetComponent<SpriteRenderer>().color;
            entityStats.GetComponent<SpriteRenderer>().color = Color.red;
            //play hit animation
            entityStats.GetComponent<Animator>().SetTrigger("Hit");
            //play hit sound
            entityStats.GetComponent<SpriteRenderer>().color = color;
            

        }
    }

}
