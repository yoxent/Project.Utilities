using System;

namespace ProjectUtilities.Options.Core
{
    public enum OptionValueType
    {
        Bool,
        Int,
        Float,
        String
    }

    /// <summary>
    /// Tagged union for option values. UI bindings can inspect Type and read the appropriate field.
    /// </summary>
    [Serializable]
    public struct OptionValue
    {
        public OptionValueType Type;
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public string StringValue;

        public static OptionValue FromBool(bool value) => new OptionValue { Type = OptionValueType.Bool, BoolValue = value };
        public static OptionValue FromInt(int value) => new OptionValue { Type = OptionValueType.Int, IntValue = value };
        public static OptionValue FromFloat(float value) => new OptionValue { Type = OptionValueType.Float, FloatValue = value };
        public static OptionValue FromString(string value) => new OptionValue { Type = OptionValueType.String, StringValue = value };

        public T As<T>()
        {
            object result = default(T);

            if (typeof(T) == typeof(bool))
            {
                result = BoolValue;
            }
            else if (typeof(T) == typeof(int))
            {
                result = IntValue;
            }
            else if (typeof(T) == typeof(float))
            {
                result = FloatValue;
            }
            else if (typeof(T) == typeof(string))
            {
                result = StringValue;
            }

            return (T)result;
        }
    }
}

