import { Component, OnInit } from '@angular/core';
import {debounceTime, first} from "rxjs/operators";
import {ForgottenpwService} from "../../services/forgottenpw/forgottenpw.service";
import {Subject} from "rxjs/internal/Subject";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-forgottenpw',
  templateUrl: './forgottenpw.component.html',
  styleUrls: ['./forgottenpw.component.css']
})
export class ForgottenpwComponent implements OnInit {

  //constructor(private forgottenService: ForgottenpwService) { }
  email: string;
  private _success = new Subject<string>();
  staticAlertClosed = false;
  successMessage: string;
  type: string;

  ngOnInit(): void {
    setTimeout(() => this.staticAlertClosed = true, 20000);

    this._success.subscribe((message) => this.successMessage = message);
    this._success.pipe(
      debounceTime(5000)
    ).subscribe(() => this.successMessage = null);
  }


  psResetForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.psResetForm = fb.group({
      'email': [null, Validators.compose([Validators.required, Validators.email])]
    });
  }

  makeRequestToResetLink(formData, valid: boolean) {
    if (valid) {
      this._success.next(`Amennyiben a megadott e-mail cím regisztrálva van nálunk, arra hamarosan megérkezik a jelszó-emlékeztető..`);
      this.type='success';
    }else {
      this._success.next(`A megadtt email formátuma nem megfelelő!`);
      this.type='danger';
    }
    // this.submitted = true;
    // if (this.loginForm.invalid) {
    //   return;
    // }
    //
    // this.forgottenService.sendForgottenPwMail(t)
    // this.authService.login(this.loginForm.controls.username.value, this.loginForm.controls.password.value)
    //   .pipe(first())
    //   .subscribe(
    //     data => {
    //       this.router.navigate([this.returnUrl]);
    //     },
    //     error => {
    //       this.error = "Helytelen felhasználónév vagy jelszó";
    //       this.loading = false;
    //     });
  }

}
