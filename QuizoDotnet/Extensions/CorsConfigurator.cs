namespace QuizoDotnet.Extensions;

public static class CorsConfigurator
{
    private const string DevelopmentPolicyName = "DevelopmentCorsPolicy";
    private const string ProductionPolicyName = "ProductionCorsPolicy";

    public static void ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(DevelopmentPolicyName, policyBuilder =>
            {
                policyBuilder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .SetIsOriginAllowed(_ => true);
            });

            options.AddPolicy(ProductionPolicyName, policyBuilder =>
            {
                policyBuilder
                    .WithOrigins("https://ludo.karizmastudio.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }

    public static void ConfigureCors(this WebApplication app)
    {
        app.UseCors(app.Environment.IsDevelopment() ? DevelopmentPolicyName : ProductionPolicyName);
    }
}