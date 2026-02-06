import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Category } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private mockCategories: Category[] = [
    { categoryId: 1, categoryName: 'Bồn cầu một khối', icon: '●', slug: 'bon-cau-mot-khoi' },
    { categoryId: 2, categoryName: 'Bồn cầu hai khối', icon: '●', slug: 'bon-cau-hai-khoi' },
    { categoryId: 3, categoryName: 'Bồn tiểu', icon: '●', slug: 'bon-tieu' },
    { categoryId: 4, categoryName: 'Chậu lavabo đặt bàn', icon: '●', slug: 'chao-lavabo-dat-ban' },
    { categoryId: 5, categoryName: 'Chậu lavabo treo tường', icon: '●', slug: 'chao-lavabo-treo-tuong' },
    { categoryId: 6, categoryName: 'Bộ vòi sen', icon: '●', slug: 'bo-voi-sen' },
    { categoryId: 7, categoryName: 'Vòi lavabo', icon: '●', slug: 'voi-lavabo' },
    { categoryId: 8, categoryName: 'Phụ kiện phòng tắm', icon: '●', slug: 'phu-kien-phong-tam' },
    { categoryId: 9, categoryName: 'Bồn tắm', icon: '●', slug: 'bon-tam' },
    { categoryId: 10, categoryName: 'Bình nóng lạnh', icon: '●', slug: 'binh-nong-lanh' }
  ];

  private mockTabCategories: Category[] = [
    { categoryId: 1, categoryName: 'Bồn cầu' },
    { categoryId: 1, categoryName: 'Bồn cầu một khối' },
    { categoryId: 2, categoryName: 'Bồn cầu hai khối' },
    { categoryId: 3, categoryName: 'Bồn tiểu' }
  ];

  constructor() { }

  getAllCategories(): Observable<Category[]> {
    return of(this.mockCategories);
  }

  getTabCategories(): Observable<Category[]> {
    return of(this.mockTabCategories);
  }

  getCategoryById(categoryId: number): Observable<Category | undefined> {
    return of(this.mockCategories.find(c => c.categoryId === categoryId));
  }
}
