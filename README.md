# PRG281 – Event Ticket Management System

A C# Console Application developed for the PRG281 Programming course. The system provides functionality for managing event guests, ticket sales, payments, and data persistence using CSV files.

## Overview

This application simulates a simple event ticketing system where administrators can:

- Register guests
- View registered guests
- Sell event tickets
- Process payments
- Search for guests
- Store and retrieve data from CSV files

The project demonstrates key Object-Oriented Programming (OOP) concepts including:

- Inheritance
- Abstraction
- Encapsulation
- Polymorphism
- Interfaces
- Custom Exceptions
- Events and Delegates
- File Handling
- Collections and Generics

---

## Features

### Guest Management
- Register new guests
- Validate guest information
- Search guests by name
- View all registered guests

### Ticket Management
- Standard Ticket support
- VIP Ticket support
- Automatic ticket ID generation
- Ticket price calculation

### Payment Processing
- Cash payments
- Card payments
- EFT payments
- Payment logging with timestamps

### Event Handling
- Custom ticket sale events
- Event-driven notifications when tickets are sold

### Data Persistence
Data is stored using CSV files:

- `guests.csv`
- `tickets.csv`
- `payments.csv`

The system automatically loads existing data on startup and saves updates during execution.

---

## Project Structure

```text
PRG281_Project/
│
├── Program.cs              # Main application and menu system
├── Guest.cs                # Person and Guest classes
├── Tickets.cs              # Ticket hierarchy
├── Payments.cs             # Payment processing
├── Events.cs               # Event handling classes
├── Datastore.cs            # CSV data persistence
├── Discounts.cs            # Discount functionality
├── Monitor.cs              # Monitoring and reporting
├── CustomExceptions.cs     # Custom exception handling
│
├── data/
│   ├── guests.csv
│   ├── tickets.csv
│   └── payments.csv
│
└── PRG281_Project.sln
```

---

## Technologies Used

- C#
- .NET Framework 4.7.2
- Visual Studio
- CSV File Storage

---

## Object-Oriented Concepts Demonstrated

### Inheritance
```csharp
Person → Guest
Tickets → StandardTicket / VIPTicket
```

### Abstraction
Abstract base classes:

- `Person`
- `Tickets`

### Encapsulation
Properties include validation logic for:

- Names
- Email addresses
- Phone numbers

### Polymorphism
Ticket pricing is calculated through overridden methods:

```csharp
public abstract decimal GetPrice();
```

### Events & Delegates
Custom event system used to notify when tickets are sold.

---

## Getting Started

### Prerequisites

- Visual Studio 2022 or later
- .NET Framework 4.7.2

### Running the Project

1. Clone the repository:

```bash
git clone https://github.com/your-username/PRG281_Project.git
```

2. Open:

```text
PRG281_Project.sln
```

3. Build and run the project.

4. Login using the credentials configured in the application.

---

### In visual studio

1. Open `PRG281_Project.sln` in Visual Studio
2. Build and run the project (F5)
3. Log in with the default credentials:
   - **Username:** `administrator`
   - **Password:** `1233212`
## Sample Menu

```text
1. Register New Guest
2. View All Guests
3. Sell Ticket
4. Search Guest by Name
5. Exit
```

---

## Learning Outcomes

This project was created to demonstrate:

- Object-Oriented Programming principles
- Data validation
- Exception handling
- Event-driven programming
- File handling and persistence
- Console-based user interface design

---

PRG281 Programming Project
