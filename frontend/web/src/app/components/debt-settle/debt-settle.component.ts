import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MemberData } from '../group-manager/group-manager.component';

@Component({
  selector: 'app-debt-settle',
  templateUrl: './debt-settle.component.html',
  styleUrls: ['./debt-settle.component.css']
})
export class DebtSettleComponent implements OnInit {
  @Input() selectedMember:MemberData;
  @Output() stopSettleDebtEvent = new EventEmitter();
  @Output() updateSelectedGroupEvent = new EventEmitter();

  updateSelectedGroup() {
    this.updateSelectedGroupEvent.next();
  }

  constructor() { }

  ngOnInit() {
  }

  unselectMember(){
    this.selectedMember=null;
    this.stopSettleDebtEvent.emit();
  }

  stopSettleDebt(){
    this.unselectMember();
  }

  settleDebt(){
    if(this.selectedMember.balance!=0){
      //TODO connect to backend to settle debt, get update GroupData
      this.updateSelectedGroup();
    }
    this.unselectMember();
  }
}
