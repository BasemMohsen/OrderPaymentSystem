# 🧾 Order & Payment Microservices with RabbitMQ and Stripe (.NET 8)

This project demonstrates a practical microservices architecture using **.NET 8**, where an `OrderService` and a `PaymentService` communicate asynchronously via **RabbitMQ**. The `OrderService` handles order creation and emits events, while the `PaymentService` consumes these events, processes payments using **Stripe**, and responds with a confirmation.

---

## 🧱 Architecture Overview

### 🟦 OrderService
- Receives and stores orders in **SQL Server**
- Publishes `OrderCreated` events to **RabbitMQ**
- Listens to `PaymentCompleted` events
- Updates order status to `"Paid"` after successful payment

### 🟪 PaymentService
- Subscribes to `OrderCreated` events
- Processes Stripe payments
- Publishes `PaymentCompleted` events to RabbitMQ

---

## 🔧 Technologies Used

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- **RabbitMQ** (asynchronous messaging)
- **SQL Server** (order storage)
- **Stripe.NET SDK** (test payment processing)
- **Entity Framework Core**
- **Docker** (for RabbitMQ / SQL Server containers)
- **Minimal APIs** (performance-oriented endpoints)

---

## 🗂️ Solution Structure

OrderPaymentSystem/
│
├── OrderService/
│ ├── Controllers/
│ ├── Events/
│ ├── Data/
│ ├── Models/
│ ├── Services/
│ └── appsettings.json
│
├── PaymentService/
│ ├── Consumers/
│ ├── Events/
│ ├── Services/
│ └── appsettings.json
│
└── README.md



---

## 🚀 Getting Started

### ✅ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop)
- Stripe account (for test secret key)

---

### 📥 Clone the Repository

```bash
git clone https://github.com/your-username/OrderPaymentSystem.git
cd OrderPaymentSystem


docker run -d --hostname rabbitmq-dev --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=OrdersDb;User Id=sa;Password=YourStrong!Passw0rd;"
}

"Stripe": {
  "SecretKey": "sk_test_..."
}

cd OrderService
dotnet ef database update

cd OrderService
dotnet run

cd PaymentService
dotnet run

