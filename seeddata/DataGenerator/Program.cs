﻿//using eShopSupport.DataGenerator;
using eShopSupport.DataGenerator.Generators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CommunityToolkit.Aspire.OllamaSharp;   // ⬅ only namespace you need

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true);


//builder.AddOpenAIChatCompletion("chatcompletion");

//builder.AddOllamaApiClient("ollama", opts =>
//{
//    opts.Endpoint = new Uri("http://localhost:11434/");
//    opts.Model = "llama3.1";
//});

var ollama = builder.AddOllamaApiClient("ollama");
ollama.AddEmbeddingGenerator();   // IEmbeddingGenerator<string,Embedding<float>>
ollama.AddChatClient();           // IChatClient   <-- call AFTER or in a separate line

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
