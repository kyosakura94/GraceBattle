using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vigro_skill02 : BaseAttack {

    public Vigro_skill02()
    {
        attackName = "Light Arrow";
        attackDescription = "Attacks with an arrow that’s lightning fast. The damage increases according to Attack Speed";
        Attacktype = "skill02";
        attackDamage = 30f;
        attackCost = 10f;

    }
}
