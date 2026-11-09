using Microsoft.AspNetCore.Components.Forms;

namespace Rizzy.Tests.Regression;

/// <summary>MVC model state keys map to the fields Blazor's validation components use.</summary>
public class ModelStateKeyTests
{
    private sealed class Address { public string? City { get; set; } }
    private sealed class Person
    {
        public string? Name { get; set; }
        public Address Address { get; set; } = new();
    }

    [Theory]
    [InlineData("Name")]
    [InlineData("Person.Name")]
    [InlineData("person.Name")]
    public void Top_level_property_with_or_without_prefix(string key)
    {
        var person = new Person();
        var field = RzModelStateProvider.ResolveField(new EditContext(person), key);
        field.Should().Be(new FieldIdentifier(person, nameof(Person.Name)));
    }

    [Theory]
    [InlineData("Address.City")]
    [InlineData("Person.Address.City")]
    public void Nested_property_resolves_to_the_owning_object(string key)
    {
        var person = new Person();
        var field = RzModelStateProvider.ResolveField(new EditContext(person), key);
        field.Should().Be(new FieldIdentifier(person.Address, nameof(Address.City)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Items[0].Name")]
    public void Unresolvable_keys_fall_back_to_the_raw_key(string key)
    {
        var person = new Person();
        var field = RzModelStateProvider.ResolveField(new EditContext(person), key);
        field.Should().Be(new FieldIdentifier(person, key));
    }
}
