import {Component, ElementRef, ViewChild} from '@angular/core';
import {UserApi} from "../../services/users.service";
import {UserLogin} from "../../entitys/user";
import {Router} from "@angular/router";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  @ViewChild('error') error: ElementRef;
  public disableLogin: boolean = false;

  constructor(private users: UserApi, private router: Router) { }

  public async login(form: HTMLFormElement, username: string, password: string) {
    if (this.disableLogin) return;
    if (!form.reportValidity()) return;

    const login: UserLogin = {usernameOrEmail: username, password: password};
    const response = await this.users.login(login);

    if (!response.success) {
      this.showError(response.errorMessage)
      return;
    }

    await this.router.navigate([""]);
  }

  public showError(error: string) {
    this.disableLogin = true;
    this.error.nativeElement.innerText = error;

    this.error.nativeElement.style.opacity = "1";
    this.error.nativeElement.style.bottom = "150px";

    setTimeout(() => {
      this.error.nativeElement.style.opacity = "0";
      this.error.nativeElement.style.bottom = "120px";
      this.disableLogin = false;
    }, 5000)
  }

}
