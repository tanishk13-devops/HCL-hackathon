import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, catchError, shareReplay, tap, throwError } from 'rxjs';
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

  constructor(private http: HttpClient) {}

  loadFoods(): void {
    this.getRestaurantMenu(1).subscribe(foods => {
      this.foodsSubject.next(foods);
    });
  }

  getFoods(): Observable<Food[]> { return this.getRestaurantMenu(1); }

  getRestaurantMenu(restaurantId: number, categoryId?: number): Observable<Food[]> {
    const cacheKey = `${restaurantId}:${categoryId ?? 'all'}`;

    if (!this.menuCache.has(cacheKey)) {
      const query = categoryId ? `?categoryId=${categoryId}` : '';
      const request$ = this.http.get<Food[]>(`${this.apiUrl}/restaurant/${restaurantId}${query}`).pipe(
        catchError(error => {
          this.menuCache.delete(cacheKey);
          return throwError(() => error);
        }),
        shareReplay(1)
      );

      this.menuCache.set(cacheKey, request$);
    }

    return this.menuCache.get(cacheKey)!;
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
    }).pipe(
      tap(() => this.menuCache.clear())
    );
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
    }).pipe(
      tap(() => this.menuCache.clear())
    );
  }

  deleteFood(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.menuCache.clear())
    );
  }

  getFoodsByCategory(categoryId: number): Observable<Food[]> {
    return this.http.get<Food[]>(`${this.apiUrl}/restaurant/1?categoryId=${categoryId}`);
  }
}
