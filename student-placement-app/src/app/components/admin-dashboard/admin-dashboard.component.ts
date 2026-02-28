import { Component, OnInit } from '@angular/core';
import { Book } from '../../models/book.model';
import { Order } from '../../models/order.model';
import { BookService } from '../../services/book.service';
import { OrderService } from '../../services/order.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  stats = {
    totalBooks: 0,
    totalOrders: 0,
    pendingOrders: 0,
    totalRevenue: 0
  };

  recentOrders: Order[] = [];
  loading = false;

  constructor(
    private bookService: BookService,
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading = true;

    this.bookService.getAllBooks().subscribe({
      next: (books) => {
        this.stats.totalBooks = books.length;
      }
    });

    this.orderService.getAllOrders().subscribe({
      next: (orders) => {
        this.stats.totalOrders = orders.length;
        this.stats.pendingOrders = orders.filter(o => o.status === 'Pending').length;
        this.stats.totalRevenue = orders
          .filter(o => o.status !== 'Cancelled')
          .reduce((sum, o) => sum + o.totalAmount, 0);
        
        this.recentOrders = orders
          .sort((a, b) => new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime())
          .slice(0, 5);
        
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  navigateToBooks(): void {
    this.router.navigate(['/admin/books']);
  }

  navigateToOrders(): void {
    this.router.navigate(['/admin/orders']);
  }
}
