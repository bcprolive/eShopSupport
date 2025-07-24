using eShopSupport.DataGenerator;
using eShopSupport.DataGenerator.Generators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.AI;        // for IChatClient
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using CommunityToolkit.Aspire.OllamaSharp; // extension methods
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.AI.Ollama; // requires the package above

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);

builder.AddOpenAIChatCompletion("chatcompletion");

// 3.1  – register Ollama as the primary LLM
builder.AddOllamaSharpChatClient("ollama");                 // IChatClient
builder.AddOllamaSharpEmbeddingGenerator("ollama");         // IEmbeddingGenerator<string,Embedding<float>>


//if (!string.Equals(Environment.GetEnvironmentVariable("DISABLE_OPENAI"), "1",
//                   StringComparison.OrdinalIgnoreCase))
//{
//    builder.AddOpenAIChatCompletion("chatcompletion");
//}
//else
//{
//    // register Ollama – model name can come from configuration ("OllamaModel") or be hard‑coded
//    var modelName = builder.Configuration["OllamaModel"] ?? "llama3.1";
//    builder.Services.AddSingleton<IChatClient>(serviceProvider =>
//    {
//        var httpClient = serviceProvider.GetService<HttpClient>() ?? new HttpClient();
//        return new OllamaChatClient(new Uri("http://localhost:11434"), modelName, httpClient);
//    });
//}

var services = builder.Build().Services;

var categories = await new CategoryGenerator(services).GenerateAsync();
Console.WriteLine($"Got {categories.Count} categories");

var products = await new ProductGenerator(categories, services).GenerateAsync();
Console.WriteLine($"Got {products.Count} products");

var manualTocs = await new ManualTocGenerator(categories, products, services).GenerateAsync();
Console.WriteLine($"Got {manualTocs.Count} manual TOCs");

var manuals = await new ManualGenerator(categories, products, manualTocs, services).GenerateAsync();
Console.WriteLine($"Got {manuals.Count} manuals");

var manualPdfs = await new ManualPdfConverter(products, manuals).ConvertAsync();
Console.WriteLine($"Got {manualPdfs.Count} PDFs");

var tickets = await new TicketGenerator(products, categories, manuals, services).GenerateAsync();
Console.WriteLine($"Got {tickets.Count} tickets");

var ticketThreads = await new TicketThreadGenerator(tickets, products, manuals, services).GenerateAsync();
Console.WriteLine($"Got {ticketThreads.Count} threads");

var summarizedThreads = await new TicketSummaryGenerator(products, ticketThreads, services).GenerateAsync();
Console.WriteLine($"Got {summarizedThreads.Count} thread summaries");

var evalQuestions = await new EvalQuestionGenerator(products, categories, manuals, services).GenerateAsync();
Console.WriteLine($"Got {evalQuestions.Count} evaluation questions");
