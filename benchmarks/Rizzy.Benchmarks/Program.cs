using BenchmarkDotNet.Running;

// dotnet run -c Release --project benchmarks/Rizzy.Benchmarks -- --filter '*'
// Pass "--smoke" to run every benchmark once (a quick check that they still work).
if (args.Contains("--smoke"))
{
    var bench = new Rizzy.Benchmarks.RenderingBenchmarks();
    await bench.Setup();
    try
    {
        Console.WriteLine($"plain:   {await bench.PlainRazorComponentResult()} bytes");
        Console.WriteLine($"view:    {await bench.ViewFullPage()} bytes");
        Console.WriteLine($"htmx:    {await bench.ViewHtmxRequest()} bytes");
        Console.WriteLine($"partial: {await bench.PartialViewHtmxRequest()} bytes");
        Console.WriteLine($"post:    {await bench.PartialViewFormPost()} bytes");
    }
    finally
    {
        await bench.Cleanup();
    }
    return;
}

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
