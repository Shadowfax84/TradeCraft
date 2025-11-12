import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  isLoggedIn = false;
  userName = 'John Doe';
  userEmail = 'john.doe@example.com';

  ngOnInit(): void {
    // Check auth state from localStorage or Supabase session
    this.checkAuthState();
  }

  /**
   * Check if user is authenticated
   * This can be replaced with actual Supabase auth check
   */
  checkAuthState(): void {
    const storedSession = localStorage.getItem('auth_session');
    if (storedSession) {
      try {
        const session = JSON.parse(storedSession);
        this.isLoggedIn = !!session.user;
        this.userName = session.user?.user_metadata?.name || 'User';
        this.userEmail = session.user?.email || '';
      } catch (error) {
        console.error('Error parsing auth session:', error);
        this.isLoggedIn = false;
      }
    }
  }

  /**
   * Handle login button click
   * Navigate to login page or open login modal
   */
  onLogin(): void {
    // This will be replaced with actual navigation to login page
    console.log('Navigate to login');
    // In a real app: this.router.navigate(['/login']);
  }

  /**
   * Handle logout action
   */
  onLogout(): void {
    this.isLoggedIn = false;
    localStorage.removeItem('auth_session');
    // In a real app: this.router.navigate(['/login']);
  }

  /**
   * Navigate to profile page
   */
  onProfile(): void {
    console.log('Navigate to profile');
    // In a real app: this.router.navigate(['/profile']);
  }
}
