using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Base.Utils
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
            // T의 데이터를 미리 캐싱
            var memberDataArr = MemberExtractor<T>.MemberDataArr;
            // T left Cal(+/-/*) T right 에서 left를 담당
            var left = Expression.Parameter(typeof(T), "a");
            addFunc = GenerateOP<Func<T, T, T>>(memberDataArr, left, Expression.Parameter(typeof(T), "b"), Expression.Add, "op_Addition");
            subFunc = GenerateOP<Func<T, T, T>>(memberDataArr, left, Expression.Parameter(typeof(T), "b"), Expression.Subtract, "op_Subtraction");
            intMultiplyFunc = GenerateOP<Func<T, int, T>>(memberDataArr, left, Expression.Parameter(typeof(int), "b"), Expression.Multiply, "op_Multiply");
            floatMultiplyFunc = GenerateOP<Func<T, float, T>>(memberDataArr, left, Expression.Parameter(typeof(float), "b"), Expression.Multiply, "op_Multiply");
        }
        /// <summary> struct T a + struct Tb </summary>
        public static T Add(T a, T b) => addFunc(a, b);
        /// <summary> struct T a - struct T b </summary>
        public static T Sub(T a, T b) => subFunc(a, b);
        /// <summary> struct T a * int b </summary>
        public static T Mul(T a, int b) => intMultiplyFunc(a, b);
        /// <summary> struct T a * float b </summary>
        public static T Mul(T a, float b) => floatMultiplyFunc(a, b);
        /// <summary> add/sub/muliplyFunc에 삽입할 계산 함수 </summary>
        /// <typeparam name="TDelegate">람다 식을 컴퓨터가 인식할 수 있는 식 트리 형식 delegate로 변환</typeparam>
        /// <param name="dataArr">T의 멤버 배열</param>
        /// <param name="left">T의 왼쪽 변수</param>
        /// <param name="right">T의 오른쪽 변수</param>
        /// <param name="calFunc">실제 계산할 Expression 함수</param>
        /// <param name="opMethodName">(멤버가 다른 struct일 경우)계산에 사용될 함수 명</param>
        /// <returns>완성된 식 트리 delegate</returns>
        private static TDelegate GenerateOP<TDelegate>(MemberData<T>[] dataArr, ParameterExpression left, ParameterExpression right,
        Func<Expression, Expression, BinaryExpression> calFunc, string opMethodName)
        {
            int cnt = dataArr.Length;
            var assignments = new MemberAssignment[cnt];
            for (int i = 0; i < cnt; i++)
            {
                var f = dataArr[i].field;
                Expression fieldA = Expression.Field(left, f);

                Expression fieldB;
                if (right.Type == typeof(T))
                {
                    fieldB = Expression.Field(right, f);
                }
                else
                {
                    fieldB = f.FieldType.IsPrimitive ? Expression.Convert(right, f.FieldType) : right;
                }

                switch (dataArr[i].dataType)
                {
                    case TypeCode.Int32:
                    case TypeCode.Single:
                        assignments[i] = Expression.Bind(f, calFunc(fieldA, fieldB));
                        break;
                    default:
                        var opMethod = f.FieldType.GetMethod(opMethodName,
                            BindingFlags.Public | BindingFlags.Static,
                            null, new[] { f.FieldType, fieldB.Type }, null);
                        if (opMethod != null) assignments[i] = Expression.Bind(f, Expression.Call(opMethod, fieldA, fieldB));
                        else assignments[i] = Expression.Bind(f, Expression.Field(left, f));
                        break;
                }
            }

            var body = Expression.MemberInit(Expression.New(typeof(T)), assignments);

            return Expression.Lambda<TDelegate>(body, left, right).Compile();
        }
    }
    // /// <summary>
    // /// struct 멤버 2개를 계산할 때, 동일한 변수명, 동일한 타입일 경우 해당 변수끼리 +/- 시키는 class
    // /// </summary>
    // /// <typeparam name="T">left struct</typeparam>
    // /// <typeparam name="U">right struct</typeparam>
    // public static class StructMixCalculatorOnlySameNameSameType<T, U> where T : struct where U : struct
    // {
    //     private static readonly Func<T, U, T> mixAddFunc;
    //     private static readonly Func<T, U, T> mixSubFunc;

    //     static StructMixCalculatorOnlySameNameSameType()
    //     {
    //         var tMemArr = MemberExtractor<T>.MemberDataArr;
    //         var uMemArr = MemberExtractor<U>.MemberDataArr;

    //         var left = Expression.Parameter(typeof(T), "a");
    //         var right = Expression.Parameter(typeof(U), "b");

    //         mixAddFunc = GenerateOP(tMemArr, left, uMemArr, right, true);
    //         mixSubFunc = GenerateOP(tMemArr, left, uMemArr, right, false);
    //     }
    //     static Func<T, U, T> GenerateOP(MemberData<T>[] tMemArr, ParameterExpression left, MemberData<U>[] uMemArr, ParameterExpression right, bool isAdd)
    //     {
    //         int cnt = tMemArr.Length;
    //         var assignments = new MemberAssignment[tMemArr.Length];

    //         // T(결과물)의 멤버를 기준으로 U에서 같은 이름/타입 찾기
    //         for (int i = 0; i < cnt; i++)
    //         {
    //             var tData = tMemArr[i];
    //             // 이름과 타입이 완벽히 일치하는 멤버를 찾기
    //             MemberData<U> uData = new();
    //             foreach (var uMem in uMemArr)
    //             {
    //                 if (tData.name == uMem.name && tData.dataType == uMem.dataType)
    //                 {
    //                     uData = uMem;
    //                     break;
    //                 }
    //             }
    //             Expression fieldA = Expression.Field(left, tData.field);

    //             // 일치하는 멤버를 찾았고 연산 가능한 타입(int, float)인 경우
    //             if (uData.field != null)
    //             {
    //                 switch (tData.dataType)
    //                 {
    //                     case TypeCode.Int32:
    //                     case TypeCode.Single:
    //                         Expression fieldB = Expression.Field(right, uData.field);

    //                         // 직접 더하기/빼기 연산 (a.field +/- b.field)
    //                         BinaryExpression calExpr = isAdd ?
    //                             Expression.Add(fieldA, fieldB) :
    //                             Expression.Subtract(fieldA, fieldB);
    //                         assignments[i] = Expression.Bind(tData.field, calExpr);
    //                         break;
    //                 }
    //             }
    //             else
    //             {
    //                 // 없으면 T의 원래 값을 그대로 유지 (복사)
    //                 assignments[i] = Expression.Bind(tData.field, fieldA);
    //             }
    //         }

    //         // 현재까지의 T 계산/유지 데이터를 적용헤 new T로 생성하며 초기화 (MemberInit)
    //         var body = Expression.MemberInit(Expression.New(typeof(T)), assignments);
    //         return Expression.Lambda<Func<T, U, T>>(body, left, right).Compile();
    //     }

    //     /// <summary> 서로 다른 구조체 T와 U에서 이름이 같은 멤버끼리 더해 T를 반환 </summary>
    //     public static T Add(T a, U b) => mixAddFunc(a, b);
    //     /// <summary> 서로 다른 구조체 T와 U에서 이름이 같은 멤버끼리 뺄셈을 해 T를 반환 </summary>
    //     public static T Sub(T a, U b) => mixSubFunc(a, b);
    // }

    // class 내부 멤버 전체를 자동으로 찾아 계산하는 static class, 원본 유지
    // public static class ClassMemberCalculator<T> where T : class, new()
    // {
    //     private static readonly Func<T, T, T> addFunc;
    //     private static readonly Func<T, T, T> subFunc;
    //     private static readonly Func<T, int, T> intMultiplyFunc;
    //     private static readonly Func<T, float, T> floatMultiplyFunc;
    //     static ClassMemberCalculator()
    //     {
    //         var memberData = MemberExtractor<T>.MemberDataArr;
    //         var left = Expression.Parameter(typeof(T), "a");
    //         addFunc = GenerateOP<Func<T, T, T>>(memberData, left, Expression.Parameter(typeof(T), "b"), Expression.Add);
    //         subFunc = GenerateOP<Func<T, T, T>>(memberData, left, Expression.Parameter(typeof(T), "b"), Expression.Subtract);
    //         intMultiplyFunc = GenerateOP<Func<T, int, T>>(memberData, left, Expression.Parameter(typeof(int), "b"), Expression.Multiply);
    //         floatMultiplyFunc = GenerateOP<Func<T, float, T>>(memberData, left, Expression.Parameter(typeof(float), "b"), Expression.Multiply);
    //     }
    //     public static T Add(T a, T b) => addFunc(a, b);
    //     public static T Sub(T a, T b) => subFunc(a, b);
    //     public static T Mul(T a, int b) => intMultiplyFunc(a, b);
    //     public static T Mul(T a, float b) => floatMultiplyFunc(a, b);
    //     private static TDelegate GenerateOP<TDelegate>(MemberData<T>[] dataArr, ParameterExpression left, ParameterExpression right,
    //     Func<Expression, Expression, BinaryExpression> calFunc)
    //     {
    //         int length = 0;
    //         foreach (var d in dataArr)
    //         {
    //             switch (d.dataType)
    //             {
    //                 case TypeCode.Int32:
    //                 case TypeCode.Single:
    //                     length++;
    //                     break;
    //             }
    //         }

    //         var expressions = new Expression[length + 1];
    //         int curFieldIndex = 0;
    //         foreach (var d in dataArr)
    //         {
    //             var f = d.field;
    //             switch (d.dataType)
    //             {
    //                 case TypeCode.Int32:
    //                 case TypeCode.Single:
    //                     var fieldA = Expression.Field(left, f);
    //                     Expression fieldB = (right.Type == typeof(T))
    //                         ? Expression.Field(right, f)
    //                         : Expression.Convert(right, f.FieldType);

    //                     expressions[curFieldIndex++] = Expression.Assign(fieldA, calFunc(fieldA, fieldB));
    //                     break;
    //             }
    //         }
    //         expressions[length] = left;

    //         var body = Expression.Block(expressions);
    //         return Expression.Lambda<TDelegate>(body, left, right).Compile();
    //     }
    // }
}