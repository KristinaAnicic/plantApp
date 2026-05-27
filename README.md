# 🌱 PlantApp

PlantApp is a full-stack web application designed for plant enthusiasts to manage and track their plants, organize collections, monitor plant activity, and connect with other users through a plant exchange platform.

The application provides advanced plant management features including reminders, analytics, multilingual support, image uploads, and plant identification powered by the PlantNet API.

---

# Features

- User authentication with JWT & Refresh Tokens
- Secure refresh token storage using HTTP-only cookies
- Add and manage plants
- Create plant logs and activity records
- Organize plants into groups
- Create logs for plant groups
- Plant care reminder system
- Analytics dashboard
- Image upload support (AppWrite)
- Plant identification using PlantNet API
- Multilingual support
- Detailed plant information pages
- Detailed planted plant pages
- Plant exchange marketplace
- Rate limiting protection
- Global exception handling
- Responsive design

---

# Tech Stack

## Frontend
- Angular
- TypeScript
- Tailwind CSS
- Reactive Forms
- HTTP Interceptors
- Route Guards
- Multilingual support
- Responsive UI design
- Reusable shared components

## Backend
- ASP.NET Core
- JWT Authentication
- Refresh Token Authentication
- Dependency Injection
- Global Exception Handling
- Rate Limiting
- Entity Framework Core Migrations

## Database
- PostgreSQL
- Neon (hosted database)
- Entity Framework Core

## Storage
- Appwrite (image storage)

## External APIs
- PlantNet API (plant identification)

## Deployment
- Frontend hosted on Vercel
- Backend hosted on Azure

---

# Project Architecture

The backend follows a clean, layered architecture:

## Data Layer
- Entity Framework Core DbContext
- Migrations
- Repositories
- Enums

## Domain Layer
- Models
- DTOs
- Interfaces
- Services

## ML Layer
- Machine learning-related functionality

## PlantBackend Layer
- Controllers
- API configuration
- Middleware configuration

---

# Technical Highlights

- Clean architecture approach
- Separation of concerns
- Dependency Injection
- Repository pattern
- Secure authentication (JWT + Refresh Tokens)
- HTTP-only cookie token storage
- Rate limiting for API protection
- Global exception handling middleware
- External API integration (PlantNet)
- Entity Framework Core migrations
- Scalable backend structure
