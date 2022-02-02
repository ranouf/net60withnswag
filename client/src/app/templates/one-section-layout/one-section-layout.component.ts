import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-one-section-layout',
  templateUrl: './one-section-layout.component.html',
  styleUrls: ['./one-section-layout.component.scss']
})
export class OneSectionLayoutComponent implements OnInit {

  @Input() class: string = "app-login";

  constructor() { }

  ngOnInit(): void {
  }

}
