# Rizzy.Benchmarks

End-to-end rendering benchmarks (roadmap RM-D2, finding RIZZY-032). Each benchmark sends one
request through a real ASP.NET Core pipeline hosted in memory (TestServer) and reads the whole
response. The timings include routing, layout selection, the htmx config meta tag, antiforgery
tokens and HTML rendering.

| Benchmark | What it measures |
|---|---|
| `PlainRazorComponentResult` | Baseline: ASP.NET Core's `RazorComponentResult` without Rizzy |
| `View<T>, full page` | A normal browser request: root layout, `<head>` with the htmx config and antiforgery token, main layout |
| `View<T>, htmx request` | The same endpoint with `HX-Request: true` (no root layout) |
| `PartialView<T>, htmx request` | A fragment without any layout |
| `PartialView<T>, form POST` | A form post bound through `[SupplyParameterFromForm]` (Rizzy's form-data hand-off) |

The component in each case is a 20-row table.

## Running

```bash
dotnet run -c Release --project benchmarks/Rizzy.Benchmarks -- --filter '*'
```

For a quick check that every benchmark still runs, without measuring anything:

```bash
dotnet run -c Release --project benchmarks/Rizzy.Benchmarks -- --smoke
```

CI builds this project as part of `src/Rizzy.sln` but does not run it.

## Baseline (2026-11-09)

These numbers come from a `--job short` run on a Windows 11 developer machine with .NET 10.0. The
short job has wide error margins, so treat them as rough. For a figure to quote, run the default
job on a quiet machine.

| Method | Mean | Allocated | Alloc ratio |
|---|---:|---:|---:|
| PlainRazorComponentResult | 208.5 µs | 42.25 KB | 1.00 |
| View&lt;T&gt;, full page | 453.7 µs | 68.59 KB | 1.62 |
| View&lt;T&gt;, htmx request | 213.5 µs | 55.82 KB | 1.32 |
| PartialView&lt;T&gt;, htmx request | 197.8 µs | 50.46 KB | 1.19 |
| PartialView&lt;T&gt;, form POST | 232.2 µs | 42.92 KB | 1.02 |

A full page costs about twice the baseline, mostly because of the extra layouts, the head outlet
and antiforgery token generation. htmx requests skip the root layout and cost about the same as
the baseline.
