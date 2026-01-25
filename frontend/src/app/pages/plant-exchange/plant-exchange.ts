import { Component, computed, inject, Input, OnInit, signal } from '@angular/core';
import { PlantExchangeService } from '../../services/plant-exchange.service';
import { PlantExchangeGetDto } from '../../models/plant-exchange.interface';
import { DecimalPipe } from '@angular/common';
import { TimeAgoPipe } from "../../utils/time-ago.pipe";
import { Router } from "@angular/router";
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { AddEditReviewModal } from '../../components/add-edit-review-modal/add-edit-review-modal';
import { UpdateUserRatingDto, UserRatingDto } from '../../models/user-rating.interface';
import { UserRatingService } from '../../services/user-rating.service';

@Component({
  selector: 'app-plant-exchange',
  imports: [DecimalPipe, TimeAgoPipe, AddEditReviewModal],
  templateUrl: './plant-exchange.html',
  styleUrl: './plant-exchange.css',
})
export class PlantExchange implements OnInit {
  @Input() id!: string;
  
  service = inject(PlantExchangeService);
  ratingService = inject(UserRatingService);
  notif = inject(NotificationService);
  authService = inject(AuthService);
  router = inject(Router);
  exchange = signal<PlantExchangeGetDto | null>(null);
  reviewToEdit = signal<UpdateUserRatingDto | null>(null);

  isMenuOpened = signal(false);
  isReviewModalOpen = signal(false);
  openedReviewMenuId = signal<number | null>(null);

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

    this.openedReviewMenuId.set(null);
  }

  displayImages = computed(() => {
    const all = this.exchange()?.images?.filter(im => im.url !== this.exchange()?.image) ?? [];
    return all.length > 4 ? all.slice(0, 3) : all.slice(0, 4);
  })

  hasMoreImages = computed(() => (this.exchange()?.images?.length ?? 0) > 4);

  toggleMenu() {
    this.isMenuOpened.update(val => !val);
  }

  toggleReviewModal() {
    this.isReviewModalOpen.update(val => !val);
  }

  toggleReviewMenu(id: number, event: Event){
    event.stopPropagation();
    this.openedReviewMenuId.update(current => current === id ? null : id);
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

  addReview(){
    this.reviewToEdit.set(null);
    this.isReviewModalOpen.set(true);
  }

  editReview(id: number){
    const currentReview = this.exchange()?.userRatings?.find(r => r.id === id);
    if(!currentReview) return;

    const editReview: UpdateUserRatingDto = {
      ...currentReview
    };

    this.reviewToEdit.set(editReview);
    this.isReviewModalOpen.set(true);
  }

  deleteReview(id: number){
    this.ratingService.removeRating(id).subscribe({
      next: () => {
        this.notif.showSuccess("Successfully deleted review");
        this.loadExchange();
      },
      error: () => this.notif.showError("Couldn't delete review, try again later!")
    });
    this.openedReviewMenuId.set(null);
  }

  showAddReviewButton = computed(() => {
    const currentUser = this.authService.currentUser();
    if (!currentUser) return false;

    const exchangeData = this.exchange();
    if(this.authService.isAdmin())
      return true;

    if(exchangeData?.user.id === currentUser.id)
      return false;

    const hasAlreadyRated = exchangeData?.userRatings?.find(r => r.rater.id === currentUser.id);
    if (hasAlreadyRated)
      return false;

    return true;
  })
}
