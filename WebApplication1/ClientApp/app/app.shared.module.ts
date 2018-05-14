import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { WinnersComponent } from './components/winners/winners.component';
import { CardsComponent } from './components/cards/cards.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CardsComponent,
        WinnersComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'cards', pathMatch: 'full' },
            { path: 'cards', component: CardsComponent },
            { path: 'winners', component: WinnersComponent },
            { path: '**', redirectTo: 'cards' }
        ])
    ]
})
export class AppModuleShared {
}
