import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Cart, CartItem, AddToCartRequest, UpdateCartItemRequest } from '../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = 'http://localhost:5000/api/cart'; // Update with your backend URL
  private cartItemCountSubject = new BehaviorSubject<number>(0);
  public cartItemCount$ = this.cartItemCountSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadCartItemCount();
  }

  getCart(): Observable<Cart> {
    return this.http.get<Cart>(this.apiUrl)
      .pipe(
        tap(cart => this.updateCartItemCount(cart))
      );
  }

  addToCart(request: AddToCartRequest): Observable<CartItem> {
    return this.http.post<CartItem>(`${this.apiUrl}/items`, request)
      .pipe(
        tap(() => this.loadCartItemCount())
      );
  }

  updateCartItem(itemId: number, request: UpdateCartItemRequest): Observable<CartItem> {
    return this.http.put<CartItem>(`${this.apiUrl}/items/${itemId}`, request)
      .pipe(
        tap(() => this.loadCartItemCount())
      );
  }

  removeCartItem(itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/items/${itemId}`)
      .pipe(
        tap(() => this.loadCartItemCount())
      );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>(this.apiUrl)
      .pipe(
        tap(() => this.cartItemCountSubject.next(0))
      );
  }

  private loadCartItemCount(): void {
    this.getCart().subscribe(
      cart => this.updateCartItemCount(cart),
      () => this.cartItemCountSubject.next(0)
    );
  }

  private updateCartItemCount(cart: Cart): void {
    const count = cart.items?.reduce((sum, item) => sum + item.quantity, 0) || 0;
    this.cartItemCountSubject.next(count);
  }

  getCartItemCount(): number {
    return this.cartItemCountSubject.value;
  }
}
