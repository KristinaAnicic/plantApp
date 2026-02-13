import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { provideTranslateHttpLoader, TranslateHttpLoader } from '@ngx-translate/http-loader';
import { LanguageService } from '../services/language.service';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    TranslateModule.forRoot({
      fallbackLang: 'hr'
    })
  ],
  exports: [TranslateModule],
  providers: [
    LanguageService,
    provideTranslateHttpLoader({
      prefix: '/assets/i18n/',
      suffix: '.json'
    })
  ]
})
export class AppTranslateModule { }
