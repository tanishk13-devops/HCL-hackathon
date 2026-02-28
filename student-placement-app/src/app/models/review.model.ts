export interface Review {
  reviewId: number;
  userId: number;
  bookId: number;
  rating: number;
  comment: string;
  createdAt: Date;
  userFullName?: string;
}

export interface CreateReviewRequest {
  bookId: number;
  rating: number;
  comment: string;
}
