using WebApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

//Terminate the request pipleine with app.run
app.Run();
