import { inject, Pipe, PipeTransform } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Pipe({
  name: 'timeAgo',
  standalone: true
})
export class TimeAgoPipe implements PipeTransform {
  private translate = inject(TranslateService);
    
  transform(value: string | Date | undefined, args?: any): any {
    if (!value) return value;

    const seconds = Math.floor((+new Date() - +new Date(value)) / 1000);
    if (seconds < 29)
      return this.translate.instant('timeAgo.justNow');

    const intervals: { [key: string]: number } = {
      'year': 31536000,
      'month': 2592000,
      'week': 604800,
      'day': 86400,
      'hour': 3600,
      'minute': 60,
      'second': 1
    }

    let counter;
    for (const key in intervals){
      counter = Math.floor(seconds / intervals[key]);
      if (counter > 0){
        const unitKey = key.charAt(0).toUpperCase() + key.slice(1);

        if (counter === 1) {
          return this.translate.instant(`timeAgo.single${unitKey}`, { count: counter });
        }
        else {
          if (counter % 10 === 2 || counter % 10 === 3 || counter % 10 === 4) {
            return this.translate.instant(`timeAgo.pluralSmall${unitKey}`, { count: counter });
          } else {
            return this.translate.instant(`timeAgo.pluralLarge${unitKey}`, { count: counter });
          }
        }
      }
    }
    return value;
  }

}
