import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContentLayoutComponent } from './content-layout/content-layout.component';
import { OneSectionLayoutComponent } from './one-section-layout/one-section-layout.component';
import { MasterLayoutComponent } from './master-layout/master-layout.component';
import { FooterComponent } from './footer/footer.component';

@NgModule({
  declarations: [
    ContentLayoutComponent,
    OneSectionLayoutComponent,
    MasterLayoutComponent,
    FooterComponent
  ],
  exports: [
    ContentLayoutComponent,
    OneSectionLayoutComponent,
    MasterLayoutComponent
  ],
  imports: [
    CommonModule
  ]
})
export class TemplatesModule { }
