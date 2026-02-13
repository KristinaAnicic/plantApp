import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { PlantExchangeService } from '../../services/plant-exchange.service';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PlantExchangeDto } from '../../models/plant-exchange.interface';
import { PlantExchangeFilterDto } from '../../models/filter.interface';
import { catchError, debounceTime, distinctUntilChanged, Observable, of, Subject, switchMap } from 'rxjs';
import { Pagination } from "../../components/pagination/pagination";
import { SearchComponent } from "../../components/search-component/search-component";
import { TimeAgoPipe } from "../../utils/time-ago.pipe";
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-plant-exchange-list',
  imports: [Pagination, RouterLink, SearchComponent, TimeAgoPipe, TranslateModule],
  templateUrl: './plant-exchange-list.html',
  styleUrl: './plant-exchange-list.css',
})
export class PlantExchangeList implements OnInit {
  private service = inject(PlantExchangeService);
  public authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  
  exchanges = signal<PlantExchangeDto[]>([]);
  total = signal(0);
  currentPage = signal(1);
  isAddModalOpen = signal(false);
  filter: PlantExchangeFilterDto = {};

  private searchSubject = new Subject<string>();
  private searchCitySubject = new Subject<string>();
  currentSearchTerm = signal<string>('');
  currentCitySearchTerm = signal<string>('');

  ngOnInit(): void {
    this.route.queryParams.pipe(
      switchMap(params => {
        const page = +params['page'] || 1;
        const searchTerm = params['search'] || '';
        const cityTerm = params['city'] || '';
        
        this.currentSearchTerm.set(searchTerm);
        this.currentCitySearchTerm.set(cityTerm);
        this.currentPage.set(page);

        this.filter.name = searchTerm;
        this.filter.city = cityTerm;

        return this.loadData(searchTerm, cityTerm, page);
      })
    ).subscribe({
      next: (response) => {
        this.exchanges.set(response.items);
        this.total.set(response.total);
      },
      error: (err) => {
        console.error("Error while fetching searched exchanges: ", err);
      }
    })

    this.setSearchSubject(this.searchSubject, 'search');
    this.setSearchSubject(this.searchCitySubject, 'city');
  }

  setSearchSubject(subject: Subject<string>, paramName: string) {
    subject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(value => {
        this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { [paramName]: value, page: 1 },
        queryParamsHandling: 'merge'
      });
    })
  }

  search(searchString: string) {
    this.searchSubject.next(searchString);
  }

  searchCity(searchString: string) {
    this.searchCitySubject.next(searchString);
  }


  totalPages = computed(() =>{
    return Math.ceil(this.total()/25)
  })
  
  loadData(searchString: string, cityString: string, page?: number): Observable<any>{
    this.filter.name = searchString;
    this.filter.city = cityString;

    const isFilterEmpty = searchString.trim() === '' && cityString.trim() === '';

    const request$ = isFilterEmpty 
      ? this.service.getAllActivePlantExchanges(page) 
      : this.service.getAllPlantExchangesFiltered(this.filter, page);

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

  ngOnDestroy(): void {
    this.searchSubject.complete();
    this.searchCitySubject.complete();
  }

  public getExchangeClass(typeName: string | undefined): string {
    switch (typeName?.toLowerCase()){
      case 'free': return 'bg-emerald-600 text-white border border-emerald-800 shadow shadow-md';
      case 'swap': return 'bg-violet-600 text-white border border-violet-700 shadow shadow-md';
      case 'sell': return 'bg-amber-500 text-black border border-amber-600 shadow shadow-md';
      default: return 'bg-gray-200';
    }
  }

  /*toggleAddModal(){
    this.isAddModalOpen.update(val => !val)
  }*/
}
