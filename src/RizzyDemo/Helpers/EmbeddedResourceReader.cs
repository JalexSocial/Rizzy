using System.Reflection;

namespace RizzyDemo.Helpers;

public class EmbeddedResourceReader
{
    public static string ReadResourceText(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (Stream? stream = assembly.GetManifestResourceStream(resourcePath))
        {
            if (stream == null) throw new InvalidOperationException($"Resource {resourcePath} not found. Ensure it's set as an EmbeddedResource.");

            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}