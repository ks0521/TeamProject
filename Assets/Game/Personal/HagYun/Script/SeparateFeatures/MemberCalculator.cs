using Personal.HagYun;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;

namespace Personal.HagYun
{
    public interface IMemberReceiver
    {
        public void Receive(string name, int value);
        public void Receive(string name, float value);
        public void Receive(string name, string value);
        public void ReceiveOther(string name, object value);
    }
    public struct MemberData<T>
    {
        public string name;
        public TypeCode dataType;
        public FieldInfo field;
        public Action<T, IMemberReceiver> valueGetter;
    }
    public static class MemberExtractor<T>
    {
        private static readonly Dictionary<string, MemberData<T>> memberDic;
        private static readonly MemberData<T>[] memberDataArr;
        public static MemberData<T>[] MemberDataArr => memberDataArr;
        static MemberExtractor()
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            int cnt = fields.Length;
            memberDataArr = new MemberData<T>[cnt];
            memberDic = new Dictionary<string, MemberData<T>>(cnt);
            for (int i = 0; i < cnt; i++)
            {
                var f = fields[i];
                MemberData<T> md = new MemberData<T>();
                md.name = f.Name;
                md.field = f;

                var targetParam = Expression.Parameter(typeof(T), "target");
                var receiverParam = Expression.Parameter(typeof(IMemberReceiver), "receiver");
                var fieldExpr = Expression.Field(targetParam, f);

                MethodInfo method;

                if (f.FieldType == typeof(int))
                {
                    method = typeof(IMemberReceiver).GetMethod(nameof(IMemberReceiver.Receive), new[] { typeof(string), typeof(int) });
                    md.dataType = TypeCode.Int32;
                }
                else if (f.FieldType == typeof(float))
                {
                    method = typeof(IMemberReceiver).GetMethod(nameof(IMemberReceiver.Receive), new[] { typeof(string), typeof(float) });
                    md.dataType = TypeCode.Single;
                }
                else if (f.FieldType == typeof(string))
                {
                    method = typeof(IMemberReceiver).GetMethod(nameof(IMemberReceiver.Receive), new[] { typeof(string), typeof(string) });
                    md.dataType = TypeCode.String;
                }
                else
                {
                    method = typeof(IMemberReceiver).GetMethod(nameof(IMemberReceiver.ReceiveOther));
                    md.dataType = TypeCode.Object;

                    var boxedField = Expression.Convert(fieldExpr, typeof(object));
                    var unknownCall = Expression.Call(receiverParam, method, Expression.Constant(f.Name), boxedField);
                    md.valueGetter = Expression.Lambda<Action<T, IMemberReceiver>>(unknownCall, targetParam, receiverParam).Compile();
                }

                if (md.valueGetter == null)
                {
                    var call = Expression.Call(receiverParam, method, Expression.Constant(f.Name), fieldExpr);
                    md.valueGetter = Expression.Lambda<Action<T, IMemberReceiver>>(call, targetParam, receiverParam).Compile();
                }

                memberDataArr[i] = md;
                memberDic.Add(md.name, md);
            }
        }

        public static void ExtractAll(T target, IMemberReceiver receiver)
        {
            for (int i = 0; i < memberDataArr.Length; i++)
            {
                memberDataArr[i].valueGetter(target, receiver);
            }
        }

        public static bool TryExtract(T target, string fieldName, IMemberReceiver receiver)
        {
            if (memberDic.TryGetValue(fieldName, out MemberData<T> data))
            {
                data.valueGetter(target, receiver);
                return true;
            }
            return false;
        }
    }

    // struct 내부 멤버 전체를 자동으로 찾아 계산하는 static class, new 생성
    public static class StructMemberCalculator<T> where T : struct
    {
        private static readonly Func<T, T, T> addFunc;
        private static readonly Func<T, T, T> subFunc;
        private static readonly Func<T, int, T> intMultiplyFunc;
        private static readonly Func<T, float, T> floatMultiplyFunc;
        static StructMemberCalculator()
        {
            var memberDataArr = MemberExtractor<T>.MemberDataArr;
            var left = Expression.Parameter(typeof(T), "a");
            addFunc = GenerateOP<Func<T, T, T>>(memberDataArr, left, Expression.Parameter(typeof(T), "b"), Expression.Add);
            subFunc = GenerateOP<Func<T, T, T>>(memberDataArr, left, Expression.Parameter(typeof(T), "b"), Expression.Subtract);
            intMultiplyFunc = GenerateOP<Func<T, int, T>>(memberDataArr, left, Expression.Parameter(typeof(int), "b"), Expression.Multiply);
            floatMultiplyFunc = GenerateOP<Func<T, float, T>>(memberDataArr, left, Expression.Parameter(typeof(float), "b"), Expression.Multiply);
        }
        /// <summary> struct T a + struct T b </summary>
        public static T Add(T a, T b) => addFunc(a, b);
        /// <summary> struct T a - struct T b </summary>
        public static T Sub(T a, T b) => subFunc(a, b);
        /// <summary> struct T a + int b </summary>
        public static T Mul(T a, int b) => intMultiplyFunc(a, b);
        /// <summary> struct T a + float b </summary>
        public static T Mul(T a, float b) => floatMultiplyFunc(a, b);
        private static TDelegate GenerateOP<TDelegate>(MemberData<T>[] dataArr, ParameterExpression left, ParameterExpression right,
        Func<Expression, Expression, BinaryExpression> calFunc)
        {
            int cnt = dataArr.Length;
            var assignments = new MemberAssignment[cnt];
            for (int i = 0; i < cnt; i++)
            {
                var f = dataArr[i].field;
                switch (dataArr[i].dataType)
                {
                    case TypeCode.Int32:
                    case TypeCode.Single:
                        Expression fieldA = Expression.Field(left, f);

                        Expression fieldB = (right.Type == typeof(T))
                            ? Expression.Field(right, f)
                            : Expression.Convert(right, f.FieldType);
                        assignments[i] = Expression.Bind(f, calFunc(fieldA, fieldB));
                        break;
                    default:
                        assignments[i] = Expression.Bind(f, Expression.Field(left, f));
                        break;
                }
            }

            var body = Expression.MemberInit(Expression.New(typeof(T)), assignments);

            return Expression.Lambda<TDelegate>(body, left, right).Compile();
        }
    }
}
// class 내부 멤버 전체를 자동으로 찾아 계산하는 static class, 원본 유지
public static class ClassMemberCalculator<T> where T : class
{
    private static readonly Func<T, T, T> addFunc;
    private static readonly Func<T, T, T> subFunc;
    private static readonly Func<T, int, T> intMultiplyFunc;
    private static readonly Func<T, float, T> floatMultiplyFunc;
    static ClassMemberCalculator()
    {
        var memberData = MemberExtractor<T>.MemberDataArr;
        var left = Expression.Parameter(typeof(T), "a");
        addFunc = GenerateOP<Func<T, T, T>>(memberData, left, Expression.Parameter(typeof(T), "b"), Expression.Add);
        subFunc = GenerateOP<Func<T, T, T>>(memberData, left, Expression.Parameter(typeof(T), "b"), Expression.Subtract);
        intMultiplyFunc = GenerateOP<Func<T, int, T>>(memberData, left, Expression.Parameter(typeof(int), "b"), Expression.Multiply);
        floatMultiplyFunc = GenerateOP<Func<T, float, T>>(memberData, left, Expression.Parameter(typeof(float), "b"), Expression.Multiply);
    }
    /// <summary> struct T a + struct T b </summary>
    public static T Add(T a, T b) => addFunc(a, b);
    /// <summary> struct T a - struct T b </summary>
    public static T Sub(T a, T b) => subFunc(a, b);
    /// <summary> struct T a + int b </summary>
    public static T Mul(T a, int b) => intMultiplyFunc(a, b);
    /// <summary> struct T a + float b </summary>
    public static T Mul(T a, float b) => floatMultiplyFunc(a, b);
    private static TDelegate GenerateOP<TDelegate>(MemberData<T>[] dataArr, ParameterExpression left, ParameterExpression right,
    Func<Expression, Expression, BinaryExpression> calFunc)
    {
        int length = 0;
        foreach (var d in dataArr)
        {
            switch (d.dataType)
            {
                case TypeCode.Int32:
                case TypeCode.Single:
                    length++;
                    break;
            }
        }

        var expressions = new Expression[length + 1];
        int curFieldIndex = 0;
        foreach (var d in dataArr)
        {
            var f = d.field;
            switch (d.dataType)
            {
                case TypeCode.Int32:
                case TypeCode.Single:
                    var fieldA = Expression.Field(left, f);
                    Expression fieldB = (right.Type == typeof(T))
                        ? Expression.Field(right, f)
                        : Expression.Convert(right, f.FieldType);

                    expressions[curFieldIndex++] = Expression.Assign(fieldA, calFunc(fieldA, fieldB));
                    break;
            }
        }
        expressions[length] = left;

        var body = Expression.Block(expressions);
        return Expression.Lambda<TDelegate>(body, left, right).Compile();
    }
}
