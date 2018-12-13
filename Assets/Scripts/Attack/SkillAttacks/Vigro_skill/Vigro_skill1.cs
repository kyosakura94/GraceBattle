using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vigro_skill1 : BaseAttack {

    public Vigro_skill1()
    {
        attackName = "Magic Arrow";
        attackDescription = "Attacks the enemy with a magical arrow which casts various harmful effects according to the attribute relation between you and the enemy.";
        Attacktype = "skill01";
        attackDamage = 30f;
        attackCost = 10f;

    }
}
