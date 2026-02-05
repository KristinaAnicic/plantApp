import { Component, computed, inject, signal } from '@angular/core';
import { AbstractControl, Form, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AddUserDto } from '../../models/user.interface';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private service = inject(AuthService);
  router = inject(Router);

  maxDate = new Date().toISOString().split('T')[0];
  showWarning = signal(false);
  errorMessage = signal('');
  formSubmitted = signal(false);

  registerForm = new FormGroup({
    email: new FormControl('', { 
      validators: [Validators.required, Validators.email],  
      nonNullable: true 
    }),
    username: new FormControl('', { validators: Validators.required, nonNullable: true }),
    password: new FormControl('', { 
      validators: [Validators.required, Validators.minLength(8)], 
      nonNullable: true 
    }),
    passwordConfirm: new FormControl('', { 
      validators: this.passwordMatchValidation(), 
      nonNullable: true 
    }),
    displayName: new FormControl('', { nonNullable: true }),
    contact: new FormControl('', { nonNullable: true }),
    gender: new FormControl('F', { nonNullable: true }),
    dateOfBirth: new FormControl(new Date().toISOString().split('T')[0], { 
      validators: [Validators.required, this.ageValidation() ], 
      nonNullable: true }),
  })

  passwordMatchValidation(){
    return (control: AbstractControl) => {
      if (!control.value) return null;

      if (control.value != this.registerForm.getRawValue().password)
        return { mismatch: true }

      return null;
    }
  }

  ageValidation(){
    return (control: AbstractControl) => {
      if (!control.value) return null;

      const birthDate = new Date(control.value);
      const minBirthDate = new Date();
      minBirthDate.setFullYear(minBirthDate.getFullYear() - 13);
      minBirthDate.setHours(0, 0, 0, 0); 
      birthDate.setHours(0, 0, 0, 0);

      if (birthDate > minBirthDate)
        return { tooYoung: true }

      return null;
    };
  }

  closeWarningClick(){
    this.showWarning.set(false);
    this.errorMessage.set('');
  }

  register(){
    this.formSubmitted.set(true);
    if (this.registerForm.invalid){
      this.registerForm.markAllAsTouched();
      return;
    }

    const data: AddUserDto = this.registerForm.getRawValue();
    this.service.register(data).subscribe({
      next:() => {
        this.router.navigate(['/login']);
      },
      error: (err: any) => {
        if (err.status === 400){
          console.log("Registration error", err);
          this.showWarning.set(true);

          const message = err.error?.error || "Registration failed";
          this.errorMessage.set(message);
        }
        else {
          this.showWarning.set(true);
          this.errorMessage.set("Registration failed");
        }
      }
    })

  }

  readonly hasRequiredErrors = computed<boolean>(() => 
    Object.values(this.registerForm.controls).some(control => 
      control.invalid && control.errors?.['required'] && (control.touched || this.formSubmitted())
    )
  )
}
