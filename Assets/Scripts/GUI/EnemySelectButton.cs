using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour {

    public GameObject EnemyPrefab;
    private bool showSelector;

    public void SelectEnemy() {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Input2(EnemyPrefab);



    }
    public void HideSelector() {
            EnemyPrefab.transform.FindChild("Selector").gameObject.SetActive(false);

    }
    public void ShowSelector() {
            EnemyPrefab.transform.FindChild("Selector").gameObject.SetActive(true );

    }
}
