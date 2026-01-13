import { Component, computed, input, Input, numberAttribute, OnInit, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  imports: [],
  templateUrl: './pagination.html',
  styleUrl: './pagination.css',
})
export class Pagination {
  totalPages = input.required<number>();
  currentPage = input.required<number>();
  newPage = output<number>();

  pages = computed(() => {
    const current = this.currentPage();
    const total = this.totalPages();

    if (total <= 6) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    let start = 1;
    let end = 5;

    if (current <= 4) {
      start = 1;
      end = Math.min(5, total);;
    } 
    else if (current > total - 3) {
      start = total - 3;
      end = total;
    }
    else {
      start = current - 1;
      end = current + 1;
    }

    const length = end - start + 1;
    return Array.from({ length: length }, (_, i) => i + start);
  })

  changePageClick(page: number){
    const total = this.totalPages();
    if (page < 1)
      page = 1;

    else if (page > total)
      page = total;

    this.newPage.emit(page);
  }
}
