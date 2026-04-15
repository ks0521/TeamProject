using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base.Utils
{
    // 행동 트리(Behaviour Tree) 제작 Base

    public enum NodeState
    {
        /// <summary> 다음 Frame에 현재 Node 재실행 </summary>
        Run,
        /// <summary> 현재 Node 성공 처리, (Selector) Success 반환 후 멈춤, (Sequence) 바로 다음 Node 실행 </summary>
        Success,
        /// <summary> 현재 Node 실패 처리, (Selector) 바로 다음 Node 실행, (Sequence) Fail 반환 후 멈춤 </summary>
        Fail
    }
    /// <summary> 모든 Node의 Base class </summary>
    public abstract class Node
    {
        /// <summary> 객체가 파괴될 때 실행되어야 하는 기능 </summary>
        public abstract void DestroyFeat();
        /// <summary> Node 실행 기능 (Action : 실제 실행, Selector : 하나라도 Success이면 Success, Sequence : 전부 다 성공해야 Success</summary>
        /// <returns>Evaluate 실행 시 NodeState 반환</returns>
        public abstract NodeState Evaluate();
    }

    public class ConditionNode : Node
    {
        public Func<bool> condition;
        public override void DestroyFeat() => condition = null;
        public ConditionNode(Func<bool> condition) => this.condition = condition;
        public override NodeState Evaluate()
        {
            if (condition == null) return NodeState.Fail;
            return condition.Invoke() ? NodeState.Success : NodeState.Fail;
        }
    }
    /// <summary> 실제 실행을 담당하는 Node </summary>
    public class ActionNode : Node
    {
        /// <summary> 해당 Action Node에서 실행할 기능, 실행 시 NodeState 반환 </summary>
        Func<NodeState> action;
        // public void AddAction(Func<NodeState> action) => this.action = action;
        public ActionNode(Func<NodeState> action) => this.action = action;
        /// <summary> action에 저장된 함수에 대한 참조를 해제 </summary>
        public override void DestroyFeat() => action = null;
        /// <summary> Action Node 실행 기능 </summary>
        /// <returns>Action실행 실패/완료에 대한 NodeState 반환</returns>
        public override NodeState Evaluate() => action?.Invoke() ?? NodeState.Fail;
    }
    /// <summary> 저장된 Node 중 하나라도 Success 시 Success를 반환하는 class </summary>
    public class SelectorNode : Node
    {
        /// <summary> 해당 Selector Node에서 Evaluate를 실행할 Node의 List, 왼쪽(처음)부터 마지막에 추가된 Node까지 차례대로 Evaluate 실행 </summary>
        List<Node> nodes = new List<Node>();
        /// <summary> 객체 파괴 시 내부 Node의 Destroy 기능 실행 </summary>
        public override void DestroyFeat()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].DestroyFeat();
            }
        }
        /// <summary> Selector 로 확인할 Node 추가 </summary>
        /// <param name="node">Action/Selector/Sequence Node</param>
        public void AddNode(Node node)
        {
            nodes.Add(node);
            //Debug.Log($"SelectorNode에 Node 추가{nodes.Count}");
        }
        /// <summary> 내부 Evaluate 실행 결과에 따라 NodeState를 반환 </summary>
        /// <returns>1개라도 성공 시 Success 반환</returns>
        public override NodeState Evaluate()
        {
            if (nodes.Count <= 0) return NodeState.Fail;
            foreach (Node node in nodes)
            {
                NodeState state = node.Evaluate();
                switch (state)
                {
                    case NodeState.Run:
                        return NodeState.Run;
                    case NodeState.Success:
                        return NodeState.Success;
                }
            }

            return NodeState.Fail;
        }
    }
    /// <summary> 저장된 Node 전부 Success 시 Success를 반환하는 class </summary>
    public class SequenceNode : Node
    {
        /// <summary> 해당 Sequence Node에서 Evaluate를 실행할 Node의 List, 왼쪽(처음)부터 마지막에 추가된 Node까지 차례대로 Evaluate 실행 </summary>
        List<Node> nodes = new List<Node>();
        /// <summary> 객체 파괴 시 내부 Node의 Destroy 기능 실행 </summary>
        public override void DestroyFeat()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].DestroyFeat();
            }
        }
        /// <summary> Sequence 로 확인할 Node 추가 </summary>
        /// <param name="node">Action/Selector/Sequence Node</param>
        public void AddNode(Node node)
        {
            nodes.Add(node);
            //Debug.Log($"SequenceNode에 Node 추가{nodes.Count}");
        }
        /// <summary> 내부 Evaluate 실행 결과에 따라 NodeState를 반환(Success일 경우 다음 Node 실행/Run일 경우 다음 Frame에 현재 Node 재실행) </summary>
        /// <returns>전부 Success 반환 시 Success 반환</returns>
        public override NodeState Evaluate()
        {
            if (nodes.Count <= 0) return NodeState.Fail;
            foreach (Node node in nodes)
            {
                NodeState state = node.Evaluate();
                switch (state)
                {
                    case NodeState.Run:
                        return NodeState.Run;
                    case NodeState.Success:
                        continue;
                    case NodeState.Fail:
                        return NodeState.Fail;
                }
            }
            return NodeState.Success;
        }
    }
}