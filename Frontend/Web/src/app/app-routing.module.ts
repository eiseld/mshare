import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomepageComponent } from './components/homepage/homepage.component';
import { AboutComponent } from './components/about/about.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ConfirmComponent } from './components/confirm/confirm.component';
import { AuthGuard } from './guards/auth.guard';
import {ForgottenpwComponent} from "./components/forgottenpw/forgottenpw.component";
import {ResetpasswordComponent} from "./components/resetpassword/resetpassword.component";

const routes: Routes = [
  { path: '', component: HomepageComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'about', component: AboutComponent },
  { path: 'fpwd', component: ForgottenpwComponent},
  { path: 'reset', component: ResetpasswordComponent},
  { path: 'register', component: RegisterComponent },
  { path: 'account/confirm/:token', component: ConfirmComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
