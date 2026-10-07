using LocalAgenticUi.Components;
using LocalAgenticUi.Configuration;
using LocalAgenticUi.Tools;
using System.ClientModel;
using OpenAI;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var lmStudio = builder.Configuration
    .GetRequiredSection(LmStudioOptions.SectionName)
    .Get<LmStudioOptions>()!;

var lmStudioClient = new OpenAIClient(
    new ApiKeyCredential(lmStudio.ApiKey),
    new OpenAIClientOptions { Endpoint = new Uri(lmStudio.Endpoint) }
);

// UseFunctionInvocation runs the tools on the server and turns tools that
// need approval into approval requests the UI can answer.
builder.Services.AddChatClient(lmStudioClient.GetChatClient(lmStudio.ModelName).AsIChatClient())
    .UseFunctionInvocation();

// One note list per circuit
builder.Services.AddScoped<NoteStore>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
