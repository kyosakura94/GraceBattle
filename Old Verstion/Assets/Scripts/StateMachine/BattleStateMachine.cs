using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour {

    public enum PerformAction {

        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public PerformAction battleStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();

    public List<GameObject> HeroesInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();


    public enum HeroGui {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    public HeroGui HeroInput;

    public List<GameObject> HeroToMagage = new List<GameObject>();

    private HandleTurn HeroChoice;

    public GameObject enemyButton;

    public Transform Spacer;


    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;

    public GameObject MagicPanel;

    //atack to hero
    public Transform actionSpacer;
    public Transform magicSpacer;

    public GameObject actionButton;
    public GameObject magicButton;

    private List<GameObject> atkBtns = new List<GameObject>();

    //enemy button

    private List<GameObject> enemyBtns = new List<GameObject>();

    void Start () {
        battleStates = PerformAction.WAIT;

        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
   
        HeroInput = HeroGui.ACTIVATE;

        AttackPanel.SetActive(false);

        EnemySelectPanel.SetActive(false);

        MagicPanel.SetActive(false);

        EnemyButton();


    }
	
	// Update is called once per frame
	void Update () {

        switch (battleStates) {

            case (PerformAction.WAIT):
                if (PerformList.Count > 0 )
                {
                    battleStates = PerformAction.TAKEACTION;

                }
                break;
            case (PerformAction.TAKEACTION):

                GameObject performer = GameObject.Find(PerformList[0].attacker);

                if (PerformList[0].type == "Enemy")
                {
                    
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    for (int i = 0; i < HeroesInBattle.Count; i++)
                    {
                        if (PerformList[0].AttackerTarget ==  HeroesInBattle[i])
                        {
                            ESM.HerotoAttack = PerformList[0].AttackerTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            PerformList[0].AttackerTarget = HeroesInBattle[Random.Range(0, HeroesInBattle.Count)];
                            ESM.HerotoAttack = PerformList[0].AttackerTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                            break;
                        }
                    }
                    
                }

                if (PerformList[0].type == "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.EnemyToAttack = PerformList[0].AttackerTarget;
                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                    Debug.Log("Hero is go to choose");
                }
                battleStates = PerformAction.PERFORMACTION;
                
                break;
            case (PerformAction.PERFORMACTION):
                //idle
            break;

            case (PerformAction.CHECKALIVE):
                if (HeroesInBattle.Count < 1)
                {
                    battleStates = PerformAction.LOSE;
                    //lose game
                }
                else if (EnemiesInBattle.Count < 1)
                {
                    battleStates = PerformAction.WIN;
                    //win game
                }
                else
                {
                    clearAttackPanel();

                    HeroInput = HeroGui.ACTIVATE;

                }
            break;
            case (PerformAction.WIN):

                Debug.Log("You win battle");
                for (int i = 0; i < HeroesInBattle.Count; i++)
                {
                    HeroesInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
                }
            break;

            case (PerformAction.LOSE):

                Debug.Log("You lose battle");
            break;
        }

        switch (HeroInput) {
            case (HeroGui.ACTIVATE):
                if (HeroToMagage.Count > 0 )
                {
                    HeroToMagage[0].transform.FindChild("Selector").gameObject.SetActive(true);

                    HeroChoice = new HandleTurn();

                    AttackPanel.SetActive(true);

                    CreateAttackButton();

                    HeroInput = HeroGui.WAITING;
                }
            break;

            case (HeroGui.WAITING):
                //idle

            break;

            case (HeroGui.DONE):
                HeroInputDone();
            break;
        }
    }

    public void Collections(HandleTurn input) {

        PerformList.Add(input);

    }

    public void EnemyButton() {

        //clean up
        foreach (GameObject enemyBtn in enemyBtns)
        {
            Destroy(enemyBtn);
        }
        enemyBtns.Clear();

        foreach (GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;

            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.transform.FindChild("Text").gameObject.GetComponent<Text>();

            buttonText.text = cur_enemy.enemy.theName;

            button.EnemyPrefab = enemy;

            newButton.transform.SetParent(Spacer,false);

            enemyBtns.Add(newButton);
        }
    }

    //atack button
    public void Input1() {

        HeroChoice.attacker = HeroToMagage[0].name;
        HeroChoice.AttackerGameObjects = HeroToMagage[0];
        HeroChoice.type = "Hero";

        HeroChoice.ChoosenAttack = HeroToMagage[0].GetComponent<HeroStateMachine>().hero.attacks[0];
        AttackPanel.SetActive(false);

        EnemySelectPanel.SetActive(true);


    }

    //enemy selection
    public void Input2(GameObject choosenEnemy) {

        HeroChoice.AttackerTarget = choosenEnemy;

        HeroInput = HeroGui.DONE;



    }

    void HeroInputDone() {

        PerformList.Add(HeroChoice);

        clearAttackPanel();

        HeroToMagage[0].transform.FindChild("Selector").gameObject.SetActive(false);

        HeroToMagage.RemoveAt(0);

        HeroInput = HeroGui.ACTIVATE;
    }

    void clearAttackPanel() {

        EnemySelectPanel.SetActive(false);
        AttackPanel.SetActive(false);

        MagicPanel.SetActive(false);

        foreach (GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();
    }

    void CreateAttackButton() {

        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        Text AttackButtonText = AttackButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() =>Input1());
        AttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(AttackButton);

        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        Text MagicAttackButtonText = MagicAttackButton.transform.FindChild("Text").gameObject.GetComponent<Text>();
        MagicAttackButtonText.text = "Magic";
        
        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() =>Input3());
        MagicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(MagicAttackButton);

        if (HeroToMagage[0].GetComponent<HeroStateMachine>().hero.magicAttacks.Count > 0)
        {
            foreach (BaseAttack magicAtk in HeroToMagage[0].GetComponent<HeroStateMachine>().hero.magicAttacks)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;

                Text MagicButtonText = MagicButton.transform.FindChild("Text").gameObject.GetComponent<Text>();

                MagicButtonText.text = magicAtk.attackName;

                AttackButton ATB = MagicButton.GetComponent<AttackButton>();

                ATB.magicAttackToPerform = magicAtk;

                MagicButton.transform.SetParent(magicSpacer, false);
                atkBtns.Add(MagicButton);


            }
        }
        else
        {
            MagicAttackButton.GetComponent<Button>().interactable = false;
        }

    }


    public void Input4(BaseAttack choosenMagic) {
        HeroChoice.attacker = HeroToMagage[0].name;
        HeroChoice.AttackerGameObjects = HeroToMagage[0];

        HeroChoice.type = "Hero";

        HeroChoice.ChoosenAttack = choosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true); 

    }

    public void Input3() {

        AttackPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }

}
