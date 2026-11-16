using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rizzy.Htmx;
using Rizzy.Utility;

namespace Rizzy;

public static class HttpContextExtensions
{
    /// <summary>
    /// Retrieves the nonce values for the current HTTP request. New nonce values are generated and cached if not already present.
    /// The recommended way to generate nonce values is to utilize an IRizzyNonceProvider service implementation directly that can
    /// be injected into a Controller, Component, View, etc.
    /// </summary>
    /// <returns>An instance of <see cref="string"/> containing nonce value.</returns>
    public static string GetNonce(this HttpContext context)
    {
        var provider = context.RequestServices.GetRequiredService<IRizzyNonceProvider>();

        return provider.GetNonce();
    }

    /// <summary>
    /// Returns the <c>id</c> of the form control for <paramref name="field"/>. When no control has
    /// registered the field yet, reserves a new id that the Rizzy input for the field adopts when it
    /// renders (if it has no explicit <c>Id</c>). This lets a label that renders before its input
    /// point at it with <c>for</c>.
    /// </summary>
    /// <param name="context">The current <see cref="HttpContext"/>.</param>
    /// <param name="editContext">The form's <see cref="EditContext"/>.</param>
    /// <param name="field">The field the control edits.</param>
    /// <returns>The registered or reserved id.</returns>
    public static string GetOrReserveFieldId(this HttpContext context, EditContext editContext, FieldIdentifier field)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(editContext);

        var mapping = context.GetOrAddFieldMapping(editContext);
        if (mapping.TryGetValue(field, out var existing) && !string.IsNullOrEmpty(existing.Id))
            return existing.Id;

        // A reservation has an id but no field name; the input fills in the name when it registers.
        var id = IdGenerator.UniqueId(field.FieldName);
        mapping[field] = new RzFormFieldMap { Id = id };
        return id;
    }
}