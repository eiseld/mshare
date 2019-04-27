import { Component, OnInit, Input } from '@angular/core';
import { GroupedObservable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment'
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-group-manager',
  templateUrl: './group-manager.component.html',
  styleUrls: ['./group-manager.component.css']
})
export class GroupManagerComponent implements OnInit {

  constructor( private http: HttpClient, private authenticationService: AuthService) {}

  @Input() newGroup: string = "";
  createGroupAttempt = false;
  
  groupInfos: GroupInfo[] = [];
  error : string = "";

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
  
  selectedGroup: GroupData = null;
  
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
  }
}

export class GroupInfo {
  id: number;
  name: string;
  creator: string;
  memberCount: number;
  myCurrentBalance: number;
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
