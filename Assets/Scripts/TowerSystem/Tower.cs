using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Towers/Default Tower")]
public class Tower : ScriptableObject
{
    public new string name;

    public float maxWeight;
    public float weight;
    public float tiltAngleThreshold;

    public int maxDamage;
    public int damage;

    public float maxRange;
    public float range;
    public float fireRate;  
    public int cost;
    
    public Material selectedMaterial;
    public Material defaultMaterial;
    
    public bool isUpgradable;
    public bool isStatic;

    public float yOffset;
    public float enemyYOffset;

    public GameObject[] upgradePrefabs;
}
