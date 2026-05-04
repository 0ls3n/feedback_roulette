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

### Sidebar
- **Collapsible**: Folds to 64px (icons only) by default, expands to 260px on hover
- **Transitions**: `0.3s cubic-bezier(0.4, 0, 0.2, 1)` on width, padding, and text reveal
- **Review item**: Primary action with purple gradient background, placed at top of nav
- **Icon positioning**: Centered when folded, left-aligned when expanded
- **Profile footer**: Transparent when folded, card background with avatar border on hover
- CSS controlled via `:hover` on `.dashboard-sidebar` — no JS required

### Navigation
- Use `NavLink` for internal links (auto `active` class)
- Sidebar nav defined in `DashboardLayout.razor`
- Review is the primary page (first nav item, highlighted with gradient)
- Routes: `/review`, `/dashboard`, `/upload`, `/submissions`, `/feedback-received`, `/feedback-received/{ItemId:int}`, `/leaderboard`, `/settings`

### Feedback Received Page
- **Optional ID route**: `/feedback-received` shows list view with sidebar of all submissions with feedback, displaying the latest feedback. `/feedback-received/{ItemId}` shows detailed view without sidebar (original behavior).
- **Two view modes**:
  - **List view (no ID)**: Shows sidebar with all submissions that have feedback, ordered by most recent. Clicking a submission navigates to the detail view.
  - **Detail view (with ID)**: Shows full feedback details for a specific submission without the sidebar.
- **Component lifecycle**: Uses `OnParametersSetAsync` instead of `OnInitializedAsync` to handle route parameter changes when navigating between items.
- **Empty states**: Uses consistent `empty-state card-custom` styling with icon, heading, message, and action button (matching My Submissions page style).

### Empty State Styling
- **Global styles** in `wwwroot/dashboard.css`: Shared `.empty-state` class with centered layout, large icon, and consistent spacing.
- **Usage**: Apply `empty-state card-custom` classes together for consistent appearance across all pages.
- **Pages updated**: FeedbackReceived, Dashboard (Recent Submissions section), Review (already had it), Submissions (original reference).

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
