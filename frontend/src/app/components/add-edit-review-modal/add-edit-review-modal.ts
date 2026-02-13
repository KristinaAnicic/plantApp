import { Component, computed, inject, input, OnInit, output } from '@angular/core';
import { AddUserRatingDto, UpdateUserRatingDto } from '../../models/user-rating.interface';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Reference } from '../../models/reference.interface';
import { UserRatingService } from '../../services/user-rating.service';
import { NotificationService } from '../../services/notification.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-add-edit-review-modal',
  imports: [ReactiveFormsModule, TranslateModule],
  templateUrl: './add-edit-review-modal.html',
  styleUrl: './add-edit-review-modal.css',
})
export class AddEditReviewModal implements OnInit {
  private service = inject(UserRatingService);
  private notif = inject(NotificationService);
  private translate = inject(TranslateService);
  
  editReview = input<UpdateUserRatingDto | null>(null);
  user = input.required<Reference | undefined>();
  close = output<void>();
  updateList = output<void>();

  isEditing = computed(() => !!this.editReview());
  headerText = computed(() => (this.isEditing() ? this.translate.instant('review.editTitle') : this.translate.instant('review.addTitle')) + ' ' + this.user()?.name);
  buttonText = computed(() => this.isEditing() ? this.translate.instant('forms.saveChanges') : this.translate.instant('review.submitReview'));

  reviewForm = new FormGroup ({
    rating: new FormControl(4, { validators:Validators.min(1), nonNullable: true }),
    ratedUserId: new FormControl(0, { nonNullable: true }),
    comment: new FormControl('', { 
      validators:[
        Validators.required,
        Validators.minLength(10),
        Validators.maxLength(200)
      ], 
      nonNullable: true})
  });

  ngOnInit(): void {
    this.reviewForm.patchValue({
      ratedUserId: this.user()?.id
    });

    const editReview = this.editReview();
    if (editReview) {
      this.reviewForm.patchValue({
        rating: editReview.rating,
        comment: editReview.comment
      })
    }
  }

  addEditReview(){
    this.reviewForm.markAllAsTouched();

    const commentValue = this.reviewForm.get('comment')?.value.trim();
    if (this.reviewForm.invalid || !commentValue) return;

    const data = this.reviewForm.getRawValue();
    
    if (this.isEditing()) {
      const id = this.editReview()?.id;
      if (id === undefined) {
        console.error("Cannot save changes for plant without providing id.");
        return;
      }

      this.service.updateRating(id, data).subscribe({
        next:() => {
          this.notif.showSuccess('Successfully saved changes');   
          this.updateList.emit();
          this.close.emit();
        },
        error:(err:any) => {
          this.notif.showError(err.error.error);
          console.log("Error on editing review: ", err);
        }
      })
      
    }
    else {
        this.service.addRating(data).subscribe({
        next:() => {
          this.notif.showSuccess('Successfully added planted');
          this.close.emit();
          this.updateList.emit();
        },
        error:(err:any) => {
          this.notif.showError(err.error.error);
          console.log("Error on adding planted: ", err);
        }
      })
    }
  }

  onCloseClick(){
    this.close.emit();
  }

  setRating(score: number){
    this.reviewForm.patchValue({ rating: score });
    this.reviewForm.get('rating')?.markAsTouched();
  }
}
