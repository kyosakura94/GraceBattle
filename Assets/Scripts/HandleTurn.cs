using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class HandleTurn{

    public string attacker; // name of atacker
    public string type;
    public GameObject AttackerGameObjects;

    public GameObject AttackerTarget;

    public BaseAttack ChoosenAttack;
    // which attack is performed


}
