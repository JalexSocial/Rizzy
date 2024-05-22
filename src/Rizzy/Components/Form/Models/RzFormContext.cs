using Microsoft.AspNetCore.Components.Forms;

namespace Rizzy.Components.Form.Models;

/// <summary>
/// Represents a form context with a strongly typed model. This record encapsulates form identification, naming, and state management through an EditContext.
/// </summary>
public sealed record RzFormContext
{
    /// <summary>
    /// Initializes a new instance of the RzFormContext class with the specified form name and model, generating an identifier automatically.
    /// </summary>
    /// <param name="formName">The name of the form.</param>
    /// <param name="model">The model associated with the form.</param>
    public RzFormContext(string formName, object model) : this(CreateId(), formName, string.Empty, model)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RzFormContext class with the specified form name and model, generating an identifier automatically.
    /// </summary>
    /// <param name="formName">The name of the form.</param>
    /// <param name="formUrl"></param>
    /// <param name="model">The model associated with the form.</param>
    public RzFormContext(string formName, string formUrl, object model) : this(CreateId(), formName, formUrl, model)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RzFormContext class with the specified identifier, form name, and model.
    /// </summary>
    /// <param name="id">The unique identifier for the form context.</param>
    /// <param name="formName">The name of the form.</param>
    /// <param name="formUrl"></param>
    /// <param name="model">The model associated with the form.</param>
    public RzFormContext(string id, string formName, string formUrl, object model)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(formName, nameof(formName));
        ArgumentNullException.ThrowIfNull(formUrl, nameof(formUrl));
        ArgumentNullException.ThrowIfNull(model, nameof(model));

        Id = id;
        FormName = formName;
        FormUrl = formUrl;
        EditContext = new EditContext(model)
        {
            ShouldUseFieldIdentifiers = true
        };
    }

    /// <summary>
    /// Initializes a new instance of the RzFormContext class with the specified identifier, form name, and existing EditContext.
    /// This constructor allows for reusing an existing EditContext with a new form context.
    /// </summary>
    /// <param name="id">The unique identifier for the form context.</param>
    /// <param name="formName">The name of the form.</param>
    /// <param name="formUrl">Callback url for the form</param>
    /// <param name="context">The EditContext to be associated with the form.</param>
    public RzFormContext(string id, string formName, string formUrl, EditContext context)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(formName, nameof(formName));
        ArgumentNullException.ThrowIfNull(formUrl, nameof(formUrl));
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        Id = id;
        FormName = formName;
        FormUrl = formUrl;
        EditContext = context;
        EditContext.ShouldUseFieldIdentifiers = true;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the form context.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the name of the form.
    /// </summary>
    public string FormName { get; }

    /// <summary>
    /// Gets the action url for the form. 
    /// </summary>
    public string FormUrl { get; }

    /// <summary>
    /// Gets the EditContext associated with the form.
    /// </summary>
    public EditContext EditContext { get; }

    /// <summary>
    /// Retrieves the current data strongly-typed data model for EditContext. Note that EditContext must be set with a strongly-typed
    /// model for the form model to be accessible.  This is essentially a shortcut for EditContext.Model which does the typecasting for
    /// you.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public TModel Model<TModel>()
    {
        if (EditContext?.Model is null)
            throw new NullReferenceException("EditContext was not configured properly on this context");

        if (EditContext.Model is not TModel model)
            throw new InvalidOperationException($"FormModel is not of type '{typeof(TModel).Name}'. EditContext may have not been configured properly.");

        return model;
    }

    /// <summary>
    /// Generates a unique identifier for a form context.
    /// </summary>
    /// <returns>A unique identifier string.</returns>
    private static string CreateId()
    {
        return "frm" + Guid.NewGuid().ToString("N");
    }
}
