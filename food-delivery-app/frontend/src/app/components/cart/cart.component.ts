import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { Cart } from '../../models/cart.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart: Cart = { cartItems: [] };
  error = '';

  constructor(
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cartService.loadCart().subscribe({
      next: c => {
        this.cart = c;
        this.error = '';
      },
      error: (err) => {
        if (err?.status === 401) {
          this.error = 'Please login to view your cart.';
          this.router.navigate(['/login']);
          return;
        }

        this.error = 'Unable to load cart right now.';
      }
    });
    this.cartService.cart$.subscribe(c => this.cart = c);
  }

  removeItem(foodItemId: number): void {
    this.cartService.removeFromCart(foodItemId).subscribe({
      next: c => this.cart = c,
      error: () => this.error = 'Unable to remove item right now.'
    });
  }

  updateQuantity(foodItemId: number, quantity: number): void {
    if (quantity > 0) {
      this.cartService.updateQuantity(foodItemId, quantity).subscribe({
        next: c => this.cart = c,
        error: () => this.error = 'Unable to update quantity right now.'
      });
    }
  }

  checkout(): void {
    this.router.navigate(['/checkout']);
  }

  get totalItems(): number {
    return this.cart.cartItems.reduce((sum, item) => sum + item.quantity, 0);
  }

  get totalAmount(): number {
    return this.cart.cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
  }
}
