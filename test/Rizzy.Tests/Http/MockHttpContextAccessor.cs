using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Rizzy.Http;
internal class MockHttpContextAccessor : IHttpContextAccessor
{
    private readonly HttpContext _context;

    public MockHttpContextAccessor(BunitServiceProvider services)
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
