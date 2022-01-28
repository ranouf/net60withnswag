import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';

@Injectable()
export class AuthenticationInterceptor {

  constructor(private authenticationService:AuthenticationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.authenticationService.isAuthenticated) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${this.authenticationService.token}`,
        }
      });
    }
    return next.handle(request);
  }
}
