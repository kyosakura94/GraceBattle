using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy: BaseClass{

    public enum Type {

        FIRE,
        ICE, 
        EARTH,
        WATER,
        LIGHT,
        DARK
    }

    public enum Rarity {
        COMMON,
        UNCOMON,
        RARE,
        SUPERRARE
    }

    public Type EnenmyType;
    public Rarity rarity;

}
