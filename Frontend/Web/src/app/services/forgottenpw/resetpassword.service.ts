import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ResetpasswordService {

  constructor(private http: HttpClient){}

resetPassword(email: string, password: string, token: string) {

  const httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };
  return this.http.post<any>(`${environment.API_URL}/forgottenpassword/resetpass`, {
    'email': email,
    'token' : token,
    'password' : password}, httpOptions)
    .pipe(map(response => {
      if (response) {
        return response;
      }
      else{
        return 'Probléma történt a jelszó megváltoztatásakor!';
      }
    }));
}


}
