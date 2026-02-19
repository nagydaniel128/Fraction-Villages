using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager Instance;
    public static GameManager instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject go = new GameObject();
                go.AddComponent<GameManager>();
            }
            return Instance;
        }
    }

    private void Awake()
    {
        Instance = this;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    public List<Elf> elves = new List<Elf>();
    public List<Human> humans = new List<Human>();
    public List<Goblin> goblins = new List<Goblin>();
    public List<Orc> orcs = new List<Orc>();


    public List<Tree> trees = new List<Tree>();
    public List<IronGroup> ironGroups = new List<IronGroup>();

    public List<Emergency> emergencies = new List<Emergency>();

    public GameObject emergency;

    //buildings
    [Header("Buildings")]
    public GameObject temple;

    //beacons
    [Header("Beacons")]
    public GameObject orcBeacon;



    //mobs
    [Header("Mobs")]
    public GameObject elf;
    public GameObject human;
    public GameObject goblin;



    //mob groups
    [Header("Mob Groups")]
    public GameObject elfGroup;
    public GameObject humanGroup;
    public GameObject goblinGroup;



    //resources
    [Header("Resources")]
    public GameObject tree;
    public GameObject iron;
    public GameObject ironGroup;
}
