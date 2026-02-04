var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin();

var financeDb = postgres.AddDatabase("FinanceDb");

var apiService = builder.AddProject<Projects.FinanceAggregator_ApiService>("apiservice")
                        .WithReference(financeDb);

builder.AddProject<Projects.FinanceAggregator_Web>("webfrontend")
       .WithReference(apiService);

builder.Build().Run();
