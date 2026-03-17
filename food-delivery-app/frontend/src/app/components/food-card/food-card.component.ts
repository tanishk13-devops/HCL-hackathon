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
    if (!this.food?.name?.trim()) {
      return this.placeholder;
    }

    const dishSlug = this.food.name
      .toLowerCase()
      .trim()
      .replace(/[^a-z0-9\s-]/g, '')
      .replace(/\s+/g, '-');

    const signature = this.getSignature();
    return `https://source.unsplash.com/400x300/?${encodeURIComponent(dishSlug)}&sig=${signature}`;
  }

  private getSignature(): number {
    const seed = `${this.food.id}-${this.food.restaurantId}-${this.food.name}`;
    let hash = 0;

    for (let i = 0; i < seed.length; i++) {
      hash = ((hash << 5) - hash) + seed.charCodeAt(i);
      hash |= 0;
    }

    return Math.abs(hash % 100000);
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
