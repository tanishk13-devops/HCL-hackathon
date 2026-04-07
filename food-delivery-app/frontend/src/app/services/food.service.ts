import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, catchError, of, shareReplay } from 'rxjs';
import { map } from 'rxjs/operators';
import { Food } from '../models/food.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FoodService {
  private apiUrl = `${environment.apiUrl}/food-items`;
  private foodsSubject = new BehaviorSubject<Food[]>([]);
  public foods$ = this.foodsSubject.asObservable();
  private menuCache = new Map<string, Observable<Food[]>>();
  private readonly fallbackFoods: Food[] = [
    {
      id: 101,
      name: 'Butter Chicken',
      description: 'Classic creamy tomato gravy with tender chicken.',
      price: 349,
      categoryId: 1,
      categoryName: 'Main Course',
      restaurantId: 1,
      imageUrl: 'https://images.pexels.com/photos/7625056/pexels-photo-7625056.jpeg?auto=compress&cs=tinysrgb&w=1200',
      isAvailable: true
    },
    {
      id: 102,
      name: 'Paneer Tikka',
      description: 'Tandoor roasted paneer cubes and peppers.',
      price: 229,
      categoryId: 3,
      categoryName: 'Starters',
      restaurantId: 1,
      imageUrl: 'https://images.unsplash.com/photo-1599487488170-d11ec9c172f0?q=80&w=1200',
      isAvailable: true
    },
    {
      id: 201,
      name: 'Prawn Curry',
      description: 'Coconut-based spicy prawn curry.',
      price: 399,
      categoryId: 1,
      categoryName: 'Main Course',
      restaurantId: 2,
      imageUrl: 'https://images.unsplash.com/photo-1604908176997-4315f57d89b4?q=80&w=1200',
      isAvailable: true
    },
    {
      id: 202,
      name: 'Gulab Jamun',
      description: 'Soft milk dumplings in rose sugar syrup.',
      price: 89,
      categoryId: 2,
      categoryName: 'Desserts',
      restaurantId: 2,
      imageUrl: 'https://images.unsplash.com/photo-1666190092159-3171cf0fbb12?q=80&w=1200',
      isAvailable: true
    },
    {
      id: 301,
      name: 'Veg Biryani',
      description: 'Aromatic basmati rice with vegetables and spices.',
      price: 259,
      categoryId: 1,
      categoryName: 'Main Course',
      restaurantId: 3,
      imageUrl: 'https://images.pexels.com/photos/5410401/pexels-photo-5410401.jpeg?auto=compress&cs=tinysrgb&w=1200',
      isAvailable: true
    },
    {
      id: 302,
      name: 'Chocolate Mousse',
      description: 'Silky smooth dessert topped with dark chocolate.',
      price: 139,
      categoryId: 2,
      categoryName: 'Desserts',
      restaurantId: 3,
      imageUrl: 'https://images.pexels.com/photos/4110008/pexels-photo-4110008.jpeg?auto=compress&cs=tinysrgb&w=1200',
      isAvailable: true
    }
  ];

  constructor(private http: HttpClient) {}

  loadFoods(): void {
    this.getRestaurantMenu(1).subscribe(foods => {
      this.foodsSubject.next(foods);
    });
  }

  getFoods(): Observable<Food[]> { return this.getRestaurantMenu(1); }

  getRestaurantMenu(restaurantId: number, categoryId?: number): Observable<Food[]> {
    const normalizedRestaurantId = this.normalizeRestaurantId(restaurantId);
    const cacheKey = `${normalizedRestaurantId}:${categoryId ?? 'all'}`;

    if (!this.menuCache.has(cacheKey)) {
      const query = categoryId ? `?categoryId=${categoryId}` : '';
      const request$ = this.http.get<Food[]>(`${this.apiUrl}/restaurant/${restaurantId}${query}`).pipe(
        map((foods) => {
          if (foods.length > 0) {
            return foods;
          }

          return this.getFallbackFoods(restaurantId, categoryId);
        }),
        catchError(() => of(this.getFallbackFoods(restaurantId, categoryId))),
        shareReplay(1)
      );

      this.menuCache.set(cacheKey, request$);
    }

    return this.menuCache.get(cacheKey)!;
  }

  private getFallbackFoods(restaurantId: number, categoryId?: number): Food[] {
    const normalizedRestaurantId = this.normalizeRestaurantId(restaurantId);
    let data = this.fallbackFoods.filter(f => f.restaurantId === normalizedRestaurantId);

    if (categoryId) {
      data = data.filter(f => f.categoryId === categoryId);
    }

    return data;
  }

  private normalizeRestaurantId(restaurantId: number): number {
    if (restaurantId >= 10001 && restaurantId <= 19999) {
      return restaurantId - 10000;
    }

    return restaurantId;
  }

  getFoodById(id: number): Observable<Food> {
    return this.http.get<Food>(`${this.apiUrl}/${id}`);
  }

  addFood(food: Food): Observable<Food> {
    return this.http.post<Food>(this.apiUrl, {
      name: food.name,
      description: food.description,
      price: food.price,
      categoryId: food.categoryId,
      restaurantId: food.restaurantId,
      imageUrl: food.imageUrl,
      isAvailable: food.isAvailable ?? true
    });
  }

  updateFood(id: number, food: Food): Observable<Food> {
    return this.http.put<Food>(`${this.apiUrl}/${id}`, {
      name: food.name,
      description: food.description,
      price: food.price,
      categoryId: food.categoryId,
      restaurantId: food.restaurantId,
      imageUrl: food.imageUrl,
      isAvailable: food.isAvailable ?? true
    });
  }

  deleteFood(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getFoodsByCategory(categoryId: number): Observable<Food[]> {
    return this.http.get<Food[]>(`${this.apiUrl}/restaurant/1?categoryId=${categoryId}`);
  }
}
