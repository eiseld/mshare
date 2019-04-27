import { Component, OnInit, Input, OnChanges, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { MemberData, GroupData } from '../group-manager/group-manager.component'
import { parseIntAutoRadix } from '@angular/common/src/i18n/format_number';
import { environment } from '../../../environments/environment'
import { AuthService } from '../../services/auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
<<<<<<< HEAD
import { empty } from 'rxjs';
import { NgOnChangesFeature } from '@angular/core/src/render3';
=======
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component

@Component({
  selector: 'app-spending-creator',
  templateUrl: './spending-creator.component.html',
  styleUrls: ['./spending-creator.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SpendingCreatorComponent implements OnChanges {
  @Input()
  spending: Spending;
  @Input()
<<<<<<< HEAD
  newDebtor: DebtorData;
  @Input()
  spendingForGroupData: GroupData;
  defaultBalanceSum:number;
=======
  newDebtor: MemberData;
  @Input()
  spendingForGroupData: GroupData;
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
  defaultBalance:number;
  @Input()
  createSpendingAttempt:boolean;
  @Input()
  addDebtorAttempt:boolean=false;
  changeDetected:boolean;
  searchList:string[]=[];
  error:string="";
<<<<<<< HEAD
  errorAddDebtor:string="";
=======
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component

  @Output() createSpendingAttemptStop = new EventEmitter();

  @Output() updateSelectedGroupEvent = new EventEmitter();

  stopCreateSpendingAttempt(){
    this.addDebtorAttempt=false;
    this.error="";
<<<<<<< HEAD
    this.errorAddDebtor="";
    delete this.spending.debtors;
    this.spending.name="";
    this.spending.moneyOwed=undefined;
    this.spending.debtors=[];
=======
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
    this.createSpendingAttempt=false;
    this.createSpendingAttemptStop.emit();
  }

  updateSelectedGroup(){
<<<<<<< HEAD
    this.addDebtorAttempt=false;
    this.error="";
    this.errorAddDebtor="";
    this.createSpendingAttempt=false;
    this.createSpendingAttemptStop.emit();
    this.updateSelectedGroupEvent.next();
  }

=======
    this.stopCreateSpendingAttempt();
    this.updateSelectedGroupEvent.next();
  }


>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
  constructor( private http: HttpClient ) { 
  }

  ngOnInit() {
    if(this.createSpendingAttempt){
      this.startCreateSpending(this.spendingForGroupData);
    }
  }

  ngOnChanges(){
    if(this.createSpendingAttempt){
      this.startCreateSpending(this.spendingForGroupData);
    }
    this.calcDefaultDebt();
  }

  onSearchChange(searchValue : string ) {  
<<<<<<< HEAD
    this.errorAddDebtor="";
    this.searchList=["Mindenki",...this.spendingForGroupData.members.map((member)=> member.name).filter((name)=>name.slice(0,searchValue.length)==searchValue)];
  }
  numberInputChanged(inputFieldValue:string,member:MemberData){
    if(inputFieldValue!=undefined&&inputFieldValue.length>0){
      member.balance=Number(inputFieldValue);
    }
    else{
      member.balance=undefined;
    }
    this.ngOnChanges();
  }
  calcDefaultDebt(){
    if(this.spending.moneyOwed==undefined){
      this.defaultBalanceSum=0;
      this.defaultBalance=0;
    }
    else{
      var exactDebtors=this.spending.debtors.filter((debtor)=>(debtor.balance!=undefined));
      var defaultBalanceSum=this.spending.moneyOwed;
      var exactBalanceSum:number;
      if(exactDebtors.length!=0){
          exactBalanceSum=exactDebtors.map((debtor)=>{return debtor.balance}).reduce((partial_sum, a) => Number(partial_sum) + Number(a));
          defaultBalanceSum-=exactBalanceSum;
      }
      var defaultDebtors=this.spending.debtors.filter((debtor)=>(debtor.balance==undefined));
      var memberCount=defaultDebtors.length;
      var defaultBalance=Math.trunc(defaultBalanceSum/memberCount);
      for(let debtor of defaultDebtors){
        if(defaultBalance*memberCount!=defaultBalanceSum){
          debtor.defaultBalance=defaultBalance+1;
        }
        else{
          debtor.defaultBalance=defaultBalance;
        }
        memberCount--;
        defaultBalanceSum-=debtor.defaultBalance;
=======
    this.searchList=["Mindenki",...this.spendingForGroupData.members.map((member)=> member.name).filter((name)=>name.slice(0,searchValue.length)==searchValue)];
  }

  calcDefaultDebt(){
    if(this.spending.moneyOwed==undefined){
      this.defaultBalance=0;
    }
    else{
      this.defaultBalance=this.spending.moneyOwed;
      var memberCount=this.spending.debtors.length;
      for(let member of this.spending.debtors){
        if(member.balance!=undefined){
          this.defaultBalance-=member.balance;
          memberCount--;
        }
      }
      if(memberCount>0){
        this.defaultBalance/=memberCount;
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
      }
    }
  }

  startAddDebtor(){
    this.addDebtorAttempt=true;
<<<<<<< HEAD
    this.newDebtor=new DebtorData();
=======
    this.newDebtor=new MemberData();
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
    this.onSearchChange('');
  }

  addDebtor(){
    if(this.newDebtor.name=="Mindenki"){
      this.addAllAsDebtor();
    }
    else{
      var foundMember=this.spendingForGroupData.members.filter(({name}) => name == this.newDebtor.name);
        if(foundMember.length!=0){
          this.newDebtor.id=foundMember[0].id;
          this.newDebtor.name=foundMember[0].name;
          if(!this.spending.debtors.some( ({id}) => id == this.newDebtor.id)){
            this.spending.debtors=[...this.spending.debtors, this.newDebtor];
          }
<<<<<<< HEAD
          else{
            this.errorAddDebtor="A hozzáadni kívánt személy már szerepel a listán!"
          }
=======
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
        }
    }
    this.addDebtorAttempt=false;
  }

  stopAddDebtor(){
    this.addDebtorAttempt=false;
    delete this.newDebtor;
  }

  addAllAsDebtor(){
    for(let member of this.spendingForGroupData.members){
<<<<<<< HEAD
      var newDebtor=new DebtorData();
=======
      var newDebtor=new MemberData();
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
      newDebtor.id=member.id;
      newDebtor.name=member.name;
      if(!this.spending.debtors.some( ({id}) => id == newDebtor.id)){
        this.spending.debtors=[...this.spending.debtors, newDebtor];
      }
    }
<<<<<<< HEAD
    this.calcDefaultDebt();
=======
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
  }

  removeDebtor(member : MemberData){
    this.spending.debtors.splice(this.spending.debtors.findIndex((debtor)=> debtor==member),1);
  }

  startCreateSpending(spendingForGroupData:GroupData){
    if(this.spending==null){
      this.spending=new Spending();
      this.spending.debtors=[];
    }
    this.spendingForGroupData=spendingForGroupData;
  }

  stopCreateSpending(){
    this.spendingForGroupData=null;
    delete this.spending;
    this.stopCreateSpendingAttempt();
  }

  createSpending(){
    if(this.spending.name==undefined||this.spending.name.length==0||this.spending.name.length>32){
      this.error="Adjon egy legfeljebb 32 karakter hosszú nevet a költésnek!";
    }
<<<<<<< HEAD
    else if(this.spending.debtors.map((debtor) => debtor.balance).filter((balance)=>balance==undefined).length==0
      &&this.spending.debtors.map((debtor)=>{return debtor.balance}).reduce(
        (partial_sum, a) => Number(partial_sum) + Number(a)) != this.spending.moneyOwed)
    {
=======
    else if(this.spending.debtors.map((debtor) => debtor.balance).filter((balance)=>balance!=undefined).length!=0
      &&this.spending.debtors.map((debtor)=>{
      if(debtor.balance!=undefined){
        return debtor.balance;
      } else{
        return this.defaultBalance;
      }
    }).reduce((partial_sum, a) => Number(partial_sum) + Number(a)) != this.spending.moneyOwed)
    {
      var sum=this.spending.debtors.map((debtor) => debtor.balance).filter((balance)=>balance!=undefined).length!=0
      &&this.spending.debtors.map((debtor)=>{
      if(debtor.balance!=undefined){
        return debtor.balance;
      } else{
        return this.defaultBalance;
      }
    }).reduce((partial_sum, a) => Number(partial_sum) + Number(a));
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
      this.error="Az egyéni költések összege nem egyezik meg a költés összegével!";
    }
    else if(this.defaultBalance<0||this.spending.debtors.some((debtor)=>debtor.balance<=0)){
      this.error="Csak pozitív értékű költés engedélyezett!";
    }
    else{
        const httpOptions = {
          headers: new HttpHeaders({
            'Content-Type': 'application/json'
          })
        };
<<<<<<< HEAD
        if(this.spending.debtors.length==0){
          this.addAllAsDebtor();
        }
=======
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
        var debtors:Debtor[]=this.spending.debtors.map(item => new Debtor(item));
        this.http.post<any>(`${environment.API_URL}/spending/create`, 
        {
          'Name': this.spending.name,
          'GroupId': this.spendingForGroupData.id,
          'MoneySpent': this.spending.moneyOwed,
          'Debtors': debtors
        },
      httpOptions)
        .subscribe(
          data => {this.updateSelectedGroup();},
          error => {this.error="Sikertelen a költés létrehozása!";this.stopCreateSpendingAttempt();}
        );
<<<<<<< HEAD
=======
        delete this.spending;
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
        this.spending=null;
        this.stopCreateSpendingAttempt();
    }
  }
}

export class Spending {
  name: string;
  moneyOwed: number;
<<<<<<< HEAD
  debtors: DebtorData[];
=======
  debtors: MemberData[];
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
}

export class Debtor{
  DebtorId: number;
  Debt:number;

<<<<<<< HEAD
  constructor(debtorData: DebtorData) {
    this.DebtorId = debtorData.id;
    if(debtorData.balance!=undefined){
      this.Debt = debtorData.balance;
    }
    else{
      this.Debt = debtorData.defaultBalance;
    }
  }
}


export class DebtorData extends MemberData {
  defaultBalance:number;
}
=======
  constructor(memberData: MemberData) {
    this.DebtorId = memberData.id;
    this.Debt = memberData.balance;
  }
}
>>>>>>> f48cf20... Angular: added spending-creator component, added spending list to group-detail component
