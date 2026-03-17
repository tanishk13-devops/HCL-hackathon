import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { RestaurantService } from '../../services/restaurant.service';
import { FoodService } from '../../services/food.service';
import { Restaurant } from '../../models/restaurant.model';
import { Food } from '../../models/food.model';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  title = 'Ziggy';
  animatedHeroText = 'Hot meals from your favorite restaurants, delivered in minutes.';
  readonly heroText = 'Hot meals from your favorite restaurants, delivered in minutes.';

  featuredRestaurants: Restaurant[] = [];
  trendingFoods: Food[] = [];
  loadingFeatured = true;
  loadingTrending = true;

  restaurantsCount = 0;
  deliveredOrdersCount = 0;
  activeSlide = 0;
  apiError = '';
  readonly foodPlaceholder = 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=1200&q=80';

  features = [
    'Fresh, chef-crafted meals every day',
    'Fast delivery with live order tracking',
    'Secure, simple checkout experience',
    'Curated menus with real food photos'
  ];

  constructor(
    private restaurantService: RestaurantService,
    private foodService: FoodService
  ) {}

  ngOnInit(): void {
    this.loadFeaturedRestaurants();
  }

  ngOnDestroy(): void {
    // no-op
  }

  nextSlide(): void {
    if (!this.trendingFoods.length) return;
    this.activeSlide = (this.activeSlide + 1) % this.trendingFoods.length;
  }

  prevSlide(): void {
    if (!this.trendingFoods.length) return;
    this.activeSlide = this.activeSlide === 0 ? this.trendingFoods.length - 1 : this.activeSlide - 1;
  }

  goToSlide(index: number): void {
    this.activeSlide = index;
  }

  getFoodImageUrl(food: Food): string {
    if (!food?.name?.trim()) {
      return this.foodPlaceholder;
    }

    const dishSlug = food.name
      .toLowerCase()
      .trim()
      .replace(/[^a-z0-9\s-]/g, '')
      .replace(/\s+/g, '-');

    const signature = this.getFoodSignature(food);
    return `https://source.unsplash.com/400x300/?${encodeURIComponent(dishSlug)}&sig=${signature}`;
  }

  onFoodImageError(event: Event): void {
    (event.target as HTMLImageElement).src = this.foodPlaceholder;
  }

  private getFoodSignature(food: Food): number {
    const seed = `${food.id}-${food.restaurantId}-${food.name}`;
    let hash = 0;

    for (let i = 0; i < seed.length; i++) {
      hash = ((hash << 5) - hash) + seed.charCodeAt(i);
      hash |= 0;
    }

    return Math.abs(hash % 100000);
  }

  private loadFeaturedRestaurants(): void {
    this.restaurantService.getRestaurants().subscribe({
      next: (data) => {
        this.apiError = '';
        this.featuredRestaurants = data.slice(0, 6);
        this.loadingFeatured = false;
        this.animateCounter('restaurants', data.length);
        this.animateCounter('orders', Math.max(1800, data.length * 320));
        this.loadTrendingFoods(this.featuredRestaurants.map(r => r.id).slice(0, 3));
      },
      error: () => {
        this.loadingFeatured = false;
        this.loadingTrending = false;
        this.apiError = 'Backend is not reachable. Please start API server and refresh.';
      }
    });
  }

  private loadTrendingFoods(restaurantIds: number[]): void {
    if (!restaurantIds.length) {
      this.loadingTrending = false;
      return;
    }

    const requests = restaurantIds.map(id => this.foodService.getRestaurantMenu(id).pipe(catchError(() => of([] as Food[]))));
    forkJoin(requests).subscribe({
      next: (foodGroups) => {
        this.trendingFoods = foodGroups
          .flat()
          .filter(f => f.isAvailable !== false)
          .slice(0, 10);

        this.loadingTrending = false;
      },
      error: () => {
        this.loadingTrending = false;
      }
    });
  }

  private animateCounter(type: 'restaurants' | 'orders', target: number): void {
    const duration = 1400;
    const steps = 40;
    const increment = Math.ceil(target / steps);
    let current = 0;

    const timer = setInterval(() => {
      current = Math.min(current + increment, target);
      if (type === 'restaurants') {
        this.restaurantsCount = current;
      } else {
        this.deliveredOrdersCount = current;
      }

      if (current >= target) {
        clearInterval(timer);
      }
    }, Math.floor(duration / steps));
  }
}
