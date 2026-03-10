using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Monster : Character
    {
        public event Action RequestMonDie;
        private void OnDestroy()
        {
            RequestMonDie?.Invoke();
        }
        protected override void FixedUpdateFeat()
        {

        }

        protected override void UpdateFeat()
        {

        }
    }
}