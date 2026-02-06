import { Component, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { ContactInfo, FooterLink, SocialLink } from '../../../core/models/models';
import { DataService } from '../../../core/services/data.service';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer class="main-footer" role="contentinfo">
      <div class="container">
        <div class="footer-grid">
          <div class="footer-column about">
            <h3 class="footer-title">
              <span class="material-icons">business</span>
              VỀ CHÚNG TÔI
            </h3>
            <p class="footer-desc">
              MacVilla - Tổng Kho Thiết Bị Vệ Sinh hàng đầu Việt Nam. 
              Cam kết mang đến sản phẩm chính hãng với giá tốt nhất.
            </p>
          </div>
          
          <div class="footer-column">
            <h3 class="footer-title">
              <span class="material-icons">contact_phone</span>
              LIÊN HỆ
            </h3>
            <address class="contact-info" *ngIf="contactInfo$ | async as contact">
              <p class="contact-item">
                <span class="material-icons">phone</span>
                <strong>Hotline:</strong> {{ contact.phone }}
              </p>
              <p class="contact-item">
                <span class="material-icons">email</span>
                <strong>Email:</strong> {{ contact.email }}
              </p>
              <p class="contact-item">
                <span class="material-icons">location_on</span>
                <strong>Địa chỉ:</strong> {{ contact.address }}
              </p>
            </address>
          </div>
          
          <div class="footer-column">
            <h3 class="footer-title">
              <span class="material-icons">policy</span>
              CHÍNH SÁCH
            </h3>
            <ul class="footer-links">
              <li *ngFor="let link of footerLinks$ | async">
                <a href="{{ link.url }}">
                  <span class="material-icons">arrow_right</span>
                  {{ link.title }}
                </a>
              </li>
            </ul>
          </div>
          
          <div class="footer-column">
            <h3 class="footer-title">
              <span class="material-icons">share</span>
              KẾT NỐI
            </h3>
            <div class="social-icons">
              <a *ngFor="let social of socialLinks$ | async" 
                 href="{{ social.url }}" 
                 class="social-link"
                 [style.--social-color]="social.color"
                 [attr.aria-label]="social.name">
                <span class="material-icons">{{ social.icon }}</span>
                <span class="social-name">{{ social.name }}</span>
              </a>
            </div>
          </div>
        </div>
        
        <div class="footer-bottom">
          <p>&copy; 2024 MacVilla. All rights reserved.</p>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    :host {
      display: block;
    }
    
    .main-footer {
      background: var(--color-primary-dark, #1b5e20);
      color: #fff;
      padding: 60px 0 30px;
      margin-top: 60px;
    }
    
    .footer-grid {
      display: grid;
      grid-template-columns: 1.5fr 1fr 1fr 1fr;
      gap: 40px;
    }
    
    .footer-title {
      display: flex;
      align-items: center;
      gap: 10px;
      font-size: 16px;
      font-weight: 700;
      text-transform: uppercase;
      margin: 0 0 20px;
      color: #fff;
      padding-bottom: 12px;
      border-bottom: 2px solid rgba(255, 255, 255, 0.2);
    }
    
    .footer-title .material-icons {
      font-size: 22px;
    }
    
    .footer-desc {
      font-size: 14px;
      line-height: 1.7;
      color: rgba(255, 255, 255, 0.85);
      margin: 0;
    }
    
    .contact-info {
      font-style: normal;
    }
    
    .contact-item {
      display: flex;
      align-items: flex-start;
      gap: 10px;
      font-size: 14px;
      color: rgba(255, 255, 255, 0.85);
      margin-bottom: 12px;
    }
    
    .contact-item .material-icons {
      font-size: 18px;
      color: #4caf50;
    }
    
    .footer-links {
      list-style: none;
      padding: 0;
      margin: 0;
    }
    
    .footer-links li {
      margin-bottom: 10px;
    }
    
    .footer-links a {
      display: flex;
      align-items: center;
      gap: 8px;
      color: rgba(255, 255, 255, 0.85);
      text-decoration: none;
      font-size: 14px;
      transition: all 0.3s ease;
    }
    
    .footer-links a:hover {
      color: #fff;
      padding-left: 4px;
    }
    
    .footer-links .material-icons {
      font-size: 16px;
      color: #4caf50;
    }
    
    .social-icons {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    
    .social-link {
      display: inline-flex;
      align-items: center;
      gap: 12px;
      padding: 10px 16px;
      background: rgba(255, 255, 255, 0.1);
      border-radius: var(--radius-sm, 6px);
      color: #fff;
      text-decoration: none;
      transition: all 0.3s ease;
    }
    
    .social-link:hover {
      background: var(--social-color, rgba(255, 255, 255, 0.2));
      transform: translateX(4px);
    }
    
    .social-name {
      font-size: 14px;
      font-weight: 500;
    }
    
    .footer-bottom {
      text-align: center;
      padding-top: 30px;
      margin-top: 40px;
      border-top: 1px solid rgba(255, 255, 255, 0.1);
    }
    
    .footer-bottom p {
      font-size: 14px;
      color: rgba(255, 255, 255, 0.6);
      margin: 0;
    }
    
    @media (max-width: 992px) {
      .footer-grid {
        grid-template-columns: repeat(2, 1fr);
        gap: 30px;
      }
      
      .about {
        grid-column: span 2;
      }
    }
    
    @media (max-width: 576px) {
      .footer-grid {
        grid-template-columns: 1fr;
      }
      
      .about {
        grid-column: span 1;
      }
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterComponent {
  contactInfo$: Observable<ContactInfo>;
  footerLinks$: Observable<FooterLink[]>;
  socialLinks$: Observable<SocialLink[]>;

  constructor(private dataService: DataService) {
    this.contactInfo$ = this.dataService.getContactInfo();
    this.footerLinks$ = this.dataService.getFooterLinks();
    this.socialLinks$ = this.dataService.getSocialLinks();
  }
}
