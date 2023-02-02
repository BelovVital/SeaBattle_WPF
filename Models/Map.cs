using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle_WPF.Models
{
    public class Map
    {
        public Cell[,] Cells;
        public const int _mapSize = 10;

        public int[] ShipsLife;

        public int ShipId { get; set; }

        public Map()
        {
            Cells = new Cell[_mapSize, _mapSize];
            MapClear();
        }

        public void MapClear()
        {
            ShipId = 1;
            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    Cells[i, j] = new Cell(0, (i*10+j), false);
                }
            }
            ShipsLife = new int[11] { 0, 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        }

        public void MapClear(bool hidden)
        {
            ShipId = 1;
            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    Cells[i, j] = new Cell(0, (i * 10 + j), hidden);
                }
            }
            ShipsLife = new int[11] { 0, 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        }

        //Расстановка кораблей в соответсвии с палубами и количеством 
        public void MapCreate()
        {
            SetRandomShip(4, 1);    //1 шт. 4-палубный
            SetRandomShip(3, 2);    //2 шт. 3-палубных
            SetRandomShip(2, 3);    //3 шт. 2-палубных
            SetRandomShip(1, 4);    //4 шт. 1-палубных
        }

        //Автоматическая постановка корабля
        public void SetRandomShip(int countDecks, int countShips)
        {
            int x, y;           //Координаты первой палубы
            int direction;      //Направление добавления палуб
            Random random = new Random();

            //Расставляем корабли в указанном количестве
            while (countShips > 0)
            {
                //Генерация позиции и направления
                x = random.Next(_mapSize);
                y = random.Next(_mapSize);
                direction = random.Next(4);

                //Установка корабля если есть возможность
                if (IsShipPosibleSetRandom(x, y, direction, countDecks))
                { 
                    for (int i = 0; i < countDecks; i++)
                    {
                        Cells[x,y].Status = ShipId;         //Присваиваем порядковый номер кораблю
                        switch (direction)
                        {        //Сдвигаем координаты для постановки палубы в зависимости от направления
                            case 0:                     //вправо
                                x++;
                                break;
                            case 1:                     //вверх
                                y++;
                                break;
                            case 2:                     //влево
                                x--;
                                break;
                            case 3:                     //вниз
                                y--;
                                break;
                        }
                    }
                    ShipId += 1;                       //Увеличиваем значение порядкового номера для нового корабля
                    countShips -= 1;                    //Учитываем, что корабль поставлен
                }
            }
        }

        //Проверка возможности постановки корабля
        public bool IsShipPosibleSetRandom(int x, int y, int direction, int countDeck)
        {
            for (int i = 0; i < countDeck; i++)
            {
                //Проверка по границам поля
                if (x < 0 || y < 0 || x >= _mapSize || y >= _mapSize)
                    return false;
      
                //Проверка соприкосновения с другим кораблем
                //Проверяются центральная точка и все вокруг нее
                if (Cells[x,y].Status >= 1)
                {
                    return false;
                }
                if (y < _mapSize - 1)
                    if (Cells[x,y + 1].Status >= 1)
                    {   //юг
                        return false;
                    }
                if (x < _mapSize - 1)
                    if (Cells[x + 1,y].Status >= 1)
                    {   //восток
                        return false;
                    }
                if (y > 0)
                    if (Cells[x,y - 1].Status >= 1)
                    {   //север
                        return false;
                    }
                if (x > 0)
                    if (Cells[x - 1, y].Status >= 1)
                    {   //запад
                        return false;
                    }
                if (x < _mapSize - 1 && y < _mapSize - 1)
                    if (Cells[x + 1, y + 1].Status >= 1)
                    {   //юго-восток
                        return false;
                    }
                if (x > 0 && y > 0)
                    if (Cells[x - 1, y - 1].Status >= 1)
                    {   //северо-запад
                        return false;
                    }
                if (x < _mapSize - 1 && y > 0)
                    if (Cells[x + 1, y - 1].Status >= 1)
                    {   //северо-восток
                        return false;
                    }
                if (x > 0 && y < _mapSize - 1)
                    if (Cells[x - 1, y + 1].Status >= 1)
                    {   //юго-запад
                        return false;
                    }
                //Сдвигаем кординаты проверки следующей палубы
                switch (direction)
                {
                    case 0:     //вправо
                        x++;
                        break;
                    case 1:     //вверх
                        y++;
                        break;
                    case 2:     //влево
                        x--;
                        break;
                    case 3:     //вниз
                        y--;
                        break;
                }
            }
            return true;
        }

        public bool IsShipAlive(int x, int y)
        {
            if (ShipsLife[Cells[x, y].Status] == 0)
                return false;
            return true;
        }

        //Отмечание позиций вокруг подбитого корабля
        public void HitAroundShip(int x, int y)
        {
            short direction = 1;            //Направление потопленного корабля для проверки
            int startX = x;
            int startY = y;

            //Запоминаем направление корабля для проверки палуб
            if (y < _mapSize - 1)
                if (Cells[x, y + 1].Status == -1)
                {      //юг
                    direction = 1;
                }
            if (x < _mapSize - 1)
                if (Cells[x + 1, y].Status == -1)
                {      //восток
                    direction = 2;
                }
            if (y > 0)
                if (Cells[x, y - 1].Status == -1)
                {      //север
                    direction = 3;
                }
            if (x > 0)
                if (Cells[x - 1, y].Status == -1)
                {      //запад
                    direction = 4;
                }

            CheckCellsAroundDeck(x, y, direction);

            //Меняем направление
            if (direction == 1) direction = 3;
            if (direction == 3) direction = 1;
            if (direction == 2) direction = 4;
            if (direction == 4) direction = 2;

            x = startX;
            y = startY;

            CheckCellsAroundDeck(x, y, direction);

        }

        public void CheckCellsAroundDeck(int x, int y, short direction)
        {
            bool around;
            //Проверяем пустые позиции вокруг палубы подбитого корабля и помечаем их
            do
            {
                around = false;
                if (y < _mapSize - 1)
                    if (Cells[x, y + 1].Status == 0)
                    {   //юг
                        Cells[x, y + 1].Status = -2;
                    }
                if (x < _mapSize - 1)
                    if (Cells[x + 1, y].Status == 0)
                    {   //восток
                        Cells[x + 1, y].Status = -2;
                    }
                if (y > 0)
                    if (Cells[x, y - 1].Status == 0)
                    {   //север
                        Cells[x, y - 1].Status = -2;
                    }
                if (x > 0)
                    if (Cells[x - 1, y].Status == 0)
                    {   //запад
                        Cells[x - 1, y].Status = -2;
                    }
                if (x < _mapSize - 1 && y < _mapSize - 1)
                    if (Cells[x + 1, y + 1].Status == 0)
                    {   //юго-восток
                        Cells[x + 1, y + 1].Status = -2;
                    }
                if (x > 0 && y > 0)
                    if (Cells[x - 1, y - 1].Status == 0)
                    {   //северо-запад
                        Cells[x - 1, y - 1].Status = -2;
                    }
                if (x < _mapSize - 1 && y > 0)
                    if (Cells[x + 1, y - 1].Status == 0)
                    {   //северо-восток
                        Cells[x + 1, y - 1].Status = -2;
                    }
                if (x > 0 && y < _mapSize - 1)
                    if (Cells[x - 1, y + 1].Status == 0)
                    {   //юго-запад
                        Cells[x - 1, y + 1].Status = -2;
                    }
                //Проверяем рядом палубы и если есть сдвигаемся на нее для последующей проверки
                switch (direction)
                {
                    case 1:
                        if (y < _mapSize-1)
                            if (Cells[x, y + 1].Status == -1)
                            {  //юг
                                y++;
                                around = true;
                            }
                        break;
                    case 2:
                        if (x < _mapSize-1)
                            if (Cells[x + 1, y].Status == -1)
                            {  //восток
                                x++;
                                around = true;
                            }
                        break;
                    case 3:
                        if (y > 0)
                            if (Cells[x, y - 1].Status == -1)
                            {  //север
                                y--;
                                around = true;
                            }
                        break;
                    case 4:
                        if (x > 0)
                            if (Cells[x - 1, y].Status == -1)
                            {  //запад
                                x--;
                                around = true;
                            }
                        break;
                }
            } while (around);
        }
    }
}
