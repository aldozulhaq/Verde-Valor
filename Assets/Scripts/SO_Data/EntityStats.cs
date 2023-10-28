using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats_", menuName = "Entity/Stats")]
public class EntityStats : ScriptableObject
{
    public int health;
    public int currentHealth;
    public float knockbackRate;
}
