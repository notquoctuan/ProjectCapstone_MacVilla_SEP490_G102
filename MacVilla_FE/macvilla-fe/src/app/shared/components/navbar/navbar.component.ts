import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { MenuItem } from '../../../core/models/models';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="main-nav" role="navigation" aria-label="Main navigation">
      <div class="container nav-container">
        <button class="category-trigger" aria-label="Danh mục sản phẩm">
          <span class="material-icons">menu</span>
          <span>DANH MỤC SẢN PHẨM</span>
        </button>
        
        <ul class="nav-menu" role="menubar">
          <li *ngFor="let item of menuItems$ | async" role="none">
            <a 
              [routerLink]="item.url" 
              class="nav-link"
              [class.active]="item.isActive"
              role="menuitem"
              [attr.aria-current]="item.isActive ? 'page' : null">
              {{ item.name }}
            </a>
          </li>
        </ul>
      </div>
    </nav>
  `,
  styles: [`
    :host {
      display: block;
    }
    
    .main-nav {
      background: var(--color-primary, #2e7d32);
      position: sticky;
      top: 0;
      z-index: 1000;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }
    
    .nav-container {
      display: flex;
      align-items: stretch;
      max-width: var(--container, 1300px);
      margin: 0 auto;
      padding: 0 20px;
    }
    
    .category-trigger {
      display: flex;
      align-items: center;
      gap: 10px;
      background: var(--color-primary-dark, #1b5e20);
      color: #fff;
      padding: 0 24px;
      border: none;
      font-size: 13px;
      font-weight: 700;
      text-transform: uppercase;
      cursor: pointer;
      min-height: 52px;
      transition: background-color 0.3s ease;
    }
    
    .category-trigger:hover {
      background: #144418;
    }
    
    .category-trigger .material-icons {
      font-size: 22px;
    }
    
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
      transition: background-color 0.3s ease;
      min-height: 52px;
    }
    
    .nav-link:hover {
      background: rgba(255, 255, 255, 0.1);
    }
    
    .nav-link.active {
      background: rgba(255, 255, 255, 0.15);
    }
    
    @media (max-width: 992px) {
      .category-trigger span:last-child {
        display: none;
      }
      
      .category-trigger {
        padding: 0 16px;
      }
      
      .nav-link {
        padding: 0 14px;
        font-size: 12px;
      }
    }
    
    @media (max-width: 768px) {
      .nav-container {
        padding: 0;
      }
      
      .category-trigger {
        flex: 1;
        max-width: 100px;
        justify-content: center;
      }
      
      .nav-menu {
        flex-wrap: wrap;
        justify-content: flex-start;
        overflow-x: auto;
      }
      
      .nav-link {
        padding: 14px 16px;
        min-height: auto;
        white-space: nowrap;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NavbarComponent {
  menuItems$: Observable<MenuItem[]>;

  constructor(private dataService: DataService) {
    this.menuItems$ = this.dataService.getMenuItems();
  }
}
