import {Component, ElementRef, ViewChild} from '@angular/core';
import {UserApi} from "../../services/users.service";
import {Router} from "@angular/router";
import {UserEditor} from "../../entitys/user";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  @ViewChild('error') error: ElementRef;
  public disableRegister: boolean = false;

  constructor(private users: UserApi, private router: Router) { }

  public async register(form: HTMLFormElement, register: UserEditor, pwRepeat: string) {
    if (!form.reportValidity()) return;

    if (register.password !== pwRepeat) {
      this.showError("Passwörter stimmen nicht überein!");
      return;
    }

    if (!register.email.includes("@") || !register.email.includes(".")) {
      this.showError("Bitte gebe eine gültige E-Mail Adresse ein!");
      return;
    }

    if (register.username.includes("@")) {
      this.showError("Benutzername enthätlt ungültige Zeichen!");
      return;
    }

    if (register.password.length < 8) {
      this.showError("Dein Passwort muss mindestens 8 Zeichen lang sein!");
      return;
    }

    const response = await this.users.register(register);

    if (!response.success) {
      this.showError(response.errorMessage);
      return;
    }

    await this.router.navigate([""]);
  }

  public showError(error: string) {
    this.disableRegister = true;
    this.error.nativeElement.innerText = error;

    this.error.nativeElement.style.opacity = "1";
    this.error.nativeElement.style.bottom = "150px";

    setTimeout(() => {
      this.error.nativeElement.style.opacity = "0";
      this.error.nativeElement.style.bottom = "120px";
      this.disableRegister = false;
    }, 5000)
  }

}
