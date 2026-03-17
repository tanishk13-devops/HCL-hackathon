import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FoodService } from '../../services/food.service';
import { CartService } from '../../services/cart.service';
import { Food } from '../../models/food.model';
import { CartItem } from '../../models/cart.model';
import { FoodCardComponent } from '../food-card/food-card.component';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, FoodCardComponent],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {
  restaurantId = 1;
  foods: Food[] = [];
  categories: string[] = [];
  selectedCategory = '';
  searchTerm = '';
  sortBy: 'recommended' | 'priceLowToHigh' | 'priceHighToLow' | 'nameAsc' = 'recommended';
  filteredFoods: Food[] = [];
  loading = true;
  readonly skeletonItems = Array.from({ length: 8 });
  addSuccessMessage = '';

  constructor(
    private foodService: FoodService,
    private cartService: CartService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.restaurantId = Number(params.get('restaurantId') || 1);
      this.loadFoods();
    });
  }

  loadFoods(): void {
    this.foodService.getRestaurantMenu(this.restaurantId).subscribe({
      next: (foods) => {
        this.foods = foods;
        this.extractCategories();
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  extractCategories(): void {
    this.categories = [...new Set(this.foods.map(f => f.category?.name || f.categoryName).filter(Boolean) as string[])];
  }

  filterByCategory(): void {
    this.applyFilters();
  }

  onCategoryChange(): void {
    this.filterByCategory();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onSortChange(): void {
    this.applyFilters();
  }

  clearFilters(): void {
    this.selectedCategory = '';
    this.searchTerm = '';
    this.sortBy = 'recommended';
    this.applyFilters();
  }

  private applyFilters(): void {
    let list = [...this.foods];

    if (this.selectedCategory) {
      list = list.filter(f => (f.category?.name || f.categoryName) === this.selectedCategory);
    }

    const search = this.searchTerm.trim().toLowerCase();
    if (search) {
      list = list.filter(f =>
        f.name.toLowerCase().includes(search) ||
        (f.description || '').toLowerCase().includes(search));
    }

    switch (this.sortBy) {
      case 'priceLowToHigh':
        list.sort((a, b) => a.price - b.price);
        break;
      case 'priceHighToLow':
        list.sort((a, b) => b.price - a.price);
        break;
      case 'nameAsc':
        list.sort((a, b) => a.name.localeCompare(b.name));
        break;
      default:
        break;
    }

    this.filteredFoods = list;
  }

  addToCart(food: Food): void {
    if (food.id) {
      const cartItem: CartItem = {
        foodItemId: food.id,
        price: food.price,
        quantity: 1
      };
      this.cartService.addToCart(cartItem).subscribe({
        next: () => {
          this.addSuccessMessage = `${food.name} added to cart`;
          setTimeout(() => (this.addSuccessMessage = ''), 1200);
        },
        error: (err) => {
          if (err?.status === 401) {
            this.router.navigate(['/login']);
            return;
          }
        }
      });
    }
  }
}
