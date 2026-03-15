export interface OrderItem {
  foodId: number;
  foodName: string;
  quantity: number;
  price: number;
  subtotal: number;
}

export interface Order {
  id?: number;
  customerId: number;
  customerName: string;
  customerPhone: string;
  customerAddress: string;
  items: OrderItem[];
  totalAmount: number;
  status: 'Pending' | 'Confirmed' | 'Preparing' | 'Packed' | 'Ready' | 'Out for Delivery' | 'Delivered' | 'Cancelled';
  deliveryAddress: string;
  specialNotes?: string;
  createdAt?: Date;
  updatedAt?: Date;
}
