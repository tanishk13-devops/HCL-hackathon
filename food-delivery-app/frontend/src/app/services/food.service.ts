import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Food } from '../models/food.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FoodService {
  private apiUrl = `${environment.apiUrl}/food-items`;
  private foodsSubject = new BehaviorSubject<Food[]>([]);
  public foods$ = this.foodsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadFoods();
  }

  loadFoods(): void {
    this.getRestaurantMenu(1).subscribe(foods => {
      this.foodsSubject.next(foods);
    });
  }

  getFoods(): Observable<Food[]> { return this.getRestaurantMenu(1); }

  getRestaurantMenu(restaurantId: number, categoryId?: number): Observable<Food[]> {
    const query = categoryId ? `?categoryId=${categoryId}` : '';
    return this.http.get<Food[]>(`${this.apiUrl}/restaurant/${restaurantId}${query}`);
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
