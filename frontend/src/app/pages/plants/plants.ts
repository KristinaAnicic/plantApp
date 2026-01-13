import { Component, computed, inject, OnInit, signal, ViewEncapsulation } from '@angular/core';
import { PlantDto } from '../../models/plant.interface';
import { PlantService } from '../../services/plant.service';
import { SearchComponent } from "../../components/search-component/search-component";
import { PlantFilterDto } from '../../models/filter.interface';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { RouterLink } from "@angular/router";
import { Pagination } from "../../components/pagination/pagination";

@Component({
  selector: 'app-plants',
  imports: [SearchComponent, RouterLink, Pagination],
  encapsulation: ViewEncapsulation.None,
  templateUrl: './plants.html',
  styleUrl: './plants.css',
})
export class Plants implements OnInit {
  private service = inject(PlantService);
  
  plants = signal<PlantDto[]>([]);
  total = signal(0);
  currentPage = signal(1);
  filter: PlantFilterDto = {};

  private searchSubject = new Subject<string>();
  currentSearchTerm = signal<string>('');

  ngOnInit(): void {
    this.service.getAllPlants().subscribe({
      next: (response) => {
        this.plants.set(response.items);
        this.total.set(response.total);
      },
      error: (err) => {
        console.error("Error while fetching plants: ", err);
      }
    })

    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(searchTerm => {
      this.executeSearch(searchTerm);
    })
  }

  search(searchString: string) {
    this.searchSubject.next(searchString);
  }

  isSearching(): boolean {
    return this.currentSearchTerm().trim().length > 0;
  }

  totalPages = computed(() =>{
    return Math.ceil(this.total()/25)
  })
  
  executeSearch(searchString: string, page?: number){
    this.filter.name = searchString;

    if (page === null)
      this.currentPage.set(1);

    this.currentSearchTerm.set(searchString); 

    const request$ = searchString.trim() === '' 
      ? this.service.getAllPlants(page) 
      : this.service.getAllPlantsFiltered(this.filter, page);

    request$.subscribe({
      next: (response) => {
        this.plants.set(response.items);
        this.total.set(response.total);
      },
      error: (err) => {
        console.error("Error while fetching searched plants: ", err);
      }
    })
  }

  nextPage(page: number){
    this.currentPage.set(page);

    if(this.isSearching()){
      this.executeSearch(this.currentSearchTerm(), page);
    }
    else{
      this.service.getAllPlants(page).subscribe({
        next: (response) => {
          this.plants.set(response.items);
          this.total.set(response.total);
        },
        error: (err) => {
          console.error("Error while fetching plants: ", err);
        }
      })
    }
    
  }

  ngOnDestroy(): void {
    this.searchSubject.complete();
  }
}
function compute(arg0: () => number) {
  throw new Error('Function not implemented.');
}

