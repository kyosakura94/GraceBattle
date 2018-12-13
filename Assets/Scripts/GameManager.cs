using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    //class random monster
    [System.Serializable]
    public class RegionData {
        public string regionName;
        public int maxAmountEnemys = 4;
        public string BattleScene;
        public List<GameObject> possiblEnemys = new List<GameObject>();

    }

    public int curRegions;
    public string nextSpawnPoint;

    //region
    public List<RegionData> Regions = new List<RegionData>();
    public GameObject heroCharacter;

    //Hero
    public Vector3 nextHeroPosition;
    public Vector3 lastHeroPosition;


    //scenes

    public string sceneToLoad;
    public string lastScene;

    //BOOLS

    public bool isWalking = false; 
    public bool canGetEncouter = false;
    public bool gotAttacked = false;

    //ENUM
    public enum GameStates {

        WORLD_STATE,
        TOWN_STATE,
        BATTLE_STATE,
        IDLE
    }
    //Battle
    public int enemyAmount;
    public List<GameObject> enemysToBattle = new List<GameObject>();
    

    public GameStates gameStates;

    void Update()
    {
        switch (gameStates) {

            case (GameStates.WORLD_STATE):
                if (isWalking)
                {
                    RandomEncounter();

                }
                if (gotAttacked) {
                    gameStates = GameStates.BATTLE_STATE;
                }
                break;
            case (GameStates.TOWN_STATE):

                break;

            case (GameStates.BATTLE_STATE):
                //Load battle sence
                StartBattle();
                //go to idle
                gameStates = GameStates.IDLE;   
                break;
            case (GameStates.IDLE):

                break;

        }
    }
    // Use this for initialization
    void Awake () {
        //check if instance exist
        if (instance == null)
        {
            //if not set the instance to this
            instance = this;
        }
        //if it exist but not this instance
        else if (instance != this)
        {
            //destroy it
            Destroy(gameObject);
        }
        //set this to be not destroyable

        DontDestroyOnLoad(gameObject);

        if (!GameObject.Find("HeroCharacter"))
        {
            GameObject Hero = Instantiate(heroCharacter, nextHeroPosition, Quaternion.identity) as GameObject;

            Hero.name = "HeroCharacter";
    
        }
        //else
        //{
        //    heroCharacter.transform.position = lastHeroPosition;
        //}
                  
	}

    public void LoadNextScene() {
        SceneManager.LoadScene(sceneToLoad);

    }
    public void LoadSceneAfterBattle() {

        SceneManager.LoadScene(lastScene);
    }
    void RandomEncounter() {
        if (isWalking && canGetEncouter)
        {
            if (Random.Range(0,1000) < 10)
            {
                Debug.Log("I got attacked");
                gotAttacked = true;
            }
        }
    }

    void StartBattle() {
        //Amount of enemy 
        enemyAmount = Random.Range(1, Regions[curRegions].maxAmountEnemys + 1);

        for (int i = 0; i < enemyAmount; i++)
        {
            enemysToBattle.Add(Regions[curRegions].possiblEnemys[Random.Range(0, Regions[curRegions].possiblEnemys.Count)]);
        }
        //Hero

        lastHeroPosition = GameObject.Find("HeroCharacter").gameObject.transform.position;

        nextHeroPosition = lastHeroPosition;

        lastScene = SceneManager.GetActiveScene().name;

        //Load level
        SceneManager.LoadScene(Regions[curRegions].BattleScene);
        //reset hero

        isWalking = false;
        gotAttacked = false;
        canGetEncouter = false;
    }
}
