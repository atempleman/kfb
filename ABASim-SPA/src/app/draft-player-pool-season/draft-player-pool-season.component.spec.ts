/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DraftPlayerPoolSeasonComponent } from './draft-player-pool-season.component';

describe('DraftPlayerPoolSeasonComponent', () => {
  let component: DraftPlayerPoolSeasonComponent;
  let fixture: ComponentFixture<DraftPlayerPoolSeasonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DraftPlayerPoolSeasonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DraftPlayerPoolSeasonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
