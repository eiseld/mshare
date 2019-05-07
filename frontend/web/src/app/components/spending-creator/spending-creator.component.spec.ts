import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpendingCreatorComponent } from './spending-creator.component';

describe('SpendingCreatorComponent', () => {
  let component: SpendingCreatorComponent;
  let fixture: ComponentFixture<SpendingCreatorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SpendingCreatorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpendingCreatorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
