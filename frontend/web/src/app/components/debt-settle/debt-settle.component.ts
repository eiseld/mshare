import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MemberData, GroupData, Debt } from '../group-manager/group-manager.component';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-debt-settle',
  templateUrl: './debt-settle.component.html',
  styleUrls: ['./debt-settle.component.css']
})
export class DebtSettleComponent implements OnInit {
  @Input() groupData:GroupData;
  @Input() selectedDebt:Debt;
  @Output() stopSettleDebtEvent = new EventEmitter();
  @Output() updateSelectedGroupEvent = new EventEmitter();
  error:string="";
  updateSelectedGroup() {
    this.updateSelectedGroupEvent.next();
  }

  constructor(private http: HttpClient, private authenticationService: AuthService) { }

  ngOnInit() {
  }

  unselectMember(){
    this.selectedDebt=null;
    this.stopSettleDebtEvent.emit();
  }

  stopSettleDebt(){
    this.unselectMember();
  }

  settleDebt(){
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    this.http.get<any>(`${environment.API_URL}/profile/`,
      httpOptions).subscribe(
        data => {
          if(this.selectedDebt.optimisedDebtAmount<0){
            this.http.post<any>(`${environment.API_URL}/group/${this.groupData.id}/settledebt/${data.id}/${this.selectedDebt.debtor.id}`,
            {}, httpOptions).subscribe(
              data => {this.updateSelectedGroup(); this.unselectMember();},
              error => {this.error="Sikertelen a tartozás rendezése!"}
            );
          }
          else if(this.selectedDebt.optimisedDebtAmount>0){
            this.http.post<any>(`${environment.API_URL}/group/${this.groupData.id}/settledebt/${this.selectedDebt.debtor.id}/${data.id}`,{},
            httpOptions).subscribe(
              data => {this.updateSelectedGroup(); this.unselectMember();},
              error => {this.error="Sikertelen a tartozás rendezése!"}
            );
          }
          else{
            this.updateSelectedGroup();
            this.unselectMember();
          }
        },
        error => {this.error="Sikertelen a tartozás rendezése!"}
      );
  }
}
