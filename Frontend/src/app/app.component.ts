import {Component, OnInit} from '@angular/core';
import {BackendService} from "./services/backend.service";
import {Router} from "@angular/router";
import {UserApi} from "./services/users.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  public loaded: boolean = false;

  constructor(private backend: BackendService, private router: Router, private users: UserApi) {}

  ngOnInit(): void {
    setTimeout(async () => {
      if (this.router.url == "/login" || this.router.url == "/register") {
        this.loaded = true;
        return;
      }

      if (await this.backend.requestToken()) this.loaded = true;
      else await this.router.navigate(["login"]);
    }, 0);
  }
}
