using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thron
{
    class GameFieldDebugProxy
    {
        private readonly GameField _gameFields;

        public GameFieldDebugProxy(GameField scientist)
        {
            _gameFields = scientist;
        }

        public string Fields
        {
            get
            {
                string result = "";
                for (int y = 0; y < GameField.HEIGHT; y++)
                {
                    for (int x = 0; x < GameField.WIDTH; x++)
                    {
                        result += printField(_gameFields.fields[x, y]);
                    }
                    result += "<br>";
                }

                return result;
            }
        }

        private string printField(Field field)
        {
            if (field.Player == null)
                return ColorString("0",Color.Black);
            else if(field.Player==0)
                return ColorString((field.Player+1).ToString(), Color.Green);
            else
                return ColorString((field.Player+1).ToString(), Color.Red);
        }

        private string ColorString(string str, Color color)
        {
            return $"<span style = \"color: {ColorTranslator.ToHtml(color)};\">{str}</span>";
        }

    }

}

