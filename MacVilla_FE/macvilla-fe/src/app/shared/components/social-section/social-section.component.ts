import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { Review } from '../../../core/models/models';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-social-section',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="social-section" aria-label="Customer reviews and social media">
      <div class="social-grid">
        <!-- Reviews Column -->
        <div class="social-column reviews-column">
          <h3 class="social-title">KHÁCH HÀNG NÓI GÌ</h3>
          <div class="reviews-list">
            <article *ngFor="let review of reviews$ | async" class="review-card">
              <div class="review-header">
                <div class="review-rating" [attr.aria-label]="review.rating + ' stars'">
                  <span *ngFor="let star of [1,2,3,4,5]" class="star" [class.filled]="star <= review.rating">★</span>
                </div>
                <span class="review-author">{{ review.author }}</span>
              </div>
              <blockquote class="review-text">"{{ review.text }}"</blockquote>
            </article>
          </div>
        </div>
        
        <!-- Facebook Column -->
        <div class="social-column facebook-column">
          <h3 class="social-title">FANPAGE</h3>
          <div class="embed-wrapper facebook-embed">
            <img src="https://via.placeholder.com/400x300/1877f2/fff?text=Facebook+Page" alt="Facebook Fanpage" loading="lazy">
          </div>
        </div>
        
        <!-- Video Column -->
        <div class="social-column video-column">
          <h3 class="social-title">VIDEO</h3>
          <div class="embed-wrapper video-embed">
            <img src="https://via.placeholder.com/400x300/cc181e/fff?text=YouTube+Video" alt="YouTube Video" loading="lazy">
          </div>
        </div>
      </div>
    </section>
  `,
  styles: [`
    :host {
      display: block;
      margin-top: 32px;
    }
    
    .social-section {
      background: var(--card-bg, #fff);
      border-radius: var(--radius-md, 8px);
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }
    
    .social-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 1px;
      background: var(--border-color, #e0e0e0);
    }
    
    .social-column {
      background: var(--card-bg, #fff);
      padding: 0;
    }
    
    .social-title {
      background: var(--bg-light, #f8f9fa);
      color: var(--color-primary, #2e7d32);
      padding: 16px 20px;
      font-size: 15px;
      font-weight: 700;
      text-transform: uppercase;
      margin: 0;
      border-bottom: 1px solid var(--border-color, #e0e0e0);
    }
    
    .reviews-list {
      padding: 16px;
    }
    
    .review-card {
      padding: 16px;
      border-radius: var(--radius-sm, 6px);
      background: var(--bg-light, #f8f9fa);
      margin-bottom: 12px;
    }
    
    .review-card:last-child {
      margin-bottom: 0;
    }
    
    .review-header {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 12px;
    }
    
    .review-rating {
      display: flex;
      gap: 2px;
    }
    
    .star {
      color: #ddd;
      font-size: 14px;
    }
    
    .star.filled {
      color: var(--color-warning, #ffc107);
    }
    
    .review-author {
      font-size: 13px;
      font-weight: 600;
      color: var(--color-primary, #2e7d32);
    }
    
    .review-text {
      font-size: 14px;
      color: var(--text-secondary, #666);
      margin: 0;
      font-style: italic;
      line-height: 1.6;
    }
    
    .embed-wrapper {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 250px;
      padding: 16px;
      background: var(--bg-light, #f8f9fa);
    }
    
    .embed-wrapper img {
      max-width: 100%;
      border-radius: var(--radius-sm, 6px);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }
    
    @media (max-width: 992px) {
      .social-grid {
        grid-template-columns: 1fr;
      }
      
      .reviews-column,
      .facebook-column {
        border-bottom: 1px solid var(--border-color, #e0e0e0);
      }
    }
    
    @media (max-width: 768px) {
      .social-title {
        font-size: 14px;
        padding: 12px 16px;
      }
      
      .review-card {
        padding: 12px;
      }
      
      .embed-wrapper {
        min-height: 200px;
        padding: 12px;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SocialSectionComponent {
  reviews$: Observable<Review[]>;

  constructor(private dataService: DataService) {
    this.reviews$ = this.dataService.getReviews();
  }
}
