import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Food } from '../../models/food.model';

@Component({
  selector: 'app-food-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './food-card.component.html',
  styleUrls: ['./food-card.component.css']
})
export class FoodCardComponent {
  @Input({ required: true }) food!: Food;
  @Output() add = new EventEmitter<Food>();
  isFavorite = false;
  isAdding = false;

  readonly placeholder = 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?auto=format&fit=crop&w=1200&q=80';

  get imageUrl(): string {
    return this.food.imageUrl || this.placeholder;
  }

  onImageError(event: Event): void {
    (event.target as HTMLImageElement).src = this.placeholder;
  }

  onAdd(): void {
    this.isAdding = true;
    setTimeout(() => (this.isAdding = false), 280);
    this.add.emit(this.food);
  }

  toggleFavorite(): void {
    this.isFavorite = !this.isFavorite;
  }
}
