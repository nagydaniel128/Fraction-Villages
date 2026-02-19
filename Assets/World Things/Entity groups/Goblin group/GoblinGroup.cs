using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinGroup : EntityGroup
{
    public List<Goblin> goblins = new List<Goblin>();

    void Awake()
    {
        for (int i = 0; i < Random.Range(5, 8); i++)
        {
            GameObject a = Instantiate(GameManager.instance.goblin);

            a.transform.parent = transform;
            a.transform.localPosition = new Vector3(Random.Range(-10f, 10f), 1, Random.Range(-10f, 10f));

            a.GetComponent<Goblin>().group = this;
            goblins.Add(a.GetComponent<Goblin>());
        }
    }

    public void RemoveGoblin(Goblin goblin)
    {
        goblins.Remove(goblin);

        if (goblins.Count == 0)
            Destroy(gameObject);
    }

    public enum states
    {
        staying,
        wandering
    }
    states state = states.staying;

    float timer;
    float timerMax = 20;

    public Vector3 wanderPoint;

    private void Update()
    {
        switch (state)
        {
            case states.staying:
                Stay();
                break;
            case states.wandering:
                Wander();
                break;
        }
    }

    void Wander()
    {
        for (int i = 0; i < goblins.Count; i++)
        {
            if (!goblins[i].arrivedToWanderPoint)
                return;

            ChangeState(states.staying);
        }
    }

    void Stay()
    {
        timer += Time.deltaTime;

        if (timer > timerMax)
        {
            timer = 0;
            timerMax = Random.Range(10, 20);

            ChangeState(states.wandering);
        }
    }

    public void ChangeState(states newState, bool newWanderPoint = true)
    {
        switch (newState)
        {
            case states.staying:
                state = states.staying;
                for (int i = 0; i < goblins.Count; i++)
                    goblins[i].state = Goblin.states.idle;
                break;
            case states.wandering:
                state = states.wandering;
                if (newWanderPoint)
                    wanderPoint = new Vector3(Random.Range(-500f, 500f), 0, Random.Range(-500, 500));

                for (int i = 0; i < goblins.Count; i++)
                {
                    goblins[i].state = Goblin.states.wandering;
                    goblins[i].wanderPoint = wanderPoint + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                }
                break;
        }
    }
}
