// See https://aka.ms/new-console-template for more information

using MartenApplyAutoCreateBeforeRun;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices(services => services.AddMartenPersistence());

var host = builder.Build();

await host.RegisterStartup<Program>();

host.Run();

Console.WriteLine("Hello, World!");