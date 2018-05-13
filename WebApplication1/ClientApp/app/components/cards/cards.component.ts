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

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;

        this.getRuleCards();
    }

    private getRuleCards() {
        let url = this.baseUrl + 'api/CardGame/GetRuleCards';
        this.http.get(url).subscribe(
            result => {
                this.ruleCards = result.json() as Card[];
            },
            error => {
                console.error(error)
                alert(error)
            }
        );
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
        switch (card.kind) {
            case 1: return "A";
            case 2: return "2";
            case 3: return "3";
            case 4: return "4";
            case 5: return "5";
            case 6: return "6";
            case 7: return "7";
            case 8: return "8";
            case 9: return "9";
            case 10: return "10";
            case 11: return "J";
            case 12: return "Q";
            case 13: return "K";
        }
        return "?";
    }

    public getSuitSymbol(card: Card): string {
        switch (card.suit) {
            case 1: return "\u2665"; // hearts
            case 2: return "\u2663"; // clubs
            case 3: return "\u2666"; // diamonds
            case 4: return "\u2660"; // spades
        }
        return "?";
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

