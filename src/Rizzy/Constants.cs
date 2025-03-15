using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy;

internal class Constants
{
	internal static class HttpContextKeys
	{
		/// <summary>
		/// Key to store nonce values in HttpContext.Items.
		/// </summary>
		public const string NonceKey = "RizzyNonce";
	}
}

