import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Cart, CartItem } from '../../models/cart.model';
import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {
  cart?: Cart;
  loading = false;
  processingCheckout = false;

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading = true;
    this.cartService.getCart().subscribe({
      next: (cart) => {
        this.cart = cart;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  updateQuantity(item: CartItem, quantity: number): void {
    if (quantity < 1) {
      return;
    }

    this.cartService.updateCartItem(item.cartItemId, { quantity }).subscribe({
      next: () => {
        this.loadCart();
      },
      error: () => {
        alert('Failed to update quantity');
      }
    });
  }

  removeItem(item: CartItem): void {
    if (confirm('Are you sure you want to remove this item from your cart?')) {
      this.cartService.removeCartItem(item.cartItemId).subscribe({
        next: () => {
          this.loadCart();
        },
        error: () => {
          alert('Failed to remove item');
        }
      });
    }
  }

  clearCart(): void {
    if (confirm('Are you sure you want to clear your entire cart?')) {
      this.cartService.clearCart().subscribe({
        next: () => {
          this.loadCart();
        },
        error: () => {
          alert('Failed to clear cart');
        }
      });
    }
  }

  getSubtotal(item: CartItem): number {
    return (item.book?.price || 0) * item.quantity;
  }

  getCartTotal(): number {
    if (!this.cart?.items) return 0;
    return this.cart.items.reduce((total, item) => total + this.getSubtotal(item), 0);
  }

  checkout(): void {
    if (!this.cart?.items || this.cart.items.length === 0) {
      return;
    }

    this.processingCheckout = true;
    this.orderService.createOrder({ cartId: this.cart.cartId }).subscribe({
      next: (order) => {
        alert('Order placed successfully!');
        this.router.navigate(['/orders', order.orderId]);
      },
      error: () => {
        alert('Failed to create order');
        this.processingCheckout = false;
      }
    });
  }
}
