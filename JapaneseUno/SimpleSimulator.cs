﻿using System.Collections.Generic;

namespace JapaneseUno
{
    public class SimpleSimulator
    {
        public List<GameResult> StartGame(GameConfig config)
        {
            var cards = new List<int>();
            var players = new List<List<int>>();
            
            for (int i = 0; i < config.maxCard; i++)
            {
                cards.Add(i);
            }
            
            for (int i = 0; i < config.playerNumber; i++)
            {
                players.Add(new List<int>(cards));
            }

            return NextGame(players, new List<int>(), 0, 0);
        }
        
        private List<GameResult> NextGame(List<List<int>> players, List<int> layout, int order, int turn)
        {
            bool emptyLayout = layout.Count == 0;
            int playersCount = players.Count;
            bool canPass = true;
            
            for (int i = 0; i < playersCount; i++)
            {
                var player = players[i];
                // end game
                if (player.Count == 0)
                {
                    // NLogService.Debug("End Game");
                    return new List<GameResult>{new GameResult(i)};
                }
                
                if (!emptyLayout && i != PrevOrder(order, playersCount) 
                                 && Max(player) > layout[0])
                {
                    canPass = false;
                }
            }
            
            // pass
            if (!emptyLayout && canPass)
            {
                // NLogService.Debug("Pass");
                return NextGame(players, new List<int>(), NextOrder(order, playersCount), turn);
            }

            var results = new List<GameResult>();
            var playing = players[order];
            
            for (int i = 0; i < playing.Count; i++)
            {
                var card = playing[i];
                if (emptyLayout || card > layout[0])
                {
                    var playedPlayers = DeepCopy(players);
                    playedPlayers[order].Remove(card);
                    var result =
                        NextGame(playedPlayers, new List<int>{card}, NextOrder(order, playersCount), turn + 1);
                    for (int j = 0; j < result.Count; j++)
                    {
                        results.Add(result[j]);
                    }
                }
            }

            return results;
        }
        
        private int PrevOrder(int order, int count)
        {
            int prev = order - 1;
            
            if (prev < 0)
            {
                prev = count - 1;
            }

            return prev;
        }

        private int NextOrder(int order, int count)
        {
            int next = order + 1;

            if (next >= count)
            {
                next = 0;
            }

            return next;
        }

        private int Max(List<int> numbers)
        {
            int max = 0;
            
            for (int i = 0; i < numbers.Count; i++)
            {
                int num = numbers[i];
                if (num > max)
                {
                    max = num;
                }
            }

            return max;
        }

        private List<List<T>> DeepCopy<T>(List<List<T>> array)
        {
            List<List<T>> newArray = new List<List<T>>();

            for (int i = 0; i < array.Count; i++)
            {
                newArray.Add(new List<T>(array[i]));
            }

            return newArray;
        }
    }

    public struct GameResult
    {
        public int WinPlayer { get; private set; }

        public GameResult(int winPlayer)
        {
            this.WinPlayer = winPlayer;
        }

        public override string ToString()
        {
            return "WinPlayer: " + WinPlayer;
        }
    }
}