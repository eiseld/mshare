import { Component, OnInit, Input, OnChanges, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { MemberData, GroupData } from '../group-manager/group-manager.component'
import { parseIntAutoRadix } from '@angular/common/src/i18n/format_number';
import { environment } from '../../../environments/environment'
import { AuthService } from '../../services/auth.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

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
  newDebtor: MemberData;
  @Input()
  spendingForGroupData: GroupData;
  defaultBalance:number;
  @Input()
  createSpendingAttempt:boolean;
  @Input()
  addDebtorAttempt:boolean=false;
  changeDetected:boolean;
  searchList:string[]=[];
  error:string="";

  @Output() createSpendingAttemptStop = new EventEmitter();

  @Output() updateSelectedGroupEvent = new EventEmitter();

  stopCreateSpendingAttempt(){
    this.addDebtorAttempt=false;
    this.error="";
    this.createSpendingAttempt=false;
    this.createSpendingAttemptStop.emit();
  }

  updateSelectedGroup(){
    this.stopCreateSpendingAttempt();
    this.updateSelectedGroupEvent.next();
  }


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
      }
    }
  }

  startAddDebtor(){
    this.addDebtorAttempt=true;
    this.newDebtor=new MemberData();
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
      var newDebtor=new MemberData();
      newDebtor.id=member.id;
      newDebtor.name=member.name;
      if(!this.spending.debtors.some( ({id}) => id == newDebtor.id)){
        this.spending.debtors=[...this.spending.debtors, newDebtor];
      }
    }
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
        delete this.spending;
        this.spending=null;
        this.stopCreateSpendingAttempt();
    }
  }
}

export class Spending {
  name: string;
  moneyOwed: number;
  debtors: MemberData[];
}

export class Debtor{
  DebtorId: number;
  Debt:number;

  constructor(memberData: MemberData) {
    this.DebtorId = memberData.id;
    this.Debt = memberData.balance;
  }
}
