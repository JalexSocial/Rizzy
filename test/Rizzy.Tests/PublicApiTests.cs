using System.Reflection;
using Rizzy.Htmx;

namespace Rizzy.Tests;

public class PublicApiTests
{
    [Fact]
    public void RizzyHtmx_PublicApi_ShouldNotContainObsoleteMembers()
    {
        var obsoleteMembers = typeof(HtmxConfig).Assembly.GetExportedTypes()
            .SelectMany(type => type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(member => member switch
                {
                    MethodBase m => !m.IsSpecialName,
                    PropertyInfo => true,
                    FieldInfo => true,
                    _ => false
                })
                .Where(member => member.GetCustomAttribute<ObsoleteAttribute>() is not null)
                .Where(member => !(member.Name == nameof(ToString) && true))
                .Where(member => member.Name != "GetObjectData")
                .Select(member => $"{type.FullName}.{member.Name}"))
            .OrderBy(name => name)
            .ToList();

        Assert.Empty(obsoleteMembers);
    }
}
