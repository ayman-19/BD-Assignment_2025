var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterServices();

// Register Quartz
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("ExpiredBlockJob");

    q.AddJob<ExpiredBlockJob>(opts => opts.WithIdentity(jobKey));

    // Trigger runs every 5 minutes
    q.AddTrigger(opts =>
        opts.ForJob(jobKey)
            .WithIdentity("ExpiredBlockJob-trigger")
            .StartNow()
            .WithSimpleSchedule(x => x.WithIntervalInMinutes(3).RepeatForever())
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandler>();
app.UseHttpsRedirection();
app.UseRouting();
app.RegisterAllEndpoints();

app.UseAuthorization();
app.MapControllers();

app.Run();
