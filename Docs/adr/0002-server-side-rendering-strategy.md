# 2. Server-Side Rendering with ASP.NET Core MVC

## Context
Modern web application development often defaults to Single Page Application (SPA) architectures using frameworks like React, Angular, or Vue, communicating with a backend API.
However, for a hackathon with strict time constraints (2 weeks), minimizing integration complexity is prioritized over creating a rich client-side application structure.
A unified architecture that eliminates the need to synchronize frontend and backend codebases is preferred.

## Decision
**Server-Side Rendering (SSR)** using **ASP.NET Core MVC (Model-View-Controller)** with Razor syntax will be utilized.

Explicit rejections include:
- **Blazor Server/WASM**: To avoid SignalR connection management complexity and large WASM download sizes.
- **Client-Side SPA (React/Vue)**: To eliminate the overhead of maintaining two separate codebases (Frontend + Backend), synchronizing API contracts, and managing client-side state.

## Consequences

**Positive**:
- **Velocity**: Logic defaults to the server. No requirement to build a REST/GraphQL API layer solely to serve internal UI views.
- **Simplicity**: No complex client-side state management (Redux, Pinia, etc.) is required.
- **Performance**: High performance for "First Contentful Paint" as HTML is delivered fully formed.

**Negative**:
- **Interactivity**: Rich, app-like interactivity (e.g., drag-and-drop, instant transitions) requires custom JavaScript or libraries rather than being native to the framework.
- **Bandwidth**: Full page reloads (or at least full HTML fragment fetches) are larger than JSON payloads.

**Mitigation**:
- The interactivity capability gap is mitigated by using **Vanilla JavaScript** and **CSS animations** (Tailwind) to provide immediate visual feedback (hover states, modal toggles) without needing a heavy frontend framework.
