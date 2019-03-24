import { Component, OnInit, ViewChild, Directive, Input, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ControlValueAccessor, FormControl } from '@angular/forms';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-confirm',
  templateUrl: './confirm.component.html',
  styleUrls: ['./confirm.component.css']
})

export class ConfirmComponent implements OnInit {
  confirmForm: FormGroup;
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
    let token = this.route.snapshot.params['token'];
    this.authService.confirm(token);
  }

  onSubmit() {

  }

}
