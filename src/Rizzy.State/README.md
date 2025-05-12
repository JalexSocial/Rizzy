# Rizzy.State

Provides stateful component features for the Rizzy framework, enabling server-authoritative state management with client-side interactions using htmx and Alpine.js.

## Features

- Secure, server-protected state tokens.
- Opt-in property persistence using `[RizzyStateAttribute]`.
- Server-side action method invocation using `[RizzyActionAttribute]`.
- (Upcoming) JSON-Patch support for efficient state updates.
- (Upcoming) Optimistic concurrency control.

## Installation

```bash
dotnet add package Rizzy.State
```

## Usage

(Details to be added as features are implemented and documented)

```csharp
// In Program.cs or Startup.cs
builder.Services.AddDataProtection(); // Configure DP key persistence (e.g., Redis, Azure Blob)
builder.Services.TryAddSingleton<IRizzyStateProtector, RizzyStateProtector>();
// ... other Rizzy and Rizzy.State service registrations ...
```

```csharp
// Example ViewModel
using Rizzy.State.Attributes;

public class MyCounterViewModel
{
    [RizzyState]
    public int Count { get; set; }

    [RizzyAction]
    public void Increment() => Count++;
}
```
