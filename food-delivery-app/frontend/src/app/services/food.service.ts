import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Food } from '../models/food.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FoodService {
  private apiUrl = `${environment.apiUrl}/foods`;
  private foodsSubject = new BehaviorSubject<Food[]>([]);
  public foods$ = this.foodsSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadFoods();
  }

  loadFoods(): void {
    this.getFoods().subscribe(foods => {
      this.foodsSubject.next(foods);
    });
  }

  getFoods(): Observable<Food[]> {
    return this.http.get<Food[]>(this.apiUrl);
  }

  getFoodById(id: number): Observable<Food> {
    return this.http.get<Food>(`${this.apiUrl}/${id}`);
  }

  addFood(food: Food): Observable<Food> {
    return this.http.post<Food>(this.apiUrl, food);
  }

  updateFood(id: number, food: Food): Observable<Food> {
    return this.http.put<Food>(`${this.apiUrl}/${id}`, food);
  }

  deleteFood(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getFoodsByCategory(category: string): Observable<Food[]> {
    return this.http.get<Food[]>(`${this.apiUrl}/category/${category}`);
  }
}
