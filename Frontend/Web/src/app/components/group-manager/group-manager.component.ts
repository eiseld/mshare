import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-group-manager',
  templateUrl: './group-manager.component.html',
  styleUrls: ['./group-manager.component.css']
})
export class GroupManagerComponent implements OnInit {

  constructor() { }

  @Input() newGroup: Group;
  createGroupAttempt=false;
  
  //TODO get groups list from server
  id =14
  groups: Group[] = [
    { id: 11, name: 'MockupGroup1', creator: 'MockupUser', balance: 100, members:["1", "2","3","4","5"]},
    { id: 12, name: 'MockupGroup2', creator: 'MockupUser', balance: -500, members: ["1", "2","3","4"]},
    { id: 13, name: 'MockupGroup3', creator: 'MockupUser', balance: 0, members: ["1", "2"]}
  ];

  ngOnInit() {
  }

  startCreateGroup(){
    this.createGroupAttempt=true;
    this.newGroup=new Group;
    this.newGroup.name="";
    this.newGroup.creator="thisUser";
    this.newGroup.members=["thisUser"];
    this.newGroup.balance=0;
    this.newGroup.id=this.id;
    this.id++;
  }
  selectedGroup:Group;
  stopCreateGroup(){
    delete this.newGroup;
    this.createGroupAttempt=false;
  }

  createGroup(){
    //TODO check and create group on server
    this.groups.push(this.newGroup);
    this.createGroupAttempt=false;
  }

  selectGroup(group : Group){
    this.selectedGroup=group;
  }
}

export class Group {
  id: number;
  name: string;
  creator: string;
  balance: number;
  members: string[];
}