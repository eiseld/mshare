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


/*
export interface ReCaptchaConfig {
  theme? : 'dark' | 'light';
  type? : 'audio' | 'image';
  size? : 'compact' | 'normal';
  tabindex?: number,
}

@Directive({
  selector: '[nbRecaptcha]'
})

export class RecaptchaDirective implements OnInit, AfterViewInit, ControlValueAccessor {
  @Input() key: string;
  @Input() config: ReCaptchaConfig = {};
  @Input() lang: string;

  private widgetId: number;
  private onChange: (value: string) => void;
  private onTouched: (value: string) => void;

  constructor(private element: ElementRef) {}

  ngOnInit() {
    this.registerReCaptchaCallback();
    this.addScript();
  }

  writeValue(obj: any): void {}

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  registerReCaptchaCallback() {
    window.reCaptchaLoad = () => {
      const config = {
        ...this.config,
        'sitekey': this.key,
        'callback': this.onSuccess.bind(this),
        'expired-callback': this.onExpired.bind(this)
      };
      this.widgetId = this.render(this.element.nativeElement, config);
    }
  }

  private render(element: HTMLElement, config): number {
    return grecaptcha.render(element, config);
  }

  addScript() {
    let script = document.createElement('script');
    const lang = this.lang ? '&hl=' + this.lang: '';
    script.src = `https://www.google.com/recaptcha/api.js?onload=reCaptchaLoad&render=explicit${lang}`;
    script.async = true;
    script.defer = true;
    document.body.appendChild(script);
  }

  onExpired() {
    this.ngZone.run(() => {
    });
  }
}

declare const grecaptcha: any;

declare global {
  interface Window {
    grecaptcha: any;
    reCaptchaLoad: () => void;
  }
} */

export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  returnUrl: String;
  error: String = "";
  loading: Boolean = false;
  submitted: Boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
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
      displayname: ['', Validators.required],
      password: ['', Validators.required]
    });
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  onSubmit() {

    this.submitted = true;
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
          this.error = "Helytelen felhasználónév vagy jelszó";
          this.loading = false;
        });
  }

}
