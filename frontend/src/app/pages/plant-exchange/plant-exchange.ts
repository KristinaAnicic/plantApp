import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantExchangeService } from '../../services/plant-exchange.service';
import { PlantExchangeGetDto } from '../../models/plant-exchange.interface';
import { DecimalPipe } from '@angular/common';
import { TimeAgoPipe } from "../../utils/time-ago.pipe";
import { Router } from "@angular/router";
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-plant-exchange',
  imports: [DecimalPipe, TimeAgoPipe],
  templateUrl: './plant-exchange.html',
  styleUrl: './plant-exchange.css',
})
export class PlantExchange implements OnInit {
  @Input() id!: string;
  
  service = inject(PlantExchangeService);
  notif = inject(NotificationService);
  authService = inject(AuthService);
  router = inject(Router);
  exchange = signal<PlantExchangeGetDto | null>(null);
  isMenuOpened = signal(false);

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

  toggleMenu() {
    this.isMenuOpened.update(val => !val);
  }

  editExchange(){
    this.router.navigate(['/trade-form', +this.id]);
  }

  deleteExchange(){
    this.service.removePlantExchange(parseInt(this.id)).subscribe({
      next: () => {
        this.notif.showSuccess("Successfully removed trade listing");
        this.router.navigate(['/trade']);
      },
      error: () => this.notif.showError("Couldn't remove trade listing, try again later!")
    });
    this.isMenuOpened.set(false);
  }
}
