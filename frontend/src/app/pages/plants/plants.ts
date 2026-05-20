import { Component, computed, inject, OnInit, signal, ViewEncapsulation } from '@angular/core';
import { PlantDto } from '../../models/plant.interface';
import { PlantService } from '../../services/plant.service';
import { SearchComponent } from "../../components/search-component/search-component";
import { PlantFilterDto } from '../../models/filter.interface';
import { catchError, debounceTime, distinctUntilChanged, Observable, of, Subject, switchMap } from 'rxjs';
import { ActivatedRoute, Router, RouterLink } from "@angular/router";
import { Pagination } from "../../components/pagination/pagination";
import { AuthService } from '../../services/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { IdentifyPlantModal } from "../../components/identify-plant-modal/identify-plant-modal";

@Component({
  selector: 'app-plants',
  imports: [SearchComponent, RouterLink, Pagination, TranslateModule, IdentifyPlantModal],
  encapsulation: ViewEncapsulation.None,
  templateUrl: './plants.html',
  styleUrl: './plants.css',
})
export class Plants implements OnInit {
  private service = inject(PlantService);
  public authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  
  plants = signal<PlantDto[]>([]);
  total = signal(0);
  currentPage = signal(1);
  filter: PlantFilterDto = {};

  private searchSubject = new Subject<string>();
  currentSearchTerm = signal<string>('');

  isIdentifyPlantModalOpen = signal(false);

  ngOnInit(): void {
    this.route.queryParams.pipe(
      switchMap(params => {
        const page = +params['page'] || 1;
        const searchTerm = params['search'] || '';
        
        this.currentSearchTerm.set(searchTerm);
        this.filter.name = searchTerm;
        this.currentPage.set(page);

        return this.loadData(searchTerm, page);
      })
    ).subscribe({
      next: (response) => {
        this.plants.set(response.items);
        this.total.set(response.total);
      },
      error: (err) => {
        console.error("Error while fetching searched plants: ", err);
      }
    })

    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(searchTerm => {
        this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { search: searchTerm, page: 1 },
        queryParamsHandling: 'merge'
      });
    })
  }

  search(searchString: string) {
    this.searchSubject.next(searchString);
  }

  totalPages = computed(() =>{
    return Math.ceil(this.total()/25)
  })
  
  loadData(searchString: string, page?: number): Observable<any>{
    this.filter.name = searchString;

    const request$ =searchString.trim() === '' 
      ? this.service.getAllPlants(page) 
      : this.service.getAllPlantsFiltered(this.filter, page);

    return request$.pipe(
      catchError(err => {
        console.log("Error:", err)
        return of({ items: [], total: 0});
      })
    )
  }

  nextPage(newPage: number){
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: newPage },
      queryParamsHandling: 'merge'
    })
  }

  addNewPlantClick(){
    this.router.navigate(['/plant-form']/*, { skipLocationChange: true }*/);
  }

  toggleIdentifyPlantModal(){
    this.isIdentifyPlantModalOpen.update(val => !val);

    if (this.isIdentifyPlantModalOpen()) {
      document.body.classList.add('overflow-hidden');
    } else {
      document.body.classList.remove('overflow-hidden');
    }
  }

  ngOnDestroy(): void {
    document.body.classList.remove('overflow-hidden');
    this.searchSubject.complete();
  }
}

