import { Component, OnInit, Input, Output, OnChanges, ChangeDetectionStrategy, EventEmitter } from '@angular/core';
import { GroupInfo, GroupData, MemberData } from '../group-manager/group-manager.component';
import { Spending } from '../spending-creator/spending-creator.component'

@Component({
  selector: 'app-group-detail',
  templateUrl: './group-detail.component.html',
  styleUrls: ['./group-detail.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GroupDetailComponent implements OnChanges {
  @Input() groupData: GroupData;
  @Output() updateSelectedGroupEvent = new EventEmitter();

  @Input() spendings: Spending[];
  calculatedSpendings: CalculatedSpending[]=[];
  pages={groupMemberDetails:0,groupSpendingDetails:1};
  selectedPage=this.pages.groupMemberDetails;
  @Output() startSpendingCreation = new EventEmitter();

  ngOnChanges(){
    this.showPage(this.selectedPage);
  }

  startCreateSpending() {
    this.startSpendingCreation.next(new GroupInfo(this.groupData));
  }

  constructor() { }

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

  ngOnInit() {
  }

  selectMember(memberData : MemberData){
    this.selectedMember=memberData;
  }

  unselectMember(){
    this.selectedMember=undefined;

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