using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;

    public BaseEnemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD

    }

    public TurnState currentState;

    //for the progessbar

    private float cur_cooldown = 0.0f;
    private float max_cooldown = 5.0f;
    //this gameobject

    private Vector3 startposition;

    public GameObject Selector;

    //time for action

    private bool actionStarted = false;

    public GameObject HerotoAttack;

    private float animSpeed = 5.0f;

    //alive
    private bool isAlive = true;


    void Start () {
        currentState = TurnState.PROCESSING;
        Selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startposition = transform.position;

    }

	void Update () {

        switch (currentState)
        {

            case (TurnState.PROCESSING):
                UpgradeProgessBar();

                break;

            case (TurnState.CHOOSEACTION):
                ChooseAction();

                currentState = TurnState.WAITING;

                break;

            case (TurnState.WAITING):

                //idle state

            break;
            case (TurnState.ACTION):

                StartCoroutine(TimeForAction());

                break;

            case (TurnState.DEAD):
                if (!isAlive)
                {
                    return;
                }
                else
                {
                    //change tag of enemy
                    this.gameObject.tag = "DeadEnemy";

                    //not atackable

                    BSM.EnemiesInBattle.Remove(this.gameObject);
                    Selector.SetActive(false);
                    if (BSM.EnemiesInBattle.Count > 0)
                    {

                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (BSM.PerformList[i].AttackerGameObjects == this.gameObject)
                            {
                                BSM.PerformList.Remove(BSM.PerformList[i]);
                            }
                            if (BSM.PerformList[i].AttackerTarget == this.gameObject)
                            {
                                BSM.PerformList[i].AttackerTarget = BSM.EnemiesInBattle[Random.Range(0, BSM.EnemiesInBattle.Count)];

                            }
                        }
                    }                    //change color or play dead enymation

                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);

                    isAlive = false;

                    BSM.EnemyButton();

                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;

                }
                break;
        }

    }

    void UpgradeProgessBar()
    {

        cur_cooldown = cur_cooldown + Time.deltaTime;

        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.CHOOSEACTION;
        }

    }

    void ChooseAction() {

        HandleTurn myAttack = new HandleTurn();

        myAttack.attacker = enemy.theName;

        myAttack.type = "Enemy";

        myAttack.AttackerGameObjects = this.gameObject;

        myAttack.AttackerTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];

        int num = Random.Range(0, enemy.attacks.Count);

        myAttack.ChoosenAttack = enemy.attacks[num];

        Debug.Log(this.gameObject.name + "has choosen" + myAttack.ChoosenAttack.attackName + "and do" + myAttack.ChoosenAttack.attackDamage);

        BSM.Collections(myAttack);

    }

    private IEnumerator TimeForAction() {

        if (actionStarted) {

            yield break;
        }

        actionStarted = true;

        //animate the enemy near the hero to attack

        Vector3 heroPosition = new Vector3(HerotoAttack.transform.position.x - 1.5f, HerotoAttack.transform.position.y, HerotoAttack.transform.position.z);

        while (MoveTowardsEnemy(heroPosition)) {
            yield return null;  
        }
        // wait abit
        yield return new WaitForSeconds(0.5f);

        DoDamage();


        // do damage

        // animate back to start position

        Vector3 firstposition = startposition;

        while (MoveTowardsStart(firstposition)) {
            yield return null;
        }

        //remover this performer form the list in BSM

        BSM.PerformList.RemoveAt(0);


        // reset BSM -> wait
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        //reset this enemy state

        actionStarted = false;
        cur_cooldown = 0.0f;
        currentState = TurnState.PROCESSING;


    }

    private bool MoveTowardsEnemy(Vector3 tagert)
    {
        return  tagert != (transform.position = Vector3.MoveTowards(transform.position, tagert, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 tagert)
    {
        return tagert != (transform.position = Vector3.MoveTowards(transform.position, tagert, animSpeed * Time.deltaTime));
    }

    void DoDamage() {
        float calc_damage = enemy.currATK + BSM.PerformList[0].ChoosenAttack.attackDamage;
        HerotoAttack.GetComponent<HeroStateMachine>().TakeDamage(calc_damage);
    }
    public void TakeDamage(float getDamageMount) {

        enemy.currHP -= getDamageMount;
        if (enemy.currHP <= 0)
        {
            enemy.currHP = 0;
            currentState = TurnState.DEAD;
            Debug.Log(enemy.theName + "is dead");
        }
    }

}
