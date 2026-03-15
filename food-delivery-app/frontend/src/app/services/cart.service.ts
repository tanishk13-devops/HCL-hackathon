import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Cart, CartItem } from '../models/cart.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = `${environment.apiUrl}/cart`;
  private cartSubject = new BehaviorSubject<Cart>({ cartItems: [] });
  public cart$ = this.cartSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadCart(): Observable<Cart> {
    return this.http.get<Cart>(this.apiUrl).pipe(tap(cart => this.cartSubject.next(cart)));
  }

  addToCart(item: CartItem): Observable<Cart> {
    return this.http.post<Cart>(`${this.apiUrl}/items`, {
      foodItemId: item.foodItemId,
      quantity: item.quantity
    }).pipe(tap(cart => this.cartSubject.next(cart)));
  }

  removeFromCart(foodItemId: number): Observable<Cart> {
    return this.http.delete<Cart>(`${this.apiUrl}/items/${foodItemId}`).pipe(tap(cart => this.cartSubject.next(cart)));
  }

  updateQuantity(foodItemId: number, quantity: number): Observable<Cart> {
    return this.http.put<Cart>(`${this.apiUrl}/items/${foodItemId}`, { quantity }).pipe(tap(cart => this.cartSubject.next(cart)));
  }

  clearCart(): void {
    this.cartSubject.next({ cartItems: [] });
  }

  getCart(): Observable<Cart> {
    return this.cart$;
  }
}
