import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Cart, CartItem } from '../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartSubject = new BehaviorSubject<Cart>({
    items: [],
    totalItems: 0,
    totalAmount: 0
  });
  public cart$ = this.cartSubject.asObservable();

  constructor() {
    this.loadCart();
  }

  private loadCart(): void {
    const savedCart = localStorage.getItem('cart');
    if (savedCart) {
      this.cartSubject.next(JSON.parse(savedCart));
    }
  }

  private saveCart(): void {
    localStorage.setItem('cart', JSON.stringify(this.cartSubject.value));
  }

  addToCart(item: CartItem): void {
    const cart = this.cartSubject.value;
    const existingItem = cart.items.find(i => i.foodId === item.foodId);

    if (existingItem) {
      existingItem.quantity += item.quantity;
      existingItem.subtotal = existingItem.price * existingItem.quantity;
    } else {
      cart.items.push(item);
    }

    this.updateCartTotals(cart);
    this.cartSubject.next(cart);
    this.saveCart();
  }

  removeFromCart(foodId: number): void {
    const cart = this.cartSubject.value;
    cart.items = cart.items.filter(i => i.foodId !== foodId);
    this.updateCartTotals(cart);
    this.cartSubject.next(cart);
    this.saveCart();
  }

  updateQuantity(foodId: number, quantity: number): void {
    const cart = this.cartSubject.value;
    const item = cart.items.find(i => i.foodId === foodId);

    if (item) {
      item.quantity = quantity;
      item.subtotal = item.price * quantity;
      this.updateCartTotals(cart);
      this.cartSubject.next(cart);
      this.saveCart();
    }
  }

  clearCart(): void {
    const emptyCart: Cart = {
      items: [],
      totalItems: 0,
      totalAmount: 0
    };
    this.cartSubject.next(emptyCart);
    localStorage.removeItem('cart');
  }

  private updateCartTotals(cart: Cart): void {
    cart.totalItems = cart.items.reduce((sum, item) => sum + item.quantity, 0);
    cart.totalAmount = cart.items.reduce((sum, item) => sum + item.subtotal, 0);
  }

  getCart(): Observable<Cart> {
    return this.cart$;
  }
}
