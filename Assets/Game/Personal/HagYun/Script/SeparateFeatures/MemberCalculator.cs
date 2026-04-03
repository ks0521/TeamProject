using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Personal.HagYun
{
    // struct 내부 멤버 전체를 자동으로 찾아 계산하는 static class, new 생성
    public static class StructMemberCalculator<T> where T : struct
    {
        private static readonly Func<T, T, T> addFunc;
        private static readonly Func<T, T, T> subFunc;
        private static readonly Func<T, int, T> intMultiplyFunc;
        private static readonly Func<T, float, T> floatMultiplyFunc;
        static StructMemberCalculator()
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var left = Expression.Parameter(typeof(T), "a");
            addFunc = GenerateOP<Func<T, T, T>>(fields, left, Expression.Parameter(typeof(T), "b"), Expression.Add);
            subFunc = GenerateOP<Func<T, T, T>>(fields, left, Expression.Parameter(typeof(T), "b"), Expression.Subtract);
            intMultiplyFunc = GenerateOP<Func<T, int, T>>(fields, left, Expression.Parameter(typeof(int), "b"), Expression.Multiply);
            floatMultiplyFunc = GenerateOP<Func<T, float, T>>(fields, left, Expression.Parameter(typeof(float), "b"), Expression.Multiply);
        }
        /// <summary> struct T a + struct Tb </summary>
        public static T Add(T a, T b) => addFunc(a, b);
        /// <summary> struct T a - struct T b </summary>
        public static T Sub(T a, T b) => subFunc(a, b);
        /// <summary> struct T a + int b </summary>
        public static T Mul(T a, int b) => intMultiplyFunc(a, b);
        /// <summary> struct T a + float b </summary>
        public static T Mul(T a, float b) => floatMultiplyFunc(a, b);
        private static TDelegate GenerateOP<TDelegate>(FieldInfo[] fields, ParameterExpression left, ParameterExpression right,
        Func<Expression, Expression, BinaryExpression> calFunc)
        {
            int cnt = fields.Length;
            var assignments = new MemberAssignment[cnt];
            for (int i = 0; i < cnt; i++)
            {
                var f = fields[i];
                if (f.FieldType == typeof(int) || f.FieldType == typeof(float))
                {
                    var fieldA = Expression.Field(left, f);

                    Expression fieldB = (right.Type == typeof(T))
                        ? Expression.Field(right, f)
                        : Expression.Convert(right, f.FieldType);
                    assignments[i] = Expression.Bind(f, calFunc(fieldA, fieldB));
                }
                else
                {
                    assignments[i] = Expression.Bind(f, Expression.Field(left, f));
                }
            }

            var body = Expression.MemberInit(Expression.New(typeof(T)), assignments);

            return Expression.Lambda<TDelegate>(body, left, right).Compile();
        }
    }
    // class 내부 멤버 전체를 자동으로 찾아 계산하는 static class, 원본 유지
    public static class ClassMemberCalculator<T> where T : class, new()
    {
        private static readonly Func<T, T, T> addFunc;
        private static readonly Func<T, T, T> subFunc;
        private static readonly Func<T, int, T> intMultiplyFunc;
        private static readonly Func<T, float, T> floatMultiplyFunc;
        static ClassMemberCalculator()
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var left = Expression.Parameter(typeof(T), "a");
            addFunc = GenerateOP<Func<T, T, T>>(fields, left, Expression.Parameter(typeof(T), "b"), Expression.Add);
            subFunc = GenerateOP<Func<T, T, T>>(fields, left, Expression.Parameter(typeof(T), "b"), Expression.Subtract);
            intMultiplyFunc = GenerateOP<Func<T, int, T>>(fields, left, Expression.Parameter(typeof(int), "b"), Expression.Multiply);
            floatMultiplyFunc = GenerateOP<Func<T, float, T>>(fields, left, Expression.Parameter(typeof(float), "b"), Expression.Multiply);
        }
        public static T Add(T a, T b) => addFunc(a, b);
        public static T Sub(T a, T b) => subFunc(a, b);
        public static T Mul(T a, int b) => intMultiplyFunc(a, b);
        public static T Mul(T a, float b) => floatMultiplyFunc(a, b);
        private static TDelegate GenerateOP<TDelegate>(FieldInfo[] fields, ParameterExpression left, ParameterExpression right,
        Func<Expression, Expression, BinaryExpression> calFunc)
        {
            int length = 0;
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(int) || f.FieldType == typeof(float))
                    length++;
            }

            var expressions = new Expression[length + 1];
            int curFieldIndex = 0;
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(int) || f.FieldType == typeof(float))
                {
                    var fieldA = Expression.Field(left, f);
                    Expression fieldB = (right.Type == typeof(T))
                        ? Expression.Field(right, f)
                        : Expression.Convert(right, f.FieldType);

                    expressions[curFieldIndex++] = Expression.Assign(fieldA, calFunc(fieldA, fieldB));
                }
            }
            expressions[length] = left;

            var body = Expression.Block(expressions);
            return Expression.Lambda<TDelegate>(body, left, right).Compile();
        }
    }
}