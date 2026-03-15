export interface OrderItem {
  id?: number;
  foodItemId: number;
  foodItem?: { id: number; name: string; imageUrl?: string };
  quantity: number;
  price: number;
}

export interface Order {
  id?: number;
  userId: number;
  addressId: number;
  items: OrderItem[];
  totalAmount: number;
  status: 'Pending' | 'Accepted' | 'Preparing' | 'OutForDelivery' | 'Delivered';
  address?: { street: string; city: string; state: string; pincode: string };
  payment?: { paymentMethod: string; paymentStatus: string; amount: number };
  createdAt?: Date;
}
