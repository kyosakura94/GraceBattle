/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{

    public enum PerformAction
    {

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


    public enum HeroGui
    {
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

    public GameObject SkillPanel;

    //atack to hero
    public Transform actionSpacer;
    public Transform magicSpacer;
    public Transform skillSpacer;

    public GameObject actionButton;
    public GameObject magicButton;
    public GameObject skillButton;



    private List<GameObject> atkBtns = new List<GameObject>();

    //enemy button

    private List<GameObject> enemyBtns = new List<GameObject>();

    //SpawnPoint
    public List<Transform> spawnPoints = new List<Transform>();

    void Awake()
    {
        for (int i = 0; i < GameManager.instance.enemyAmount; i++)
        {
            GameObject NewEnemy = Instantiate(GameManager.instance.enemysToBattle[i], spawnPoints[i].position, spawnPoints[i].rotation) as GameObject;
            NewEnemy.name = NewEnemy.GetComponent<EnemyStateMachine>().enemy.theName + "_" + (i + 1);
            NewEnemy.GetComponent<EnemyStateMachine>().enemy.theName = NewEnemy.name;

            EnemiesInBattle.Add(NewEnemy);
        }
    }


    void Start()
    {

        battleStates = PerformAction.WAIT;

        //EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        HeroInput = HeroGui.ACTIVATE;

        AttackPanel.SetActive(false);

        EnemySelectPanel.SetActive(false);

        MagicPanel.SetActive(false);

        SkillPanel.SetActive(false);

        EnemyButton();
    }

    // Update is called once per frame
    void Update()
    {

        switch (battleStates)
        {

            case (PerformAction.WAIT):
                if (PerformList.Count > 0)
                {
                    battleStates = PerformAction.TAKEACTION;

                }
                break;
            case (PerformAction.TAKEACTION):

                Debug.Log("Test??");
                GameObject performer = GameObject.Find(PerformList[0].attacker);

                if (PerformList[0].type == "Enemy")
                {

                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();

                    for (int i = 0; i < HeroesInBattle.Count; i++)
                    {
                        if (PerformList[0].AttackerTarget == HeroesInBattle[i])
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
                    Debug.Log("Bug Here");
                    clearAttackPanel();
                    HeroInput = HeroGui.ACTIVATE;
                }

                break;

            case (PerformAction.WIN):

                Debug.Log("You win battle");

                if (EnemiesInBattle.Count > 0)
                {
                    for (int i = 0; i < HeroesInBattle.Count; i++)
                    {
                        HeroesInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
                    }
                }


                GameManager.instance.LoadSceneAfterBattle();
                GameManager.instance.gameStates = GameManager.GameStates.WORLD_STATE;
                GameManager.instance.enemysToBattle.Clear();
                break;

            case (PerformAction.LOSE):

                Debug.Log("You lose battle");

                for (int i = 0; i < HeroesInBattle.Count; i++)
                {
                    HeroesInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
                }

                GameManager.instance.LoadSceneAfterBattle();
                GameManager.instance.gameStates = GameManager.GameStates.WORLD_STATE;
                GameManager.instance.enemysToBattle.Clear();
                break;
        }

        switch (HeroInput)
        {

            case (HeroGui.ACTIVATE):

                if (HeroToMagage.Count > 0)
                {
                    HeroToMagage[0].transform.Find("Selector").gameObject.SetActive(true);

                    HeroChoice = new HandleTurn();

                    AttackPanel.SetActive(true);

                    CreateAttackButton();

                    HeroInput = HeroGui.WAITING;

                    Debug.Log("Bug Here???");
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

    public void Collections(HandleTurn input)
    {

        PerformList.Add(input);

    }
    //check xem trong battle có bao nhieu enemy, thi se tao ra bay nhieu button
    public void EnemyButton()
    {
        //restart enemy button
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

            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();

            buttonText.text = cur_enemy.enemy.theName;

            button.EnemyPrefab = enemy;

            newButton.transform.SetParent(Spacer, false);

            enemyBtns.Add(newButton);
        }
    }



    void HeroInputDone()
    {

        PerformList.Add(HeroChoice);

        clearAttackPanel();

        HeroToMagage[0].transform.Find("Selector").gameObject.SetActive(false);

        HeroToMagage.RemoveAt(0);

        HeroInput = HeroGui.ACTIVATE;
    }

    void clearAttackPanel()
    {

        EnemySelectPanel.SetActive(false);
        AttackPanel.SetActive(false);
        MagicPanel.SetActive(false);
        SkillPanel.SetActive(false);


        foreach (GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }

        atkBtns.Clear();
    }

    void CreateAttackButton()
    {

        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        Text AttackButtonText = AttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        AttackButtonText.text = "Attack";

        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        AttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(AttackButton);

        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        Text MagicAttackButtonText = MagicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        MagicAttackButtonText.text = "Magic";

        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        MagicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(MagicAttackButton);

        GameObject SkillAttackButton = Instantiate(actionButton) as GameObject;
        Text SkillAttackButtonText = SkillAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        SkillAttackButtonText.text = "Skill";

        SkillAttackButton.GetComponent<Button>().onClick.AddListener(() => Input6());
        SkillAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(SkillAttackButton);



        if (HeroToMagage[0].GetComponent<HeroStateMachine>().hero.magicAttacks.Count > 0)
        {
            foreach (BaseAttack magicAtk in HeroToMagage[0].GetComponent<HeroStateMachine>().hero.magicAttacks)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;

                Text MagicButtonText = MagicButton.transform.Find("Text").gameObject.GetComponent<Text>();

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

        if (HeroToMagage[0].GetComponent<HeroStateMachine>().hero.skillAttacks.Count > 0)
        {
            foreach (BaseAttack skillAtk in HeroToMagage[0].GetComponent<HeroStateMachine>().hero.skillAttacks)
            {
                GameObject SkillButton = Instantiate(skillButton) as GameObject;

                //Text skillButtonText = SkillButton.transform.Find("Text").gameObject.GetComponent<Text>();

                //Debug.Log("Test");

                //skillButtonText.text = skillAtk.attackName;


                SkillButton.GetComponent<Image>().sprite = skillAtk.spiteImageButton;

                SkillAttackButton SATB = SkillButton.GetComponent<SkillAttackButton>();

                SATB.skillAttackToPerform = skillAtk;

                SkillButton.transform.SetParent(skillSpacer, false);

                atkBtns.Add(SkillButton);

            }
        }
        else
        {

            SkillAttackButton.GetComponent<Button>().interactable = false;
        }

    }


    //atack button
    public void Input1()
    {

        HeroChoice.attacker = HeroToMagage[0].name;
        HeroChoice.AttackerGameObjects = HeroToMagage[0];
        HeroChoice.type = "Hero";

        HeroChoice.ChoosenAttack = HeroToMagage[0].GetComponent<HeroStateMachine>().hero.attacks[0];

        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);

    }

    //enemy selection
    public void Input2(GameObject choosenEnemy)
    {

        HeroChoice.AttackerTarget = choosenEnemy;

        HeroInput = HeroGui.DONE;
    }

    public void Input4(BaseAttack choosenMagic)
    {

        HeroChoice.attacker = HeroToMagage[0].name;
        HeroChoice.AttackerGameObjects = HeroToMagage[0];

        HeroChoice.type = "Hero";

        HeroChoice.ChoosenAttack = choosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);

    }

    public void Input5(BaseAttack choosenSkill)
    {

        HeroChoice.attacker = HeroToMagage[0].name;
        HeroChoice.AttackerGameObjects = HeroToMagage[0];

        HeroChoice.type = "Hero";

        HeroChoice.ChoosenAttack = choosenSkill;
        SkillPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }

    public void Input3()
    {

        AttackPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }
    public void Input6()
    {

        AttackPanel.SetActive(false);
        SkillPanel.SetActive(true);
    }

}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour
{

    public enum PerformAction
    {

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


    public enum HeroGui
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    public HeroGui HeroInput;
    public float delaytime;

    public List<GameObject> HeroToMagage = new List<GameObject>();

    private HandleTurn HeroChoice;

    public GameObject enemyButton;

    public Transform Spacer;


    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;

    public GameObject MagicPanel;
    public GameObject SkillPanel;
    public GameObject Gameoverpanel;

    //atack to hero

    public Transform actionSpacer;
    public Transform magicSpacer;
    public Transform skillSpacer;

    public GameObject actionButton;
    public GameObject magicButton;
    public GameObject skillButton;

    private List<GameObject> atkBtns = new List<GameObject>();

    //enemy button

    private List<GameObject> enemyBtns = new List<GameObject>();

    //SpawnPoint
    public List<Transform> spawnPoints = new List<Transform>();

    void Awake()
    {
        for (int i = 0; i < GameManager.instance.enemyAmount; i++)
        {
            GameObject NewEnemy = Instantiate(GameManager.instance.enemysToBattle[i], spawnPoints[i].position, spawnPoints[i].rotation) as GameObject;
            NewEnemy.name = NewEnemy.GetComponent<EnemyStateMachine>().enemy.theName + "_" + (i + 1);
            NewEnemy.GetComponent<EnemyStateMachine>().enemy.theName = NewEnemy.name;

            EnemiesInBattle.Add(NewEnemy);
        }
    }

    void Start()
    {
        battleStates = PerformAction.WAIT;

        //EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        HeroInput = HeroGui.ACTIVATE;

        AttackPanel.SetActive(false);

        EnemySelectPanel.SetActive(false);

        MagicPanel.SetActive(false);

        SkillPanel.SetActive(false);

        Gameoverpanel.SetActive(false);

        EnemyButton();


    }

    // Update is called once per frame
    void Update()
    {

        switch (battleStates)
        {

            case (PerformAction.WAIT):
                if (PerformList.Count > 0)
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
                        if (PerformList[0].AttackerTarget == HeroesInBattle[i])
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
                    if (HeroesInBattle.Count >=1)
                    {
                        clearAttackPanel();

                        HeroInput = HeroGui.ACTIVATE;
                        battleStates = PerformAction.WAIT;
                    }
                    else
                    {
                        battleStates = PerformAction.WAIT;
                    }
                }
                break;
            case (PerformAction.WIN):
                clearAttackPanel();
                Debug.Log("You win battle");
                for (int i = 0; i < HeroesInBattle.Count; i++)
                {
                    HeroesInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
                }

                Gameoverpanel.SetActive(true);

                Text ShowtoScreen = GameObject.Find("ScreenText").gameObject.GetComponent<Text>();
                ShowtoScreen.text = "You win the battle!!!";

                GameObject backbutton = GameObject.Find("Button");

                backbutton.GetComponent<Button>().onClick.AddListener(() => Input7());

                break;

            case (PerformAction.LOSE):
                clearAttackPanel();
                Debug.Log("You lose battle");

                Gameoverpanel.SetActive(true);

                Text ShowtoScreen1 = GameObject.Find("ScreenText").gameObject.GetComponent<Text>();

                ShowtoScreen1.text = "You lose battle!!!";

                GameObject backbutton1 = GameObject.Find("Button");

                backbutton1.GetComponent<Button>().onClick.AddListener(() => Input7());
                break;
        }

        switch (HeroInput)
        {
            case (HeroGui.ACTIVATE):
                if (HeroToMagage.Count > 0)
                {
                    HeroToMagage[0].transform.Find("Selector").gameObject.SetActive(true);

                    HeroChoice = new HandleTurn();

                    AttackPanel.SetActive(true);

                    CreateAttackButton();

                    HeroInput = HeroGui.WAITING;
                }
                if (Time.deltaTime > delaytime)
                {
                    battleStates = PerformAction.PERFORMACTION;
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

    public void Collections(HandleTurn input)
    {

        PerformList.Add(input);

    }

    public void EnemyButton()
    {

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

            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();

            buttonText.text = cur_enemy.enemy.theName;

            button.EnemyPrefab = enemy;

            newButton.transform.SetParent(Spacer, false);

            enemyBtns.Add(newButton);
        }
    }

    //atack button
    public void Input1()
    {

        HeroChoice.attacker = HeroToMagage[0].name;
        HeroChoice.AttackerGameObjects = HeroToMagage[0];
        HeroChoice.type = "Hero";

        HeroChoice.ChoosenAttack = HeroToMagage[0].GetComponent<HeroStateMachine>().hero.attacks[0];
        AttackPanel.SetActive(false);

        EnemySelectPanel.SetActive(true);


    }

    //enemy selection
    public void Input2(GameObject choosenEnemy)
    {

        HeroChoice.AttackerTarget = choosenEnemy;

        HeroInput = HeroGui.DONE;



    }

    void HeroInputDone()
    {

        PerformList.Add(HeroChoice);

        clearAttackPanel();

        HeroToMagage[0].transform.Find("Selector").gameObject.SetActive(false);

        HeroToMagage.RemoveAt(0);

        HeroInput = HeroGui.ACTIVATE;
    }

    void clearAttackPanel()
    {

        EnemySelectPanel.SetActive(false);
        AttackPanel.SetActive(false);

        MagicPanel.SetActive(false);

        SkillPanel.SetActive(false);

        foreach (GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();
    }

    void CreateAttackButton()
    {

        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        Text AttackButtonText = AttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        AttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(AttackButton);

        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        Text MagicAttackButtonText = MagicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        MagicAttackButtonText.text = "Magic";

        MagicAttackButton.GetComponent<Button>().onClick.AddListener(() => Input3());
        MagicAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(MagicAttackButton);

        GameObject SkillAttackButton = Instantiate(actionButton) as GameObject;
        Text SkillAttackButtonText = SkillAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        SkillAttackButtonText.text = "Skill";

        SkillAttackButton.GetComponent<Button>().onClick.AddListener(() => Input6());
        SkillAttackButton.transform.SetParent(actionSpacer, false);
        atkBtns.Add(SkillAttackButton);


        if (HeroToMagage[0].GetComponent<HeroStateMachine>().hero.magicAttacks.Count > 0)
        {
            foreach (BaseAttack magicAtk in HeroToMagage[0].GetComponent<HeroStateMachine>().hero.magicAttacks)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;

                Text MagicButtonText = MagicButton.transform.Find("Text").gameObject.GetComponent<Text>();

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

        if (HeroToMagage[0].GetComponent<HeroStateMachine>().hero.skillAttacks.Count > 0)
        {
            foreach (BaseAttack skillAtk in HeroToMagage[0].GetComponent<HeroStateMachine>().hero.skillAttacks)
            {
                GameObject SkillButton = Instantiate(skillButton) as GameObject;

                SkillButton.GetComponent<Image>().sprite = skillAtk.spiteImageButton;

                SkillAttackButton SATB = SkillButton.GetComponent<SkillAttackButton>();

                SATB.skillAttackToPerform = skillAtk;

                SkillButton.transform.SetParent(skillSpacer, false);

                atkBtns.Add(SkillButton);

            }
        }
        else
        {

            SkillAttackButton.GetComponent<Button>().interactable = false;
        }

    }


    public void Input4(BaseAttack choosenMagic)
    {
        HeroChoice.attacker = HeroToMagage[0].name;
        HeroChoice.AttackerGameObjects = HeroToMagage[0];

        HeroChoice.type = "Hero";

        HeroChoice.ChoosenAttack = choosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);

    }

    public void Input3()
    {

        AttackPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }
    public void Input6()
    {

        AttackPanel.SetActive(false);
        SkillPanel.SetActive(true);
    }

    public void Input7()
    {
        GameManager.instance.LoadSceneAfterBattle();
        GameManager.instance.gameStates = GameManager.GameStates.WORLD_STATE;
        GameManager.instance.enemysToBattle.Clear();
    }

    public void Input5(BaseAttack choosenSkill)
    {

        HeroChoice.attacker = HeroToMagage[0].name;
        HeroChoice.AttackerGameObjects = HeroToMagage[0];

        HeroChoice.type = "Hero";

        HeroChoice.ChoosenAttack = choosenSkill;
        SkillPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }
}