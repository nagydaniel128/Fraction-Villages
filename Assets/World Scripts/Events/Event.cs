using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event
{
    public abstract void StartEvent();
    public abstract void CheckIfEnded();
    public void TakePlace()
    {
        EventHandler.instance.currentEvent = this;
    }
    public void EndEvent()
    {
        EventHandler.instance.currentEvent = null;
    }
}
