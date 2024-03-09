using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rizzy.Components.Form.Helpers;
internal static class IdProvider
{
	public static string CreateSanitizedId(string fullname)
	{
		if (string.IsNullOrEmpty(fullname))
		{
			return string.Empty;
		}

		// Remove leading and trailing spaces, replace spaces with hyphens, and remove invalid characters
		string sanitized = Regex.Replace(fullname.Trim(), "\\s+", "-");
		sanitized = Regex.Replace(sanitized, "[^a-zA-Z0-9\\-_:.]", "");

		// Ensure the ID starts with a letter or an underscore (for broader compatibility and CSS friendliness)
		if (!char.IsLetter(sanitized[0]) && sanitized[0] != '_')
		{
			sanitized = "rz" + sanitized;
		}

		return sanitized;
	}
}
