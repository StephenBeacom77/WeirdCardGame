﻿namespace WeirdCardGame.Models
{
    public class PlayerResult
    {
        public int Player { get; set; }
        public int Points { get; set; }
        public ScoredCard[] Cards { get; set; }
    }
}
