import { Component, OnInit, Input } from '@angular/core';
import { GroupedObservable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment'
import { AuthService } from '../../services/auth.service';
import { Spending, DebtorData } from '../spending-creator/spending-creator.component'

@Component({
  selector: 'app-group-manager',
  templateUrl: './group-manager.component.html',
  styleUrls: ['./group-manager.component.css']
})
export class GroupManagerComponent implements OnInit {

  constructor( private http: HttpClient, private authenticationService: AuthService) {}

  @Input() newGroup: string = "";
  createGroupAttempt = false;
  createSpendingAttempt = false;
  spendingForGroupData : GroupData;
  groupInfos: GroupInfo[] = [];
  error : string = "";
  selectedGroup: GroupData = null;
  selectedGroupSpendings: Spending[]=null;

  ngOnInit() {
    this.getGroups();
  }

  getGroups() {
    let currentUser = this.authenticationService.currentUserValue;
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    if(this.groupInfos != null){
      delete this.groupInfos;
      this.groupInfos = [];
    }

    this.http.get<GroupInfo[]>(`${environment.API_URL}/profile/groups`, httpOptions)
    .subscribe(list => {
      for (let groupInfo of list) {
        this.groupInfos.push(groupInfo);
      }
    },error => {this.error = "Sikertelen a csoportok betöltése!"});
  }

  startCreateGroup(){
    this.createGroupAttempt = true;
    this.newGroup = "";
    this.error = "";
  }
  
  stopCreateGroup(){
    this.createGroupAttempt = false;
  }

  createGroup(){
    let currentUser = this.authenticationService.currentUserValue;
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    this.http.post<any>(`${environment.API_URL}/group/create`, 
	{name: this.newGroup},
	httpOptions)
		.subscribe(
			data => {this.getGroups()},
			error => {this.error="Sikertelen a csoport létrehozása!"}
		);
    this.createGroupAttempt = false;
  }

  updateSelectedGroup(){
    if(this.selectedGroup!=null){
      this.selectGroup(new GroupInfo(this.selectedGroup));
    }
  }

  selectGroup(groupInfo : GroupInfo){
	let currentUser = this.authenticationService.currentUserValue;
	const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
	this.http.get<GroupData>(`${environment.API_URL}/group/${groupInfo.id}/data`, httpOptions)
	.subscribe(data => {
		this.selectedGroup = data
    },error => {this.error = "Sikertelen a csoport betöltése!"});
    this.http.get<any[]>(`${environment.API_URL}/spending/${groupInfo.id}`, httpOptions)
    .subscribe(data => { 
      this.selectedGroupSpendings = <Spending[]>data.map(x=><Spending>{
        name: x.name,
        moneyOwed: x.moneyOwed,
        debtors: x.debtors.map(
          (debtor)=><DebtorData>{
            id: debtor.id,
            name: debtor.name,
            balance: debtor.debt,
            defaultBalance: debtor.defaultBalance,
          }),
      });
      },error => {this.error = "Sikertelen a költések betöltése!"});
  }

  startCreateSpending(spendingForGroupInfo:GroupInfo){
      let currentUser = this.authenticationService.currentUserValue;
      const httpOptions = {
        headers: new HttpHeaders({
          'Content-Type': 'application/json'
        })
      };
    this.http.get<GroupData>(`${environment.API_URL}/group/${spendingForGroupInfo.id}/data`, httpOptions)
    .subscribe(data => {
      this.spendingForGroupData = data
      this.createSpendingAttempt = true;
      },error => {this.error = "Sikertelen a csoport betöltése!"});
  }

  stopCreateSpendingAttempt(){
    this.createSpendingAttempt=false;
  }
}

export class GroupInfo {
  id: number;
  name: string;
  creator: string;
  memberCount: number;
  myCurrentBalance: number;
  constructor(groupData: GroupData) {
    this.id = groupData.id;
    this.name = groupData.name;
    this.creator = groupData.creator.name;
    this.memberCount = groupData.memberCount;
    this.myCurrentBalance = groupData.myCurrentBalance;
  }
}

export class GroupData {
  id: number;
  name: string;
  creator: MemberData;
  members: MemberData[];
  memberCount: number;
  myCurrentBalance: number;
}

export class MemberData {
  id: number;
  name: string;
  balance: number;
}
