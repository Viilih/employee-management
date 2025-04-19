# Employee Management System

A comprehensive system for managing departments, coordinators, and employees with complete CRUD operations and relationship management.

## Table of Contents
- [Overview](#overview)
- [Technologies](#technologies)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
    - [Prerequisites](#prerequisites)
    - [Database Configuration](#database-configuration)
    - [Azure Services Configuration](#azure-services-configuration)
    - [Running the Project](#running-the-project)

## Overview

This repository provides a complete solution for employee management with CRUD operations for Departments, Coordinators, and Employees along with their relationships. The system uses a microservice architecture with Azure Service Bus for communication between services.

## Technologies

- **.NET 8**: Modern C# framework
- **CQRS Pattern**: Using MediatR for command-query separation
- **Fluent Results**: For improved result handling
- **PostgreSQL**: Primary database
- **Azure Services**:
    - Azure Service Bus: For inter-service communication
    - Azure Communication Service: For notifications
    - Azure Email Service: For email communications

## Project Structure

The solution consists of two main projects:
- **API**: Main application handling CRUD operations
- **Email-Microservice**: Handles all email communication

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- PostgreSQL instance
- Azure account with the following services:
    - Azure Service Bus
    - Azure Communication Services
    - Azure Email Communication Services

### Database Configuration

1. Set up a PostgreSQL database
2. Update the connection string in `appsettings.json` within the API project:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your_host;Database=your_db;Username=your_username;Password=your_password"
  }
}
```

Note: Migrations are applied automatically at runtime.

### Azure Services Configuration

#### API Project Configuration

Update the `appsettings.json` file with:

```json
{
  "AzureServiceBus": {
    "ConnectionString": "your_service_bus_connection_string",
    "QueueName": "your_queue_name"
  }
}
```

#### Email Microservice Configuration

Update the `appsettings.json` file with:

```json
{
  "AzureServiceBus": {
    "ConnectionString": "your_service_bus_connection_string",
    "QueueName": "your_queue_name"
  },
  "AzureCommunicationServices": {
    "ConnectionString": "your_communication_services_connection_string",
    "SenderAddress": "your_sender_email@domain.com"
  }
}
```

### Running the Project

After setting up the configurations, run the following commands:

```bash
dotnet restore
dotnet run --project API
dotnet run --project Email-Microservice
```




