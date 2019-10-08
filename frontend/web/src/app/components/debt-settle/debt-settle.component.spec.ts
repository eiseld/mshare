import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DebtSettleComponent } from './debt-settle.component';

describe('DebtSettleComponent', () => {
  let component: DebtSettleComponent;
  let fixture: ComponentFixture<DebtSettleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DebtSettleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DebtSettleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
