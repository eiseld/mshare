import { Component, OnInit } from '@angular/core';
import {ResetpasswordService} from "../../services/forgottenpw/resetpassword.service";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: 'app-resetpassword',
  templateUrl: './resetpassword.component.html',
  styleUrls: ['./resetpassword.component.css']
})
export class ResetpasswordComponent implements OnInit {

  email: string;
  password: string;
  password2: string;
  token: string;

  constructor(private route: ActivatedRoute, private resetPasswordService: ResetpasswordService) {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      console.log(this.token); // Print the parameter to the console.
    });
  }

  ngOnInit() {

  }

  resetPassword(){
    this.resetPasswordService.resetPassword(this.email, this.password, this.token);
  }

}
