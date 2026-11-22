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
    private sealed class Attendee
    {
        public string? Name { get; set; }
        public Address Address { get; set; } = new();
    }

    private sealed class Order
    {
        public List<Attendee> Attendees { get; set; } = [new(), new()];
        public Attendee[] Guests { get; set; } = [new()];
        public List<List<Attendee>> Groups { get; set; } = [[new()], [new(), new()]];
        public Dictionary<string, Attendee> ByCode { get; set; } = new() { ["vip"] = new() };
    }

    // SF-044: keys with indexers (a form that edits a list) resolved to the raw key, so a 422's
    // messages never reached the list items' fields.
    [Theory]
    [InlineData("Attendees[1].Name")]
    [InlineData("Order.Attendees[1].Name")]
    [InlineData("model.Attendees[1].Name")]
    public void Indexed_list_item_resolves_to_the_item(string key)
    {
        var order = new Order();
        var field = RzModelStateProvider.ResolveField(new EditContext(order), key);
        field.Should().Be(new FieldIdentifier(order.Attendees[1], nameof(Attendee.Name)));
    }

    [Fact]
    public void Indexed_items_resolve_through_arrays_nested_lists_dictionaries_and_properties()
    {
        var order = new Order();
        var context = new EditContext(order);

        RzModelStateProvider.ResolveField(context, "Guests[0].Name")
            .Should().Be(new FieldIdentifier(order.Guests[0], nameof(Attendee.Name)));
        RzModelStateProvider.ResolveField(context, "Groups[1][1].Name")
            .Should().Be(new FieldIdentifier(order.Groups[1][1], nameof(Attendee.Name)));
        RzModelStateProvider.ResolveField(context, "ByCode[vip].Name")
            .Should().Be(new FieldIdentifier(order.ByCode["vip"], nameof(Attendee.Name)));
        RzModelStateProvider.ResolveField(context, "Model.Attendees[0].Address.City")
            .Should().Be(new FieldIdentifier(order.Attendees[0].Address, nameof(Address.City)));
    }

    [Theory]
    [InlineData("Attendees[5].Name")]
    [InlineData("Attendees[x].Name")]
    [InlineData("Attendees[0]")]
    public void Indexes_the_walk_cannot_follow_fall_back_to_the_raw_key(string key)
    {
        var order = new Order();
        var field = RzModelStateProvider.ResolveField(new EditContext(order), key);
        field.Should().Be(new FieldIdentifier(order, key));
    }
}
