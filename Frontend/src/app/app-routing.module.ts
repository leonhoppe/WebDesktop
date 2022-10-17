import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from "./sites/login/login.component";
import {RegisterComponent} from "./sites/register/register.component";
import {DesktopComponent} from "./sites/desktop/desktop.component";

const routes: Routes = [
  {path: '', component: DesktopComponent},
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: '*', redirectTo: ''}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
