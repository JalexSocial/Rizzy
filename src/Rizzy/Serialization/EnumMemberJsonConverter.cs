using System.Collections.Concurrent; // For thread-safe dictionary
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rizzy; // Use your appropriate namespace

/// <summary>
/// A System.Text.Json converter for enum types that prioritizes the <see cref="EnumMemberAttribute"/>
/// for string representation during serialization and deserialization.
/// Falls back to the standard enum member name if the attribute is not present.
/// Also supports deserializing from the underlying numeric value of the enum.
/// Uses caching for improved performance.
/// </summary>
/// <typeparam name="T">The enum type to convert.</typeparam>
internal class EnumMemberJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    /// <summary>
    /// Thread-safe cache mapping enum values to their string representation (either EnumMemberAttribute value or name) for writing.
    /// Keyed by the Enum Type.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<T, string>> WriteCache = new();

    /// <summary>
    /// Thread-safe cache mapping string representations (EnumMemberAttribute value or name) back to enum values for reading.
    /// Keyed by the Enum Type. Uses a case-insensitive or case-sensitive comparer based on JsonSerializerOptions.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, T>> ReadCache = new(
        concurrencyLevel: Environment.ProcessorCount,
        capacity: 1
    );

    /// <summary>
    /// Initializes the read and write caches for a given enum type if they haven't been initialized yet.
    /// Performs reflection to find EnumMemberAttributes and builds the mappings.
    /// </summary>
    /// <param name="enumType">The type of the enum to initialize caches for.</param>
    /// <param name="options">The serializer options, used to determine case sensitivity for reading.</param>
    private static void InitializeCaches(Type enumType, JsonSerializerOptions options)
    {
        // Double-check locking pattern to ensure thread safety during initialization
        if (!ReadCache.ContainsKey(enumType))
        {
            // Determine the correct string comparer based on options
            var stringComparer = options?.PropertyNameCaseInsensitive ?? false
                ? StringComparer.OrdinalIgnoreCase
                : StringComparer.Ordinal;

            // Temporary dictionaries built once
            var writeMap = new Dictionary<T, string>();
            var readMap = new Dictionary<string, T>(stringComparer);

            foreach (T enumValue in Enum.GetValues(enumType).Cast<T>())
            {
                string memberName = enumValue.ToString();
                MemberInfo? member = enumType.GetMember(memberName).FirstOrDefault();
                string effectiveStringValue = memberName; // Default to name

                if (member != null)
                {
                    var attribute = member.GetCustomAttribute<EnumMemberAttribute>(false);
                    if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
                    {
                        effectiveStringValue = attribute.Value;
                        // Add attribute value mapping for reading
                        if (!readMap.TryAdd(attribute.Value, enumValue))
                        {
                           // Console.Error.WriteLine($"Warning: Duplicate EnumMemberAttribute Value '{attribute.Value}' found in enum {enumType.Name}. Reading might be ambiguous.");
                        }
                    }
                }

                // Add enum value -> effective string mapping for writing
                writeMap[enumValue] = effectiveStringValue;

                // Add enum name mapping for reading (always possible, ensure it doesn't overwrite attribute value)
                readMap.TryAdd(memberName, enumValue);
            }

            // Atomically add the constructed dictionaries to the concurrent caches
            WriteCache.TryAdd(enumType, writeMap);
            // Use a temporary dictionary for the read cache with the correct comparer
            var finalReadCache = new Dictionary<string, T>(readMap, stringComparer);
            ReadCache.TryAdd(enumType, finalReadCache);
        }
    }

    /// <summary>
    /// Reads the JSON representation of the object.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type of object to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    /// <exception cref="JsonException">Thrown if the JSON token is not a String or Number,
    /// or if the string value does not match any known enum member name or EnumMemberAttribute value,
    /// or if the numeric value is invalid for the enum type.</exception>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        InitializeCaches(typeToConvert, options); // Ensure caches are built

        if (reader.TokenType == JsonTokenType.String)
        {
            string enumString = reader.GetString() ?? string.Empty;
            if (enumString == null)
            {
               throw new JsonException($"Cannot convert null to Enum \"{typeToConvert.Name}\".");
            }

            if (ReadCache.TryGetValue(typeToConvert, out var readMap) && readMap.TryGetValue(enumString, out T enumValue))
            {
                return enumValue;
            }
            throw new JsonException($"Unable to convert \"{enumString}\" to Enum \"{typeToConvert.Name}\". No matching EnumMemberAttribute value or enum name found.");
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            // Improved numeric handling (more robust)
            Type underlyingType = Enum.GetUnderlyingType(typeToConvert);
            object numericValue;

            if (underlyingType == typeof(int) && reader.TryGetInt32(out int intValue)) numericValue = intValue;
            else if (underlyingType == typeof(uint) && reader.TryGetUInt32(out uint uintValue)) numericValue = uintValue;
            else if (underlyingType == typeof(long) && reader.TryGetInt64(out long longValue)) numericValue = longValue;
            else if (underlyingType == typeof(ulong) && reader.TryGetUInt64(out ulong ulongValue)) numericValue = ulongValue;
            else if (underlyingType == typeof(short) && reader.TryGetInt16(out short shortValue)) numericValue = shortValue;
            else if (underlyingType == typeof(ushort) && reader.TryGetUInt16(out ushort ushortValue)) numericValue = ushortValue;
            else if (underlyingType == typeof(byte) && reader.TryGetByte(out byte byteValue)) numericValue = byteValue;
            else if (underlyingType == typeof(sbyte) && reader.TryGetSByte(out sbyte sbyteValue)) numericValue = sbyteValue;
            else
            {
                throw new JsonException($"Cannot read number token for enum {typeToConvert.Name} with underlying type {underlyingType.Name}.");
            }

            try {
                // Check if the numeric value is defined in the enum
                 if (!Enum.IsDefined(typeToConvert, numericValue))
                 {
                    throw new JsonException($"Numeric value {numericValue} is not defined in Enum \"{typeToConvert.Name}\".");
                 }
                return (T)Enum.ToObject(typeToConvert, numericValue);
            } catch (ArgumentException ex) {
                 // This catch might be redundant now with Enum.IsDefined, but kept for safety.
                 throw new JsonException($"Numeric value {numericValue} is not valid for Enum \"{typeToConvert.Name}\".", ex);
            }
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum \"{typeToConvert.Name}\". Expected String or Number.");
    }

    /// <summary>
    /// Writes the JSON representation of the object.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        InitializeCaches(typeof(T), options); // Ensure caches are built

        if (WriteCache.TryGetValue(typeof(T), out var writeMap) && writeMap.TryGetValue(value, out string? stringValue))
        {
            writer.WriteStringValue(stringValue ?? string.Empty);
        }
        else
        {
            // Fallback: This case indicates the enum value wasn't processed during initialization,
            // which might happen if the enum definition changed after cache initialization
            // or if the enum has values not representable by standard members (e.g., flags combination without explicit member).
            // Writing the default ToString() representation or the underlying numeric value might be alternatives.
            // Consider logging a warning here.
            // For now, using ToString() as the ultimate fallback.
            writer.WriteStringValue(value.ToString());
        }
    }
}