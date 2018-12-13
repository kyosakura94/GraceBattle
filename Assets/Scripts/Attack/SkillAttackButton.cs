using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackButton : MonoBehaviour {

    public BaseAttack skillAttackToPerform;

    public void CastSkillAttack()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input5(skillAttackToPerform);
    }
}
