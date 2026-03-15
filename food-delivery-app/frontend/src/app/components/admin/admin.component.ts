import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FoodService } from '../../services/food.service';
import { OrderService } from '../../services/order.service';
import { Food } from '../../models/food.model';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  foods: Food[] = [];
  orders: Order[] = [];
  activeTab: 'foods' | 'orders' = 'foods';
  
  newFood: Food = {
    name: '',
    description: '',
    price: 0,
    category: '',
    availability: true
  };
  
  loading = false;

  constructor(
    private foodService: FoodService,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    this.loadFoods();
    this.loadOrders();
  }

  loadFoods(): void {
    this.foodService.getFoods().subscribe({
      next: (foods) => {
        this.foods = foods;
      }
    });
  }

  loadOrders(): void {
    this.orderService.getOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
      }
    });
  }

  addFood(): void {
    if (!this.newFood.name || !this.newFood.description || !this.newFood.category) {
      alert('Please fill in all required fields');
      return;
    }

    this.loading = true;
    this.foodService.addFood(this.newFood).subscribe({
      next: () => {
        this.loading = false;
        alert('Food added successfully!');
        this.newFood = { name: '', description: '', price: 0, category: '', availability: true };
        this.loadFoods();
      },
      error: () => {
        this.loading = false;
        alert('Failed to add food');
      }
    });
  }

  deleteFood(id: number | undefined): void {
    if (!id) return;
    if (confirm('Are you sure you want to delete this food item?')) {
      this.foodService.deleteFood(id).subscribe({
        next: () => {
          alert('Food deleted successfully!');
          this.loadFoods();
        },
        error: () => {
          alert('Failed to delete food');
        }
      });
    }
  }

  updateOrderStatus(orderId: number | undefined, newStatus: string): void {
    if (!orderId) return;
    this.orderService.updateOrderStatus(orderId, newStatus).subscribe({
      next: () => {
        alert('Order status updated!');
        this.loadOrders();
      },
      error: () => {
        alert('Failed to update order status');
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return '#ff9800';
      case 'Confirmed': return '#2196f3';
      case 'Preparing': return '#9c27b0';
      case 'Ready': return '#4caf50';
      case 'Delivered': return '#2e7d32';
      case 'Cancelled': return '#f44336';
      default: return '#999';
    }
  }
}
