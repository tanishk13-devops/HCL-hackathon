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

  newAddress = { street: '', city: '', state: '', pincode: '' };

  constructor(
    private cartService: CartService,
    private addressService: AddressService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cartService.loadCart().subscribe({ next: c => this.cart = c });
    this.loadAddresses();
  }

  loadAddresses(): void {
    this.addressService.getAddresses().subscribe({
      next: (a) => {
        this.addresses = a;
        this.selectedAddressId = a[0]?.id;
      }
    });
  }

  addAddress(): void {
    this.addressService.addAddress(this.newAddress).subscribe({
      next: () => {
        this.newAddress = { street: '', city: '', state: '', pincode: '' };
        this.loadAddresses();
      }
    });
  }

  placeOrder(): void {
    if (!this.selectedAddressId) return;

    this.loading = true;
    this.orderService.placeOrder(this.selectedAddressId, this.paymentMethod).subscribe({
      next: (order) => {
        this.loading = false;
        this.router.navigate(['/order-tracking'], { queryParams: { orderId: order.id } });
      },
      error: () => this.loading = false
    });
  }

  get total(): number {
    return this.cart.cartItems.reduce((sum, i) => sum + (i.price * i.quantity), 0);
  }
}
