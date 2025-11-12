# TradeCraft UI - Angular 18 with Material Design

A modern, gamified share market educational platform frontend built with Angular 18 and Angular Material in dark mode.

## Project Structure

```
tradecraft-ui/
├── src/
│   ├── app/
│   │   ├── app.component.ts           # Main app component with auth logic
│   │   ├── app.component.html         # App layout template
│   │   └── app.component.css          # App component styles
│   ├── styles/
│   │   └── global.css                 # Global dark theme and utilities
│   ├── main.ts                        # Application bootstrap
│   └── index.html                     # HTML entry point
├── package.json                       # Dependencies
├── tsconfig.json                      # TypeScript configuration
├── vite.config.ts                     # Vite build configuration
└── README.md                          # This file
```

## Features

### Design & Theme
- **Dark Mode**: Complete dark theme with cyan (#00bcd4) and orange (#ff9800) accent colors
- **Angular Material**: Uses Material Design components (MatToolbar, MatButton, MatMenu, MatIcon)
- **Responsive**: Mobile-first responsive design with breakpoints for 768px and 480px
- **Modern Aesthetics**: Gradient backgrounds, smooth animations, and thoughtful micro-interactions

### Navigation & Layout
- **Sticky Navbar**: Top navigation bar with TradeCraft branding and auth section
- **Auth State**: Login button for unauthenticated users, account menu with profile dropdown for logged-in users
- **Footer**: Simple footer with copyright information
- **Main Content Area**: Centered content wrapper with proper spacing and visual hierarchy

### Authentication UI
- **Login Button**: Prominent orange gradient button for unauthenticated users
- **Account Menu**: Dropdown menu showing user info with options for Profile, Settings, and Logout
- **Mock Auth State**: Uses `isLoggedIn` boolean for demo (can be integrated with Supabase)

### Global Styles
Comprehensive CSS variable system for consistent theming:
- Background colors: `--bg-primary`, `--bg-secondary`, `--bg-tertiary`
- Text colors: `--text-primary`, `--text-secondary`, `--text-tertiary`
- Accent colors: `--accent-cyan`, `--accent-orange`
- Consistent spacing, typography, and shadows
- Custom scrollbar styling matching dark theme

## Getting Started

### Installation

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

### Development Server

The dev server runs on `http://localhost:5173` and automatically opens in your browser.

## Component Details

### AppComponent (`src/app/app.component.ts`)

**Properties:**
- `isLoggedIn: boolean` - Tracks authentication state
- `userName: string` - Display name of logged-in user
- `userEmail: string` - Email of logged-in user

**Methods:**
- `checkAuthState()` - Checks localStorage for auth session (can be replaced with Supabase)
- `onLogin()` - Handle login button click (navigate to login page)
- `onLogout()` - Clear auth session and logout
- `onProfile()` - Navigate to user profile page

### Styling Approach

The application uses a three-tier CSS approach:
1. **Global Styles** (`src/styles/global.css`) - CSS variables, Material overrides, and utilities
2. **Component Styles** (`src/app/app.component.css`) - Component-specific styling
3. **Inline Classes** - Utility classes for common patterns

## Customization

### Change Theme Colors

Update CSS variables in `src/styles/global.css`:

```css
:root {
  --accent-cyan: #00bcd4;      /* Primary accent */
  --accent-orange: #ff9800;    /* Secondary accent */
  --bg-primary: #121212;       /* Dark background */
  /* ... other variables */
}
```

### Add New Routes

Define routes in `src/main.ts`:

```typescript
const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'login', component: LoginComponent },
  { path: 'profile', component: ProfileComponent },
  // ... more routes
];
```

### Integrate Supabase Auth

Replace the mock auth in `AppComponent.checkAuthState()`:

```typescript
import { createClient } from '@supabase/supabase-js';

export class AppComponent {
  private supabase = createClient(SUPABASE_URL, SUPABASE_KEY);

  checkAuthState(): void {
    this.supabase.auth.onAuthStateChange((event, session) => {
      this.isLoggedIn = !!session;
      if (session?.user) {
        this.userName = session.user.user_metadata?.name || 'User';
        this.userEmail = session.user.email || '';
      }
    });
  }
}
```

## Dependencies

### Core Angular
- `@angular/core` - Angular framework
- `@angular/common` - Common Angular utilities
- `@angular/platform-browser` - Browser platform
- `@angular/router` - Routing
- `@angular/animations` - Animation support

### Angular Material
- `@angular/material` - Material Design components
- `@angular/cdk` - Component Development Kit

### Other
- `@supabase/supabase-js` - Supabase client (for auth/database integration)
- `rxjs` - Reactive programming library
- `zone.js` - Angular zone management

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Performance

Build output:
- CSS: ~11.22 kB (gzipped)
- JS (vendor): ~164.57 kB (gzipped)
- JS (app): ~107.96 kB (gzipped)
- Total: ~283.75 kB (gzipped)

## Next Steps

1. **Implement Login Page**: Create a dedicated login component
2. **Add Dashboard**: Build the main trading dashboard
3. **Supabase Integration**: Connect authentication and database
4. **Real-time Data**: Integrate financial data with WebSockets
5. **Role-based Access**: Implement role-based routing and permissions
6. **Components Library**: Create reusable UI components for charts, tables, etc.

## Resources

- [Angular 18 Documentation](https://angular.io/docs)
- [Angular Material Components](https://material.angular.io)
- [Supabase Documentation](https://supabase.com/docs)
- [Vite Documentation](https://vitejs.dev)

## License

This project is part of TradeCraft and follows the main project license.
