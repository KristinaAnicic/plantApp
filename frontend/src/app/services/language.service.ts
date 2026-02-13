import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root',
})
export class LanguageService {
  translate = inject(TranslateService);
  private currentLanguage: string;

  constructor() {
    this.currentLanguage = localStorage.getItem('selectedLanguage') || this.translate.getDefaultLang() || 'en';

    this.translate.onLangChange.subscribe(event => {
      this.currentLanguage = event.lang;
      localStorage.setItem('selectedLanguage', event.lang);
    });
  }

  initLanguage(){
    this.translate.use(this.currentLanguage);
  }

  changeLanguage(language:string){
    this.translate.use(language);
  }

  getCurrentLanguage(): string{
    return this.currentLanguage;
  }
}
