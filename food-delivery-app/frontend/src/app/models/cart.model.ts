export interface CartItem {
  id?: number;
  cartId?: number;
  foodItemId: number;
  foodItem?: { id: number; name: string; imageUrl?: string };
  price: number;
  quantity: number;
}

export interface Cart {
  id?: number;
  userId?: number;
  createdAt?: string;
  cartItems: CartItem[];
}
