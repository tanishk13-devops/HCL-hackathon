import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FoodService } from '../../services/food.service';
import { OrderService } from '../../services/order.service';
import { RestaurantService } from '../../services/restaurant.service';
import { AdminService } from '../../services/admin.service';
import { Food } from '../../models/food.model';
import { Order } from '../../models/order.model';
import { Restaurant } from '../../models/restaurant.model';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  dashboard: any = null;
  restaurants: Restaurant[] = [];
  foods: Food[] = [];
  orders: Order[] = [];
  customerActivity: any[] = [];
  activeTab: 'dashboard' | 'restaurants' | 'foods' | 'orders' | 'customers' = 'dashboard';

  newRestaurant = {
    name: '', description: '', location: '', rating: 4.2, imageUrl: ''
  };
  
  newFood: Food = {
    name: '',
    description: '',
    price: 0,
    categoryId: 1,
    restaurantId: 1,
    isAvailable: true
  };
  
  loading = false;

  constructor(
    private foodService: FoodService,
    private orderService: OrderService,
    private restaurantService: RestaurantService,
    private adminService: AdminService
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
    this.loadRestaurants();
    this.loadFoods();
    this.loadOrders();
    this.loadCustomerActivity();
  }

  loadDashboard(): void {
    this.adminService.getDashboard().subscribe({ next: d => this.dashboard = d });
  }

  loadRestaurants(): void {
    this.restaurantService.getRestaurants().subscribe({ next: r => this.restaurants = r });
  }

  loadFoods(): void {
    this.foodService.getRestaurantMenu(this.newFood.restaurantId || 1).subscribe({
      next: (foods) => {
        this.foods = foods;
      }
    });
  }

  loadOrders(): void {
    this.orderService.getAllOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
      }
    });
  }

  loadCustomerActivity(): void {
    this.adminService.getCustomerActivity().subscribe({ next: c => this.customerActivity = c });
  }

  addRestaurant(): void {
    this.restaurantService.createRestaurant(this.newRestaurant).subscribe({
      next: () => {
        this.newRestaurant = { name: '', description: '', location: '', rating: 4.2, imageUrl: '' };
        this.loadRestaurants();
      }
    });
  }

  deleteRestaurant(id: number): void {
    this.restaurantService.deleteRestaurant(id).subscribe({ next: () => this.loadRestaurants() });
  }

  addFood(): void {
    if (!this.newFood.name || !this.newFood.description || !this.newFood.categoryId || !this.newFood.restaurantId) {
      alert('Please fill in all required fields');
      return;
    }

    this.loading = true;
    this.foodService.addFood(this.newFood).subscribe({
      next: () => {
        this.loading = false;
        alert('Food added successfully!');
        this.newFood = { name: '', description: '', price: 0, categoryId: 1, restaurantId: this.newFood.restaurantId, isAvailable: true };
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
      case 'Accepted': return '#2196f3';
      case 'Preparing': return '#9c27b0';
      case 'OutForDelivery': return '#00acc1';
      case 'Delivered': return '#2e7d32';
      default: return '#999';
    }
  }
}
