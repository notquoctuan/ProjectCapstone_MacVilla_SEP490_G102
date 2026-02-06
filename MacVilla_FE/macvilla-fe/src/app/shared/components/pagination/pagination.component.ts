import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaginationConfig } from '../../../core/models/models';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  template: `
    <nav class="pagination-wrapper" role="navigation" aria-label="Pagination">
      <button 
        class="page-btn nav-btn"
        [disabled]="pagination.currentPage === 1"
        (click)="onPageChange(pagination.currentPage - 1)"
        aria-label="Previous page">
        <span class="material-icons">chevron_left</span>
      </button>
      
      <div class="page-numbers">
        <button 
          *ngFor="let page of getPageNumbers()"
          class="page-btn"
          [class.active]="page === pagination.currentPage"
          [class.ellipsis]="page === '...'"
          [disabled]="page === '...'"
          (click)="onPageClick(page)"
          [attr.aria-current]="page === pagination.currentPage ? 'page' : null">
          <span *ngIf="page !== '...'">{{ page }}</span>
          <span *ngIf="page === '...'">...</span>
        </button>
      </div>
      
      <button 
        class="page-btn nav-btn"
        [disabled]="pagination.currentPage === pagination.totalPages"
        (click)="onPageChange(pagination.currentPage + 1)"
        aria-label="Next page">
        <span class="material-icons">chevron_right</span>
      </button>
      
      <div class="pagination-info">
        <span class="material-icons">info</span>
        Trang {{ pagination.currentPage }}/{{ pagination.totalPages }}
      </div>
    </nav>
  `,
  styles: [`
    :host {
      display: block;
    }
    
    .pagination-wrapper {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      padding: 24px 0;
    }
    
    .page-numbers {
      display: flex;
      gap: 4px;
    }
    
    .page-btn {
      min-width: 40px;
      height: 40px;
      display: flex;
      align-items: center;
      justify-content: center;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      background: #fff;
      color: #333;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s ease;
    }
    
    .page-btn:hover:not(:disabled):not(.active):not(.ellipsis) {
      border-color: #2e7d32;
      color: #2e7d32;
    }
    
    .page-btn.active {
      background: #2e7d32;
      border-color: #2e7d32;
      color: #fff;
    }
    
    .page-btn:disabled,
    .page-btn.ellipsis {
      opacity: 0.5;
      cursor: not-allowed;
    }
    
    .nav-btn {
      padding: 0 12px;
    }
    
    .nav-btn .material-icons {
      font-size: 20px;
    }
    
    .pagination-info {
      display: flex;
      align-items: center;
      gap: 6px;
      margin-left: 16px;
      padding-left: 16px;
      border-left: 1px solid #e0e0e0;
      color: #666;
      font-size: 13px;
    }
    
    .pagination-info .material-icons {
      font-size: 16px;
      color: #2e7d32;
    }
    
    @media (max-width: 768px) {
      .pagination-wrapper {
        flex-wrap: wrap;
        gap: 12px;
      }
      
      .page-btn {
        min-width: 36px;
        height: 36px;
        font-size: 13px;
      }
      
      .pagination-info {
        width: 100%;
        margin-left: 0;
        padding-left: 0;
        border-left: none;
        padding-top: 12px;
        border-top: 1px solid #e0e0e0;
        justify-content: center;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PaginationComponent {
  @Input() pagination!: PaginationConfig;
  @Output() pageChange = new EventEmitter<number>();

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.pagination.totalPages) {
      this.pageChange.emit(page);
    }
  }

  onPageClick(page: number | string): void {
    if (typeof page === 'number' && page !== this.pagination.currentPage) {
      this.onPageChange(page);
    }
  }

  getPageNumbers(): (number | string)[] {
    const total = this.pagination.totalPages;
    const current = this.pagination.currentPage;
    const pages: (number | string)[] = [];
    
    if (total <= 7) {
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      pages.push(1);
      
      if (current > 3) {
        pages.push('...');
      }
      
      const start = Math.max(2, current - 1);
      const end = Math.min(total - 1, current + 1);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
      
      if (current < total - 2) {
        pages.push('...');
      }
      
      pages.push(total);
    }
    
    return pages;
  }
}
