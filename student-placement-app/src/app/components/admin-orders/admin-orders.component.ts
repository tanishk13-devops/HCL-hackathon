import { Component, OnInit } from '@angular/core';
import { Order } from '../../models/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-admin-orders',
  templateUrl: './admin-orders.component.html',
  styleUrls: ['./admin-orders.component.css']
})
export class AdminOrdersComponent implements OnInit {
  orders: Order[] = [];
  filteredOrders: Order[] = [];
  loading = false;
  selectedStatus = '';

  statuses = ['All', 'Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'];

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.orderService.getAllOrders().subscribe({
      next: (orders) => {
        this.orders = orders.sort((a, b) => 
          new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime()
        );
        this.filteredOrders = this.orders;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  filterByStatus(status: string): void {
    this.selectedStatus = status;
    if (status === 'All' || status === '') {
      this.filteredOrders = this.orders;
    } else {
      this.filteredOrders = this.orders.filter(o => o.status === status);
    }
  }

  updateOrderStatus(order: Order, newStatus: string): void {
    this.orderService.updateOrderStatus(order.orderId, { 
      status: newStatus as any 
    }).subscribe({
      next: () => {
        alert('Order status updated successfully');
        this.loadOrders();
      },
      error: () => {
        alert('Failed to update order status');
      }
    });
  }

  getStatusClass(status: string): string {
    return 'status-' + status.toLowerCase();
  }
}
