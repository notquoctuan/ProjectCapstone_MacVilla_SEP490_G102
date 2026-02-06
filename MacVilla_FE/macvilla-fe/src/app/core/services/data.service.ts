import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { 
  Product, Category, MenuItem, Banner, NewsItem, 
  Review, ContactInfo, FooterLink, SocialLink, SiteConfig 
} from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  
  // Site Configuration
  private config: SiteConfig = {
    siteName: 'MacVilla',
    siteSlogan: 'Tổng Kho Thiết Bị Vệ Sinh',
    logo: '🏠',
    primaryColor: '#2e7d32',
    secondaryColor: '#1b5e20'
  };

  // Section Titles Configuration
  private sectionTitles = {
    news: 'Latest News',
    bestSellers: 'Best-Selling Products'
  };

  // Mock Data - Products
  private products: Product[] = [
    { productId: 1, productName: 'BỒN CẦU TOTO CW887', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=WC', salePrice: 4500000, originalPrice: 5200000, rating: 5, ratingCount: 128, categoryId: 1, isFeatured: true, badge: 'Bán chạy' },
    { productId: 2, productName: 'LAVABO ĐÁ TỰ NHIÊN', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=Lavabo', salePrice: 3200000, originalPrice: undefined, rating: 5, ratingCount: 89, categoryId: 2, isFeatured: true },
    { productId: 3, productName: 'VÒI SEN INAX', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=Sen', salePrice: 1800000, originalPrice: 2200000, rating: 5, ratingCount: 256, categoryId: 3, isFeatured: true },
    { productId: 4, productName: 'BỒN TẮM NẰM', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=Tắm', salePrice: 8500000, originalPrice: 9800000, rating: 4, ratingCount: 67, categoryId: 4, isFeatured: true },
    { productId: 5, productName: 'VÒI LAVABO CAO CẤP', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=Vòi', salePrice: 2800000, originalPrice: 3500000, rating: 5, ratingCount: 198, categoryId: 3, isFeatured: true },
    { productId: 6, productName: 'BỒN CẦU VIGLACERA C1', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=C1', salePrice: 3800000, originalPrice: 4500000, rating: 5, ratingCount: 145, categoryId: 1, isFeatured: false },
    { productId: 7, productName: 'BỒN CẦU VIGLACERA C2', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=C2', salePrice: 4200000, originalPrice: undefined, rating: 4, ratingCount: 98, categoryId: 1, isFeatured: false },
    { productId: 8, productName: 'BỒN CẦU ONE PIECE', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=One', salePrice: 5500000, originalPrice: 6500000, rating: 5, ratingCount: 176, categoryId: 1, isFeatured: false },
    { productId: 9, productName: 'BỒN CẦU TWO PIECE', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=Two', salePrice: 3900000, originalPrice: undefined, rating: 4, ratingCount: 89, categoryId: 1, isFeatured: false },
    { productId: 10, productName: 'BỒN TIỂU VIGLACERA', imageUrl: 'https://via.placeholder.com/300x300/e8f5e9/2e7d32?text=Tiểu', salePrice: 1900000, originalPrice: 2500000, rating: 5, ratingCount: 67, categoryId: 1, isFeatured: false }
  ];

  // Best Sellers - Products with high sales/rating
  private bestSellers: Product[] = [
    { productId: 1, productName: 'BỒN CẦU TOTO CW887', imageUrl: 'https://via.placeholder.com/100x100/e8f5e9/2e7d32?text=WC', salePrice: 4500000, originalPrice: 5200000, rating: 5, ratingCount: 128, categoryId: 1 },
    { productId: 3, productName: 'VÒI SEN INAX', imageUrl: 'https://via.placeholder.com/100x100/e8f5e9/2e7d32?text=Sen', salePrice: 1800000, originalPrice: 2200000, rating: 5, ratingCount: 256, categoryId: 3 },
    { productId: 5, productName: 'VÒI LAVABO CAO CẤP', imageUrl: 'https://via.placeholder.com/100x100/e8f5e9/2e7d32?text=Vòi', salePrice: 2800000, originalPrice: 3500000, rating: 5, ratingCount: 198, categoryId: 3 },
    { productId: 8, productName: 'BỒN CẦU ONE PIECE', imageUrl: 'https://via.placeholder.com/100x100/e8f5e9/2e7d32?text=One', salePrice: 5500000, originalPrice: 6500000, rating: 5, ratingCount: 176, categoryId: 1 }
  ];

  // Mock Data - Categories
  private categories: Category[] = [
    { categoryId: 1, categoryName: 'Bồn cầu một khối', slug: 'bon-cau-mot-khoi' },
    { categoryId: 2, categoryName: 'Bồn cầu hai khối', slug: 'bon-cau-hai-khoi' },
    { categoryId: 3, categoryName: 'Bồn tiểu', slug: 'bon-tieu' },
    { categoryId: 4, categoryName: 'Chậu lavabo đặt bàn', slug: 'chao-lavabo-dat-ban' },
    { categoryId: 5, categoryName: 'Chậu lavabo treo tường', slug: 'chao-lavabo-treo-tuong' },
    { categoryId: 6, categoryName: 'Bộ vòi sen', slug: 'bo-voi-sen' },
    { categoryId: 7, categoryName: 'Vòi lavabo', slug: 'voi-lavabo' },
    { categoryId: 8, categoryName: 'Phụ kiện phòng tắm', slug: 'phu-kien-phong-tam' },
    { categoryId: 9, categoryName: 'Bồn tắm', slug: 'bon-tam' },
    { categoryId: 10, categoryName: 'Bình nóng lạnh', slug: 'binh-nong-lanh' }
  ];

  private tabCategories: Category[] = [
    { categoryId: 1, categoryName: 'Bồn cầu' },
    { categoryId: 1, categoryName: 'Bồn cầu một khối' },
    { categoryId: 2, categoryName: 'Bồn cầu hai khối' },
    { categoryId: 3, categoryName: 'Bồn tiểu' }
  ];

  // Mock Data - Menu Items
  private menuItems: MenuItem[] = [
    { id: 1, name: 'Trang chủ', url: '/', hasDropdown: false },
    { id: 2, name: 'Giới thiệu', url: '/about', hasDropdown: false },
    { id: 3, name: 'Sản phẩm', url: '/products', hasDropdown: true, isActive: true },
    { id: 4, name: 'Combo', url: '/combo', hasDropdown: false },
    { id: 5, name: 'Khuyến mãi', url: '/promotions', hasDropdown: false },
    { id: 6, name: 'Tin tức', url: '/news', hasDropdown: false },
    { id: 7, name: 'Dịch vụ', url: '/services', hasDropdown: false }
  ];

  // Mock Data - Banners (for slider)
  private banners: Banner[] = [
    { id: 1, imageUrl: 'https://via.placeholder.com/1200x500/e8f5e9/2e7d32?text=VIGLACERA+Premium+Sanitary', title: 'VIGLACERA', subtitle: 'Thiết bị vệ sinh cao cấp', badge: '100% Chính hãng', badgeColor: '#ffc107', isActive: true },
    { id: 2, imageUrl: 'https://via.placeholder.com/1200x500/1b5e20/fff?text=Khuyến+mãi+50%25', title: 'Khuyến mãi 50%', subtitle: 'Giảm giá sốc các sản phẩm Viglacera', badge: 'GIẢM GIÁ', badgeColor: '#f44336' },
    { id: 3, imageUrl: 'https://via.placeholder.com/1200x500/43a047/fff?text=Giao+hàng+Miễn+phí', title: 'Giao hàng miễn phí', subtitle: 'Cho đơn hàng từ 2 triệu', badge: 'FREE SHIP', badgeColor: '#4caf50' }
  ];

  // Mock Data - News with thumbnails and publish dates
  private newsItems: NewsItem[] = [
    { 
      id: 1, 
      title: 'MacVilla khai trương chi nhánh mới tại Hà Nội', 
      thumbnail: 'https://via.placeholder.com/80x80/e8f5e9/2e7d32?text=News1',
      publishDate: '2024-01-15',
      location: 'Hà Nội',
      url: '#' 
    },
    { 
      id: 2, 
      title: 'Tư vấn lựa chọn bồn cầu phù hợp cho gia đình', 
      thumbnail: 'https://via.placeholder.com/80x80/e8f5e9/2e7d32?text=News2',
      publishDate: '2024-01-12',
      location: 'TP. HCM',
      url: '#' 
    },
    { 
      id: 3, 
      title: 'Hướng dẫn bảo quản thiết bị vệ sinh đúng cách', 
      thumbnail: 'https://via.placeholder.com/80x80/e8f5e9/2e7d32?text=News3',
      publishDate: '2024-01-10',
      location: 'Đà Nẵng',
      url: '#' 
    },
    { 
      id: 4, 
      title: 'Khuyến mãi đặc biệt - Giảm giá lên đến 30%', 
      thumbnail: 'https://via.placeholder.com/80x80/e8f5e9/2e7d32?text=News4',
      publishDate: '2024-01-08',
      location: 'Toàn quốc',
      url: '#' 
    }
  ];

  // Mock Data - Reviews
  private reviews: Review[] = [
    { id: 1, text: 'Sản phẩm chính hãng, giao hàng nhanh, nhân viên tư vấn nhiệt tình!', author: 'Anh Minh, Hà Nội', rating: 5 },
    { id: 2, text: 'Giá cả hợp lý, nhiều mẫu mã đẹp, tôi rất hài lòng!', author: 'Chị Lan, TP. HCM', rating: 5 }
  ];

  // Mock Data - Contact
  private contactInfo: ContactInfo = { phone: '1900 xxxx', email: 'contact@macvilla.vn', address: '123 Nguyễn Trãi, Hà Nội' };

  // Mock Data - Footer Links
  private footerLinks: FooterLink[] = [
    { id: 1, title: 'Chính sách bảo hành', url: '#' },
    { id: 2, title: 'Chính sách đổi trả', url: '#' },
    { id: 3, title: 'Chính sách vận chuyển', url: '#' },
    { id: 4, title: 'Chính sách thanh toán', url: '#' }
  ];

  // Mock Data - Social
  private socialLinks: SocialLink[] = [
    { id: 1, name: 'Facebook', icon: 'chat', url: '#', color: '#1877f2' },
    { id: 2, name: 'YouTube', icon: 'play_circle', url: '#', color: '#ff0000' },
    { id: 3, name: 'Zalo', icon: 'chat_bubble', url: '#', color: '#0068ff' }
  ];

  constructor() { }

  getConfig(): Observable<SiteConfig> {
    return of(this.config);
  }

  getSectionTitles(): Observable<{ news: string; bestSellers: string }> {
    return of(this.sectionTitles);
  }

  getFeaturedProducts(): Observable<Product[]> {
    return of(this.products.filter(p => p.isFeatured)).pipe(delay(100));
  }

  getBestSellers(): Observable<Product[]> {
    return of(this.bestSellers).pipe(delay(100));
  }

  getAllProducts(): Observable<Product[]> {
    return of(this.products).pipe(delay(100));
  }

  getProductsByCategory(categoryId: number): Observable<Product[]> {
    return of(this.products.filter(p => p.categoryId === categoryId)).pipe(delay(100));
  }

  getAllCategories(): Observable<Category[]> {
    return of(this.categories).pipe(delay(100));
  }

  getTabCategories(): Observable<Category[]> {
    return of(this.tabCategories).pipe(delay(100));
  }

  getMenuItems(): Observable<MenuItem[]> {
    return of(this.menuItems).pipe(delay(100));
  }

  getBanners(): Observable<Banner[]> {
    return of(this.banners).pipe(delay(100));
  }

  getNewsItems(): Observable<NewsItem[]> {
    return of(this.newsItems).pipe(delay(100));
  }

  getReviews(): Observable<Review[]> {
    return of(this.reviews).pipe(delay(100));
  }

  getContactInfo(): Observable<ContactInfo> {
    return of(this.contactInfo).pipe(delay(100));
  }

  getFooterLinks(): Observable<FooterLink[]> {
    return of(this.footerLinks).pipe(delay(100));
  }

  getSocialLinks(): Observable<SocialLink[]> {
    return of(this.socialLinks).pipe(delay(100));
  }

  addToCart(product: Product): void {
    console.log('Added to cart:', product.productName);
  }
}
