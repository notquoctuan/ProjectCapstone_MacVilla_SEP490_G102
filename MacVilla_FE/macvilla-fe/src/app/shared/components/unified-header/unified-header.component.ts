import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { MenuItem, Category, SiteConfig } from '../../../core/models/models';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-unified-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <!-- Unified Sticky Header -->
    <header class="unified-header" *ngIf="config$ | async as config">
      
      <!-- Top Info Bar -->
      <div class="info-bar">
        <div class="container info-inner">
          <span class="info-item">
            <span class="material-icons">location_on</span>
            {{ config.siteSlogan }}
          </span>
          <nav class="info-nav">
            <a href="#" class="info-link">
              <span class="material-icons">help_outline</span>
              Hướng dẫn
            </a>
            <a href="#" class="info-link">
              <span class="material-icons">phone</span>
              Liên hệ
            </a>
          </nav>
        </div>
      </div>

      <!-- Main Header Block -->
      <div class="header-block">
        <div class="container header-inner">
          <!-- Brand -->
          <a href="#" class="brand">
            <span class="brand-icon">{{ config.logo }}</span>
            <div class="brand-text">
              <span class="brand-name">{{ config.siteName }}</span>
              <span class="brand-slogan">{{ config.siteSlogan }}</span>
            </div>
          </a>

          <!-- Search -->
          <div class="search-box">
            <input 
              id="header-search"
              type="text" 
              placeholder="Tìm kiếm sản phẩm..." 
              class="search-input">
            <button class="search-btn" aria-label="Search">
              <span class="material-icons">search</span>
            </button>
          </div>

          <!-- Actions -->
          <div class="header-actions">
            <button class="action-btn hotline-btn">
              <span class="action-icon">
                <span class="material-icons">phone_in_talk</span>
              </span>
              <div class="action-info">
                <span class="action-label">Hotline 24/7</span>
                <span class="action-value">1900 xxxx</span>
              </div>
            </button>
            
            <button class="action-btn cart-btn" aria-label="Giỏ hàng (0 sản phẩm)">
              <span class="action-icon cart-icon-wrapper">
                <span class="material-icons">shopping_cart</span>
                <span class="cart-badge">0</span>
              </span>
              <span class="action-info">Giỏ hàng</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Navigation Bar with Category -->
      <nav class="nav-block" aria-label="Main navigation">
        <div class="container nav-inner">
          
          <!-- Category Trigger -->
          <div class="category-dropdown">
            <button class="category-trigger" aria-label="Danh mục sản phẩm">
              <span class="material-icons">view_list</span>
              <span>DANH MỤC</span>
            </button>
            
            <!-- Dropdown Menu -->
            <div class="category-menu">
              <h3 class="category-title">DANH MỤC SẢN PHẨM</h3>
              <ul class="category-list" role="list">
                <li *ngFor="let cat of categories$ | async">
                  <a href="#" class="category-link">
                    <span class="material-icons">stop</span>
                    <span>{{ cat.categoryName }}</span>
                    <span class="material-icons arrow">chevron_right</span>
                  </a>
                </li>
              </ul>
            </div>
          </div>

          <!-- Nav Menu -->
          <ul class="nav-menu" role="menubar">
            <li *ngFor="let item of menuItems$ | async" role="none">
              <a 
                [routerLink]="item.url" 
                class="nav-link"
                [class.active]="item.isActive"
                role="menuitem">
                {{ item.name }}
              </a>
            </li>
          </ul>
        </div>
      </nav>
    </header>
  `,
  styles: [`
    :host {
      display: block;
      position: sticky;
      top: 0;
      z-index: 10000;
    }

    /* Info Bar */
    .info-bar {
      background: linear-gradient(135deg, #2e7d32 0%, #1b5e20 100%);
      color: #fff;
      font-size: 13px;
      padding: 8px 0;
    }

    .info-inner {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .info-item {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .info-item .material-icons {
      font-size: 16px;
    }

    .info-nav {
      display: flex;
      gap: 20px;
    }

    .info-link {
      display: flex;
      align-items: center;
      gap: 6px;
      color: #fff;
      text-decoration: none;
      transition: opacity 0.2s;
    }

    .info-link:hover {
      opacity: 0.8;
    }

    .info-link .material-icons {
      font-size: 16px;
    }

    /* Header Block */
    .header-block {
      background: #fff;
      padding: 16px 0;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    }

    .header-inner {
      display: grid;
      grid-template-columns: auto 1fr auto;
      gap: 24px;
      align-items: center;
      max-width: 1400px;
      margin: 0 auto;
      padding: 0 20px;
    }

    /* Brand */
    .brand {
      display: flex;
      align-items: center;
      gap: 12px;
      text-decoration: none;
      flex-shrink: 0;
    }

    .brand-icon {
      font-size: 2.5rem;
    }

    .brand-text {
      display: flex;
      flex-direction: column;
    }

    .brand-name {
      font-size: 1.6rem;
      font-weight: 800;
      color: #2e7d32;
      line-height: 1.2;
    }

    .brand-slogan {
      font-size: 11px;
      color: #999;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    /* Search - Pill Style */
    .search-box {
      display: flex;
      max-width: 600px;
      width: 100%;
      margin: 0 auto;
      border: 2px solid #e0e0e0;
      border-radius: 50px;
      overflow: hidden;
      transition: all 0.3s ease;
      background: #f8f9fa;
    }

    .search-box:focus-within {
      border-color: #2e7d32;
      background: #fff;
      box-shadow: 0 0 0 4px rgba(46, 125, 50, 0.1);
    }

    .search-input {
      flex: 1;
      padding: 14px 24px;
      border: none;
      outline: none;
      font-size: 15px;
      background: transparent;
      color: #212121;
    }

    .search-input::placeholder {
      color: #999;
    }

    .search-btn {
      padding: 14px 24px;
      background: #2e7d32;
      color: #fff;
      transition: background 0.3s ease;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .search-btn:hover {
      background: #1b5e20;
    }

    .search-btn .material-icons {
      font-size: 22px;
    }

    /* Actions */
    .header-actions {
      display: flex;
      gap: 16px;
    }

    .action-btn {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 10px 16px;
      background: #f8f9fa;
      border-radius: 10px;
      transition: all 0.3s ease;
      border: none;
      cursor: pointer;
    }

    .action-btn:hover {
      background: #2e7d32;
      color: #fff;
    }

    .action-btn:hover .action-icon {
      color: #fff;
    }

    .action-icon {
      font-size: 1.4rem;
      color: #2e7d32;
      transition: color 0.3s ease;
    }

    .cart-icon-wrapper {
      position: relative;
    }

    .cart-badge {
      position: absolute;
      top: -8px;
      right: -8px;
      width: 20px;
      height: 20px;
      background: #f44336;
      color: #fff;
      border-radius: 50%;
      font-size: 11px;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .action-info {
      display: flex;
      flex-direction: column;
      align-items: flex-start;
      text-align: left;
    }

    .action-label {
      font-size: 11px;
      color: #999;
    }

    .action-value {
      font-size: 14px;
      font-weight: 600;
    }

    /* Nav Block */
    .nav-block {
      background: #2e7d32;
    }

    .nav-inner {
      display: flex;
      align-items: center;
    }

    /* Category Dropdown */
    .category-dropdown {
      position: relative;
    }

    .category-trigger {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 14px 24px;
      background: #1b5e20;
      color: #fff;
      border: none;
      font-size: 13px;
      font-weight: 700;
      text-transform: uppercase;
      cursor: pointer;
      transition: background 0.3s ease;
      border-radius: 0;
    }

    .category-trigger:hover {
      background: #144518;
    }

    .category-trigger .material-icons {
      font-size: 22px;
    }

    .category-menu {
      position: absolute;
      top: 100%;
      left: 0;
      min-width: 280px;
      background: #fff;
      border-radius: 0 0 10px 10px;
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
      opacity: 0;
      visibility: hidden;
      transform: translateY(-10px);
      transition: all 0.3s ease;
      z-index: 100;
    }

    .category-dropdown:hover .category-menu {
      opacity: 1;
      visibility: visible;
      transform: translateY(0);
    }

    .category-title {
      background: #2e7d32;
      color: #fff;
      padding: 14px 20px;
      font-size: 14px;
      font-weight: 700;
      text-transform: uppercase;
      margin: 0;
    }

    .category-list {
      list-style: none;
      padding: 8px 0;
      margin: 0;
    }

    .category-link {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 14px 20px;
      color: #333;
      text-decoration: none;
      transition: all 0.2s ease;
    }

    .category-link:hover {
      background: #f8f9fa;
      color: #2e7d32;
      padding-left: 28px;
    }

    .category-link .material-icons {
      font-size: 8px;
      color: #2e7d32;
    }

    .category-link .arrow {
      margin-left: auto;
      font-size: 18px;
      color: #999;
    }

    /* Nav Menu */
    .nav-menu {
      display: flex;
      flex: 1;
      justify-content: center;
      gap: 4px;
      margin: 0;
      padding: 0;
      list-style: none;
    }

    .nav-link {
      display: flex;
      align-items: center;
      padding: 0 22px;
      color: #fff;
      font-size: 13px;
      font-weight: 600;
      text-transform: uppercase;
      text-decoration: none;
      height: 52px;
      transition: background 0.3s ease;
    }

    .nav-link:hover,
    .nav-link.active {
      background: rgba(255, 255, 255, 0.1);
    }

    /* Responsive */
    @media (max-width: 1200px) {
      .header-inner {
        grid-template-columns: auto 1fr auto;
        gap: 20px;
      }

      .search-box {
        max-width: 400px;
      }
    }

    @media (max-width: 992px) {
      .header-inner {
        grid-template-columns: auto 1fr;
      }

      .header-actions {
        gap: 10px;
      }

      .action-btn {
        padding: 10px 12px;
      }

      .action-info {
        display: none;
      }

      .nav-link {
        padding: 0 16px;
        font-size: 12px;
      }
    }

    @media (max-width: 768px) {
      .info-bar {
        display: none;
      }

      .header-inner {
        grid-template-columns: 1fr auto;
        gap: 16px;
        padding: 0 16px;
      }

      .brand-slogan {
        display: none;
      }

      .brand-icon {
        font-size: 2rem;
      }

      .brand-name {
        font-size: 1.4rem;
      }

      .search-box {
        order: 3;
        grid-column: span 2;
        max-width: 100%;
        border-radius: 25px;
      }

      .search-input {
        padding: 12px 18px;
        font-size: 14px;
      }

      .search-btn {
        padding: 12px 18px;
      }

      .action-btn {
        padding: 10px 12px;
      }

      .action-info {
        display: none;
      }

      .nav-block {
        margin-top: 8px;
      }

      .nav-inner {
        flex-wrap: wrap;
      }

      .category-trigger span:last-child {
        display: none;
      }

      .nav-menu {
        flex-wrap: wrap;
        justify-content: flex-start;
        overflow-x: auto;
      }

      .nav-link {
        padding: 12px 16px;
        height: auto;
        white-space: nowrap;
      }
    }

    @media (max-width: 480px) {
      .header-block {
        padding: 12px 0;
      }

      .brand-text {
        display: none;
      }

      .search-box {
        order: 3;
        grid-column: span 2;
        border-radius: 20px;
      }

      .search-input {
        padding: 10px 16px;
        font-size: 13px;
      }

      .search-btn {
        padding: 10px 16px;
      }

      .search-btn .material-icons {
        font-size: 20px;
      }

      .action-btn {
        padding: 8px 10px;
      }

      .action-icon {
        font-size: 1.2rem;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UnifiedHeaderComponent implements OnInit {
  config$: Observable<SiteConfig>;
  menuItems$: Observable<MenuItem[]>;
  categories$: Observable<Category[]>;

  constructor(private dataService: DataService) {
    this.config$ = this.dataService.getConfig();
    this.menuItems$ = this.dataService.getMenuItems();
    this.categories$ = this.dataService.getAllCategories();
  }

  ngOnInit(): void {}
}
