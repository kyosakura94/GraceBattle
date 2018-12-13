using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;

    public BaseHero hero;
    
    public enum TurnState
    {
         PROCESSING,
         ACTIONLIST,
         WAITING, 
         SELECTING,
         ACTION,
         DEAD

    }

    public TurnState currentState;

    //for the progessbar

    private float cur_cooldown = 0.0f;
    private float max_cooldown = 5.0f;
    public Image ProgessBar;
    public GameObject Selector;

    public GameObject EnemyToAttack;
    private bool actionStarted = false;

    private Vector3 startposition;
    private float animSpeed = 5.0f;
    //dead
    private bool stillAlive = true;
    //panel hero
    private HeroPanelStas stas;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;


    void Start () {

        HeroPanelSpacer = GameObject.Find("BattleCanvas").transform.FindChild("HeroPanel").transform.FindChild("HeroPanelSpacer");
        
        //create panel and fill in info
        CreatePanelHero();
        
        //cur_cooldown = Random.Range(0, 2.5f);
        startposition = transform.position;
        Selector.SetActive(false); 
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING;
	}
	
	void Update () {

        //Debug.Log(currentState);

        switch (currentState) {

            case (TurnState.PROCESSING):
                UpgradeProgessBar();

            break;

            case (TurnState.ACTIONLIST):

                BSM.HeroToMagage.Add(this.gameObject);
                currentState = TurnState.WAITING;

            break;

            case (TurnState.WAITING):
                //idle
            break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
            break;

            case (TurnState.DEAD):
                if (!stillAlive)
                {
                    return;
                }
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadHero";

                    //set hero not attackbale by enemy

                    BSM.HeroesInBattle.Remove(this.gameObject);

                    BSM.HeroToMagage.Remove(this.gameObject);

                    Selector.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    if (BSM.HeroesInBattle.Count > 0 )
                    {             
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (BSM.PerformList[i].AttackerGameObjects == this.gameObject)
                            {
                                BSM.PerformList.Remove(BSM.PerformList[i]);
                            }
                            if (BSM.PerformList[i].AttackerTarget == this.gameObject)
                            {
                                BSM.PerformList[i].AttackerTarget = BSM.HeroesInBattle[Random.Range(0, BSM.HeroesInBattle.Count)];
                            }
                        }

                    }
                    //change color or play dead animation

                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105,105,105,255);

                    BSM.HeroInput = BattleStateMachine.HeroGui.ACTIVATE;

                    BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;

                    stillAlive = false;

                }
            break;
        }

	}

    void UpgradeProgessBar() {

        cur_cooldown = cur_cooldown + Time.deltaTime;

        float calc_cooldown = cur_cooldown / max_cooldown;

        ProgessBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), ProgessBar.transform.localScale.y, ProgessBar.transform.localScale.z);

        if (cur_cooldown >= max_cooldown)
        {
            currentState = TurnState.ACTIONLIST;
        }

    }

    private IEnumerator TimeForAction()
    {

        if (actionStarted)
        {

            yield break;
        }

        actionStarted = true;

        //animate the enemy near the hero to attack

        Vector3 enenmyPositon = new Vector3(EnemyToAttack.transform.position.x + 1.5f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);

        while (MoveTowardsEnemy(enenmyPositon))
        {
            yield return null;
        }
        // wait abit
        yield return new WaitForSeconds(0.5f);


        // do damage
        DoDamage();
        // animate back to start position

        Vector3 firstposition = startposition;

        while (MoveTowardsStart(firstposition))
        {
            yield return null;
        }

        //remover this performer form the list in BSM

        BSM.PerformList.RemoveAt(0);

        if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE)
        {
        // reset BSM -> wait
        BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

        //reset this enemy state

        actionStarted = false;
        cur_cooldown = 0.0f;
        currentState = TurnState.PROCESSING;
        }
        else
        {
            currentState = TurnState.WAITING;
        }
        actionStarted = false;
    }

    private bool MoveTowardsEnemy(Vector3 tagert)
    {
        return tagert != (transform.position = Vector3.MoveTowards(transform.position, tagert, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 tagert)
    {
        return tagert != (transform.position = Vector3.MoveTowards(transform.position, tagert, animSpeed * Time.deltaTime));
    }
    public void TakeDamage( float getDamageAmount) {

        hero.currHP -= getDamageAmount;

        if (hero.currHP <= 0)
        {
            hero.currHP = 0;
            currentState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }

    void DoDamage() {
        float calc_damage = hero.currATK + BSM.PerformList[0].ChoosenAttack.attackDamage;
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }

    void CreatePanelHero() {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stas = HeroPanel.GetComponent<HeroPanelStas>();

        stas.HeroName.text = hero.theName;
        stas.HeroHP.text = "HP: " + hero.currHP;
        stas.HeroMP.text = "MP: " + hero.currMP;

        ProgessBar = stas.ProgessBar;

        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }
    void UpdateHeroPanel() {

        stas.HeroHP.text = "HP: " + hero.currHP;
        stas.HeroMP.text = "MP: " + hero.currMP;
    }
}
