using global::Rizzy.Http.Mock;

namespace Rizzy.Http;

public class HtmxRequestTests
{
    private readonly MockHttpContext _context = new MockHttpContext();

    [Fact]
    public void IsHtmx_ReturnsTrue_WhenHtmxRequestHeaderIsPresent()
    {
        // Arrange
        _context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        var htmxRequest = new HtmxRequest(_context);

        // Act
        var isHtmx = htmxRequest.IsHtmx;

        // Assert
        Assert.True(isHtmx);
    }

    [Fact]
    public void IsBoosted_ReturnsTrue_WhenBoostedHeaderIsPresent()
    {
        // Arrange
        _context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        _context.Request.Headers[HtmxRequestHeaderNames.Boosted] = "true";
        var htmxRequest = new HtmxRequest(_context);

        // Act
        var isBoosted = htmxRequest.IsBoosted;

        // Assert
        Assert.True(isBoosted);
    }

    [Fact]
    public void IsHistoryRestore_ReturnsTrue_WhenHistoryRestoreRequestHeaderIsPresent()
    {
        // Arrange
        _context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        _context.Request.Headers[HtmxRequestHeaderNames.HistoryRestoreRequest] = "true";
        var htmxRequest = new HtmxRequest(_context);

        // Act
        var isHistoryRestore = htmxRequest.IsHistoryRestore;

        // Assert
        Assert.True(isHistoryRestore);
    }

    [Fact]
    public void CurrentURL_ReturnsUri_WhenCurrentURLHeaderIsPresent()
    {
        // Arrange
        var expectedUri = new Uri("http://localhost/test");
        _context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        _context.Request.Headers[HtmxRequestHeaderNames.CurrentURL] = expectedUri.ToString();
        var htmxRequest = new HtmxRequest(_context);

        // Act
        var currentUrl = htmxRequest.CurrentURL;

        // Assert
        Assert.Equal(expectedUri, currentUrl);
    }

    [Fact]
    public void Target_ReturnsTargetId_WhenTargetHeaderIsPresent()
    {
        // Arrange
        var expectedTarget = "main-content";
        _context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        _context.Request.Headers[HtmxRequestHeaderNames.Target] = expectedTarget;
        var htmxRequest = new HtmxRequest(_context);

        // Act
        var target = htmxRequest.Target;

        // Assert
        Assert.Equal(expectedTarget, target);
    }

    [Fact]
    public void TriggerName_ReturnsTriggerName_WhenTriggerNameHeaderIsPresent()
    {
        // Arrange
        var expectedTriggerName = "submitButton";
        _context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        _context.Request.Headers[HtmxRequestHeaderNames.TriggerName] = expectedTriggerName;
        var htmxRequest = new HtmxRequest(_context);

        // Act
        var triggerName = htmxRequest.TriggerName;

        // Assert
        Assert.Equal(expectedTriggerName, triggerName);
    }

    [Fact]
    public void Trigger_ReturnsTriggerId_WhenTriggerHeaderIsPresent()
    {
        // Arrange
        var expectedTrigger = "submit-button";
        _context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        _context.Request.Headers[HtmxRequestHeaderNames.Trigger] = expectedTrigger;
        var htmxRequest = new HtmxRequest(_context);

        // Act
        var trigger = htmxRequest.Trigger;

        // Assert
        Assert.Equal(expectedTrigger, trigger);
    }

    [Fact]
    public void Prompt_ReturnsPromptValue_WhenPromptHeaderIsPresent()
    {
        // Arrange
        var expectedPrompt = "Are you sure?";
        _context.Request.Headers[HtmxRequestHeaderNames.HtmxRequest] = "true";
        _context.Request.Headers[HtmxRequestHeaderNames.Prompt] = expectedPrompt;
        var htmxRequest = new HtmxRequest(_context);

        // Act
        var prompt = htmxRequest.Prompt;

        // Assert
        Assert.Equal(expectedPrompt, prompt);
    }
}

