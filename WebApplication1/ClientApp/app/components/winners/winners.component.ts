import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'winners',
    templateUrl: './winners.component.html'
})
export class WinnersComponent {
    public games: Game[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/CardGame/GetWinnersList').subscribe(
            result => {
                this.games = result.json() as Game[];
            },
            error => console.error(error)
        );
    }
}

interface Game {
    round: number;
    winner: number;
}
