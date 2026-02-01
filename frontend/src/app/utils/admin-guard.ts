import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const service = inject(AuthService);
  const router = inject(Router);

  if (!service.isAdmin()){
    router.navigate(['/']);
    return false;
  }

  return true;
};
