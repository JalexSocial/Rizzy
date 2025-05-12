namespace Rizzy.State.Attributes;

/// <summary>
/// Indicates that a property on a Rizzy view-model or stateful component
/// should be included in the persisted state token.
/// Only properties marked with this attribute will be serialized and deserialized.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RizzyStateAttribute : Attribute
{
}