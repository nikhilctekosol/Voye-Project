import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TripcompletionComponent } from './tripcompletion.component';

describe('TripcompletionComponent', () => {
  let component: TripcompletionComponent;
  let fixture: ComponentFixture<TripcompletionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TripcompletionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TripcompletionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
