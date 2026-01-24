import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeAgo',
  standalone: true
})
export class TimeAgoPipe implements PipeTransform {

  transform(value: string | Date | undefined, args?: any): any {
    if (!value) return value;

    const seconds = Math.floor((+new Date() - +new Date(value)) / 1000);
    if (seconds < 29)
      return 'just now';

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
        if (counter === 1) {
          return `1 ${key} ago`;
        }
        else {
          return `${counter} ${key}s ago`;
        }
      }
    }
    return value;
  }

}
