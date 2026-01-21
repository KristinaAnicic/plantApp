import { ResolveFn } from '@angular/router';
import { PlantGetDto, UpsertPlantDto } from '../models/plant.interface';
import { inject } from '@angular/core';
import { PlantService } from '../services/plant.service';
import { map, of } from 'rxjs';

export const plantEditResolver: ResolveFn<UpsertPlantDto | null> = (route, state) => {
  const id = route.paramMap.get('id');
  const plantService = inject(PlantService);
  
  if (id) {
    return plantService.getPlant(parseInt(id)).pipe(
      map((plant: PlantGetDto) => {
        return {
          ...plant,
          id: parseInt(id),
          timeToFullHeightId: plant.timeToFullHeight?.id ?? null,
          synonymParentPlantId: plant.parentPlant?.id ?? null,
          fragranceId: plant.fragrance?.id ?? null,
          hardinessLevelId: plant.hardinessLevel?.id ?? null,
          spreadTypeId: plant.spreadType?.id ?? null,
          heightTypeId: plant.heightType?.id ?? null,
          familyId: plant.family?.id ?? null,
          soilTypes: plant.soilTypes?.map(s => s.id) ?? [],
          images: plant.images?.map(im => im.url) ?? [],
          sunlights: plant.sunlights?.map(s => s.id) ?? [],
          aspects: plant.aspects?.map(s => s.id) ?? [],
          moistures: plant.moistures?.map(s => s.id) ?? [],
          phs: plant.phs?.map(s => s.id) ?? [],
          exposures: plant.exposures?.map(s => s.id) ?? [],
          habits: plant.habits?.map(s => s.id) ?? [],
          seasons: plant.seasons?.map(s => s.id) ?? [],

        } as UpsertPlantDto;
      })
    ); 
  }
  return of(null);
};

