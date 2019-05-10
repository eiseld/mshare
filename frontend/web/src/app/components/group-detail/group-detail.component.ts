import { Component, OnInit, Input, Output, OnChanges, ChangeDetectionStrategy, EventEmitter } from '@angular/core';
import { GroupInfo, GroupData, MemberData } from '../group-manager/group-manager.component';
import { Spending, DebtorData } from '../spending-creator/spending-creator.component'
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-group-detail',
  templateUrl: './group-detail.component.html',
  styleUrls: ['./group-detail.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GroupDetailComponent implements OnChanges {
  @Input() groupData: GroupData;
  @Input() spendings: Spending[];
  currentUser : MemberData;
  sortedMembers: MemberData[] = [];
  calculatedSpendings: CalculatedSpending[]=[];
  pages={groupMemberDetails:0,groupSpendingDetails:1,groupManageMembers:2};
  selectedPage=this.pages.groupMemberDetails;
  @Output() startSpendingCreation = new EventEmitter();
  @Output() startSpendingModification = new EventEmitter();
  @Output() updateSelectedGroupEvent = new EventEmitter();

  ngOnChanges(){
    if(this.groupData != null)
    {
      this.sortedMembers = this.groupData.members.sort(
        (a,b) => {
          if(a.name > b.name)
            return 1;
          if(a.name < b.name)
            return -1;
          return 0;
        }
      )
    }
    if(this.selectedPage == this.pages.groupManageMembers && this.currentUser.name != this.groupData.creator.name)
    {
      this.showPage(this.pages.groupMemberDetails);
    }
    else
      this.showPage(this.selectedPage);
  }

  getCurrentUser(){
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    this.http.get<any>(`${environment.API_URL}/profile/`, httpOptions).subscribe(
      data => { this.currentUser=data; },
      error => {}
    );
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

  selectedMember: MemberData = null;

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

  deleteFromGroup(id: number, groupid: number)
  {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    this.http.delete(`${environment.API_URL}/group/${groupid}/members/remove/${id}`, httpOptions)
      .subscribe(data => {this.updateSelectedGroupEvent.next()}, error => { });
  }

  selectMember(memberData : MemberData){
    this.selectedMember=memberData;
  }

  unselectMember(){
    this.selectedMember=undefined;
    this.selectedPage = this.pages.groupMemberDetails;
  }

  ngOnInit() {
    this.getCurrentUser();
    this.selectedPage = this.pages.groupMemberDetails;
  }

  showPage(selectedPage){
    this.selectedPage=selectedPage;
    if(this.selectedPage==this.pages.groupSpendingDetails){
      this.calcGroupSpending();
    }
  }
  
  updateSelectedGroup(){
    this.updateSelectedGroupEvent.next();
  }
}

export class CalculatedSpending{
  spending: Spending;
  groupSpending:boolean;
}
