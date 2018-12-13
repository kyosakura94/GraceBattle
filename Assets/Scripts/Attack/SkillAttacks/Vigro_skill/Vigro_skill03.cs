using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vigro_skill03 : BaseAttack {

    public Vigro_skill03()
    {
        attackName = "Keenness";
        attackDescription = "Increases Critical Hit chances by 25%, and your Critical Damage increases accordingly to how much HP you’ve lost";
        Attacktype = "skill03";
        attackDamage = 30f;
        attackCost = 10f;

    }
}
