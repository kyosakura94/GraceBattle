using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BaseAttack : MonoBehaviour {

    public string attackName;
    public string attackDescription;
    public float attackDamage;
    public float attackCost; //manacost
    public Sprite spiteImageButton;
    public string Attacktype;

}
