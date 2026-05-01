# AGENTS.md - Feedback Roulette Context

## Project Overview

**Feedback Roulette** is a peer-to-peer feedback platform built with Blazor Server and .NET 10.0. Users upload their work (music, code, design), give feedback to others to earn credits, and use those credits to receive feedback on their own submissions.

## Tech Stack

- **Framework:** .NET 10.0 (ASP.NET Core / Blazor Server, Interactive Server Render Mode)
- **Database:** MySQL (via MySql.EntityFrameworkCore), with SQLite/SQL Server packages also referenced
- **ORM:** Entity Framework Core with Identity
- **UI:** Blazor Server with CSS Isolation, Bootstrap 5, Bootstrap Icons
- **Auth:** ASP.NET Core Identity with `ApplicationUser`

## Solution Structure

```
Feedback Roulette.sln
├── Feedback Roulette/                  # Main web project (Blazor Server)
│   ├── Components/
│   │   ├── Dashboard/                  # Authenticated app pages
│   │   │   ├── DashboardLayout.razor   # Sidebar layout for app
│   │   │   └── Pages/
│   │   │       ├── Dashboard.razor     # Central hub (credits, stats)
│   │   │       ├── Review.razor        # "Roulette" feedback discovery
│   │   │       ├── Upload.razor        # Submit new work
│   │   │       ├── Submissions.razor   # Track uploaded works
│   │   │       ├── FeedbackReceived.razor
│   │   │       ├── Leaderboard.razor
│   │   │       └── Settings.razor
│   │   ├── Layout/                     # Global layouts
│   │   │   ├── MainLayout.razor        # Public pages wrapper
│   │   │   └── TopNavbar.razor
│   │   ├── Pages/                      # Public pages
│   │   │   ├── Home.razor
│   │   │   ├── Error.razor
│   │   │   └── NotFound.razor
│   │   └── Account/                    # Identity scaffolding
│   ├── Data/
│   │   └── DataContext.cs              # EF Core DbContext
│   ├── Services/
│   │   ├── IIdentityService.cs
│   │   └── IdentityService.cs          # Get current user helper
│   ├── wwwroot/
│   │   ├── app.css                     # Global app styles
│   │   ├── dashboard.css               # Dashboard theme (CSS vars)
│   │   └── uploads/                    # File upload directory
│   ├── Migrations/                     # EF migrations
│   ├── Program.cs                      # Entry point
│   └── appsettings.json                # Config (MySQL connection)
│
└── FeedbackRoulette_ClassLibrary/      # Shared models library
    ├── ApplicationUser.cs              # IdentityUser + Credits (default: 100)
    ├── Category.cs                     # Music, Programming, Design
    ├── Feedback.cs                     # Review with positive/negative/suggestion
    └── FeedbackItem.cs                 # Uploaded work with file metadata
```

## Data Models

### ApplicationUser
- Extends `IdentityUser`
- `Credits` (int, default: 100) - currency for unlocking feedback

### Category
- `Id`, `Name`, `Description`
- Seed data: Music (1), Programming (2), Design (3)

### FeedbackItem
- `Id`, `Title`, `Description`
- `ApplicationUserId` → `ApplicationUser` (owner)
- `FileUrl`, `FileType`, `FileSize`
- `CategoryId` → `Category`
- `Feedbacks` (list of received feedback)

### Feedback
- `Id`
- `ApplicationUserId` → `ApplicationUser` (reviewer)
- `FeedbackItemId` → `FeedbackItem` (what's being reviewed)
- Booleans: `HasPositiveFeedback`, `HasNegativeFeedback`, `HasSuggestion`
- Text: `PositiveFeedback`, `NegativeFeedback`, `Suggestion`

## Key Conventions

### Styling
- Dark mode by default (primary bg: `#0D0D12`, card: `#1C1C24`, accent: `#5D5FEF`)
- Global CSS vars in `wwwroot/dashboard.css`
- Component-specific styles via `.razor.css` isolation
- Bootstrap 5 + Bootstrap Icons for utilities

### Navigation
- Use `NavLink` for internal links (auto `active` class)
- Sidebar nav defined in `DashboardLayout.razor`
- Routes: `/dashboard`, `/review`, `/upload`, `/submissions`, `/feedback-received`, `/leaderboard`, `/settings`

### Authentication
- Identity with email/password, no account confirmation required
- Password: min 6 chars, no special/uppercase requirements
- `IIdentityService` injected to get current user in components

### Database
- MySQL primary (connection in `appsettings.json`)
- `DataContext` extends `IdentityDbContext<ApplicationUser>`
- Registered as scoped via `IDbContextFactory`

## Development

- **Run:** `dotnet run` from "Feedback Roulette/" directory
- **Migrations:** Use `dotnet ef migrations add` from "Feedback Roulette/"
- **Adding pages:** Place in `Components/Dashboard/Pages/` for authenticated, `Components/Pages/` for public
- **Adding styles:** Use CSS vars in `dashboard.css` for globals, `.razor.css` for component-specific
