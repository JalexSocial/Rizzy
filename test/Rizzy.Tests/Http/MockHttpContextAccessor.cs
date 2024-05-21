using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rizzy.Configuration.Htmx;
using Rizzy.Configuration;
using static Rizzy.Configuration.RizzyConfigBuilder;

namespace Rizzy.Http;
internal class MockHttpContextAccessor : IHttpContextAccessor
{
    private readonly HttpContext _context;

    public MockHttpContextAccessor(TestServiceProvider services)
    {
            _context = new DefaultHttpContext()
            {
                RequestServices = services.BuildServiceProvider()
            };
    }

    public HttpContext? HttpContext
    {
        get => _context;
        set => throw new NotImplementedException();
    }
}
