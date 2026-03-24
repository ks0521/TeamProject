using Base.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class PlayerReference : MonoBehaviour, IGameSystem
    {
        private Player player;
        private PlayerRuntimeStatus runtimeStatus;
        private Transform transform;
        public Player Player => player;
        public PlayerRuntimeStatus RuntimeStatus => runtimeStatus;
        public Transform Transform => transform;
        private void Awake()
        {
            player = GetComponent<Player>();
            runtimeStatus = GetComponent<PlayerRuntimeStatus>();
            transform = GetComponent<Transform>();
        }

        public int GetOrder() => 0;
    }
}