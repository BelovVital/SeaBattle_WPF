using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SeaBattle_WPF.Models
{
    public class Game   : CustomNotifyPropertyChanged
    {
        public Map Player;
        public Map Computer;

        public int _mapSize = 10;
        public bool IsComputerNeedHitShip { get; set; }
        public short LastDirectionForComputer { get; set; }
        public int LastX { get; set; }
        public int LastY { get; set; }

        public bool IsWinPlayer
        {
            get
            {
                if (Computer.ShipsLife.Sum() == 0)
                    return true;
                return false;
            }
        }
        public bool IsWinComputer
        {
            get
            {
                if (Player.ShipsLife.Sum() == 0)
                    return true;
                return false;
            }
        }
        public Game()
        {
            Player = new Map();
            Computer = new Map();
            IsComputerNeedHitShip = false;
            LastDirectionForComputer = 0;
            LastX = 0;
            LastY = 0;
        }

        public void CreateNewGame()
        {
            Player.MapClear(false);
            Computer.MapClear(true);

            Player.MapCreate();
            Thread.Sleep(100);
            Computer.MapCreate();
        }

        
        
    }
}
