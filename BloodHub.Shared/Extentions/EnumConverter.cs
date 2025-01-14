using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public static class EnumConverter
{
    public static ValueConverter<TEnum, string> CreateEnumToStringConverter<TEnum>(Dictionary<TEnum, string> enumToStringMap, Dictionary<string, TEnum> stringToEnumMap) where TEnum : struct, Enum
    {
        return new ValueConverter<TEnum, string>(
            v => enumToStringMap[v],
            v => stringToEnumMap[v]
        );
    }
}
