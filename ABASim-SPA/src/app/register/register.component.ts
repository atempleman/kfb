import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ContactForm } from '../_models/contactForm';
import { Team } from '../_models/team';
import { User } from '../_models/user';
import { UserRegister } from '../_models/userRegister';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { ContactService } from '../_services/contact.service';
import { LeagueService } from '../_services/league.service';
import { TeamService } from '../_services/team.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  availableTeams = false;
  availablePrivateTeams = false;
  selectableTeams: Team[] = [];
  selectableTeamsPrivate: Team[] = [];
  registerForm: FormGroup;
  user: UserRegister;

  usernameRequired = 0;
  passwordRequired = 0;
  confirmRequired = 0;
  teamnameRequired = 0;
  mascotRequired = 0;
  shortcodeRequired = 0;
  nameRequired = 0;
  emailRequired = 0;
  passwordLength = 0;
  passwordMatch = 0;

  contactForm: FormGroup;
  contactObject: ContactForm;

  currentLeagueSelection = 1;
  registerEnabled = 1;

  leagueCodeCheck = false;
  leagueCodeText = '';
  noTeamsPrivateDisplay = 0;

  constructor(private teamService: TeamService, private authService: AuthService, private alertify: AlertifyService, 
              private fb: FormBuilder, private router: Router, private contactService: ContactService,
              private leagueService: LeagueService) { }

  ngOnInit() {
    this.teamService.checkAvailableTeams().subscribe(result => {
      this.availableTeams = result;
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.getAvailableTeams();
      this.checkAvailablePrivateTeams();
      this.createRegisterForm();
    });
  }

  checkAvailablePrivateTeams() {
    this.leagueService.checkPrivateLeagueTeams().subscribe(result => {
      this.availablePrivateTeams = result;
      console.log(result);
      if (this.availablePrivateTeams) {
        this.registerEnabled = 1;
      }
    }, error => {
      this.alertify.error('Error checking private leagues');
    }, () => {
    });
  }

  getAvailableTeamsForPrivate() {
    this.teamService.getAvailableTeamsForPrivate(this.leagueCodeText).subscribe(result => {
      this.selectableTeamsPrivate = result;

      if (this.selectableTeamsPrivate.length > 0) {
        this.registerEnabled = 1;
      } else {
        this.registerEnabled = 0;
      }
    }, error => {
      this.alertify.error('Error getting available teams');
    });
  }

  getAvailableTeams() {
    this.teamService.getAvailableTeams().subscribe(result => {
      this.selectableTeams = result;
    }, error => {
      this.alertify.error('Error getting available teams');
    }, () => {
      this.createContactForm();
    });
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(100)]],
      confirmPassword: ['', Validators.required],
      email: ['', Validators.required],
      name: ['', Validators.required],
      teamSelection: ['', Validators.required],
      code: ['']
    }, { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : { mismatch: true };
  }

  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);

      this.user.code = this.leagueCodeText;

      console.log(this.user);

      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Registration successful');
      }, error => {
        this.alertify.error(error);
      }, () => {
        // Need to update the user passed in
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/dashboard']);
        });
      });
    } else {
      // Need to determine what failed the validation and then display the appropriate message
      if (this.registerForm.controls.username.value === '') {
        this.usernameRequired = 1;
      } else {
        this.usernameRequired = 0;
      }

      if (this.registerForm.controls.password.value === '') {
        this.passwordRequired = 1;
      } else {
        this.passwordRequired = 0;
      }

      if (this.registerForm.controls.confirmPassword.value === '') {
        this.confirmRequired = 1;
      } else {
        this.confirmRequired = 0;
      }

      if (this.registerForm.controls.email.value === '') {
        this.emailRequired = 1;
      } else {
        this.emailRequired = 0;
      }

      if (this.registerForm.controls.name.value === '') {
        this.nameRequired = 1;
      } else {
        this.nameRequired = 0;
      }

      if (this.registerForm.controls.teamname.value === '') {
        this.teamnameRequired = 1;
      } else {
        this.teamnameRequired = 0;
      }

      if (this.registerForm.controls.shortcode.value === '') {
        this.shortcodeRequired = 1;
      } else {
        this.shortcodeRequired = 0;
      }

      if (this.registerForm.controls.mascot.value === '') {
        this.mascotRequired = 1;
      } else {
        this.mascotRequired = 0;
      }

      if (this.registerForm.controls.password.value.legnth < 4) {
        this.passwordLength = 1;
      } else {
        this.passwordLength = 0;
      }

      // Mismath password
      if (this.registerForm.errors != null) {
        this.passwordMatch = 1;
      } else {
        this.passwordMatch = 0;
      }
    }
  }

  createContactForm() {
    this.contactForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required]],
      contact: ['', Validators.required],
    });
  }

  submitContactForm() {
    if (this.contactForm.valid) {
      this.contactObject = Object.assign({}, this.contactForm.value);
      this.contactService.saveContact(this.contactObject).subscribe(() => {
        this.alertify.success('Contact submitted successfully');
      }, error => {
        this.alertify.error(error);
      }, () => {
        // clear the form
        this.contactForm.reset();
      });
    }
  }

  radioToggle(selection: number) {
    // console.log(selection);

    if (this.currentLeagueSelection != selection) {
      // The we change the selection, otherwise we do nothing
      this.currentLeagueSelection = selection;
      this.leagueCodeText = '';
      if (this.currentLeagueSelection == 2) {
        this.registerEnabled = 0;
      } else {
        this.registerEnabled = 1;
      }
    }
  }

  checkPrivateLeagueCode() {
    this.leagueCodeText = this.registerForm.controls['code'].value
    this.leagueService.checkLeagueCode(this.leagueCodeText).subscribe(result => {
      this.leagueCodeCheck = result;
    }, error => {
      this.alertify.error('Error checking league code');
    }, () => {
      if (!this.leagueCodeCheck) {
        this.noTeamsPrivateDisplay = 1;
      }
      this.getAvailableTeamsForPrivate();
    });
  }

}
