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

  @Input() newGroup: string ="";
  createGroupAttempt=false;
  
  groupIds : string[]=[];
  groups: Group[]=[];
  error : string="";

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
    if( this.groupIds!=null){
      delete this.groupIds;
      this.groupIds=[];
    }
    if(this.groups!=null){
      delete this.groups;
      this.groups=[];
    }

    this.http.get<string[]>(`${environment.API_URL}/users/listgroups`, httpOptions)
    .subscribe(data => {this.groupIds= data as string[];
      for (let groupId of this.groupIds) {
        this.http.get<Group>(`${environment.API_URL}/groups/${groupId}`, httpOptions)
        .subscribe(data => {this.groups.push(data as Group)});
      }
    },error => {this.error = "Sikertelen a csoportok betöltése!"});
  }

  startCreateGroup(){
    this.createGroupAttempt=true;
    this.newGroup="";
    this.error="";
  }
  selectedGroup:Group=null;
  stopCreateGroup(){
    this.createGroupAttempt=false;
  }

  createGroup(){
    let currentUser = this.authenticationService.currentUserValue;
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    this.http.post<any>(`${environment.API_URL}/groups/newgroup/${this.newGroup}`, {}, httpOptions)
    .subscribe(data => {this.http.post<any>(`${environment.API_URL}/users/updategroups/`, {}, httpOptions)
                              .subscribe( data => {this.getGroups()})}, error => {this.error="Sikertelen a csoport létrehozása!"}
    );
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
  members: string[];
  memberCount: number;
  balance: number;
}