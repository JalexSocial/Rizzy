
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Rizzy.Internal;

namespace Rizzy;

    /// <summary>
    /// A fluent builder for creating a dictionary of parameters to pass to a Blazor component.
    /// This builder uses runtime reflection with caching to determine available parameters.
    /// </summary>
    /// <typeparam name="TComponent">The type of the component for which parameters are being built.
    /// Must implement <see cref="IComponent"/>.</typeparam>
    public class RizzyComponentParameterBuilder<TComponent> where TComponent : IComponent
    {
        private readonly Dictionary<string, object?> _parameters = new(StringComparer.Ordinal);
        private static readonly Type ComponentTypeStatic = typeof(TComponent);
        private readonly IReadOnlyList<ParameterMetadata> _componentParameterMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="RizzyComponentParameterBuilder{TComponent}"/> class.
        /// Retrieves and caches parameter metadata for <typeparamref name="TComponent"/>.
        /// </summary>
        public RizzyComponentParameterBuilder()
        {
            _componentParameterMetadata = ComponentMetadataCache.GetParameterMetadata(ComponentTypeStatic);
        }

        /// <summary>
        /// Adds or updates a parameter in the collection.
        /// </summary>
        /// <typeparam name="TValue">The type of the parameter's value. This is inferred from the
        /// <paramref name="parameterSelector"/> and the <paramref name="value"/>.</typeparam>
        /// <param name="parameterSelector">An expression identifying the component parameter property (e.g., <c>p => p.MyParameter</c>).</param>
        /// <param name="value">The value to assign to the parameter.</param>
        /// <returns>The same <see cref="RizzyComponentParameterBuilder{TComponent}"/> instance for fluent chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameterSelector"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// Thrown if the <paramref name="parameterSelector"/> does not point to a valid public instance property,
        /// if the selected property is not decorated with <see cref="ParameterAttribute"/>,
        /// or if the type of the provided <paramref name="value"/> is incompatible with the parameter's declared type.
        /// </exception>
        public RizzyComponentParameterBuilder<TComponent> Add<TValue>(
            Expression<Func<TComponent, TValue>> parameterSelector,
            TValue value)
        {
            ArgumentNullException.ThrowIfNull(parameterSelector);

            if (parameterSelector.Body is not MemberExpression memberExpression ||
                memberExpression.Member is not PropertyInfo selectedPropertyInfo)
            {
                throw new ArgumentException(
                    "The parameter selector must be a simple property access expression (e.g., p => p.MyProperty).",
                    nameof(parameterSelector));
            }

            string propertyName = selectedPropertyInfo.Name;

            var parameterMeta = _componentParameterMetadata.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.Ordinal));

            if (parameterMeta is null)
            {
                throw new ArgumentException($"Property '{propertyName}' is not a recognized [Parameter] on component '{ComponentTypeStatic.FullName}'. Ensure the property is public, on the component or a qualifying base class, and decorated with [Parameter].", nameof(parameterSelector));
            }

            ValidateAndAssignValue(propertyName, parameterMeta.ParameterType, value);

            _parameters[propertyName] = value;
            return this;
        }

        private static void ValidateAndAssignValue<TValue>(string propertyName, Type expectedType, TValue value)
        {
            if (value is null)
            {
                // Allow null if expectedType is a reference type or a Nullable<ValueType>
                if (expectedType.IsValueType && Nullable.GetUnderlyingType(expectedType) is null)
                {
                    throw new ArgumentException($"Cannot assign null to non-nullable value type parameter '{propertyName}' of type '{expectedType.FullName}'.");
                }
            }
            else
            {
                Type actualValueType = value.GetType();
                if (!expectedType.IsAssignableFrom(actualValueType))
                {
                    // Allow assigning T to Nullable<T> if T is the underlying type of Nullable<T>
                    Type? underlyingExpectedType = Nullable.GetUnderlyingType(expectedType);
                    if (underlyingExpectedType is null || underlyingExpectedType != actualValueType)
                    {
                        throw new ArgumentException($"Type mismatch for parameter '{propertyName}'. Component expects an assignable type to '{expectedType.FullName}' but the provided value is of type '{actualValueType.FullName}'.");
                    }
                }
            }
        }

        /// <summary>
        /// Adds a <c>ChildContent</c> <see cref="RenderFragment"/> parameter.
        /// </summary>
        /// <param name="childContentFragment">The <see cref="RenderFragment"/> for the <c>ChildContent</c>.</param>
        /// <returns>The same <see cref="RizzyComponentParameterBuilder{TComponent}"/> instance for fluent chaining.</returns>
        /// <exception cref="InvalidOperationException">If the component <typeparamref name="TComponent"/> does not have a
        /// public instance [Parameter] property named "ChildContent" of type <see cref="RenderFragment"/>.</exception>
        public RizzyComponentParameterBuilder<TComponent> AddChildContent(RenderFragment? childContentFragment)
        {
            const string childContentName = "ChildContent"; // Standard Blazor parameter name
            var childContentMeta = _componentParameterMetadata.FirstOrDefault(p => p.Name.Equals(childContentName, StringComparison.Ordinal));

            if (childContentMeta is null)
                throw new InvalidOperationException($"Component '{ComponentTypeStatic.FullName}' does not have a [Parameter] named '{childContentName}'.");

            if (childContentMeta.ParameterType != typeof(RenderFragment))
                throw new InvalidOperationException($"The '{childContentName}' parameter on component '{ComponentTypeStatic.FullName}' is not of type '{nameof(RenderFragment)}'. Its actual type is '{childContentMeta.ParameterType.FullName}'.");

            _parameters[childContentName] = childContentFragment;
            return this;
        }

        /// <summary>
        /// Adds a <c>ChildContent</c> <see cref="RenderFragment"/> parameter from a markup string.
        /// </summary>
        /// <param name="markup">The markup string for the <c>ChildContent</c>. If null or empty, an empty ChildContent will be added.</param>
        /// <returns>The same <see cref="RizzyComponentParameterBuilder{TComponent}"/> instance for fluent chaining.</returns>
        public RizzyComponentParameterBuilder<TComponent> AddChildContent(string? markup) =>
            AddChildContent(builder => builder.AddMarkupContent(0, markup));

        /// <summary>
        /// Adds an <see cref="EventCallback"/> parameter created from the provided <see cref="Action"/>.
        /// </summary>
        /// <param name="selector">An expression identifying the <see cref="EventCallback"/> parameter property.</param>
        /// <param name="callback">The <see cref="Action"/> to invoke when the callback is triggered. If null, a no-op callback is assigned.</param>
        /// <returns>The same <see cref="RizzyComponentParameterBuilder{TComponent}"/> instance for fluent chaining.</returns>
        public RizzyComponentParameterBuilder<TComponent> Add(Expression<Func<TComponent, EventCallback>> selector, Action? callback)
        {
            return Add(selector, EventCallback.Factory.Create(callback?.Target!, callback ?? (() => { })));
        }

        /// <summary>
        /// Adds an <see cref="EventCallback"/> parameter created from the provided asynchronous <see cref="Func{Task}"/>.
        /// </summary>
        /// <param name="selector">An expression identifying the <see cref="EventCallback"/> parameter property.</param>
        /// <param name="callback">The asynchronous <see cref="Func{Task}"/> to invoke. If null, a no-op callback is assigned.</param>
        /// <returns>The same <see cref="RizzyComponentParameterBuilder{TComponent}"/> instance for fluent chaining.</returns>
        public RizzyComponentParameterBuilder<TComponent> Add(Expression<Func<TComponent, EventCallback>> selector, Func<Task>? callback)
        {
            return Add(selector, EventCallback.Factory.Create(callback?.Target!, callback ?? (() => Task.CompletedTask)));
        }

        /// <summary>
        /// Adds a generic <see cref="EventCallback{TArg}"/> parameter created from the provided <see cref="Action{TArg}"/>.
        /// </summary>
        /// <typeparam name="TArg">The type of the argument for the event callback.</typeparam>
        /// <param name="selector">An expression identifying the <see cref="EventCallback{TArg}"/> parameter property.</param>
        /// <param name="callback">The <see cref="Action{TArg}"/> to invoke. If null, a no-op callback is assigned.</param>
        /// <returns>The same <see cref="RizzyComponentParameterBuilder{TComponent}"/> instance for fluent chaining.</returns>
        public RizzyComponentParameterBuilder<TComponent> Add<TArg>(Expression<Func<TComponent, EventCallback<TArg>>> selector, Action<TArg>? callback)
        {
            return Add(selector, EventCallback.Factory.Create<TArg>(callback?.Target!, callback ?? ((TArg _) => { })));
        }

        /// <summary>
        /// Adds a generic <see cref="EventCallback{TArg}"/> parameter created from the provided asynchronous <see cref="Func{TArg, Task}"/>.
        /// </summary>
        /// <typeparam name="TArg">The type of the argument for the event callback.</typeparam>
        /// <param name="selector">An expression identifying the <see cref="EventCallback{TArg}"/> parameter property.</param>
        /// <param name="callback">The asynchronous <see cref="Func{TArg, Task}"/> to invoke. If null, a no-op callback is assigned.</param>
        /// <returns>The same <see cref="RizzyComponentParameterBuilder{TComponent}"/> instance for fluent chaining.</returns>
        public RizzyComponentParameterBuilder<TComponent> Add<TArg>(Expression<Func<TComponent, EventCallback<TArg>>> selector, Func<TArg, Task>? callback)
        {
            return Add(selector, EventCallback.Factory.Create<TArg>(callback?.Target!, callback ?? ((TArg _) => Task.CompletedTask)));
        }

        /// <summary>
        /// Builds the dictionary of parameters that have been added.
        /// This method also validates that all parameters marked with <see cref="EditorRequiredAttribute"/> have been provided.
        /// </summary>
        /// <returns>A new <see cref="Dictionary{String, Object}"/> containing the collected parameters.
        /// Returns an empty dictionary if no parameters were added.</returns>
        /// <exception cref="ArgumentException">Thrown if an <see cref="EditorRequiredAttribute"/> parameter was not provided.</exception>
        public Dictionary<string, object?> Build()
        {
            foreach (var meta in _componentParameterMetadata)
            {
                if (meta.IsEditorRequired && !_parameters.ContainsKey(meta.Name))
                {
                    throw new ArgumentException($"Required parameter '{meta.Name}' for component '{ComponentTypeStatic.FullName}' was not provided. Ensure all parameters marked with [EditorRequired] are set using the builder.");
                }
            }
            return new Dictionary<string, object?>(_parameters, StringComparer.Ordinal);
        }
    }