import { Component, OnInit } from '@angular/core';
import { Book } from '../../models/book.model';
import { BookService } from '../../services/book.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.css']
})
export class BookListComponent implements OnInit {
  books: Book[] = [];
  filteredBooks: Book[] = [];
  loading = false;
  searchTerm = '';
  selectedAuthor = '';
  minPrice?: number;
  maxPrice?: number;
  authors: string[] = [];

  constructor(
    private bookService: BookService,
    private cartService: CartService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.loading = true;
    this.bookService.getAllBooks().subscribe({
      next: (books) => {
        this.books = books;
        this.filteredBooks = books;
        this.authors = [...new Set(books.map(b => b.author))];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredBooks = this.books.filter(book => {
      const matchesSearch = !this.searchTerm || 
        book.title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        book.author.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      const matchesAuthor = !this.selectedAuthor || book.author === this.selectedAuthor;
      
      const matchesMinPrice = this.minPrice === undefined || book.price >= this.minPrice;
      const matchesMaxPrice = this.maxPrice === undefined || book.price <= this.maxPrice;

      return matchesSearch && matchesAuthor && matchesMinPrice && matchesMaxPrice;
    });
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.selectedAuthor = '';
    this.minPrice = undefined;
    this.maxPrice = undefined;
    this.filteredBooks = this.books;
  }

  addToCart(book: Book): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/books' } });
      return;
    }

    if (book.stockQuantity <= 0) {
      alert('This book is out of stock');
      return;
    }

    this.cartService.addToCart({ bookId: book.bookId, quantity: 1 }).subscribe({
      next: () => {
        alert('Book added to cart successfully!');
      },
      error: () => {
        alert('Failed to add book to cart');
      }
    });
  }

  viewDetails(bookId: number): void {
    this.router.navigate(['/books', bookId]);
  }
}
