using ThanyMarcus.Cloud.Api.Features.Settings;

namespace ThanyMarcus.Cloud.Api.Infrastructure.Llm;

public sealed class LlmClientFactory : ILlmClientFactory
{
    private readonly SafeLlmClient safe;
    private readonly IServiceProvider services;

    public LlmClientFactory(SafeLlmClient safe, IServiceProvider services)
    {
        this.safe = safe;
        this.services = services;
    }

    public ILlmClient Resolve(CloudSettings settings, out bool fallbackToSafe)
    {
        ArgumentNullException.ThrowIfNull(settings);
        fallbackToSafe = false;
        if (settings.LlmMode == LlmModes.Safe) return safe;
        // Unsafe-mode dispatch falls back to safe until the unsafe LLM client ships.
        var unsafeClient = (IUnsafeLlmClient?)services.GetService(typeof(IUnsafeLlmClient));
        if (unsafeClient is null)
        {
            fallbackToSafe = true;
            return safe;
        }
        return unsafeClient;
    }
}
