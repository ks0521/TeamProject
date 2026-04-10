using Battle;
using Growth.Equipment;
using UnityEngine;

namespace Growth.Skill
{
    public class PassiveSkill : Skill
    {
        [SerializeField] protected PassiveSkillSO skillData;
        public override SkillSO SkillData => skillData;
        public PassiveSkillSO PassiveSkillData => skillData;
        private StatIncrease resultSkillData;
        public StatIncrease ResultSkillData => resultSkillData;
        public override void Init(Character owner)
        {
            base.Init(owner);
            InitExtractors();
        }
        public override void StatUpdate()
        {
            resultSkillData = skillData.ResultAddStat(curLv);
        }
        List<IStatExtractor> extractors;
        public List<IStatExtractor> Extractors => extractors;
        void InitExtractors()
        {
            extractors = new List<IStatExtractor>();
            var fields = typeof(StatIncrease).GetFields();
            var stat = ResultSkillData;
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(int))
                {
                    extractors.Add(new StatIncreaseExtractor<int>(f.Name, (s) => (int)f.GetValue(s), 1));
                }
                else if (f.FieldType == typeof(float))
                {
                    extractors.Add(new StatIncreaseExtractor<float>(f.Name, (s) => (float)f.GetValue(s), 0.001f));
                }
            }
        }
    }
    public interface IStatExtractor
    {
        public string Name { get; }
        public bool IsEffective(StatIncrease stats);
        public void GetValue(StatIncrease stats, Action<string, string> func);
    }
    public class StatIncreaseExtractor<T> : IStatExtractor where T : IComparable
    {
        public string Name { get; }
        private Func<StatIncrease, T> statGetter;
        private T threshold;
        public StatIncreaseExtractor(string name, Func<StatIncrease, T> statGetter, T threshold)
        {
            Name = name;
            this.statGetter = statGetter;
            this.threshold = threshold;
        }

        public bool IsEffective(StatIncrease stats)
        {
            return statGetter(stats).CompareTo(threshold) >= 0;
        }
        public void GetValue(StatIncrease stats, Action<string, string> func)
        {
            string valueTxt;
            T value = statGetter(stats);
            if (value is float fValue)
                valueTxt = (fValue * 100f).ToString();
            else
                valueTxt = statGetter(stats).ToString();
                func?.Invoke(Name, valueTxt);
        }
    }
}