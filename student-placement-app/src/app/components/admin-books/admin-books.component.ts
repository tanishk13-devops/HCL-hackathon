import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Book, CreateBookRequest } from '../../models/book.model';
import { BookService } from '../../services/book.service';

@Component({
  selector: 'app-admin-books',
  templateUrl: './admin-books.component.html',
  styleUrls: ['./admin-books.component.css']
})
export class AdminBooksComponent implements OnInit {
  books: Book[] = [];
  loading = false;
  showForm = false;
  editingBook?: Book;
  bookForm!: FormGroup;

  constructor(
    private bookService: BookService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.loadBooks();
  }

  initializeForm(): void {
    this.bookForm = this.fb.group({
      title: ['', Validators.required],
      author: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      description: ['', Validators.required],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      imageUrl: ['']
    });
  }

  loadBooks(): void {
    this.loading = true;
    this.bookService.getAllBooks().subscribe({
      next: (books) => {
        this.books = books;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  openAddForm(): void {
    this.editingBook = undefined;
    this.bookForm.reset({ price: 0, stockQuantity: 0 });
    this.showForm = true;
  }

  openEditForm(book: Book): void {
    this.editingBook = book;
    this.bookForm.patchValue(book);
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.editingBook = undefined;
    this.bookForm.reset();
  }

  submitForm(): void {
    if (this.bookForm.invalid) {
      return;
    }

    const formData = this.bookForm.value;

    if (this.editingBook) {
      this.bookService.updateBook(this.editingBook.bookId, formData).subscribe({
        next: () => {
          alert('Book updated successfully');
          this.loadBooks();
          this.closeForm();
        },
        error: () => {
          alert('Failed to update book');
        }
      });
    } else {
      this.bookService.createBook(formData).subscribe({
        next: () => {
          alert('Book created successfully');
          this.loadBooks();
          this.closeForm();
        },
        error: () => {
          alert('Failed to create book');
        }
      });
    }
  }

  deleteBook(book: Book): void {
    if (confirm(`Are you sure you want to delete "${book.title}"?`)) {
      this.bookService.deleteBook(book.bookId).subscribe({
        next: () => {
          alert('Book deleted successfully');
          this.loadBooks();
        },
        error: () => {
          alert('Failed to delete book');
        }
      });
    }
  }
}
