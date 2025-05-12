using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Rizzy.State.Serialization; // Add this
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy.State.StateHandling;

public sealed class RizzyStateProtector : IRizzyStateProtector
{
    private readonly IDataProtector _protector;
    private readonly ILogger<RizzyStateProtector> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptionsForEnvelope; // Renamed for clarity
    private readonly IRizzyViewModelSerializer _viewModelSerializer; // Added

    private record struct StateEnvelope<TModelPayload>(
        [property: JsonPropertyName("ver")] ulong Version,
        [property: JsonPropertyName("model")] TModelPayload Model
    );

    public RizzyStateProtector(
        IDataProtectionProvider dataProtectionProvider,
        ILogger<RizzyStateProtector> logger,
        IRizzyViewModelSerializer viewModelSerializer) // Added
    {
        ArgumentNullException.ThrowIfNull(dataProtectionProvider);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(viewModelSerializer); // Added

        _protector = dataProtectionProvider.CreateProtector("Rizzy.StateFeatures.StateHandling.v1"); // Consistent naming from Sprint 0
        _logger = logger;
        _viewModelSerializer = viewModelSerializer; // Added
        _jsonSerializerOptionsForEnvelope = new JsonSerializerOptions // Options specifically for the envelope
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public string Protect<TViewModel>(TViewModel model, ulong version) where TViewModel : class
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            // Use the serializer to get only [RizzyState] properties
            IDictionary<string, object?> persistentDict = _viewModelSerializer.ExtractPersistentState(model);

            var envelope = new StateEnvelope<IDictionary<string, object?>>(version, persistentDict);
            byte[] jsonBytes = JsonSerializer.SerializeToUtf8Bytes(envelope, _jsonSerializerOptionsForEnvelope);

            using var outputStream = new MemoryStream();
            using (var brotliStream = new BrotliStream(outputStream, CompressionLevel.Fastest, leaveOpen: true))
            {
                brotliStream.Write(jsonBytes, 0, jsonBytes.Length);
            }
            byte[] compressedBytes = outputStream.ToArray();
            byte[] protectedBytes = _protector.Protect(compressedBytes);
            return WebEncoders.Base64UrlEncode(protectedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error protecting Rizzy state for model type {ViewModelType}.", typeof(TViewModel).FullName);
            throw new StateProtectionException($"Failed to protect state for model type {typeof(TViewModel).FullName}.", ex);
        }
    }

    public bool TryUnprotect<TViewModel>(string token, [NotNullWhen(true)] out TViewModel? model, out ulong version) where TViewModel : class
    {
        model = null;
        version = 0;

        if (string.IsNullOrEmpty(token))
        {
            _logger.LogTrace("Attempted to unprotect an empty or null Rizzy state token.");
            return false;
        }

        try
        {
            byte[] protectedBytes = WebEncoders.Base64UrlDecode(token);
            byte[] compressedBytes = _protector.Unprotect(protectedBytes);

            using var inputStream = new MemoryStream(compressedBytes);
            using var outputStream = new MemoryStream();
            using (var brotliStream = new BrotliStream(inputStream, CompressionMode.Decompress, leaveOpen: true))
            {
                brotliStream.CopyTo(outputStream);
            }
            outputStream.Position = 0;

            // Deserialize the envelope containing the version and the model as a dictionary of JsonElements
            var envelope = JsonSerializer.Deserialize<StateEnvelope<Dictionary<string, JsonElement>>>(outputStream, _jsonSerializerOptionsForEnvelope);
            
            if (typeof(TViewModel).GetConstructor(Type.EmptyTypes) == null && !typeof(TViewModel).IsAbstract)
            {
                _logger.LogError("ViewModel type {ViewModelType} does not have a public parameterless constructor, which is required for deserialization by RizzyStateProtector.", typeof(TViewModel).FullName);
                return false;
            }
            
            model = Activator.CreateInstance<TViewModel>();

            // Use the serializer to populate the model instance with [RizzyState] properties
            _viewModelSerializer.PopulateViewModel(model, envelope.Model);
            
            version = envelope.Version;
            return model != null;
        }
        // ... (existing catch blocks from Sprint 0, ensuring logger and exception types are consistent) ...
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize Rizzy state token for model type {ViewModelType}. Token (first 50 chars): '{TokenStart}'.", typeof(TViewModel).FullName, token.Length > 50 ? token.Substring(0, 50) : token);
            return false;
        }
        catch (System.Security.Cryptography.CryptographicException ex)
        {
            _logger.LogWarning(ex, "Failed to unprotect Rizzy state token (cryptographic error). This could be due to a tampered token, key rotation, or misconfiguration.");
            return false;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is NotSupportedException innerEx && innerEx.Message.Contains("parameterless constructor"))
        {
             _logger.LogError(ex, "ViewModel type {ViewModelType} could not be instantiated by Activator.CreateInstance. Ensure it has a public parameterless constructor or handle its creation differently if DI is involved for view models.", typeof(TViewModel).FullName);
             return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while unprotecting Rizzy state token for model type {ViewModelType}. Token (first 50 chars): '{TokenStart}'.", typeof(TViewModel).FullName, token.Length > 50 ? token.Substring(0, 50) : token);
            return false;
        }
    }
}