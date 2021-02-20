import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { User } from '../_models/user';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { TeamService } from '../_services/team.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginModel: any = {};
  user: User;
  loginForm: FormGroup;
  
  constructor(private authService: AuthService, private alertify: AlertifyService, private fb: FormBuilder, 
              private router: Router) { }

  ngOnInit() {
    this.createLoginForm();
  }

  createLoginForm() {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  login() {
    this.loginModel = Object.assign({}, this.loginForm.value);
    this.authService.login(this.loginModel).subscribe(next => {
      this.alertify.success('Logged in successfully');
      localStorage.setItem('currentUserId', this.authService.decodedToken.nameid);
      localStorage.setItem('isAdmin', this.authService.decodedToken.primarygroupsid);
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/dashboard']);
    });
  }
}
