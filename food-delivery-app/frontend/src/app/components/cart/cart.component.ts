import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { Cart } from '../../models/cart.model';
import { Order } from '../../models/order.model';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart: Cart = { items: [], totalItems: 0, totalAmount: 0 };
  customerName = '';
  customerPhone = '';
  customerAddress = '';
  specialNotes = '';
  loading = false;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cartService.cart$.subscribe(cart => {
      this.cart = cart;
    });
  }

  removeItem(foodId: number): void {
    this.cartService.removeFromCart(foodId);
  }

  updateQuantity(foodId: number, quantity: number): void {
    if (quantity > 0) {
      this.cartService.updateQuantity(foodId, quantity);
    }
  }

  placeOrder(): void {
    if (!this.customerName || !this.customerPhone || !this.customerAddress) {
      alert('Please fill in all required fields');
      return;
    }

    if (this.cart.items.length === 0) {
      alert('Your cart is empty');
      return;
    }

    const order: Order = {
      customerId: this.authService.getCurrentUser()?.id || 0,
      customerName: this.customerName,
      customerPhone: this.customerPhone,
      customerAddress: this.customerAddress,
      items: this.cart.items,
      totalAmount: this.cart.totalAmount,
      status: 'Pending',
      deliveryAddress: this.customerAddress,
      specialNotes: this.specialNotes
    };

    this.loading = true;
    this.orderService.placeOrder(order).subscribe({
      next: (createdOrder) => {
        this.loading = false;
        alert('Order placed successfully!');
        this.cartService.clearCart();
        this.resetForm();
        this.router.navigate(['/order-tracking'], {
          queryParams: { orderId: createdOrder.id }
        });
      },
      error: (err) => {
        this.loading = false;
        alert(this.getOrderErrorMessage(err));
      }
    });
  }

  private getOrderErrorMessage(error: unknown): string {
    const err = error as HttpErrorResponse;

    if (!err) {
      return 'Failed to place order. Please try again.';
    }

    if (typeof err.error === 'string' && err.error.trim()) {
      return err.error;
    }

    if (err.error && typeof err.error === 'object' && 'message' in err.error) {
      const message = (err.error as { message?: string }).message;
      if (message) {
        return message;
      }
    }

    if (err.status === 0) {
      return `Network error: backend server is not reachable. Please ensure API is running at ${environment.apiUrl}.`;
    }

    return err.message || 'Failed to place order. Please try again.';
  }

  private resetForm(): void {
    this.customerName = '';
    this.customerPhone = '';
    this.customerAddress = '';
    this.specialNotes = '';
  }
}
