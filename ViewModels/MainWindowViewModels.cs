using SeaBattle_WPF.Models;
using SeaBattle_WPF.Models.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Input;
using System.Windows;
using SeaBattle_WPF.Views;

namespace SeaBattle_WPF.ViewModels
{
    public class MainWindowViewModels : CustomNotifyPropertyChanged
    {
        public Cell SelectedCell { get; set; }
        public int SelectedIndex { get; set; } = -1;

        public Game Game = new Game();

        public static Random random = new Random();

        public static bool NowShootComputer { get; private set;  } = false;
                
        public IList<Cell> ComputerMap { get; } =
            new ObservableCollection<Cell>();
        public IList<Cell> PlayerMap { get; } =
            new ObservableCollection<Cell>();

      

        private ICommand _newGameCommand;
        public ICommand NewGameCommand =>
            _newGameCommand ??
            (_newGameCommand = new CustomCommand(
                param =>
                {
                    Game.CreateNewGame();
                    ReplaceGameMap();
                }));

        private ICommand _hitSelectedCellCommand;
        public ICommand HitSelectedCellCommand =>
            _hitSelectedCellCommand ??
            (_hitSelectedCellCommand = new CustomCommand(
                param =>
                {
                    ShootPlayer();
                    ShootComputer();
                }));

        public void ReplaceGameMap()
        {
            ReplaceComputerMap();
            ReplacePlayerMap();
        }

        public void ReplacePlayerMap()
        {
            PlayerMap.Clear();
            foreach (var item in Game.Player.Cells)
            {
                PlayerMap.Add(item);
            }
            OnPropertyChanged(nameof(PlayerMap));
        }

        public void ReplaceComputerMap()
        {
            ComputerMap.Clear();
            foreach (var item in Game.Computer.Cells)
            {
                ComputerMap.Add(item);
            }
            OnPropertyChanged(nameof(ComputerMap));
        }

        public bool IsAnyWinGame() 
        {
            if (Game.IsWinComputer == true)
            {
                MessageBoxResult result = MessageBox.Show("ВЫ ПРОИГРАЛИ!", "Конец игры", MessageBoxButton.OK);
                Game.CreateNewGame();
                NowShootComputer = false;
                ReplaceGameMap();
            }
            else if (Game.IsWinPlayer == true)
            {
                MessageBoxResult result = MessageBox.Show("ВЫ ВЫИГРАЛИ!", "Конец игры", MessageBoxButton.OK);
                Game.CreateNewGame();
                ReplaceGameMap();
            }
            return Game.IsWinComputer || Game.IsWinPlayer;
        }

        public void ShootPlayer()
        {
            
            if (NowShootComputer == false)
            {
                int adressString;
                if (SelectedIndex == -1)
                    return;
                else
                    adressString = ComputerMap.ElementAt(SelectedIndex).Adress;         //Координаты стрельбы
                int x = adressString / 10;
                int y = adressString % 10;
                                
                if (Game.Computer.Cells[x, y].Status >= 1)
                {
                                                                                        //Если попал
                    Game.Computer.ShipsLife[Game.Computer.Cells[x, y].Status] -= 1;     //Уменьшаем жизнь корабля на 1

                    if (Game.Computer.IsShipAlive(x, y) == false)                       //Если корабль убит
                    {
                        //if (IsAnyWinGame() == true)
                        //    return;
                            Game.Computer.Cells[x, y].Status = -1;                      //Отмечаем поле потопления корабля
                            Game.Computer.HitAroundShip(x, y);                          //Отмечаем поля вокруг потопленного корабля
                    }
                    else
                    {
                        Game.Computer.Cells[x, y].Status = -1;                          //Отмечаем поле попадания в корабль
                    }
                    ReplaceComputerMap();
                }
                //else if (Game.Computer.Cells[x, y].Status < 0)                        //Если в точку уже стреляли разрешить повторный выстрел
                //{
                //    shootOneMore = true;
                //}
                else if (Game.Computer.Cells[x, y].Status == 0)                         //Если промах
                {
                    Game.Computer.Cells[x, y].Status = -2;                              //Отмечаем битое пустое поле
                    NowShootComputer = true;
                }

                if (IsAnyWinGame() == true)
                    return;
                else
                    ReplaceComputerMap();
            }
        }

        //Стрельба компьютера (с добиванием подбитых кораблей) 
        public void ShootComputer()
        {
            if (NowShootComputer == true)
            {
                int x = 0, y = 0;                                               //Координаты стельбы
                int _mapSize = 10;
                
                    if (Game.IsComputerNeedHitShip == true)
                    {
                        x = Game.LastX;
                        y = Game.LastY;
                    }
                do
                {
                    if (Game.IsComputerNeedHitShip == false)                        //Режим случайного выстрела
                    {
                        do
                        {
                            x = random.Next(_mapSize);
                            y = random.Next(_mapSize);
                        } while (Game.Player.Cells[x, y].Status < 0);


                        if (Game.Player.Cells[x, y].Status >= 1)                    //если в этой точке корабль
                        {
                            Game.IsComputerNeedHitShip = true;                      //включаем режим добивания
                            Game.LastX = x;
                            Game.LastY = y;
                        }
                        else if (Game.Player.Cells[x, y].Status == 0)
                        {
                            Game.Player.Cells[x, y].Status = -2;                    //отмечаем попадание в пустоту
                            Game.IsComputerNeedHitShip = false;                     //отключаем рем добивания
                            NowShootComputer = false;
                        }
                    }
                    else if (Game.IsComputerNeedHitShip == true)                    //Режим добивания
                    {
                        switch (Game.Player.Cells[x, y].Status)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                            case 10:
                                Game.Player.ShipsLife[Game.Player.Cells[x, y].Status] -= 1;

                                if (Game.Player.IsShipAlive(x, y) == false)         //Если был убит
                                {
                                    Game.Player.Cells[x, y].Status = -1;
                                    Game.IsComputerNeedHitShip = false;
                                    Game.LastDirectionForComputer = 0;
                                    Game.Player.HitAroundShip(x, y);
                                }
                                else                                                //Если был ранен
                                {
                                    Game.Player.Cells[x, y].Status = -1;
                                    Game.IsComputerNeedHitShip = true;
                                }
                                //Thread.Sleep(200);
                                ReplacePlayerMap();
                                break;
                            case 0:                                                 //Если попадание в пустое место
                                Game.Player.Cells[x, y].Status = -2;
                                NowShootComputer = false;

                                break;
                            case -1:
                            case -2:                                                //Если поле уже битое сдвигаемся или изменяем направление
                                NowShootComputer = true;
                                switch (Game.LastDirectionForComputer)
                                {
                                    case 0:
                                        if (x > 0 && Game.Player.Cells[x - 1, y].Status != -2)
                                        {  //Проверяем направление вверх
                                            x--;
                                        }
                                        else
                                        {
                                            Game.LastDirectionForComputer = 1;
                                            x = Game.LastX;
                                        }
                                        break;
                                    case 1:
                                        if (x < _mapSize - 1 && Game.Player.Cells[x + 1, y].Status != -2)
                                        {   //Проверяем направление вниз
                                            x++;
                                        }
                                        else
                                        {
                                            Game.LastDirectionForComputer = 2;
                                            x = Game.LastX;
                                        }
                                        break;
                                    case 2:
                                        if (y > 0 && Game.Player.Cells[x, y - 1].Status != -2)
                                        {   //Проверяем направление влево
                                            y--;
                                        }
                                        else
                                        {
                                            Game.LastDirectionForComputer = 3;
                                            y = Game.LastY;
                                        }
                                        break;
                                    case 3:
                                        if (y < _mapSize - 1 && Game.Player.Cells[x, y + 1].Status != -2)
                                        {   //Проверяем направление вправо
                                            y++;
                                        }
                                        else
                                        {
                                            Game.LastDirectionForComputer = 0;
                                            Game.IsComputerNeedHitShip = false;
                                            y = Game.LastY;
                                        }
                                        break;
                                }
                                break;
                        }
                    }

                    if (IsAnyWinGame() == true)
                        return;
                    else 
                    {
                        //Thread.Sleep(100);
                        ReplaceGameMap();
                    }

                } while (NowShootComputer == true);
                
            }
        }


        private string GetContent(object param)
        {
            var control = param as ContentControl;
            return control?.Content?.ToString();
        }
    }
}
