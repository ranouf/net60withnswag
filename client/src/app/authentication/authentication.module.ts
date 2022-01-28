import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { EmailResetPasswordComponent } from './email-reset-password/email-reset-password.component';
import { EmailConfirmationComponent } from './email-confirmation/email-confirmation.component';

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ResetPasswordComponent,
    ConfirmEmailComponent,
    EmailResetPasswordComponent,
    EmailConfirmationComponent
  ],
  imports: [
    CommonModule
  ]
})
export class AuthenticationModule { }
