import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { MenuItem, Banner, NewsItem, Review, ContactInfo, FooterLink, SocialLink } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  private mockMenuItems: MenuItem[] = [
    { id: 1, name: 'Home', url: '/', hasDropdown: false },
    { id: 2, name: 'About', url: '/about', hasDropdown: false },
    { id: 3, name: 'Products', url: '/products', hasDropdown: false },
    { id: 4, name: 'Combo', url: '/combo', hasDropdown: false },
    { id: 5, name: 'Promotions', url: '/promotions', hasDropdown: false },
    { id: 6, name: 'News', url: '/news', hasDropdown: false },
    { id: 7, name: 'Services', url: '/services', hasDropdown: false }
  ];

  private mockBanners: Banner[] = [
    {
      id: 1,
      imageUrl: 'https://via.placeholder.com/800x400/e8f5e9/2e7d32?text=VIGLACERA+Sanitary+Products',
      title: 'VIGLACERA',
      subtitle: 'Sanitary Products',
      badge: '100% Authentic',
      link: '/products'
    }
  ];

  private mockNews: NewsItem[] = [
    { id: 1, title: 'Hà Nội: 123 Nguyễn Trãi, Q. Thượng Đình', location: 'Hà Nội', url: '#' },
    { id: 2, title: 'TP. HCM: 456 Lê Văn Sỹ, Q.3', location: 'TP. HCM', url: '#' },
    { id: 3, title: 'Đà Nẵng: 789 Nguyễn Văn Linh', location: 'Đà Nẵng', url: '#' },
    { id: 4, title: 'Hải Phòng: 321 Trần Nguyên Hãn', location: 'Hải Phòng', url: '#' },
    { id: 5, title: 'Cần Thơ: 654 Đường 30/4', location: 'Cần Thơ', url: '#' }
  ];

  private mockReviews: Review[] = [
    {
      id: 1,
      text: 'Sản phẩm chính hãng, giao hàng nhanh, nhân viên tư vấn nhiệt tình!',
      author: 'Anh Minh, Hà Nội',
      rating: 5
    },
    {
      id: 2,
      text: 'Giá cả hợp lý, nhiều mẫu mã đẹp, tôi rất hài lòng!',
      author: 'Chị Lan, TP. HCM',
      rating: 5
    }
  ];

  private mockContactInfo: ContactInfo = {
    phone: '1900 xxxx',
    email: 'contact@macvilla.vn',
    address: '123 Nguyễn Trãi, Hà Nội'
  };

  private mockFooterLinks: FooterLink[] = [
    { id: 1, title: 'Chính sách bảo hành', url: '#' },
    { id: 2, title: 'Chính sách đổi trả', url: '#' },
    { id: 3, title: 'Chính sách vận chuyển', url: '#' },
    { id: 4, title: 'Chính sách thanh toán', url: '#' }
  ];

  private mockSocialLinks: SocialLink[] = [
    { id: 1, name: 'Facebook', icon: '📘', url: '#' },
    { id: 2, name: 'YouTube', icon: '📺', url: '#' },
    { id: 3, name: 'Zalo', icon: '💬', url: '#' }
  ];

  constructor() { }

  getMenuItems(): Observable<MenuItem[]> {
    return of(this.mockMenuItems);
  }

  getBanners(): Observable<Banner[]> {
    return of(this.mockBanners);
  }

  getNewsItems(): Observable<NewsItem[]> {
    return of(this.mockNews);
  }

  getReviews(): Observable<Review[]> {
    return of(this.mockReviews);
  }

  getContactInfo(): Observable<ContactInfo> {
    return of(this.mockContactInfo);
  }

  getFooterLinks(): Observable<FooterLink[]> {
    return of(this.mockFooterLinks);
  }

  getSocialLinks(): Observable<SocialLink[]> {
    return of(this.mockSocialLinks);
  }
}
