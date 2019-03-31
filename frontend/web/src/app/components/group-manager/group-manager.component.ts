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

    this.http.get<Profile>(`${environment.API_URL}/profile`, httpOptions)
    .subscribe(data => {
      let profile = data as Profile;
      for (let group of profile.groups) {
        //this.http.get<Group>(`${environment.API_URL}/groups/${groupId}`, httpOptions)
        //.subscribe(data => {this.groups.push(data as Group)});
        this.groups.push(group);
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
    this.http.post<any>(`${environment.API_URL}/group/create`, {
      name: this.newGroup
    }, httpOptions)
    .subscribe(data => {
      this.getGroups()
      /*this.http.post<any>(`${environment.API_URL}/users/updategroups/`, {}, httpOptions)
  .subscribe( data => {this.getGroups()})}, error => {this.error="Sikertelen a csoport létrehozása!"*/
                          },
                error => {this.error="Sikertelen a csoport létrehozása!"}
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
  creatorUser: Profile;
  members: Profile[];
  memberCount: number;
  balance: number;
}

export class Profile {
  displayName: string;
  groups: Group[];
}
