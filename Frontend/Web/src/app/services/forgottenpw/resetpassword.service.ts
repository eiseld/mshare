import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ResetpasswordService {

  constructor(private http: HttpClient){}

resetPassword(email: string, token: string, password: string) {

  const httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'email': email,
      'token' : token,
      'password' : password
    })
  };
  return this.http.post<any>(`${environment.API_URL}/resetpassword`, {}, httpOptions)
    .pipe(map(response => {
      if (response) {
        return response;
      }
    }));
}


}
