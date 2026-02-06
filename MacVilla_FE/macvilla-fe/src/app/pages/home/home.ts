import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { 
  Product, Category, SiteConfig, MenuItem, NewsItem,
  PaginationConfig, PaginatedResponse 
} from '../../core/models/models';
import { DataService } from '../../core/services/data.service';
import { ProductCardComponent } from '../../shared/components/product-card/product-card.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { BannerSliderComponent } from '../../shared/components/banner-slider/banner-slider.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import { UnifiedHeaderComponent } from '../../shared/components/unified-header/unified-header.component';
import { NewsSidebarComponent } from '../../shared/components/news-sidebar/news-sidebar.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ProductCardComponent,
    PaginationComponent,
    BannerSliderComponent,
    FooterComponent,
    UnifiedHeaderComponent,
    NewsSidebarComponent
  ],
  templateUrl: './home.html',
  styleUrls: ['./home.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent implements OnInit {
  config$: Observable<SiteConfig>;
  menuItems$: Observable<MenuItem[]>;
  categories$: Observable<Category[]>;
  featuredProducts$: Observable<Product[]>;
  tabCategories$: Observable<Category[]>;
  tabProducts$: Observable<Product[]>;
  newsItems$: Observable<NewsItem[]>;
  bestSellers$: Observable<Product[]>;
  sectionTitles$: Observable<{ news: string; bestSellers: string }>;
  
  featuredPagination: PaginationConfig = {
    currentPage: 1,
    pageSize: 10,
    totalItems: 10,
    totalPages: 1
  };
  
  tabPagination: PaginationConfig = {
    currentPage: 1,
    pageSize: 10,
    totalItems: 10,
    totalPages: 1
  };

  constructor(private dataService: DataService) {
    this.config$ = this.dataService.getConfig();
    this.menuItems$ = this.dataService.getMenuItems();
    this.categories$ = this.dataService.getAllCategories();
    this.featuredProducts$ = this.dataService.getFeaturedProducts();
    this.tabCategories$ = this.dataService.getTabCategories();
    this.tabProducts$ = this.dataService.getProductsByCategory(1);
    this.newsItems$ = this.dataService.getNewsItems();
    this.bestSellers$ = this.dataService.getBestSellers();
    this.sectionTitles$ = this.dataService.getSectionTitles();
  }

  ngOnInit(): void {
    // Data loaded via observables
  }

  trackByCategory(index: number, item: Category): number {
    return item.categoryId;
  }

  trackByProduct(index: number, item: Product): number {
    return item.productId;
  }

  onAddToCart(product: Product): void {
    this.dataService.addToCart(product);
  }

  onFeaturedPageChange(page: number): void {
    this.featuredPagination = { ...this.featuredPagination, currentPage: page };
  }

  onTabPageChange(page: number): void {
    this.tabPagination = { ...this.tabPagination, currentPage: page };
  }

  onTabChange(category: Category): void {
    this.dataService.getProductsByCategory(category.categoryId).subscribe(products => {
      this.tabPagination = {
        ...this.tabPagination,
        totalItems: products.length,
        totalPages: Math.ceil(products.length / this.tabPagination.pageSize)
      };
    });
  }

  scrollToTop(): void {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
