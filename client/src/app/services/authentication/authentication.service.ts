import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService as ApiAuthenticationService, LoginRequestDto, LoginResponseDto, UserDto } from '../api/api.services';
import { JwtHelperService } from '@auth0/angular-jwt';
import { catchError, map } from 'rxjs/operators';
import { Observable, BehaviorSubject, of, throwError } from 'rxjs';

@Injectable()
export class AuthenticationService {
  private jwtHelper: JwtHelperService = new JwtHelperService();

  private _authenticated: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.isAuthenticated());

  public get authenticationStatus(): Observable<boolean> {
    return this._authenticated.asObservable();
  }

  public currentUser: BehaviorSubject<UserDto> = new BehaviorSubject<UserDto>(this._currentUser);
  private get _currentUser(): UserDto {
    if (this.isAuthenticated()) {
      return JSON.parse(localStorage.getItem('currentUser'));
    }
    return null;
  }

  private _token: string = null;
  public get token(): string {
    if (this.isAuthenticated()) {
      if (!this._token) {
        this._token = localStorage.getItem('token');
      }
    }
    return this._token;
  }

  public get isAdministrator(): boolean {
    return this.currentUser.value.roleName == "Administrator";
  }

  public get isManager(): boolean {
    return this.currentUser.value.roleName == "Manager";
  }

  public get isUser(): boolean {
    return this.currentUser.value.roleName == "User";
  }

  constructor(
    private router: Router,
    private apiAuthenticationService: ApiAuthenticationService,
  ) { }

  public isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    return token != null && !this.jwtHelper.isTokenExpired(token);
  }

  public isAllowed(roles: string[]): boolean {
    var result = false;
    if (this.isAuthenticated()) {
      const currentuser = this.currentUser;
      result = roles == null || roles.indexOf(currentuser.value.roleName) !== -1;
    } else {
      this.router.navigate(['/authentication']);
    }
    return result;
  }

  public login(credentialsDto: LoginRequestDto): Observable<LoginResponseDto> {
    return this.apiAuthenticationService.loginUser(credentialsDto)
      .pipe(
        map(response => {
          localStorage.setItem('token', response.token);
          this.setCurrentUser(response.currentUser);
          this._authenticated.next(this.isAuthenticated());
          console.log("login succeeded");
          return response;
        }),
        catchError(error => {
          console.error(error)
          return throwError(error);
        })
      );
  }

  public setCurrentUser(user: UserDto) {
    this.currentUser.next(user);
    localStorage.setItem('currentUser', JSON.stringify(user));
  }

  public logout(): void {
    this.currentUser.next(null);
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    this._authenticated.next(false);
    this.router.navigate(['/authentication']);
    console.log("logout succeeded");
  }
}
