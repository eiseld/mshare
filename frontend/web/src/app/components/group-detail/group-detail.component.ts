import { Component, OnInit, Input } from '@angular/core';
import { GroupData, MemberData } from '../group-manager/group-manager.component';
@Component({
  selector: 'app-group-detail',
  templateUrl: './group-detail.component.html',
  styleUrls: ['./group-detail.component.css']
})
export class GroupDetailComponent implements OnInit {
  @Input() groupData: GroupData;
  constructor() { }

  ngOnInit() {
  }

}
