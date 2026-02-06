import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Product } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private mockProducts: Product[] = [
    {
      productId: 1,
      productName: 'BỒN CẦU TOTO CW887',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=WC',
      salePrice: 4500000,
      originalPrice: 5200000,
      rating: 5,
      ratingCount: 128,
      categoryId: 1,
      isFeatured: true
    },
    {
      productId: 2,
      productName: 'LAVABO ĐÁ TỰ NHIÊN',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=Lavabo',
      salePrice: 3200000,
      originalPrice: undefined,
      rating: 5,
      ratingCount: 89,
      categoryId: 2,
      isFeatured: true
    },
    {
      productId: 3,
      productName: 'VÒI SEN INAX',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=Sen',
      salePrice: 1800000,
      originalPrice: 2200000,
      rating: 5,
      ratingCount: 256,
      categoryId: 3,
      isFeatured: true
    },
    {
      productId: 4,
      productName: 'BỒN TẮM NẰM',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=Tắm',
      salePrice: 8500000,
      originalPrice: 9800000,
      rating: 4,
      ratingCount: 67,
      categoryId: 4,
      isFeatured: true
    },
    {
      productId: 5,
      productName: 'VÒI LAVABO CAO CẤP',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=Vòi',
      salePrice: 2800000,
      originalPrice: 3500000,
      rating: 5,
      ratingCount: 198,
      categoryId: 3,
      isFeatured: true
    },
    {
      productId: 6,
      productName: 'BỒN CẦU VIGLACERA C1',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=C1',
      salePrice: 3800000,
      originalPrice: 4500000,
      rating: 5,
      ratingCount: 145,
      categoryId: 1,
      isFeatured: false
    },
    {
      productId: 7,
      productName: 'BỒN CẦU VIGLACERA C2',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=C2',
      salePrice: 4200000,
      originalPrice: undefined,
      rating: 4,
      ratingCount: 98,
      categoryId: 1,
      isFeatured: false
    },
    {
      productId: 8,
      productName: 'BỒN CẦU ONE PIECE',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=One',
      salePrice: 5500000,
      originalPrice: 6500000,
      rating: 5,
      ratingCount: 176,
      categoryId: 1,
      isFeatured: false
    },
    {
      productId: 9,
      productName: 'BỒN CẦU TWO PIECE',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=Two',
      salePrice: 3900000,
      originalPrice: undefined,
      rating: 4,
      ratingCount: 89,
      categoryId: 1,
      isFeatured: false
    },
    {
      productId: 10,
      productName: 'BỒN TIỂU VIGLACERA',
      imageUrl: 'https://via.placeholder.com/200x200/e8f5e9/2e7d32?text=Tiểu',
      salePrice: 1900000,
      originalPrice: 2500000,
      rating: 5,
      ratingCount: 67,
      categoryId: 1,
      isFeatured: false
    }
  ];

  constructor() { }

  getFeaturedProducts(): Observable<Product[]> {
    return of(this.mockProducts.filter(p => p.isFeatured));
  }

  getAllProducts(): Observable<Product[]> {
    return of(this.mockProducts);
  }

  getProductsByCategory(categoryId: number): Observable<Product[]> {
    return of(this.mockProducts.filter(p => p.categoryId === categoryId));
  }

  getProductById(productId: number): Observable<Product | undefined> {
    return of(this.mockProducts.find(p => p.productId === productId));
  }

  addToCart(product: Product): void {
    console.log('Added to cart:', product.productName);
  }
}
