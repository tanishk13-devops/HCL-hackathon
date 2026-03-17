import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterModule, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { CartService } from './services/cart.service';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { animate, style, transition, trigger } from '@angular/animations';
import { PremiumFooterComponent } from './components/premium-footer/premium-footer.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterOutlet, PremiumFooterComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  animations: [
    trigger('routeTransition', [
      transition('* => *', [
        style({ opacity: 0.98, transform: 'translateY(4px)' }),
        animate('160ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ])
  ]
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Ziggy';
  cartCount = 0;
  cartBump = false;
  routeState = 'init';
  private previousCount = 0;
  private readonly subscriptions = new Subscription();

  constructor(
    public authService: AuthService,
    private router: Router,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.cartService.loadCart().subscribe({ error: () => {} });
    }

    this.subscriptions.add(
      this.cartService.cart$.subscribe(cart => {
        const nextCount = cart.cartItems.reduce((sum, item) => sum + item.quantity, 0);
        if (nextCount > this.previousCount) {
          this.cartBump = true;
          setTimeout(() => (this.cartBump = false), 450);
        }

        this.previousCount = nextCount;
        this.cartCount = nextCount;
      })
    );

    this.subscriptions.add(
      this.router.events
        .pipe(filter(event => event instanceof NavigationEnd))
        .subscribe(() => {
          this.routeState = this.router.url;
        })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  logout(): void {
    this.authService.logout();
    this.cartCount = 0;
    this.previousCount = 0;
    this.cartService.clearCart();
    this.router.navigate(['/home']);
  }

}
