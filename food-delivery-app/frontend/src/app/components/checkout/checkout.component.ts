import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Cart } from '../../models/cart.model';
import { Address } from '../../models/address.model';
import { CartService } from '../../services/cart.service';
import { AddressService } from '../../services/address.service';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {
  cart: Cart = { cartItems: [] };
  addresses: Address[] = [];
  selectedAddressId?: number;
  paymentMethod = 'CashOnDelivery';
  loading = false;
  error = '';

  newAddress = { street: '', city: '', state: '', pincode: '' };

  constructor(
    private cartService: CartService,
    private addressService: AddressService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cartService.loadCart().subscribe({
      next: c => this.cart = c,
      error: () => {
        this.error = 'Unable to load cart. Please login and retry.';
      }
    });
    this.loadAddresses();
  }

  loadAddresses(): void {
    this.addressService.getAddresses().subscribe({
      next: (a) => {
        this.addresses = a;
        this.selectedAddressId = a[0]?.id;
      },
      error: () => {
        this.error = 'Unable to load addresses. Please login first.';
      }
    });
  }

  addAddress(): void {
    this.addressService.addAddress(this.newAddress).subscribe({
      next: () => {
        this.newAddress = { street: '', city: '', state: '', pincode: '' };
        this.loadAddresses();
      },
      error: () => {
        this.error = 'Could not add address. Check your session and try again.';
      }
    });
  }

  placeOrder(): void {
    if (!this.selectedAddressId) {
      this.error = 'Please select or add a delivery address before placing the order.';
      return;
    }

    if (!this.cart.cartItems.length) {
      this.error = 'Your cart is empty. Add items before placing an order.';
      return;
    }

    this.loading = true;
    this.error = '';
    this.orderService.placeOrder(this.selectedAddressId, this.paymentMethod).subscribe({
      next: (order) => {
        this.loading = false;
        this.router.navigate(['/order-tracking'], { queryParams: { orderId: order.id } });
      },
      error: (err) => {
        this.loading = false;
        if (err?.status === 401) {
          this.error = 'Session expired. Please login again.';
          this.router.navigate(['/login']);
          return;
        }

        if (err?.status === 403) {
          this.error = 'Your role is not allowed to place orders. Login as Customer/Admin.';
          return;
        }

        this.error = typeof err?.error === 'string'
          ? err.error
          : (err?.error?.message || 'Failed to place order. Please ensure you are logged in and backend is running.');
      }
    });
  }

  get total(): number {
    return this.cart.cartItems.reduce((sum, i) => sum + (i.price * i.quantity), 0);
  }
}
