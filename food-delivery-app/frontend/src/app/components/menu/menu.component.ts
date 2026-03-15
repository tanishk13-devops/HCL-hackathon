import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FoodService } from '../../services/food.service';
import { CartService } from '../../services/cart.service';
import { Food } from '../../models/food.model';
import { CartItem } from '../../models/cart.model';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit {
  foods: Food[] = [];
  categories: string[] = [];
  selectedCategory: string = '';
  filteredFoods: Food[] = [];
  loading = true;

  constructor(
    private foodService: FoodService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.loadFoods();
  }

  loadFoods(): void {
    this.foodService.getFoods().subscribe({
      next: (foods) => {
        this.foods = foods;
        this.extractCategories();
        this.filterByCategory();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  extractCategories(): void {
    this.categories = [...new Set(this.foods.map(f => f.category))];
  }

  filterByCategory(): void {
    if (this.selectedCategory) {
      this.filteredFoods = this.foods.filter(f => f.category === this.selectedCategory);
    } else {
      this.filteredFoods = this.foods;
    }
  }

  onCategoryChange(): void {
    this.filterByCategory();
  }

  getFoodImage(food: Food): string {
    const imageMap: Record<string, string> = {
      biryani: 'https://images.pexels.com/photos/12737656/pexels-photo-12737656.jpeg?auto=compress&cs=tinysrgb&w=1200',
      'butter chicken': 'https://images.pexels.com/photos/7625056/pexels-photo-7625056.jpeg?auto=compress&cs=tinysrgb&w=1200',
      'paneer tikka': 'https://images.pexels.com/photos/5410400/pexels-photo-5410400.jpeg?auto=compress&cs=tinysrgb&w=1200',
      'tandoori chicken': 'https://images.pexels.com/photos/616354/pexels-photo-616354.jpeg?auto=compress&cs=tinysrgb&w=1200',
      'garlic naan': 'https://images.pexels.com/photos/9797029/pexels-photo-9797029.jpeg?auto=compress&cs=tinysrgb&w=1200',
      samosa: 'https://www.cubesnjuliennes.com/wp-content/uploads/2020/08/Best-Indian-Punjabi-Samosa-Recipe.jpg',
      'dal makhani': 'https://sinfullyspicy.com/wp-content/uploads/2015/03/3-1.jpg',
      'chole bhature': 'https://madhurasrecipe.com/wp-content/uploads/2025/09/MR-Chole-Bhature-featured.jpg',
      'shahi tukda': 'https://palatesdesire.com/wp-content/uploads/2021/05/shahi-tukda-recipe@palates-desire-1.jpg',
      'gulab jamun': 'https://images.pexels.com/photos/3026808/pexels-photo-3026808.jpeg?auto=compress&cs=tinysrgb&w=1200'
    };

    const key = (food.name || '')
      .trim()
      .toLowerCase()
      .replace(/-/g, ' ')
      .replace(/\s+/g, ' ');

    // Prefer image URL coming from API/DB so admin or DB updates are reflected immediately.
    return food.imageUrl || imageMap[key] || 'https://via.placeholder.com/800x600?text=Ziggy+Food';
  }

  addToCart(food: Food): void {
    if (food.id) {
      const cartItem: CartItem = {
        foodId: food.id,
        foodName: food.name,
        price: food.price,
        quantity: 1,
        subtotal: food.price,
        imageUrl: this.getFoodImage(food)
      };
      this.cartService.addToCart(cartItem);
      alert(`${food.name} added to cart!`);
    }
  }
}
