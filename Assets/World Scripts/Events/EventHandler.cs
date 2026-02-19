using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    static EventHandler Instance;
    public static EventHandler instance
    {
        get
        {
            if (Instance == null)
            {
                GameObject go = new GameObject();
                go.AddComponent<EventHandler>();
            }
            return Instance;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public Event currentEvent;

    public enum eventTypes
    {
        nothing,
        humanAttack,
        elfAttack,
        goblinAttack
    }

    public eventTypes nextEvent = eventTypes.humanAttack;

    float eventTimer;
    float eventTimerMax = 50;

    private void Update()
    {
        //if there is no event, then timer goes brrr then let there be an event and generate the next event
        if (currentEvent == null)
        {
            eventTimer += Time.deltaTime;

            if (eventTimer > eventTimerMax)
            {
                eventTimer = 0;

                eventTimerMax = Random.Range(50, 100);

                MakeNewEvent(nextEvent);
                nextEvent = (eventTypes)Random.Range(1, eventTypes.GetNames(typeof(eventTypes)).Length);
            }
        }
        else
            currentEvent.CheckIfEnded();
    }

    void MakeNewEvent(eventTypes newEventType)
    {
        switch(newEventType)
        {
            case eventTypes.humanAttack:
                currentEvent = new HumanAttack();
                currentEvent.StartEvent();
                break;
            case eventTypes.elfAttack:
                currentEvent = new ElfAttack();
                currentEvent.StartEvent();
                break;
            case eventTypes.goblinAttack:
                currentEvent = new GoblinAttack();
                currentEvent.StartEvent();
                break;
        }
    }
}
