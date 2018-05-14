import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'cards',
    templateUrl: './cards.component.html'
})
export class CardsComponent {
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

        this.getCardKinds();
        this.getCardSuits();
        this.getRuleCards();
        this.getNextRound();
    }

    public playGame() {
        this.roundNumber++;
        let url = this.baseUrl + 'api/CardGame/PlayGame?playerCount=' + this.playerCount;
        this.http.get(url).subscribe(
            result => {
                let gameResult = result.json() as GameResult;
                this.playerResults = gameResult.playerResults;
                this.wildcard = gameResult.wildcard;
                this.winner = this.getWinner();
            },
            error => {
                console.error(error)
                alert(error)
            }
        );
    }

    public getWinningPlayers(): string {
        let winners = this.playerResults.filter((pr) => pr.points >= this.playerResults[0].points);
        return winners.map(w => w.player.toString()).join(", ");
    }

    public getWinningPoints(): number {
        return this.playerResults[0].points;
    }

    public getWinner(): PlayerResult | null {
        var winner: PlayerResult  | null;
        let topScores = this.playerResults.filter((pr) => pr.points >= this.playerResults[0].points);
        winner = topScores.length === 1 ? this.playerResults[0] : null;
        return winner;
    }

    public getPlayerPlace(place: number): string {
        switch (place) {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
        }
        return place + "th";
    }

    public getKindSymbol(card: Card): string {
        let kind = this.kinds.find(k => k.id === card.kind);
        return !kind ? "-" : kind.symbol;
    }

    public getSuitSymbol(card: Card): string {
        let suit = this.suits.find(s => s.id === card.suit);
        return !suit ? "-" : suit.symbol;
    }

    private getNextRound() {
        let url = this.baseUrl + 'api/CardGame/GetNextRound';
        this.http.get(url).subscribe(
            result => {
                this.roundNumber = result.json() as number;
            },
            error => {
                console.error(error);
                alert(error);
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
                console.error(error);
                alert(error);
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
                console.error(error);
                alert(error);
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
                console.error(error);
                alert(error);
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
