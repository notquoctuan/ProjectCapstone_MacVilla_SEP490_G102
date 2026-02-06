import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NewsItem } from '../../../core/models/models';

@Component({
  selector: 'app-news-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <aside class="news-sidebar">
      <header class="sidebar-header">
        <span class="material-icons header-icon">article</span>
        <h3 class="sidebar-title">{{ title }}</h3>
      </header>
      
      <div class="news-list">
        <a 
          *ngFor="let news of newsItems; trackBy: trackByNews" 
          href="#" 
          class="news-item">
          <img 
            [src]="news.thumbnail" 
            [alt]="news.title" 
            class="news-thumb">
          <div class="news-info">
            <span class="news-title">{{ news.title }}</span>
            <div class="news-meta">
              <span class="news-date" *ngIf="news.publishDate">
                <span class="material-icons">calendar_today</span>
                {{ news.publishDate | date:'dd/MM/yyyy' }}
              </span>
            </div>
          </div>
        </a>
      </div>
    </aside>
  `,
  styles: [`
    :host {
      display: block;
      height: 100%;
    }

    .news-sidebar {
      background: #fff;
      border-radius: 10px;
      overflow: hidden;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    .sidebar-header {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 16px 20px;
      background: #1b5e20;
      color: #fff;
      flex-shrink: 0;
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

    .news-list {
      padding: 16px;
      display: flex;
      flex-direction: column;
      gap: 12px;
      flex: 1;
      overflow-y: auto;
    }

    .news-item {
      display: flex;
      gap: 12px;
      padding: 12px;
      border-radius: 8px;
      background: #f8f9fa;
      text-decoration: none;
      transition: all 0.3s ease;
    }

    .news-item:hover {
      background: #e8f5e9;
      transform: translateX(4px);
    }

    .news-thumb {
      width: 70px;
      height: 70px;
      object-fit: cover;
      border-radius: 6px;
      flex-shrink: 0;
    }

    .news-info {
      display: flex;
      flex-direction: column;
      justify-content: center;
      flex: 1;
      min-width: 0;
    }

    .news-title {
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

    .news-meta {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .news-date {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 12px;
      color: #666;
    }

    .news-date .material-icons {
      font-size: 14px;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NewsSidebarComponent {
  @Input() title: string = 'Tin Tức Mới Nhất';
  @Input() newsItems: NewsItem[] = [];

  trackByNews(index: number, item: NewsItem): number {
    return item.id;
  }
}
