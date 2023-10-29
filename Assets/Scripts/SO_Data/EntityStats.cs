using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats_", menuName = "Entity/Stats")]
public class EntityStats : ScriptableObject
{
    public int health = 3;
    public float knockbackRate = 0.5f;
}
