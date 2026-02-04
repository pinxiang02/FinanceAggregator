# High-Performance Financial Aggregator 🚀

A cloud-native .NET Aspire application that simulates, ingests, and visualizes real-time financial data.

## 🏗 Tech Stack
- **Orchestration:** .NET Aspire 9.0
- **Backend:** C# (Worker Services), NATS (Messaging)
- **Database:** PostgreSQL (Timescale-ready)
- **Frontend:** Blazor Web App (Interactive Server)
- **Visualization:** ApexCharts (Real-time updates)

## ⚡ Features
- **Microservices Architecture:** Decoupled Ingestor and Aggregator services.
- **Real-Time Dashboard:** Live price updates using SignalR and Blazor Server.
- **Automated Infrastructure:** Database provisioning and migrations via Aspire AppHost.

## 🏃‍♂️ How to Run
1. Install **Docker Desktop**.
2. Open the solution in **Visual Studio 2022**.
3. Set `FinanceAggregator.AppHost` as the startup project.
4. Press **F5**.
