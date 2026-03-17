import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Restaurant } from '../../models/restaurant.model';
import { RestaurantService } from '../../services/restaurant.service';

@Component({
  selector: 'app-restaurant-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './restaurant-list.component.html',
  styleUrls: ['./restaurant-list.component.css']
})
export class RestaurantListComponent implements OnInit {
  allRestaurants: Restaurant[] = [];
  restaurants: Restaurant[] = [];
  errorMessage = '';
  search = '';
  locations: string[] = [];
  selectedLocation = '';
  sortBy: 'recommended' | 'ratingDesc' | 'ratingAsc' | 'nameAsc' = 'recommended';
  loading = false;

  constructor(private restaurantService: RestaurantService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.errorMessage = '';
    this.restaurantService.getRestaurants(this.search).subscribe({
      next: (data) => {
        this.allRestaurants = data;
        this.locations = [...new Set(data.map(r => r.location).filter(Boolean) as string[])].sort();
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.allRestaurants = [];
        this.restaurants = [];
        this.errorMessage = 'Could not load restaurants. Start backend API and try again.';
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    let list = [...this.allRestaurants];

    if (this.selectedLocation) {
      list = list.filter(r => r.location === this.selectedLocation);
    }

    switch (this.sortBy) {
      case 'ratingDesc':
        list.sort((a, b) => (b.rating || 0) - (a.rating || 0));
        break;
      case 'ratingAsc':
        list.sort((a, b) => (a.rating || 0) - (b.rating || 0));
        break;
      case 'nameAsc':
        list.sort((a, b) => a.name.localeCompare(b.name));
        break;
      default:
        break;
    }

    this.restaurants = list;
  }

  clearFilters(): void {
    this.selectedLocation = '';
    this.sortBy = 'recommended';
    this.applyFilters();
  }
}
