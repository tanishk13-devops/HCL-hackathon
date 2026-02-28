import { Book } from './book.model';

export interface Cart {
  cartId: number;
  userId: number;
  createdAt: Date;
  items: CartItem[];
}

export interface CartItem {
  cartItemId: number;
  cartId: number;
  bookId: number;
  book?: Book;
  quantity: number;
}

export interface AddToCartRequest {
  bookId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}
