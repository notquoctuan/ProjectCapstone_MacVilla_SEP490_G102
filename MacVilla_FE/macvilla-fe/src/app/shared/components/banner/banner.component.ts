import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { Banner, NewsItem } from '../../../core/models/models';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-banner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="hero-section" aria-label="Featured banner">
      <div class="hero-banner-wrapper">
        <div class="hero-banner">
          <img [src]="(banners$ | async)?.[0]?.imageUrl" 
               [alt]="(banners$ | async)?.[0]?.title || 'Featured banner'"
               class="hero-image"
               loading="lazy">
          <div class="hero-overlay">
            <div class="hero-content">
              <h2 class="hero-title">{{ (banners$ | async)?.[0]?.title }}</h2>
              <p class="hero-subtitle">{{ (banners$ | async)?.[0]?.subtitle }}</p>
            </div>
          </div>
          <span *ngIf="(banners$ | async)?.[0]?.badge" 
                class="hero-badge"
                [style.background-color]="(banners$ | async)?.[0]?.badgeColor || '#ffc107'">
            {{ (banners$ | async)?.[0]?.badge }}
          </span>
          <div class="hero-nav">
            <button class="hero-nav-btn prev" aria-label="Previous slide">❮</button>
            <button class="hero-nav-btn next" aria-label="Next slide">❯</button>
          </div>
        </div>
      </div>
      
      <div class="featured-news">
        <h3 class="news-title">HỆ THỐNG CỬA HÀNG</h3>
        <ul class="news-list" role="list">
          <li *ngFor="let news of newsItems$ | async">
            <a href="{{ news.url }}" class="news-link">
              <span class="news-icon" aria-hidden="true">📍</span>
              <span class="news-text">{{ news.title }}</span>
              <span class="news-arrow" aria-hidden="true">›</span>
            </a>
          </li>
        </ul>
      </div>
    </section>
  `,
  styles: [`
    :host {
      display: block;
    }
    
    .hero-section {
      display: grid;
      grid-template-columns: 1fr 280px;
      gap: 20px;
    }
    
    .hero-banner-wrapper {
      position: relative;
    }
    
    .hero-banner {
      position: relative;
      border-radius: var(--radius-md, 8px);
      overflow: hidden;
      aspect-ratio: 16 / 9;
      background: var(--bg-light, #f8f9fa);
    }
    
    .hero-image {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }
    
    .hero-overlay {
      position: absolute;
      bottom: 0;
      left: 0;
      right: 0;
      padding: 40px 30px;
      background: linear-gradient(transparent, rgba(0, 0, 0, 0.7));
    }
    
    .hero-content {
      color: #fff;
    }
    
    .hero-title {
      font-size: 32px;
      font-weight: 800;
      text-transform: uppercase;
      margin: 0 0 8px;
      text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
    }
    
    .hero-subtitle {
      font-size: 16px;
      margin: 0;
      opacity: 0.9;
    }
    
    .hero-badge {
      position: absolute;
      top: 20px;
      left: 20px;
      padding: 8px 16px;
      border-radius: var(--radius-sm, 4px);
      font-size: 13px;
      font-weight: 700;
      color: #333;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    }
    
    .hero-nav {
      position: absolute;
      top: 50%;
      left: 0;
      right: 0;
      transform: translateY(-50%);
      display: flex;
      justify-content: space-between;
      padding: 0 16px;
      pointer-events: none;
    }
    
    .hero-nav-btn {
      width: 44px;
      height: 44px;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.9);
      border: none;
      font-size: 18px;
      cursor: pointer;
      pointer-events: auto;
      transition: all 0.3s ease;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
    }
    
    .hero-nav-btn:hover {
      background: #fff;
      transform: scale(1.1);
    }
    
    .featured-news {
      background: var(--card-bg, #fff);
      border-radius: var(--radius-md, 8px);
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }
    
    .news-title {
      background: var(--bg-light, #f8f9fa);
      color: var(--color-primary, #2e7d32);
      padding: 16px 20px;
      font-size: 14px;
      font-weight: 700;
      text-transform: uppercase;
      margin: 0;
      border-bottom: 1px solid var(--border-color, #e0e0e0);
    }
    
    .news-list {
      list-style: none;
      padding: 8px 0;
      margin: 0;
    }
    
    .news-link {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 12px 20px;
      color: var(--text-primary, #333);
      text-decoration: none;
      font-size: 13px;
      transition: all 0.2s ease;
    }
    
    .news-link:hover {
      background: var(--bg-light, #f8f9fa);
      color: var(--color-primary, #2e7d32);
      padding-left: 24px;
    }
    
    .news-icon {
      font-size: 14px;
    }
    
    .news-text {
      flex: 1;
    }
    
    .news-arrow {
      color: var(--text-muted, #999);
      font-size: 16px;
      transition: transform 0.2s ease;
    }
    
    .news-link:hover .news-arrow {
      transform: translateX(4px);
      color: var(--color-primary, #2e7d32);
    }
    
    @media (max-width: 1200px) {
      .hero-section {
        grid-template-columns: 1fr;
      }
      
      .featured-news {
        display: none;
      }
    }
    
    @media (max-width: 768px) {
      .hero-banner {
        aspect-ratio: 16 / 10;
      }
      
      .hero-title {
        font-size: 24px;
      }
      
      .hero-subtitle {
        font-size: 14px;
      }
      
      .hero-overlay {
        padding: 24px 20px;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BannerComponent {
  banners$: Observable<Banner[]>;
  newsItems$: Observable<NewsItem[]>;

  constructor(private dataService: DataService) {
    this.banners$ = this.dataService.getBanners();
    this.newsItems$ = this.dataService.getNewsItems();
  }
}
