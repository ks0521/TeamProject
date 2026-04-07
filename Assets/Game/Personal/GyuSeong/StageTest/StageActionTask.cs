using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageActionTaskUpdateResult
{
    Start, Running, End //각각 OnStart, OnUpdate, OnEnd와 대응
}

public class StageActionTask
{
    public virtual void OnStart()
    {
        
    }
    public virtual StageActionTaskUpdateResult OnUpdate()
    {
        return StageActionTaskUpdateResult.Running;
    }
    public virtual void OnEnd()
    {

    }
}
