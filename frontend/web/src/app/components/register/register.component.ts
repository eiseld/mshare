import { Component, OnInit, ViewChild, Directive, Input, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ControlValueAccessor, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { first } from 'rxjs/operators';
import { CaptchaComponent } from 'angular-captcha';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})

export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  returnUrl: String;
  error: String = "";
  emailInvalid: String = "";
  passwordInvalid: String = "";
  passwordNotDupli: String = "";
  usernameNotDupli: String = "";
  loading: Boolean = false;
  submitted: Boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) { }

  myRecaptcha = new FormControl(false);

  onScriptLoad() {
    console.log('Google reCAPTCHA loaded and ready for use!')
  }

  onScriptError() {
    console.log('Error loading Google reCAPTCHA')
  }

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      usernameDupli: ['', Validators.required],
      displayname: ['', Validators.required],
      password: ['', Validators.required],
      passwordDupli: ['', Validators.required],
    });
    this.returnUrl = '/sucreg';
  }

  onSubmit() {
    this.error = this.emailInvalid = this.passwordInvalid = this.usernameNotDupli = this.passwordNotDupli = "";

    this.submitted = true;
    var flag = false;
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!(re.test(String(this.registerForm.controls.username.value).toLowerCase()))){
      this.emailInvalid = "Helytelen formátumú emailt adott meg!";
      flag = true;
    };
    var re = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{8,}$/;
    if (!(re.test(String(this.registerForm.controls.password.value)))) {
      this.passwordInvalid = "Jelszóban szükséges legalább 8 karakter, köztük legalább egy-egy kis- és nagybetű, valamint szám";
      flag = true;
    }
    if (this.registerForm.controls.username.value != this.registerForm.controls.usernameDupli.value) {
      this.usernameNotDupli = "Nem eggyezik meg a két megadott email!";
      flag = true;
    }
    if (this.registerForm.controls.password.value != this.registerForm.controls.passwordDupli.value) {
      this.passwordNotDupli = "Nem eggyezik meg a két megadott jelszó!";
      flag = true;
    }
    if (flag) {
      return;
    }
    if (this.registerForm.invalid) {
      return;
    }
    this.authService.register(this.registerForm.controls.username.value, this.registerForm.controls.displayname.value, this.registerForm.controls.password.value)
      .pipe(first())
      .subscribe(
        data => {
          this.router.navigate([this.returnUrl]);
        },
        error => {
          if ((error.status == 200) || (error.status == 201)) {
            this.router.navigate([this.returnUrl]);
          } else if (error.status == 422) {
            this.error = "Elírta az email címét vagy a jelszavát, próbálja újra";
          } else if (error.status == 409) {
            this.error = "Már van ilyen email címre regisztrált felhasználó!";
          } else if (error.status == 500) {
            this.error = "Belső hiba történt a szerveren!";
          } else {
            this.error = "Nem felismert hiba történt, kérjük próbálja meg újra a regisztrációt.";
          }
        });
  }

}
