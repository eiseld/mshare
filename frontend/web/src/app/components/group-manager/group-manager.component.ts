import {Component, OnInit, Input, EventEmitter, Output} from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment'
import { AuthService } from '../../services/auth.service';
import {NgbModal, ModalDismissReasons} from '@ng-bootstrap/ng-bootstrap';
import {Observable} from "rxjs";
import {debounceTime, map} from "rxjs/operators";
import {distinctUntilChanged} from "rxjs/internal/operators/distinctUntilChanged";
import { Spending } from '../spending-creator/spending-creator.component'

@Component({
  selector: 'app-group-manager',
  templateUrl: './group-manager.component.html',
  styleUrls: ['./group-manager.component.css']
})
export class GroupManagerComponent implements OnInit {

  constructor( private http: HttpClient, private authenticationService: AuthService,public modalService: NgbModal) {}

  @Input() newGroup: string = "";
  createGroupAttempt = false;
  createSpendingAttempt = false;
  spendingForGroupData : GroupData;
  groupInfos: GroupInfo[] = [];
  error : string = "";
  selectedGroup: GroupData = null;
  selectedGroupSpendings: Spending[]=null;
  closeResult: string;
  users: any[] = [];
  @Input() selectedUser: any;
  selectedGroupId: number;
  userModel : any;
  @Output() passEntry: EventEmitter<any> = new EventEmitter();

  search = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map(term => term.length <= 3 ? []
        : this.users.filter(v => v.displayName.toLowerCase().startsWith(term.toLocaleLowerCase())).
        splice(0, 10).map(user => user.displayName + '-' + user.email)
    ));

  ngOnInit() {
    this.getGroups();
  }

  onKeyDown(event: any){
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'filter' : event.target.value
      })
    };

    if(4 == event.target.value.length) {
      this.users = [];
      this.http.get<any[]>(`${environment.API_URL}/Group/searchinallusers/${event.target.value}`, httpOptions)
        .subscribe(list => {
          for (let user of list) {
          this.users.push(user);
          }
          },error => {this.error = "Sikertelen a felhasználók betöltése!"});
    }
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
    this.http.get<Spending[]>(`${environment.API_URL}/spending/${groupInfo.id}`, httpOptions)
    .subscribe(data => {
      this.selectedGroupSpendings = data;
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

  open(content, groupInfo: any) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
      this.closeResult = `Closed with: ${result}`;
    }, (reason) => {
      this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
    });
    this.selectedGroupId = groupInfo.id;
  }

  stringToUser() {
   this.userModel =this.users.find(e => e.displayName ===this.selectedUser.split('-')[0] && e.email === this.selectedUser.split('-')[1]);

    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    };
    this.http.post<any>(`${environment.API_URL}//Group/${this.selectedGroupId}/members/add/${this.userModel.id}`,
      {name: this.newGroup},
      httpOptions)
      .subscribe(
        data => {this.selectedUser = null;this.error = 'A felhasználó hozzáadása sikeresen megtörtént'},
        error => {this.selectedUser = {};this.error="Sikertelen a személy hozzáadása a kiválasztott csoporthoz!"}
      );
  }

  private getDismissReason(reason: any): string {
    if (reason === ModalDismissReasons.ESC) {
      return 'by pressing ESC';
    } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
      return 'by clicking on a backdrop';
    } else {
      return  `with: ${reason}`;
    }
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
