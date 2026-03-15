import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-order-tracking',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './order-tracking.component.html',
  styleUrls: ['./order-tracking.component.css']
})
export class OrderTrackingComponent implements OnInit {
  orders: Order[] = [];
  loading = true;
  orderId?: number;

  steps = ['Pending', 'Packed', 'Out for Delivery', 'Delivered'];

  constructor(
    private orderService: OrderService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const idParam = params['orderId'];
      this.orderId = idParam ? Number(idParam) : undefined;
      this.loadOrders();
    });
  }

  loadOrders(): void {
    this.loading = true;
    this.orderService.getOrders().subscribe({
      next: (orders) => {
        const list = this.orderId ? orders.filter(o => o.id === this.orderId) : orders;
        this.orders = list.sort((a, b) => (b.id || 0) - (a.id || 0));
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getStepIndex(status: string): number {
    switch (status) {
      case 'Pending':
      case 'Confirmed':
        return 0;
      case 'Preparing':
      case 'Packed':
      case 'Ready':
        return 1;
      case 'Out for Delivery':
        return 2;
      case 'Delivered':
        return 3;
      default:
        return -1;
    }
  }

  isStepDone(status: string, stepIndex: number): boolean {
    const current = this.getStepIndex(status);
    return current >= stepIndex;
  }
}
