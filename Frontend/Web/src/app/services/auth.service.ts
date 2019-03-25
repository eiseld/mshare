import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { BehaviorSubject, Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';
import { analyzeAndValidateNgModules } from '@angular/compiler';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  currentUserSubject: BehaviorSubject<User>;
  currentUser: Observable<User>;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User {
    return this.currentUserSubject.value;
  }

  isLoggedIn(): Boolean {
    return (this.currentUserSubject.value != null);
  }

  login(email: string, password: string) {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'email': email,
        'password': password
      })
    };
    return this.http.post<any>(`${environment.API_URL}/users/login`, {}, httpOptions)
      .pipe(map(user => {
        if (user && user.token) {
          localStorage.setItem('currentUser', JSON.stringify(user));
          this.currentUserSubject.next(user);
        }

        return user;
      }));
  }

  register(email: string, displayname: string, password: string): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({
         'Content-Type': 'application/json',
      })
    };
    return this.http.post<any>(`${environment.API_URL}/users/createUser`, {
      'email': email,
      'displayname': displayname,
      'password': password
    }, httpOptions)
       .pipe(map(user => {
         if (user && user.token) {
          localStorage.setItem('currentUser', JSON.stringify(user));
          this.currentUserSubject.next(user);
         }

         console.log(user);
         return user;
       }));
  }

  confirm(token: string) {
    const httpOptions = {
      headers: new HttpHeaders({
         'Content-Type': 'application/json',
      })
    };
    return this.http.post<any>(`${environment.API_URL}/users/validateemail/`+token,{}, httpOptions);
  }

  logout() {
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }
}
