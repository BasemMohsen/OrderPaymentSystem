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

