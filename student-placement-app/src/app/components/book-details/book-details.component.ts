import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Book } from '../../models/book.model';
import { Review, CreateReviewRequest } from '../../models/review.model';
import { BookService } from '../../services/book.service';
import { CartService } from '../../services/cart.service';
import { ReviewService } from '../../services/review.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-book-details',
  templateUrl: './book-details.component.html',
  styleUrls: ['./book-details.component.css']
})
export class BookDetailsComponent implements OnInit {
  book?: Book;
  reviews: Review[] = [];
  loading = false;
  quantity = 1;
  
  // Review form
  showReviewForm = false;
  newReview: CreateReviewRequest = {
    bookId: 0,
    rating: 5,
    comment: ''
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService,
    private cartService: CartService,
    private reviewService: ReviewService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    const bookId = Number(this.route.snapshot.paramMap.get('id'));
    if (bookId) {
      this.loadBook(bookId);
      this.loadReviews(bookId);
    }
  }

  loadBook(id: number): void {
    this.loading = true;
    this.bookService.getBookById(id).subscribe({
      next: (book) => {
        this.book = book;
        this.newReview.bookId = book.bookId;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/books']);
      }
    });
  }

  loadReviews(bookId: number): void {
    this.reviewService.getReviewsByBook(bookId).subscribe({
      next: (reviews) => {
        this.reviews = reviews;
      }
    });
  }

  addToCart(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    if (!this.book || this.book.stockQuantity <= 0) {
      return;
    }

    this.cartService.addToCart({ 
      bookId: this.book.bookId, 
      quantity: this.quantity 
    }).subscribe({
      next: () => {
        alert('Book added to cart successfully!');
      },
      error: () => {
        alert('Failed to add book to cart');
      }
    });
  }

  submitReview(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    if (!this.newReview.comment.trim()) {
      alert('Please enter a comment');
      return;
    }

    this.reviewService.createReview(this.newReview).subscribe({
      next: () => {
        alert('Review submitted successfully!');
        this.showReviewForm = false;
        this.newReview.comment = '';
        this.newReview.rating = 5;
        if (this.book) {
          this.loadReviews(this.book.bookId);
          this.loadBook(this.book.bookId);
        }
      },
      error: () => {
        alert('Failed to submit review');
      }
    });
  }

  getStarsArray(rating: number): boolean[] {
    return Array(5).fill(false).map((_, i) => i < rating);
  }
}
