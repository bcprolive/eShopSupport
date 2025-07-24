using Microsoft.Extensions.AI;
using SmartComponents.LocalEmbeddings;
using CommunityToolkit.Aspire.OllamaSharp;
using Microsoft.Extensions.AI;
//using Microsoft.Extensions.AI.Embeddings; // Embedding<T>

namespace eShopSupport.DataGenerator.Generators;
//namespace SmartComponents.LocalEmbeddings;

public class LocalTextEmbeddingGenerator : IEmbeddingGenerator<string, Embedding<float>>
{
    private readonly LocalEmbedder _embedder = new();

    public EmbeddingGeneratorMetadata Metadata => new("local");

    public void Dispose() => _embedder.Dispose();

    public Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var results = values.Select(v => new Embedding<float>(_embedder.Embed(v).Values)).ToList();
        return Task.FromResult(new GeneratedEmbeddings<Embedding<float>>(results));
    }

    public TService? GetService<TService>(object? key = null) where TService : class
        => typeof(TService) == typeof(IEmbeddingGenerator<string, Embedding<float>>) ? this as TService : null;
}
