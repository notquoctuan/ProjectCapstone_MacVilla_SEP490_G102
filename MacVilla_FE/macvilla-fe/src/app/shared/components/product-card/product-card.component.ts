import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { Product } from '../../../core/models/models';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, CurrencyPipe],
  template: `
    <article class="product-card" (click)="onCardClick()">
      <div class="product-image-wrapper">
        <img [src]="product.imageUrl" [alt]="product.productName" class="product-image" loading="lazy">
        <span *ngIf="product.originalPrice" class="sale-badge">Sale</span>
      </div>
      <div class="product-content">
        <h3 class="product-name">{{ product.productName }}</h3>
        <div class="product-prices">
          <span class="sale-price">{{ product.salePrice | currency:'VND':'symbol':'1.0-0' }}</span>
          <span class="original-price" *ngIf="product.originalPrice">
            {{ product.originalPrice | currency:'VND':'symbol':'1.0-0' }}
          </span>
        </div>
        <div class="product-rating" aria-label="Rating: {{ product.rating }} out of 5 stars">
          <span class="stars" aria-hidden="true">{{ getStars() }}</span>
          <span class="rating-count">({{ product.ratingCount }})</span>
        </div>
        <button class="add-to-cart-btn" (click)="onAddToCart($event)" aria-label="Add {{ product.productName }} to cart">
          <span class="cart-icon">🛒</span>
          <span>Thêm vào giỏ</span>
        </button>
      </div>
    </article>
  `,
  styles: [`
    :host {
      display: block;
      height: 100%;
    }
    
    .product-card {
      background: var(--card-bg, #fff);
      border: 1px solid var(--border-color, #e0e0e0);
      border-radius: var(--radius-md, 8px);
      overflow: hidden;
      cursor: pointer;
      height: 100%;
      display: flex;
      flex-direction: column;
      /* Base state - no hover effects */
      transform: translateY(0);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
      transition: transform 0.3s ease, box-shadow 0.3s ease, border-color 0.3s ease;
    }
    
    /* Hover state - ONLY applies when mouse is over the card */
    .product-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 12px 24px rgba(0, 0, 0, 0.12);
      border-color: var(--color-primary, #2e7d32);
    }
    
    .product-image-wrapper {
      position: relative;
      padding-top: 100%;
      overflow: hidden;
      background: var(--bg-light, #f8f9fa);
    }
    
    .product-image {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      object-fit: contain;
      padding: 16px;
      transition: transform 0.4s ease;
      pointer-events: none;
    }
    
    /* Image scales ONLY when card is hovered */
    .product-card:hover .product-image {
      transform: scale(1.05);
    }
    
    .sale-badge {
      position: absolute;
      top: 12px;
      left: 12px;
      background: var(--color-danger, #f44336);
      color: #fff;
      padding: 4px 10px;
      border-radius: var(--radius-sm, 4px);
      font-size: 11px;
      font-weight: 600;
      text-transform: uppercase;
    }
    
    .product-content {
      padding: 16px;
      flex: 1;
      display: flex;
      flex-direction: column;
      text-align: center;
    }
    
    .product-name {
      font-size: 13px;
      font-weight: 600;
      color: var(--text-primary, #333);
      margin: 0 0 12px;
      text-transform: uppercase;
      line-height: 1.5;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
      min-height: 39px;
    }
    
    .product-prices {
      margin-bottom: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      flex-wrap: wrap;
    }
    
    .sale-price {
      font-size: 16px;
      font-weight: 700;
      color: var(--color-danger, #f44336);
    }
    
    .original-price {
      font-size: 13px;
      color: var(--text-muted, #999);
      text-decoration: line-through;
    }
    
    .product-rating {
      margin-bottom: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 4px;
    }
    
    .stars {
      color: var(--color-warning, #ffc107);
      font-size: 14px;
      letter-spacing: 1px;
    }
    
    .rating-count {
      color: var(--text-secondary, #666);
      font-size: 12px;
    }
    
    .add-to-cart-btn {
      margin-top: auto;
      width: 100%;
      padding: 12px 16px;
      background: var(--color-primary, #2e7d32);
      color: #fff;
      border: none;
      border-radius: var(--radius-sm, 6px);
      font-size: 13px;
      font-weight: 600;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      transition: background-color 0.3s ease, transform 0.2s ease;
    }
    
    .add-to-cart-btn:hover {
      background: var(--color-primary-dark, #1b5e20);
    }
    
    .add-to-cart-btn:active {
      transform: scale(0.98);
    }
    
    .cart-icon {
      font-size: 16px;
    }
    
    @media (max-width: 768px) {
      .product-content {
        padding: 12px;
      }
      
      .product-name {
        font-size: 12px;
        min-height: 36px;
      }
      
      .sale-price {
        font-size: 14px;
      }
      
      .add-to-cart-btn {
        padding: 10px 12px;
        font-size: 12px;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductCardComponent {
  @Input() product!: Product;
  @Output() addToCart = new EventEmitter<Product>();

  getStars(): string {
    return '★'.repeat(this.product.rating) + '☆'.repeat(5 - this.product.rating);
  }

  onCardClick(): void {
    console.log('Product clicked:', this.product.productName);
  }

  onAddToCart(event: Event): void {
    event.stopPropagation();
    this.addToCart.emit(this.product);
  }
}
