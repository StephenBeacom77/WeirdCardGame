import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'cards',
    templateUrl: './cards.component.html'
})
export class CardsComponent {

    public error: string | null;

    public ruleCards: Card[];
    public wildcard: Card;

    public roundNumber: number = 1;
    public playerCount: number = 2;

    public playerResults: PlayerResult[];
    public winner: PlayerResult | null;

    private http: Http;
    private baseUrl: string;

    private kinds: Kind[];
    private suits: Suit[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;

        this.initGame();
    }

    private playGame() {
        this.initError();
        this.roundNumber++;
        let url = this.baseUrl + 'api/CardGame/PlayGame?playerCount=' + this.playerCount;
        this.http.get(url).subscribe(
            result => {
                let gameResult = result.json() as GameResult;
                this.playerResults = gameResult.playerResults;
                this.wildcard = gameResult.wildcard;
                this.winner = this.getClearWinner();
            },
            error => {
                this.handleError(error);
            }
        );
    }

    private getClearWinner(): PlayerResult | null {
        var winner: PlayerResult | null;
        let topScores = this.playerResults.filter((pr) => pr.points >= this.playerResults[0].points);
        winner = topScores.length === 1 ? this.playerResults[0] : null;
        return winner;
    }

    private getWinningPlayers(): string {
        let winners = this.playerResults.filter((pr) => pr.points >= this.playerResults[0].points);
        return winners.map(w => w.player.toString()).join(", ");
    }

    private getWinningPoints(): number {
        return this.playerResults[0].points;
    }

    private getKindSymbol(card: Card): string {
        let kind = this.kinds.find(k => k.id === card.kind);
        return !kind ? "-" : kind.symbol;
    }

    private getSuitSymbol(card: Card): string {
        let suit = this.suits.find(s => s.id === card.suit);
        return !suit ? "-" : suit.symbol;
    }

    private getPlayerPlace(place: number): string {
        switch (place) {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
        }
        return place + "th";
    }

    private initGame(): void {
        this.initError();
        this.getCardKinds();
        this.getCardSuits();
        this.getRuleCards();
        this.getNextRound();
    }

    private initError(): void {
        this.error = null;
    }

    private handleError(error: any): void {
        this.error = error;
        console.error(error);
    }

    private getNextRound() {
        let url = this.baseUrl + 'api/CardGame/GetNextRound';
        this.http.get(url).subscribe(
            result => {
                this.roundNumber = result.json() as number;
            },
            error => {
                this.handleError(error);
            }
        );
    }

    private getRuleCards() {
        let url = this.baseUrl + 'api/CardGame/GetRuleCards';
        this.http.get(url).subscribe(
            result => {
                this.ruleCards = result.json() as Card[];
            },
            error => {
                this.handleError(error);
            }
        );
    }

    private getCardKinds() {
        let url = this.baseUrl + 'api/CardGame/GetCardKinds';
        this.http.get(url).subscribe(
            result => {
                this.kinds = result.json() as Kind[];
            },
            error => {
                this.handleError(error);
            }
        );
    }

    private getCardSuits() {
        let url = this.baseUrl + 'api/CardGame/GetCardSuits';
        this.http.get(url).subscribe(
            result => {
                this.suits = result.json() as Suit[];
            },
            error => {
                this.handleError(error);
            }
        );
    }
}

interface GameResult {
    wildcard: Card;
    playerResults: PlayerResult[];
}

interface PlayerResult {
    player: number;
    cards: Card[];
    points: number;
    ranking: string;
}

interface Card {
    kind: number;
    suit: number;
    points: number;
}

interface Kind {
    id: number;
    symbol: string;
}

interface Suit {
    id: number;
    symbol: string;
}
