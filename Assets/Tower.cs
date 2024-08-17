using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Towers/Default Tower")]
public class Tower : ScriptableObject
{
    public String name;
    public  int damage;
    public float range;
    public Material selectedMaterial;
    public Material defaultMaterial;
    public GameObject prefab;
    
}
