using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageActionTask_Spawner : StageActionTask
{
    private StageSO stage;
    public void Init(StageSO stage)
    {
        this.stage = stage;
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
