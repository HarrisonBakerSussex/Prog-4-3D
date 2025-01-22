using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wingsuit
{
    public string name = "undefined";
    public bool purchased = false;
    public float maxThrust = 3000f;
    public int cost = 0;
    public Animator wingsuitPrefab;

}
