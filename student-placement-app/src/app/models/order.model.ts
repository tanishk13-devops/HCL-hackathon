import { Book } from './book.model';

export interface Order {
  orderId: number;
  userId: number;
  totalAmount: number;
  status: 'Pending' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled';
  orderDate: Date;
  items: OrderItem[];
}

export interface OrderItem {
  orderItemId: number;
  orderId: number;
  bookId: number;
  book?: Book;
  quantity?: number;
  price?: number;
}

export interface CreateOrderRequest {
  cartId?: number;
}

export interface UpdateOrderStatusRequest {
  status: 'Pending' | 'Confirmed' | 'Shipped' | 'Delivered' | 'Cancelled';
}
