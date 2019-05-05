import { Component, OnInit, Input, OnChanges, ChangeDetectionStrategy } from '@angular/core';
import { GroupInfo, GroupData, MemberData } from '../group-manager/group-manager.component';
import { Spending, DebtorData } from '../spending-creator/spending-creator.component'
import { Output, EventEmitter } from '@angular/core'; 
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-group-detail',
  templateUrl: './group-detail.component.html',
  styleUrls: ['./group-detail.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GroupDetailComponent implements OnChanges {
  @Input() groupData: GroupData;
  @Input() spendings: Spending[];
  calculatedSpendings: CalculatedSpending[]=[];
  pages={groupMemberDetails:0,groupSpendingDetails:1};
  selectedPage=this.pages.groupMemberDetails;
  currentUser:MemberData;
  @Output() startSpendingCreation = new EventEmitter();
  @Output() startSpendingModification = new EventEmitter();

  ngOnChanges(){
    this.showPage(this.selectedPage);
  }

  startCreateSpending() {
    this.startSpendingCreation.emit(new GroupInfo(this.groupData));
  }

  constructor(private http: HttpClient) { }

  startModifySpending(spending:Spending) {
    var modifiableSpending=new Spending();
    modifiableSpending.id=spending.id;
    modifiableSpending.name=spending.name;
    modifiableSpending.moneyOwed=spending.moneyOwed;
    modifiableSpending.creditorUserId=spending.creditorUserId;
    modifiableSpending.debtors=[];
    for(let debtor of spending.debtors){
      var newDebtor=new DebtorData();
      newDebtor.id=debtor.id;
      newDebtor.name=debtor.name;
      newDebtor.balance=debtor.balance;
      newDebtor.defaultBalance=debtor.defaultBalance;
      modifiableSpending.debtors=[...modifiableSpending.debtors, newDebtor];
    }
    this.startSpendingModification.emit(modifiableSpending);
    this.startCreateSpending();
  }

  calcGroupSpending(){
    if(this.calculatedSpendings!=undefined){
      delete this.calculatedSpendings;
      this.calculatedSpendings=[];
    }
    for(let spending of this.spendings){
      var calculatedSpending=new CalculatedSpending();
      calculatedSpending.spending=spending;
      if(spending.debtors.length==this.groupData.memberCount){
        calculatedSpending.groupSpending=true;
      }
      else{
        calculatedSpending.groupSpending=false;
      }
      this.calculatedSpendings=[...this.calculatedSpendings,calculatedSpending];
    }
  }

  ngOnInit() {
    this.getCurrentUser();
  }

  getCurrentUser(){
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    this.http.get<any>(`${environment.API_URL}/profile/`,
    httpOptions).subscribe(
      data => { this.currentUser=data; },
      error => {}
    );
  }

  showPage(selectedPage){
    this.selectedPage=selectedPage;
    if(this.selectedPage==this.pages.groupSpendingDetails){
      this.calcGroupSpending();
    }
  }
}

export class CalculatedSpending{
  spending: Spending;
  groupSpending:boolean;
}