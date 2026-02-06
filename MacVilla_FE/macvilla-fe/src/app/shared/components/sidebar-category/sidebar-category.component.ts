import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { Category } from '../../../core/models/models';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-sidebar-category',
  standalone: true,
  imports: [CommonModule],
  template: `
    <aside class="sidebar" role="complementary" aria-label="Product categories">
      <h2 class="sidebar-title">
        <span class="title-icon" aria-hidden="true">☰</span>
        DANH MỤC SẢN PHẨM
      </h2>
      <nav class="category-nav" aria-label="Categories">
        <ul class="category-list" role="list">
          <li *ngFor="let category of categories$ | async; trackBy: trackByCategory">
            <a href="#" class="category-link">
              <span class="category-icon" aria-hidden="true">{{ category.icon }}</span>
              <span class="category-name">{{ category.categoryName }}</span>
              <span class="arrow" aria-hidden="true">›</span>
            </a>
          </li>
        </ul>
      </nav>
    </aside>
  `,
  styles: [`
    :host {
      display: block;
    }
    
    .sidebar {
      background: var(--card-bg, #fff);
      border-radius: var(--radius-md, 8px);
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }
    
    .sidebar-title {
      display: flex;
      align-items: center;
      gap: 10px;
      background: var(--color-primary, #2e7d32);
      color: #fff;
      padding: 16px 20px;
      font-size: 15px;
      font-weight: 700;
      text-transform: uppercase;
      margin: 0;
    }
    
    .title-icon {
      font-size: 18px;
    }
    
    .category-nav {
      border: 1px solid var(--border-color, #e0e0e0);
      border-top: none;
      border-radius: 0 0 var(--radius-md, 8px) var(--radius-md, 8px);
    }
    
    .category-list {
      margin: 0;
      padding: 8px 0;
      list-style: none;
    }
    
    .category-link {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 14px 20px;
      color: var(--text-primary, #333);
      text-decoration: none;
      font-size: 14px;
      transition: all 0.2s ease;
      border-left: 3px solid transparent;
    }
    
    .category-link:hover {
      background: var(--bg-light, #f8f9fa);
      color: var(--color-primary, #2e7d32);
      border-left-color: var(--color-primary, #2e7d32);
      padding-left: 24px;
    }
    
    .category-icon {
      color: var(--color-primary, #2e7d32);
      font-size: 8px;
    }
    
    .category-name {
      flex: 1;
    }
    
    .arrow {
      color: var(--text-muted, #999);
      font-size: 16px;
      transition: transform 0.2s ease;
    }
    
    .category-link:hover .arrow {
      transform: translateX(4px);
      color: var(--color-primary, #2e7d32);
    }
    
    @media (max-width: 992px) {
      .sidebar {
        display: none;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarCategoryComponent {
  categories$: Observable<Category[]>;

  constructor(private dataService: DataService) {
    this.categories$ = this.dataService.getAllCategories();
  }

  trackByCategory(index: number, item: Category): number {
    return item.categoryId;
  }
}
