import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ForgottenpwComponent } from './forgottenpw.component';

describe('ForgottenpwComponent', () => {
  let component: ForgottenpwComponent;
  let fixture: ComponentFixture<ForgottenpwComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ForgottenpwComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ForgottenpwComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
