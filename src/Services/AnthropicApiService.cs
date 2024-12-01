using Anthropic.SDK;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;

namespace Fulcrum.Services;

public class AnthropicApiService
{
    private readonly IConfiguration _configuration;
    private readonly AnthropicClient _client;

    public AnthropicApiService(IConfiguration configuration)
    {
        _configuration = configuration;
        string? apiKey = _configuration["ApiKeys:Anthropic"];
        if (string.IsNullOrEmpty(apiKey)) throw new Exception("Anthropic API key is not set in configuration");
        _client = new AnthropicClient(apiKey);
    }

    public async IAsyncEnumerable<MessageResponse> StreamResponse(List<Message> messages, int maxTokens, string systemPrompt, decimal temperature = 0.0m)
    {
        if (string.IsNullOrEmpty(systemPrompt)) systemPrompt = "You are a helpful assistant.";

        await foreach (MessageResponse messageResponse in _client.Messages.StreamClaudeMessageAsync(new MessageParameters()
        {
            Messages = messages,
            MaxTokens = maxTokens,
            Model = AnthropicModels.Claude35Sonnet,
            Stream = true,
            System = new List<SystemMessage>() { new SystemMessage(systemPrompt) },
            Temperature = temperature
        }))
        {
            yield return messageResponse;
        }
    }
}
