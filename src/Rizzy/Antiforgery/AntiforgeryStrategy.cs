using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Rizzy.Antiforgery;

public enum AntiforgeryStrategy
{
	None = 1,
	GenerateTokensPerPage = 2,
	GenerateTokensPerRequest = 3
}
