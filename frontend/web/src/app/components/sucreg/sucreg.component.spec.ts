import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SucregComponent } from './sucreg.component';

describe('SucregComponent', () => {
  let component: SucregComponent;
  let fixture: ComponentFixture<SucregComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SucregComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SucregComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
