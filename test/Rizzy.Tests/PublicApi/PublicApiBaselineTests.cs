using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Rizzy.Htmx;

namespace Rizzy.Tests.PublicApi;

/// <summary>
/// Public API baselines for Rizzy and Rizzy.Htmx (RIZZY-030, RM-C4).
/// </summary>
/// <remarks>
/// Any change to the public surface fails this test and shows the difference. If the change is
/// intended, regenerate the baselines and commit them with the change so reviewers see the API diff:
/// <code>
/// RIZZY_UPDATE_API_BASELINE=1 dotnet test test/Rizzy.Tests --filter PublicApiBaselineTests
/// </code>
/// </remarks>
public class PublicApiBaselineTests
{
    public static TheoryData<string> Assemblies => new() { "Rizzy", "Rizzy.Htmx" };

    [Theory]
    [MemberData(nameof(Assemblies))]
    public void Public_api_matches_the_baseline(string assemblyName)
    {
        var assembly = assemblyName == "Rizzy" ? typeof(IRizzyService).Assembly : typeof(HtmxConfig).Assembly;
        var actual = Describe(assembly);
        var path = Path.Combine(BaselineDirectory(), assemblyName + ".txt");

        if (Environment.GetEnvironmentVariable("RIZZY_UPDATE_API_BASELINE") == "1")
        {
            File.WriteAllText(path, actual);
            return;
        }

        File.Exists(path).Should().BeTrue($"the baseline {path} must exist; regenerate it with RIZZY_UPDATE_API_BASELINE=1");
        var expected = File.ReadAllText(path).ReplaceLineEndings("\n");

        var added = actual.Split('\n').Except(expected.Split('\n')).Where(l => l.Length > 0).ToList();
        var removed = expected.Split('\n').Except(actual.Split('\n')).Where(l => l.Length > 0).ToList();

        (added.Count + removed.Count).Should().Be(0,
            $"the public API of {assemblyName} changed.\nAdded:\n  {string.Join("\n  ", added)}\nRemoved:\n  {string.Join("\n  ", removed)}\n" +
            "If intended, regenerate the baseline with RIZZY_UPDATE_API_BASELINE=1 and commit it.");
    }

    private static string BaselineDirectory([CallerFilePath] string sourceFile = "") => Path.GetDirectoryName(sourceFile)!;

    private static string Describe(Assembly assembly)
    {
        var lines = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var type in assembly.GetExportedTypes())
        {
            if (type.IsDefined(typeof(CompilerGeneratedAttribute), false))
                continue;

            var typeName = Name(type);
            lines.Add($"{Kind(type)} {typeName}{BaseList(type)}");

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            foreach (var member in type.GetMembers(flags))
            {
                var line = member switch
                {
                    ConstructorInfo c => $"{typeName}.ctor({Parameters(c)})",
                    MethodInfo m when !m.IsSpecialName || m.Name.StartsWith("op_", StringComparison.Ordinal)
                        => $"{typeName}.{(m.IsStatic ? "static " : "")}{m.Name}{Generic(m)}({Parameters(m)}) -> {Name(m.ReturnType)}",
                    PropertyInfo p => $"{typeName}.{p.Name} : {Name(p.PropertyType)} {{{(p.GetMethod?.IsPublic == true ? " get;" : "")}{(p.SetMethod?.IsPublic == true ? " set;" : "")} }}",
                    FieldInfo f when !type.IsEnum => $"{typeName}.{(f.IsLiteral ? "const " : f.IsStatic ? "static " : "")}{f.Name} : {Name(f.FieldType)}",
                    FieldInfo f when type.IsEnum && f.IsStatic => $"{typeName}.{f.Name} = {Convert.ToInt64(f.GetRawConstantValue(), System.Globalization.CultureInfo.InvariantCulture)}",
                    EventInfo e => $"{typeName}.event {e.Name} : {Name(e.EventHandlerType!)}",
                    _ => null,
                };

                if (line is not null)
                    lines.Add(line);
            }
        }

        return string.Join("\n", lines) + "\n";
    }

    private static string Kind(Type t) =>
        t.IsEnum ? "enum" : t.IsInterface ? "interface" : t.IsValueType ? "struct" :
        t.IsAbstract && t.IsSealed ? "static class" : t.IsSealed ? "sealed class" : t.IsAbstract ? "abstract class" : "class";

    private static string BaseList(Type t)
    {
        var bases = new List<string>();
        if (t.BaseType is { } b && b != typeof(object) && b != typeof(ValueType) && b != typeof(Enum))
            bases.Add(Name(b));
        bases.AddRange(t.GetInterfaces().Where(i => i.IsPublic || i.IsNestedPublic).Select(Name).Order(StringComparer.Ordinal));
        return bases.Count == 0 ? "" : " : " + string.Join(", ", bases);
    }

    private static string Generic(MethodInfo m) =>
        m.IsGenericMethodDefinition ? "<" + string.Join(", ", m.GetGenericArguments().Select(a => a.Name)) + ">" : "";

    private static string Parameters(MethodBase m) =>
        string.Join(", ", m.GetParameters().Select(p =>
            $"{(p.IsOut ? "out " : p.ParameterType.IsByRef ? "ref " : "")}{(p.IsDefined(typeof(ParamArrayAttribute)) ? "params " : "")}{Name(p.ParameterType)} {p.Name}{(p.HasDefaultValue ? " = " + (p.DefaultValue?.ToString() ?? "null") : "")}"));

    private static string Name(Type t)
    {
        if (t.IsByRef) return Name(t.GetElementType()!);
        if (t.IsArray) return Name(t.GetElementType()!) + "[]";
        if (t.IsGenericParameter) return t.Name;

        var name = (t.IsNested ? Name(t.DeclaringType!) + "." : t.Namespace + ".") + t.Name;
        var tick = name.IndexOf('`');
        if (tick >= 0) name = name[..tick];

        return t.IsGenericType
            ? name + "<" + string.Join(", ", t.GetGenericArguments().Select(Name)) + ">"
            : name;
    }
}
