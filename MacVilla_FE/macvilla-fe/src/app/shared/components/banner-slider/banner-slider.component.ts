import { Component, OnInit, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable, interval, Subscription } from 'rxjs';
import { Banner } from '../../../core/models/models';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-banner-slider',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="banner-slider" aria-label="Promotional banners">
      <div class="slider-container" [style.transform]="'translateX(-' + currentSlide * 100 + '%)'">
        <div 
          *ngFor="let banner of banners$ | async; let i = index" 
          class="slide"
          [class.active]="i === currentSlide">
          <img [src]="banner.imageUrl" [alt]="banner.title || 'Promotional banner'" class="slide-image">
          <div class="slide-content" *ngIf="banner.title || banner.subtitle">
            <h2 class="slide-title">{{ banner.title }}</h2>
            <p class="slide-subtitle">{{ banner.subtitle }}</p>
          </div>
          <span *ngIf="banner.badge" class="slide-badge" [style.background]="banner.badgeColor || '#ffc107'">
            {{ banner.badge }}
          </span>
        </div>
      </div>
      
      <div class="slider-controls">
        <button 
          class="slider-btn prev" 
          (click)="prevSlide()"
          aria-label="Previous banner">
          <span class="material-icons">chevron_left</span>
        </button>
        
        <div class="slider-dots">
          <button 
            *ngFor="let _ of banners$ | async; let i = index"
            class="dot"
            [class.active]="i === currentSlide"
            (click)="goToSlide(i)"
            [attr.aria-label]="'Go to slide ' + (i + 1)">
          </button>
        </div>
        
        <button 
          class="slider-btn next" 
          (click)="nextSlide()"
          aria-label="Next banner">
          <span class="material-icons">chevron_right</span>
        </button>
      </div>
    </section>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
      height: 100%;
    }
    
    .banner-slider {
      position: relative;
      border-radius: 12px;
      overflow: hidden;
      background: #f8f9fa;
      width: 100%;
      height: 100%;
      display: flex;
    }
    
    .slider-container {
      display: flex;
      width: 100%;
      height: 100%;
      transition: transform 0.5s cubic-bezier(0.4, 0, 0.2, 1);
    }
    
    .slide {
      flex: 0 0 100%;
      width: 100%;
      height: 100%;
      position: relative;
      min-height: 0;
      opacity: 0;
      visibility: hidden;
      transition: opacity 0.5s ease, visibility 0.5s ease;
    }
    
    .slide.active {
      opacity: 1;
      visibility: visible;
    }
    
    .slide-image {
      width: 100%;
      height: 100%;
      object-fit: cover;
      display: block;
    }
    
    .slide-content {
      position: absolute;
      bottom: 0;
      left: 0;
      right: 0;
      padding: 48px 32px 32px;
      background: linear-gradient(transparent, rgba(0, 0, 0, 0.7));
      color: #fff;
    }
    
    .slide-title {
      font-size: 32px;
      font-weight: 800;
      text-transform: uppercase;
      margin: 0 0 8px;
      text-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
    }
    
    .slide-subtitle {
      font-size: 16px;
      margin: 0;
      opacity: 0.9;
    }
    
    .slide-badge {
      position: absolute;
      top: 20px;
      left: 20px;
      padding: 10px 20px;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 700;
      color: #333;
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
    }
    
    .slider-controls {
      position: absolute;
      bottom: 20px;
      right: 20px;
      display: flex;
      align-items: center;
      gap: 12px;
      z-index: 10;
    }
    
    .slider-btn {
      width: 44px;
      height: 44px;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.95);
      border: none;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
      transition: all 0.3s ease;
    }
    
    .slider-btn:hover {
      background: #fff;
      transform: scale(1.1);
    }
    
    .slider-btn .material-icons {
      font-size: 24px;
      color: #333;
    }
    
    .slider-dots {
      display: flex;
      gap: 8px;
      padding: 8px 16px;
      background: rgba(0, 0, 0, 0.3);
      border-radius: 20px;
    }
    
    .dot {
      width: 10px;
      height: 10px;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.5);
      border: none;
      cursor: pointer;
      transition: all 0.3s ease;
    }
    
    .dot:hover {
      background: rgba(255, 255, 255, 0.8);
    }
    
    .dot.active {
      background: #fff;
      transform: scale(1.2);
    }
    
    @media (max-width: 768px) {
      .slide-content {
        padding: 32px 20px 24px;
      }
      
      .slide-title {
        font-size: 24px;
      }
      
      .slide-subtitle {
        font-size: 14px;
      }
      
      .slider-controls {
        bottom: 16px;
        right: 16px;
      }
      
      .slider-btn {
        width: 40px;
        height: 40px;
      }
      
      .slider-dots {
        padding: 6px 12px;
      }
      
      .dot {
        width: 8px;
        height: 8px;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BannerSliderComponent implements OnInit, OnDestroy {
  banners$: Observable<Banner[]>;
  currentSlide = 0;
  private slideSubscription?: Subscription;
  private bannerCount = 0;

  constructor(private dataService: DataService) {
    this.banners$ = this.dataService.getBanners();
  }

  ngOnInit(): void {
    this.banners$.subscribe(banners => {
      this.bannerCount = banners.length;
      if (banners.length > 1) {
        this.startAutoSlide();
      }
    });
  }

  ngOnDestroy(): void {
    this.stopAutoSlide();
  }

  private startAutoSlide(): void {
    this.stopAutoSlide();
    this.slideSubscription = interval(5000).subscribe(() => {
      this.nextSlide();
    });
  }

  private stopAutoSlide(): void {
    if (this.slideSubscription) {
      this.slideSubscription.unsubscribe();
    }
  }

  nextSlide(): void {
    this.currentSlide = (this.currentSlide + 1) % this.bannerCount;
  }

  prevSlide(): void {
    this.currentSlide = this.currentSlide === 0 ? this.bannerCount - 1 : this.currentSlide - 1;
  }

  goToSlide(index: number): void {
    this.currentSlide = index;
  }
}
