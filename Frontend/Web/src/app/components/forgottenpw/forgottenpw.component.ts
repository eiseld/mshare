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

  constructor(private forgottenService: ForgottenpwService, private fb: FormBuilder) {
    this.psResetForm = fb.group({
      'email': [null, Validators.compose([Validators.required, Validators.email])]
    });
  }

  makeRequestToResetLink(formData, valid: boolean) {
    if (valid) {
      this._success.next(`A jelszó változtatása sikeresen megtörtént`);
      this.type='success';
      this.forgottenService.sendForgottenPwMail(formData.email)
        .pipe(first())
        .subscribe(
          data => {
          },
          error => {
          });
    }else {
      this._success.next(`A megadtt email formátuma nem megfelelő!`);
      this.type='danger';
    }
  }

}
