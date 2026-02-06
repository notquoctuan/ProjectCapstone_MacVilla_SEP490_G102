// ==================== ICONS ====================
// Using Material Icons via Google Fonts
// Add to index.html: <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

// ==================== INTERFACES ====================

export interface Product {
  productId: number;
  productName: string;
  imageUrl: string;
  salePrice: number;
  originalPrice?: number;
  rating: number;
  ratingCount: number;
  categoryId: number;
  isFeatured?: boolean;
  description?: string;
  badge?: string;
}

export interface Category {
  categoryId: number;
  categoryName: string;
  icon?: string;
  slug?: string;
  imageUrl?: string;
  productCount?: number;
}

export interface MenuItem {
  id: number;
  name: string;
  url: string;
  isActive?: boolean;
  hasDropdown?: boolean;
}

export interface Banner {
  id: number;
  imageUrl: string;
  title?: string;
  subtitle?: string;
  badge?: string;
  badgeColor?: string;
  link?: string;
  isActive?: boolean;
}

export interface NewsItem {
  id: number;
  title: string;
  thumbnail?: string;
  publishDate?: string;
  location?: string;
  url?: string;
}

export interface Review {
  id: number;
  text: string;
  author: string;
  rating: number;
  avatar?: string;
}

export interface PaginationConfig {
  currentPage: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  pagination: PaginationConfig;
}

export interface ContactInfo {
  phone: string;
  email: string;
  address: string;
  hotline?: string;
}

export interface FooterLink {
  id: number;
  title: string;
  url: string;
}

export interface SocialLink {
  id: number;
  name: string;
  icon: string;
  url: string;
  color?: string;
}

export interface SiteConfig {
  siteName: string;
  siteSlogan: string;
  logo: string;
  primaryColor: string;
  secondaryColor: string;
}
