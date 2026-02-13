import { Component, inject, signal } from '@angular/core';
import { LoginCredentials } from '../../models/auth/login.interface';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  showWarning = signal(false);
  service = inject(AuthService);
  router = inject(Router);

  loginForm = new FormGroup({
    usernameOrEmail: new FormControl('', { validators: [Validators.required], nonNullable: true }),
    password: new FormControl('', { validators: [Validators.required], nonNullable: true })
  })
  
  closeWarningClick(){
    this.showWarning.set(false);
  }

  login(){
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const data: LoginCredentials = this.loginForm.getRawValue();
    const cleanData: LoginCredentials = {
      usernameOrEmail: data.usernameOrEmail.trim().toLowerCase(),
      password: data.password
    };

    this.service.login(cleanData).subscribe({
      next:() => {
        this.router.navigate(['']);
      },
      error:(err:any) => {
        this.showWarning.set(true);
        this.loginForm.get('password')?.reset();
        console.log("Error on login", err);
      }
    })
  }
}
