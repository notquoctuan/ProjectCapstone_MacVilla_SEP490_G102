import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  private apiUrl = 'https://localhost:7262/api/Home';

  constructor(private http: HttpClient) {}

  getHome(): Observable<any> {
    return this.http.get<any>(this.apiUrl);
  }
}
