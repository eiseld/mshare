import { Component, OnInit } from '@angular/core';
import {ResetpasswordService} from "../../services/forgottenpw/resetpassword.service";
import { ActivatedRoute } from "@angular/router";
import {Subject} from "rxjs/internal/Subject";
import {debounceTime, first} from "rxjs/operators";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Observable} from "rxjs/internal/Observable";

@Component({
  selector: 'app-resetpassword',
  templateUrl: './resetpassword.component.html',
  styleUrls: ['./resetpassword.component.css']
})
export class ResetpasswordComponent implements OnInit {

  email: string;
  token: string;
  private _success = new Subject<string>();
  staticAlertClosed = false;
  successMessage: string;
  type: string;

  constructor(private route: ActivatedRoute, private resetPasswordService: ResetpasswordService, private fb: FormBuilder) {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
    });
    this.psResetForm = fb.group({
      'email': [null, Validators.compose([Validators.required, Validators.email])],
      'password': [ '',[Validators.required,Validators.pattern('^.*(?=.{6,})((?=.*[!@#$%^&*()\\-_=+{};:,<.>]){0})(?=.*\\d)((?=.*[a-z]){1})((?=.*[A-Z]){1}).*$')]],
      'confirmPassword': [ '',[Validators.required,Validators.pattern('^.*(?=.{6,})((?=.*[!@#$%^&*()\\-_=+{};:,<.>]){0})(?=.*\\d)((?=.*[a-z]){1})((?=.*[A-Z]){1}).*$')]],
    });
  }

  ngOnInit() {
    setTimeout(() => this.staticAlertClosed = true, 30000);

    this._success.subscribe((message) => this.successMessage = message);
    this._success.pipe(
      debounceTime(10000)
    ).subscribe(() => this.successMessage = null);
  }


  psResetForm: FormGroup;

  makeRequestToResetLink(formData, valid: boolean) {
    if (valid){
      if(this.psResetForm.get('password').value != this.psResetForm.get('confirmPassword').value){
        this._success.next(`Kérem ellenőrizze email címének formátumát.\n
                           Emellett bizonyosodjon meg,hogy jelszava tartalmaz legalább 1 nagy és kis betűt,
                           valamint legalább 1 számot, és minimum 6 karakter hosszú!`);
        this.type='danger';
        return;
      }
      this.resetPassword();
    }else {
      this._success.next(`Kérem ellenőrizze email címének formátumát.\n
      Emellett bizonyosodjon meg,hogy jelszava tartalmaz legalább 1 nagy és kis betűt,
       valamint legalább 1 számot, és minimum 6 karakter hosszú!`);
      this.type='danger';
    }
  }

  resetPassword(){
     this.resetPasswordService.
     resetPassword(this.psResetForm.get('email').value, this.psResetForm.get('password').value, this.token).pipe(first())
       .subscribe(
         data => {
           this._success.next('Sikeres jelszóváltoztatás!');
           this.type='success';
         },
         error => {
           this._success.next('Probléma történt a jelszó változtatásakor!');
           this.type='danger';
         });
  }

}
