import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Category, Product } from '../../../core/models/models';
import { ProductCardComponent } from '../product-card/product-card.component';

@Component({
  selector: 'app-category-tabs',
  standalone: true,
  imports: [CommonModule, ProductCardComponent],
  template: `
    <section class="tabs-section" aria-label="Product categories">
      <header class="tabs-header">
        <nav class="tabs-nav" role="tablist" aria-label="Category filters">
          <button 
            *ngFor="let category of categories; let i = index"
            class="tab-btn"
            [class.active]="i === 0"
            role="tab"
            [attr.aria-selected]="i === 0"
            [attr.aria-controls]="'tabpanel-' + i"
            (click)="onTabClick(category, i)">
            {{ category.categoryName }}
          </button>
        </nav>
        <a href="#" class="view-all-link">
          Xem tất cả
          <span class="arrow">›</span>
        </a>
      </header>
      
      <div class="tabs-content" role="tabpanel" [id]="'tabpanel-0'">
        <div class="products-grid" role="list">
          <app-product-card 
            *ngFor="let product of products; trackBy: trackByProduct"
            [product]="product"
            (addToCart)="onAddToCart($event)"
            role="listitem">
          </app-product-card>
        </div>
      </div>
    </section>
  `,
  styles: [`
    :host {
      display: block;
      margin-top: 32px;
    }
    
    .tabs-section {
      background: var(--card-bg, #fff);
      border-radius: var(--radius-md, 8px);
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }
    
    .tabs-header {
      display: flex;
      justify-content: space-between;
      align-items: stretch;
      background: var(--bg-light, #f8f9fa);
      border-bottom: 3px solid var(--color-primary, #2e7d32);
    }
    
    .tabs-nav {
      display: flex;
      gap: 4px;
      padding: 0;
    }
    
    .tab-btn {
      padding: 16px 24px;
      background: transparent;
      border: none;
      font-size: 14px;
      font-weight: 600;
      color: var(--text-primary, #333);
      cursor: pointer;
      transition: all 0.3s ease;
      border-bottom: 3px solid transparent;
      margin-bottom: -3px;
      white-space: nowrap;
    }
    
    .tab-btn:hover {
      color: var(--color-primary, #2e7d32);
      background: rgba(46, 125, 50, 0.05);
    }
    
    .tab-btn.active {
      color: var(--color-primary, #2e7d32);
      background: #fff;
      border-bottom-color: var(--color-primary, #2e7d32);
    }
    
    .view-all-link {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 0 24px;
      color: var(--color-primary, #2e7d32);
      text-decoration: none;
      font-size: 14px;
      font-weight: 600;
      transition: opacity 0.3s ease;
    }
    
    .view-all-link:hover {
      opacity: 0.8;
    }
    
    .arrow {
      font-size: 18px;
    }
    
    .tabs-content {
      padding: 24px;
    }
    
    .products-grid {
      display: grid;
      grid-template-columns: repeat(5, 1fr);
      gap: 1px;
      background: var(--border-color, #e0e0e0);
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
      .tabs-header {
        flex-direction: column;
      }
      
      .tabs-nav {
        overflow-x: auto;
        padding: 8px;
        gap: 2px;
      }
      
      .tab-btn {
        padding: 12px 16px;
        font-size: 13px;
      }
      
      .view-all-link {
        padding: 12px 16px;
        justify-content: center;
        border-top: 1px solid var(--border-color, #e0e0e0);
      }
      
      .tabs-content {
        padding: 16px;
      }
      
      .products-grid {
        grid-template-columns: repeat(2, 1fr);
        gap: 8px;
        background: transparent;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CategoryTabsComponent {
  @Input() categories: Category[] = [];
  @Input() products: Product[] = [];
  @Output() addToCart = new EventEmitter<Product>();
  @Output() tabChange = new EventEmitter<Category>();

  trackByProduct(index: number, item: Product): number {
    return item.productId;
  }

  onTabClick(category: Category, index: number): void {
    this.tabChange.emit(category);
  }

  onAddToCart(product: Product): void {
    this.addToCart.emit(product);
  }
}
