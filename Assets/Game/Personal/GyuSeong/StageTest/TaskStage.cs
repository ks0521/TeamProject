using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskStage
{
    List<StageActionTask> ActionTaskList = new List<StageActionTask>();

    public TaskStage()
    {
        
    }
    private void Start()
    {
        AddActionTask<StageActionTask_Init>();
        AddActionTask<StageActionTask_Timer>().Init(3.0f);
    }

    private T AddActionTask<T>() where T : StageActionTask, new()
    {
        T newTask = new T();
        ActionTaskList.Add(newTask);
        return newTask;
    }
}
