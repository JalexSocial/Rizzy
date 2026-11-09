using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rizzy.SourceGenerators;

namespace Rizzy.Tests.SourceGenerators;

/// <summary>The [RizzyParameterize] source generator shipped to consumers (RIZZY-022, RM-C2).</summary>
public class RizzyParameterGeneratorTests
{
    private static readonly MetadataReference[] References = BuildReferences();

    private static MetadataReference[] BuildReferences()
    {
        var trusted = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator);
        return trusted
            .Append(typeof(ComponentBase).Assembly.Location)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToArray();
    }

    private static CSharpCompilation Compile(params string[] sources)
        => CSharpCompilation.Create(
            "Consumer",
            sources.Select((s, i) => CSharpSyntaxTree.ParseText(s, path: $"File{i}.cs")),
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

    private static (GeneratorDriver Driver, Compilation Output, IReadOnlyList<Diagnostic> Diagnostics) Run(CSharpCompilation compilation)
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new RizzyParameterGenerator().AsSourceGenerator()],
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);
        return (driver, output, diagnostics);
    }

    private const string Greeting = """
        using Microsoft.AspNetCore.Components;
        namespace App;

        [Rizzy.RizzyParameterize]
        public partial class Greeting : ComponentBase
        {
            [Parameter, EditorRequired] public string Name { get; set; } = "";
            [Parameter] public int Times { get; set; }
            [Parameter] public string? Title { get; set; }
            public string NotAParameter { get; set; } = "";
        }
        """;

    [Fact]
    public void Generates_Params_with_required_and_optional_arguments()
    {
        var (_, output, diagnostics) = Run(Compile(Greeting, """
            namespace App;
            public static class Use
            {
                public static object Call() => Greeting.Params(name: "Ada", times: 3);
            }
            """));

        diagnostics.Should().BeEmpty();
        output.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).Should().BeEmpty();

        var generated = output.SyntaxTrees.Single(t => t.FilePath.EndsWith(".RizzyParams.g.cs", StringComparison.Ordinal)).ToString();
        generated.Should().Contain("Params(string name, global::System.Nullable<int> times = null, string? title = null)");
        generated.Should().NotContain("NotAParameter");
    }

    [Fact]
    public void Non_partial_component_reports_RZ0001()
    {
        var (_, _, diagnostics) = Run(Compile("""
            using Microsoft.AspNetCore.Components;
            [Rizzy.RizzyParameterize]
            public class NotPartial : ComponentBase { [Parameter] public string? Value { get; set; } }
            """));

        diagnostics.Should().ContainSingle(d => d.Id == "RZ0001" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Nested_component_reports_RZ0002()
    {
        var (_, _, diagnostics) = Run(Compile("""
            using Microsoft.AspNetCore.Components;
            public partial class Outer
            {
                [Rizzy.RizzyParameterize]
                public partial class Inner : ComponentBase { [Parameter] public string? Value { get; set; } }
            }
            """));

        diagnostics.Should().ContainSingle(d => d.Id == "RZ0002");
    }

    [Fact]
    public void Unrelated_edits_reuse_cached_output()
    {
        var other = CSharpSyntaxTree.ParseText("namespace App; public static class Other { public const int X = 1; }", path: "Other.cs");
        var compilation = Compile(Greeting).AddSyntaxTrees(other);
        var (driver, _, _) = Run(compilation);

        var edited = compilation.ReplaceSyntaxTree(other,
            CSharpSyntaxTree.ParseText("namespace App; public static class Other { public const int X = 2; }", path: "Other.cs"));
        driver = driver.RunGenerators(edited);

        var steps = driver.GetRunResult().Results.Single().TrackedOutputSteps;
        var reasons = steps.SelectMany(s => s.Value).SelectMany(s => s.Outputs).Select(o => o.Reason).ToList();

        reasons.Should().NotBeEmpty().And.OnlyContain(r => r == IncrementalStepRunReason.Cached || r == IncrementalStepRunReason.Unchanged);
    }

    [Fact]
    public void Attribute_is_emitted_into_the_consumer_compilation()
    {
        var (_, output, _) = Run(Compile("public class Nothing { }"));

        output.GetTypeByMetadataName("Rizzy.RizzyParameterizeAttribute").Should().NotBeNull();
        output.GetTypeByMetadataName("Rizzy.RizzyParameterizeAttribute")!.DeclaredAccessibility.Should().Be(Accessibility.Internal);
    }
}
