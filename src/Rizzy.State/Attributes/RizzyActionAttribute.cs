namespace Rizzy.State.Attributes;

/// <summary>
/// Marks a public method on a Rizzy view-model or stateful component as callable
/// from client-side events (e.g., via the x-rz-on directive).
/// Only methods marked with this attribute can be invoked through Rizzy's event handling mechanism.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RizzyActionAttribute : Attribute
{
}