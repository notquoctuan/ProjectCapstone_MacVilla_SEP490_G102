import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Product } from '../../../core/models/models';

@Component({
  selector: 'app-best-seller-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <aside class="best-seller-sidebar">
      <header class="sidebar-header">
        <span class="material-icons header-icon">local_fire_department</span>
        <h3 class="sidebar-title">{{ title }}</h3>
      </header>
      
      <div class="products-list">
        <a 
          *ngFor="let product of products; trackBy: trackByProduct" 
          href="#" 
          class="product-item">
          <img 
            [src]="product.imageUrl" 
            [alt]="product.productName" 
            class="product-thumb">
          <div class="product-info">
            <span class="product-name">{{ product.productName }}</span>
            <div class="product-price-row">
              <span class="product-price">{{ product.salePrice | currency:'VND':'symbol':'1.0-0' }}</span>
              <span class="product-original" *ngIf="product.originalPrice">
                {{ product.originalPrice | currency:'VND':'symbol':'1.0-0' }}
              </span>
            </div>
            <div class="product-rating">
              <span class="material-icons star">star</span>
              <span class="rating-count">({{ product.ratingCount }})</span>
            </div>
          </div>
        </a>
      </div>
    </aside>
  `,
  styles: [`
    .best-seller-sidebar {
      background: #fff;
      border-radius: 10px;
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    }

    .sidebar-header {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 16px 20px;
      background: #2e7d32;
      color: #fff;
    }

    .header-icon {
      font-size: 24px;
    }

    .sidebar-title {
      font-size: 14px;
      font-weight: 700;
      text-transform: uppercase;
      margin: 0;
    }

    .products-list {
      padding: 16px;
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .product-item {
      display: flex;
      gap: 12px;
      padding: 12px;
      border-radius: 8px;
      background: #f8f9fa;
      text-decoration: none;
      transition: all 0.3s ease;
    }

    .product-item:hover {
      background: #e8f5e9;
      transform: translateX(4px);
    }

    .product-thumb {
      width: 70px;
      height: 70px;
      object-fit: cover;
      border-radius: 6px;
      flex-shrink: 0;
    }

    .product-info {
      display: flex;
      flex-direction: column;
      justify-content: center;
      flex: 1;
      min-width: 0;
    }

    .product-name {
      font-size: 13px;
      color: #212121;
      font-weight: 600;
      margin-bottom: 4px;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
      line-height: 1.4;
    }

    .product-price-row {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 4px;
    }

    .product-price {
      font-size: 14px;
      font-weight: 700;
      color: #f44336;
    }

    .product-original {
      font-size: 12px;
      color: #999;
      text-decoration: line-through;
    }

    .product-rating {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .star {
      font-size: 14px;
      color: #ffc107;
    }

    .rating-count {
      font-size: 12px;
      color: #666;
    }

    @media (max-width: 992px) {
      .best-seller-sidebar {
        order: 3;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BestSellerSidebarComponent {
  @Input() title: string = 'Sản Phẩm Bán Chạy';
  @Input() products: Product[] = [];

  trackByProduct(index: number, item: Product): number {
    return item.productId;
  }
}
