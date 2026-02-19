using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcVillage : MonoBehaviour
{
    //instance
    static OrcVillage Instance;
    public static OrcVillage instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject go = new GameObject();
                go.AddComponent<OrcVillage>();
            }
            return Instance;
        }
    }

    //basic buildings
    public Tent tent;
    public BreedingPlace breedingPlace;
    public Campfire campfire;

    //points
    public OrcGuardPoint guardPoint1, guardPoint2;
    public Transform [] patrolPoints;

    private void Awake()
    {
        Instance = this;
    }

    public BuildablePlace[] buildablePlaces = new BuildablePlace[6];




    public int wood;
    public int iron;



    public List<OrcBeacon> ownedIronCamps = new List<OrcBeacon>();
    public List<SimpleOrc> simpleOrcs = new List<SimpleOrc>();





    const float POINTRADIUS = 2;
    public Vector3 legitPointToWanderInsideVillage()
    {
        while(true)
        {
            Vector3 point = new Vector3(Random.Range(-40, 40), 0, Random.Range(-40, 40));
            Bounds boundAround = new Bounds(point, new Vector3(POINTRADIUS, POINTRADIUS, POINTRADIUS) * 2);

            if (tent.area.bounds.Intersects(boundAround))
                continue;
            if (campfire.area.bounds.Intersects(boundAround))
                continue;
            if (breedingPlace.area.bounds.Intersects(boundAround))
                continue;

            bool bad = false;
            for (int i = 0; i < buildablePlaces.Length; i++)
            {
                if (buildablePlaces[i].hasBuilding)
                    if (buildablePlaces[i].buildingOnIt.area.bounds.Intersects(boundAround))
                    {
                        bad = true;
                        break;
                    }
            }

            if (bad)
                continue;
            else
                return point;
        }
    }


    private void Update()
    {
        AssignOrcsToGuardPoints();
        AssignOrcsToPatrolPoints();
        AssignOrcsToGatherTree();
    }

    void AssignOrcsToGuardPoints()
    {
        if (guardPoint1.guardingOrc == null)
        {
            for (int i = 0; i < simpleOrcs.Count; i++)
            {
                if (simpleOrcs[i].state == SimpleOrc.states.wanderInVillage)
                {
                    guardPoint1.SetOrcToGuard(simpleOrcs[i]);
                    break;
                }
            }
        }
        if (guardPoint2.guardingOrc == null)
        {
            for (int i = 0; i < simpleOrcs.Count; i++)
            {
                if (simpleOrcs[i].state == SimpleOrc.states.wanderInVillage)
                {
                    guardPoint2.SetOrcToGuard(simpleOrcs[i]);
                    break;
                }
            }
        }
    }

    public int patrollingOrcsCounter = 0;
    void AssignOrcsToPatrolPoints()
    {
        if (patrollingOrcsCounter < 2)
        {
            for (int i = 0; i < simpleOrcs.Count; i++)
            {
                if (simpleOrcs[i].state == SimpleOrc.states.wanderInVillage)
                {
                    simpleOrcs[i].ChangeState(SimpleOrc.states.patrol);
                }

                if (patrollingOrcsCounter >= 2)
                    break;
            }
        }
    }



    public int woodGathererCounter = 0;
    void AssignOrcsToGatherTree()
    {
        for (int i = 0; i < simpleOrcs.Count; i++)
        {
            if (simpleOrcs[i].state == SimpleOrc.states.wanderInVillage)
            {
                simpleOrcs[i].ChangeState(SimpleOrc.states.gatheringWood);
            }

            if (woodGathererCounter >= 1)
                break;
        }
    }

}
