# Feedback Roulette

A modern, peer-to-peer feedback platform built with Blazor Server and .NET 10.0. This application allows users to upload their work (music, code, etc.), give feedback to others to earn credits, and receive detailed reviews on their own submissions.

## Tech Stack

- **Framework:** .NET 10.0 (ASP.NET Core / Blazor Server)
- **UI Framework:** Blazor with Interactive Server Render Mode
- **Styling:**
    - Custom CSS with CSS Variables (see `wwwroot/dashboard.css`)
    - CSS Isolation (`.razor.css` files) for component-specific styles
    - Bootstrap 5 (for base utilities)
    - Bootstrap Icons (`bi bi-*`)
- **Fonts:** Inter, system fonts

## Core Features

- **Dashboard:** Central hub showing user credits, activity stats, and recent feedback received.
- **Review System:** A "roulette" style discovery for giving feedback on others' submissions.
- **Credit System:** Users earn credits by providing quality feedback, which they can then use to "unlock" feedback for their own submissions.
- **Submission Management:** Track and manage uploaded works.
- **Leaderboard:** Community rankings based on feedback quality and quantity.

## Architecture & Conventions

### UI Structure
- **Layouts:**
    - `MainLayout.razor`: The primary application wrapper.
    - `DashboardLayout.razor`: The sidebar-based layout used for all dashboard and authenticated pages.
- **Pages:** Located in `Components/Dashboard/Pages` (for app-specific features) and `Components/Pages` (for general pages).

### CSS & Styling
- **Global Styles:** Defined in `wwwroot/dashboard.css` using CSS variables for theming (dark mode by default).
- **Theming:**
    - Primary Background: `#0D0D12`
    - Card Background: `#1C1C24`
    - Accent Purple: `#5D5FEF` (Primary action color)
- **Components:** Favor CSS Isolation for component-specific layout and styling to keep the global CSS lean.

### Coding Standards
- **C#:** Standard .NET PascalCase for methods and properties. Use explicit access modifiers.
- **Blazor:** 
    - Use `NavLink` for all internal navigation to benefit from automatic `active` class management.
    - Prefer component composition over large, monolithic files.
    - Maintain strict separation between UI (`.razor`) and styles (`.razor.css`).

## Project Layout

- `Feedback Roulette/Components/`: All Blazor components.
    - `Dashboard/`: Contains `DashboardLayout` and specific feature pages.
    - `Layout/`: Global layout components.
    - `Pages/`: General utility pages (Home, Counter, Weather, Error).
- `Feedback Roulette/wwwroot/`: Static assets, global CSS, and third-party libraries.
- `Feedback Roulette/Program.cs`: Application entry point and service configuration.

## Development Workflow

- **Running the App:** Use `dotnet run` or Rider's run configuration.
- **Adding Styles:** Add variables to `wwwroot/dashboard.css` if they are global, otherwise use isolated CSS files.
- **Navigation:** Update `DashboardLayout.razor`'s sidebar when adding new functional areas.
