import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-premium-footer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './premium-footer.component.html',
  styleUrls: ['./premium-footer.component.css']
})
export class PremiumFooterComponent {
  readonly title = 'Developed by Tanishk Jaiswal & Ritika Pandey';
  readonly phone = '9336519832';
  readonly github = 'https://github.com/tanishk13-devops';
  readonly linkedIn = 'https://www.linkedin.com/';
  readonly email = 'mailto:tanishk13.dev@gmail.com';
  readonly portfolio = 'https://example.com/portfolio';
}
