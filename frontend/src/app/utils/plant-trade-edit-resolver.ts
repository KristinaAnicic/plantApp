import { inject } from '@angular/core';
import { ResolveFn, Router } from '@angular/router';
import { PlantExchangeService } from '../services/plant-exchange.service';
import { catchError, EMPTY, map } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { UpsertPlantExchangeDto } from '../models/plant-exchange.interface';

export const plantTradeEditResolver: ResolveFn<UpsertPlantExchangeDto | null> = (route, state) => {
  const id = route.paramMap.get('id');
  const service = inject(PlantExchangeService);
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!id) {
    router.navigate(['/trade']);
    return EMPTY;
  }

  return service.getPlantExchange(+id).pipe(
    map((exchange) => {
      const currentUserId = authService.currentUser()?.id;

      if (exchange.user.id !== currentUserId && !authService.isAdmin()) {
        router.navigate(['/trade']);
        return null;
      }

      return {
        ...exchange,
        plantedId: exchange.planted?.id,
        exchangeTypeId: exchange.exchangeType?.id,
        countryId: exchange.country?.id,
        mainImage: exchange.image,
        images: exchange.images?.map(im => im.url) ?? []
      } as UpsertPlantExchangeDto
    }),
    catchError(() => {
      router.navigate(['/trade']);
      return EMPTY;
    })
  )
};
