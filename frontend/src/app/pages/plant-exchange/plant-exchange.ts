import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantExchangeService } from '../../services/plant-exchange.service';
import { PlantExchangeGetDto } from '../../models/plant-exchange.interface';
import { DecimalPipe } from '@angular/common';
import { Dir } from "../../../../node_modules/@angular/cdk/types/_bidi-module-chunk";
import { TimeAgoPipe } from "../../utils/time-ago.pipe";

@Component({
  selector: 'app-plant-exchange',
  imports: [DecimalPipe, TimeAgoPipe],
  templateUrl: './plant-exchange.html',
  styleUrl: './plant-exchange.css',
})
export class PlantExchange implements OnInit {
  @Input() id!: string;
  
  service = inject(PlantExchangeService);
  exchange = signal<PlantExchangeGetDto | null>(null);

  ngOnInit(): void {
    this.loadExchange();
  }

  loadExchange(){
    this.service.getPlantExchange(parseInt(this.id)).subscribe({
      next: (result) => {
        this.exchange.set(result);
      },
      error: (err) => {
        console.log("Error while fetching exchange: ", err);
      }
    })
  }

  displayImages = computed(() => {
    const all = this.exchange()?.images?.filter(im => im.url !== this.exchange()?.image) ?? [];
    return all.length > 4 ? all.slice(0, 3) : all.slice(0, 4);
  })

  hasMoreImages = computed(() => (this.exchange()?.images?.length ?? 0) > 4);
}
