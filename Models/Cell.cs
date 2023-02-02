using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBattle_WPF.Models
{
    public class Cell
    {
        public int Adress { get; set; }
        public int Status { get; set; }

        public bool Hidden { get; }

        public string ImageSourcePath
        {
            get
            {
                switch (Status)
                {
                    case 0:
                        return "\\Resources\\HiddenCell.jpg";
                    case 1: case 2: case 3: case 4: case 5:
                    case 6: case 7: case 8: case 9: case 10: 
                        if (Hidden == false)
                            return "\\Resources\\DeckAlive.jpg";
                        else
                            return "\\Resources\\HiddenCell.jpg";

                    case -1: return "\\Resources\\DeckHit.jpg";
                    case -2: return "\\Resources\\EmptyHit.jpg";
                    default: return "\\Resources\\HiddenCell.jpg";
                }
            }
        }

        public Cell() 
        {
            Adress = 0;
            Status = 0;
            Hidden = false;
        }

        public Cell(int status, int adress, bool hidden)
        {
            Adress = adress;
            Status = status;
            Hidden = hidden;
        }
    }
}
