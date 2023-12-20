using FlashOWare.Tool.Cli.Diagnostics;
using System.Reflection;

namespace FlashOWare.Tool.Cli.Tests.Diagnostics;

public class ExitCodesTests
{
    private static readonly Type _type = typeof(ExitCodes);
    private static readonly BindingFlags _bindingAttr = GetBindingFlags();

    [Fact]
    public void ExitCodes_Zero_IndicatesThatTheProcessCompletedSuccessfully()
    {
        //Arrange
        var fields = _type.GetMembers(_bindingAttr).OfType<FieldInfo>();
        //Act
        var values = fields.Where(IsZero);
        //Assert
        var value = Assert.Single(values);
        Assert.Equal("Success", value.Name);
    }

    [Fact]
    public void ExitCodes_NonZero_IndicatesAnError()
    {
        //Arrange
        var fields = _type.GetMembers(_bindingAttr).OfType<FieldInfo>();
        //Act
        var values = fields.Where(IsNonZero);
        //Assert
        Assert.All(values, static value => Assert.NotEqual("Success", value.Name));
    }

    [Fact]
    public void ExitCodes_Distinct_NoDuplicates()
    {
        //Arrange
        var members = _type.GetMembers(_bindingAttr);
        //Act
        var fields = members.OfType<FieldInfo>();
        //Assert
        Assert.Distinct(fields, FieldInfoValueEqualityComparer.Instance);
    }

    private static BindingFlags GetBindingFlags()
    {
        return BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    }

    private static bool IsZero(FieldInfo field)
    {
        var value = field.GetValue(null);
        Assert.NotNull(value);

        return (int)value == 0;
    }

    private static bool IsNonZero(FieldInfo field)
    {
        var value = field.GetValue(null);
        Assert.NotNull(value);

        return (int)value != 0;
    }

    private sealed class FieldInfoValueEqualityComparer : IEqualityComparer<FieldInfo>
    {
        public static FieldInfoValueEqualityComparer Instance { get; } = new FieldInfoValueEqualityComparer();

        private FieldInfoValueEqualityComparer() { }

        public bool Equals(FieldInfo? x, FieldInfo? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            var left = x.GetValue(null);
            var right = y.GetValue(null);

            if (left is int first && right is int second)
            {
                return first == second;
            }

            return left == right;
        }

        public int GetHashCode(FieldInfo obj)
        {
            var value = obj.GetValue(null);
            return value?.GetHashCode() ?? Int32.MinValue;
        }
    }
}
