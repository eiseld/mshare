import { Component, OnInit, ViewChild, Directive, Input, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ControlValueAccessor, FormControl } from '@angular/forms';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-sucreg',
  templateUrl: './sucreg.component.html',
  styleUrls: ['./sucreg.component.css']
})

export class SucregComponent implements OnInit {
  returnUrl: String;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService
  ) { }

  ngOnInit() {
  }

  onSubmit() {
  }

}
