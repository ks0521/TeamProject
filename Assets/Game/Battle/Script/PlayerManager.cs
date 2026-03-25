using Base.Managers;
using UnityEngine;

namespace Battle
{
    public class PlayerManager : MonoBehaviour, IManager
    {
        private Player player;
        private PlayerRuntimeStatus runtimeStatus;
        public Player Player => player;
        public PlayerRuntimeStatus RuntimeStatus => runtimeStatus;
        private void Awake()
        {
            player = GetComponent<Player>();
            runtimeStatus = GetComponentInChildren<PlayerRuntimeStatus>();
        }
        public void Init()
        {
            player.Init();
        }

        public int GetOrder() => 5; //StageManager에서 PlayerManager를 호출하기 때문에 StageManager(10)보다 먼저 초기화 되어야함
    }
}