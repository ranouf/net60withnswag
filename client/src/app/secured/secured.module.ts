import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileComponent } from './account/profile/profile.component';
import { PasswordComponent } from './account/password/password.component';
import { AccountComponent } from './account/account.component';
import { SettingsComponent } from './settings/settings.component';
import { UsersComponent } from './settings/users/users.component';
import { UserComponent } from './settings/users/user/user.component';
import { NotificationsComponent } from './notifications/notifications.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ItemsComponent } from './items/items.component';



@NgModule({
  declarations: [
    ProfileComponent,
    PasswordComponent,
    AccountComponent,
    SettingsComponent,
    UsersComponent,
    UserComponent,
    NotificationsComponent,
    DashboardComponent,
    ItemsComponent
  ],
  imports: [
    CommonModule
  ]
})
export class SecuredModule { }
