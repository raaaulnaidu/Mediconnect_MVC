# MediConnect - Healthcare Management System

A full-stack ASP.NET Core MVC web application for managing patients, providers, and medical appointments with real-time NPI provider lookup integration.

## Features

✅ **Patient Management** - Create, read, update, delete patient records
✅ **Provider Management** - Manage healthcare providers with contact details  
✅ **Appointment Scheduling** - Schedule appointments linking patients and providers
✅ **NPI Provider Lookup** - Search 1M+ verified providers via NPPES API
✅ **Dashboard Analytics** - Real-time metrics on patients and appointments
✅ **Data Persistence** - JSON-backed storage with singleton pattern

## Architecture

### MVC Pattern
- **Models:** Patient, Provider, Appointment entities with BaseEntity audit tracking
- **Views:** Razor Pages with Bootstrap responsive design
- **Controllers:** 6 specialized controllers handling routing and coordination

### Data Layer
- `InMemoryRepository<T>` - Generic CRUD operations
- `JsonBackedStore` - Singleton service writing to AppData.json
- Relational data with foreign key support

### External Integration
- **NPPES NPI Registry API** - Healthcare provider search
- Fetch-only implementation (data not persisted)
- Support for search by specialty, state, organization, location

## Technology Stack

- **Backend:** ASP.NET Core MVC, C#
- **Frontend:** Razor Pages, Bootstrap 5, jQuery
- **Data Storage:** JSON (AppData.json)
- **API:** NPPES NPI Registry REST API
- **Deployment:** Azure App Service (or local development)

## Getting Started

### Prerequisites
- .NET 6.0 or later
- Visual Studio Code or Visual Studio

### Installation

