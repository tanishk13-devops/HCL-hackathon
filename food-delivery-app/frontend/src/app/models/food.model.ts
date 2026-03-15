export interface Food {
  id?: number;
  name: string;
  description: string;
  price: number;
  categoryId?: number;
  category?: { id: number; name: string };
  restaurantId?: number;
  imageUrl?: string;
  categoryName?: string;
  isAvailable?: boolean;
  createdAt?: Date;
}
