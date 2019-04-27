import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { GroupData, MemberData } from '../group-manager/group-manager.component';
@Component({
  selector: 'app-group-detail',
  templateUrl: './group-detail.component.html',
  styleUrls: ['./group-detail.component.css']
})
export class GroupDetailComponent implements OnInit {
  @Input() groupData: GroupData;
  @Output() updateSelectedGroupEvent = new EventEmitter();

  constructor() { }

  selectedMember: MemberData = null;

  ngOnInit() {
  }

  selectMember(memberData : MemberData){
    this.selectedMember=memberData;
  }

  unselectMember(){
    this.selectedMember=undefined;
  }
  
  updateSelectedGroup(){
    this.updateSelectedGroupEvent.next();
  }

}
