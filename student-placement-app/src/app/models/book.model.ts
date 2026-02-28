export interface Book {
  bookId: number;
  title: string;
  author: string;
  price: number;
  description: string;
  stockQuantity: number;
  createdAt: Date;
  imageUrl?: string;
  averageRating?: number;
  reviewCount?: number;
}

export interface CreateBookRequest {
  title: string;
  author: string;
  price: number;
  description: string;
  stockQuantity: number;
  imageUrl?: string;
}
