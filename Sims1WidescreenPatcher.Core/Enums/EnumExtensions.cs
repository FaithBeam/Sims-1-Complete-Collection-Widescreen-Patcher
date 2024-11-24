using System.Reflection;
using System.Runtime.Serialization;

namespace Sims1WidescreenPatcher.Core.Enums;

public static class EnumExtensions
{
    public static string? GetEnumMemberValue<T>(T value)
        where T : struct, IConvertible
    {
        return typeof(T)
            .GetTypeInfo()
            .DeclaredMembers.SingleOrDefault(x => x.Name == value.ToString())
            ?.GetCustomAttribute<EnumMemberAttribute>(false)
            ?.Value;
    }
}
