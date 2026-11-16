using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Rizzy.Htmx;
using Rizzy.Utility;

namespace Rizzy;

/// <summary>
/// Registers a Rizzy input in the per-request field mapping used by labels and validation messages.
/// </summary>
internal static class FormFieldRegistration
{
    /// <summary>
    /// Resolves the input's id and registers it for <paramref name="field"/>.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>An explicit <paramref name="id"/> is always used.</item>
    /// <item>Without one, the input adopts an id reserved by a component that rendered earlier (see
    /// <see cref="HttpContextExtensions.GetOrReserveFieldId"/>), or generates one.</item>
    /// <item>The first input to register a field keeps the mapping; a reservation is completed with
    /// the input's name, and with its explicit id if it has one.</item>
    /// </list>
    /// </remarks>
    /// <returns>The id the input renders, and the mapping it registered in (for removal on dispose).</returns>
    internal static string Register(
        HttpContext? httpContext,
        EditContext editContext,
        FieldIdentifier field,
        string? name,
        string? id,
        string fallbackPrefix,
        out IDictionary<FieldIdentifier, RzFormFieldMap>? mapping)
    {
        mapping = httpContext?.GetOrAddFieldMapping(editContext);

        if (mapping != null && mapping.TryGetValue(field, out var existing) && string.IsNullOrEmpty(existing.FieldName))
        {
            // A reservation (id only): complete it with this input.
            if (string.IsNullOrEmpty(id))
                id = string.IsNullOrEmpty(existing.Id) ? IdGenerator.UniqueId(name ?? fallbackPrefix) : existing.Id;

            existing.Id = id;
            existing.FieldName = name ?? string.Empty;
            return id;
        }

        if (string.IsNullOrEmpty(id))
            id = IdGenerator.UniqueId(name ?? fallbackPrefix);

        if (mapping != null && !mapping.ContainsKey(field))
            mapping[field] = new RzFormFieldMap { FieldName = name ?? string.Empty, Id = id };

        return id;
    }
}
