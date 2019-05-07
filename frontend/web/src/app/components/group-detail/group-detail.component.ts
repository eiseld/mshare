import { Component, OnInit, Input, OnChanges, ChangeDetectionStrategy } from '@angular/core';
import { GroupInfo, GroupData, MemberData, GroupManagerComponent } from '../group-manager/group-manager.component';
import { Spending } from '../spending-creator/spending-creator.component'
import { Output, EventEmitter } from '@angular/core'; 
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment'

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
    this.startSpendingCreation.next(new GroupInfo(this.groupData));
  }

  addNewMember() {

  }


  constructor(private http: HttpClient) {

  }

  calcDefaultDebts(){
    if(this.calculatedSpendings!=undefined){
      delete this.calculatedSpendings;
      this.calculatedSpendings=[];
    }
    for(let spending of this.spendings){
      var calculatedSpending=new CalculatedSpending();
      calculatedSpending.spending=spending;
      if(spending.moneyOwed==undefined){
        calculatedSpending.defaultDebt=0;
      }
      else{
        calculatedSpending.defaultDebt=spending.moneyOwed;
        var memberCount=spending.debtors.length;
        for(let member of spending.debtors){
          if(member.balance!=undefined){
            calculatedSpending.defaultDebt-=member.balance;
            memberCount--;
          }
        }
        if(memberCount>0){
          calculatedSpending.defaultDebt/=memberCount;
          calculatedSpending.groupSpending=false;
        }
        else{
          calculatedSpending.groupSpending=true;
        }
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
    this.selectedPage = this.pages.groupMemberDetails;
  }

  ngOnInit() {
    this.getCurrentUser();
    this.selectedPage = this.pages.groupMemberDetails; 
  }

  showPage(selectedPage){
    this.selectedPage=selectedPage;
    if(this.selectedPage==this.pages.groupSpendingDetails){
      this.calcDefaultDebts();
    }
   
  }
}

export class CalculatedSpending{
  spending: Spending;
  defaultDebt: number;
  groupSpending:boolean;
}
