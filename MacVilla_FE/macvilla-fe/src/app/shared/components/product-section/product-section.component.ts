import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product } from '../../../core/models/models';
import { ProductCardComponent } from '../product-card/product-card.component';

@Component({
  selector: 'app-product-section',
  standalone: true,
  imports: [CommonModule, ProductCardComponent],
  template: `
    <section class="product-section" [attr.aria-labelledby]="sectionId">
      <header class="section-header">
        <h2 [id]="sectionId" class="section-title">{{ title }}</h2>
        <a href="#" class="view-all-link" aria-label="View all {{ title }}">
          {{ viewAllText }}
          <span class="arrow" aria-hidden="true">›</span>
        </a>
      </header>
      
      <div class="products-grid" role="list">
        <app-product-card 
          *ngFor="let product of products; trackBy: trackByProduct"
          [product]="product"
          (addToCart)="onAddToCart($event)"
          role="listitem">
        </app-product-card>
      </div>
    </section>
  `,
  styles: [`
    :host {
      display: block;
      margin-top: 24px;
    }
    
    .product-section {
      background: var(--card-bg, #fff);
      border-radius: var(--radius-md, 8px);
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }
    
    .section-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      background: var(--color-primary, #2e7d32);
      padding: 16px 24px;
    }
    
    .section-title {
      color: #fff;
      font-size: 16px;
      font-weight: 700;
      text-transform: uppercase;
      margin: 0;
    }
    
    .view-all-link {
      display: flex;
      align-items: center;
      gap: 6px;
      color: #fff;
      text-decoration: none;
      font-size: 13px;
      font-weight: 500;
      transition: opacity 0.3s ease;
    }
    
    .view-all-link:hover {
      opacity: 0.8;
    }
    
    .arrow {
      font-size: 16px;
    }
    
    .products-grid {
      display: grid;
      grid-template-columns: repeat(5, 1fr);
      gap: 1px;
      background: var(--border-color, #e0e0e0);
      padding: 1px;
    }
    
    @media (max-width: 1400px) {
      .products-grid {
        grid-template-columns: repeat(4, 1fr);
      }
    }
    
    @media (max-width: 1200px) {
      .products-grid {
        grid-template-columns: repeat(3, 1fr);
      }
    }
    
    @media (max-width: 768px) {
      .products-grid {
        grid-template-columns: repeat(2, 1fr);
        gap: 8px;
        padding: 8px;
        background: transparent;
      }
      
      .section-header {
        padding: 12px 16px;
      }
      
      .section-title {
        font-size: 14px;
      }
    }
    
    @media (max-width: 480px) {
      .products-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductSectionComponent {
  @Input() title = 'SẢN PHẨM';
  @Input() viewAllText = 'Xem tất cả';
  @Input() products: Product[] = [];
  @Input() sectionId = 'products-section';
  @Output() addToCart = new EventEmitter<Product>();

  trackByProduct(index: number, item: Product): number {
    return item.productId;
  }

  onAddToCart(product: Product): void {
    this.addToCart.emit(product);
  }
}
