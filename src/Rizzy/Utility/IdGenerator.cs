using Sqids;

namespace Rizzy.Utility;

public static class IdGenerator
{
    private static Random _random = new(System.Environment.TickCount);

    /// <summary>
    /// A thread-safe counter used to ensure uniqueness of the IDs.
    /// Initialized with the current UTC timestamp ticks.
    /// </summary>
    private static long _counter = DateTime.UtcNow.Ticks;

    private static SqidsEncoder<long> _encoder = new(new SqidsOptions()
    {
        Alphabet = new string("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".OrderBy(x => _random.Next()).ToArray()),
    });

    /// <summary>
    /// Generates a unique ID with an optional prefix.
    /// </summary>
    /// <param name="prefix">
    /// An optional string to prefix the unique ID.
    /// If null or whitespace, "id" is used as the default prefix.
    /// </param>
    /// <returns>
    /// A unique ID string composed of the prefix and a Base62 encoded unique number.
    /// </returns>
    public static string UniqueId(string prefix)
    {
        long uniqueNumber = Interlocked.Increment(ref _counter);

        var encodedNumber = _encoder.Encode(uniqueNumber);

        string finalPrefix = string.IsNullOrWhiteSpace(prefix) ? "id" : prefix;
        return $"{finalPrefix}{encodedNumber}";
    }
}
