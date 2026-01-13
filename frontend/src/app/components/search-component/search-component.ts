import { Component, EventEmitter, output, Output, signal, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-search-component',
  imports: [],
  encapsulation: ViewEncapsulation.None,
  templateUrl: './search-component.html',
  styleUrl: './search-component.css',
})
export class SearchComponent {
  @Output() search = new EventEmitter<void>();

  searchQuery = signal('');
  searchTriggered = output<string>();
  
  onTyping(event: Event){
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery.set(value);
    this.searchTriggered.emit(value);
  }

  onSearchClick(){
    this.searchTriggered.emit(this.searchQuery());
  }

  onResetClick(){
    this.searchQuery.set("");
    this.searchTriggered.emit("");
  }
}
