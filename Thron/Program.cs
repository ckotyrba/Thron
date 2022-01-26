using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Thron
{
    /**
     * Auto-generated code below aims at helping you parse
     * the standard input according to the problem statement.
     **/
    public class Program
    {

        public static void Main(string[] args)
        {
            var thron = new Thron();
            thron.Play();
        }
    }

    class Thron
    {

        private GameField gameField = new GameField();
        public static int Round = 0;

        public void Play()
        {
            string[] inputs;

            // game loop
            while (true)
            {
                inputs = ReadConsole().Split(' ');
                int N = int.Parse(inputs[0]); // total number of players (2 to 4).
                int P = int.Parse(inputs[1]); // your player number (0 to 3).
                int currentX = 0;
                int currentY = 0;
                for (int i = 0; i < N; i++)
                {
                    inputs = ReadConsole().Split(' ');
                    int X0 = int.Parse(inputs[0]); // starting X coordinate of lightcycle (or -1)
                    int Y0 = int.Parse(inputs[1]); // starting Y coordinate of lightcycle (or -1)
                    int X1 = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
                    int Y1 = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)
                    if (i == P)
                    {
                        currentX = X1;
                        currentY = Y1;
                    }
                    UpdateField(X0, Y0, X1, Y1, i);
                }
                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");

                int currentMax = SimulateMoveAndGetPlayerFieldCount(currentX, currentY, Direction.UP);
                Direction bestDirection = Direction.UP;
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    int fieldsBelongToPlayer = SimulateMoveAndGetPlayerFieldCount(currentX, currentY, direction);
                    Console.Error.WriteLine($"{direction} bringt: {fieldsBelongToPlayer}");
                    if (fieldsBelongToPlayer > currentMax)
                    {
                        currentMax = fieldsBelongToPlayer;
                        bestDirection = direction;
                    }
                }

                Console.WriteLine(bestDirection.ToString()); // A single line with UP, DOWN, LEFT or RIGHT
                Console.Error.WriteLine("Round:" + Round);
                Round++;
            }
        }

        private static string ReadConsole()
        {
            string consoleLine = Console.ReadLine();
            Console.Error.WriteLine($"debug:^{consoleLine}debug:$");
            return consoleLine;
        }


        private void UpdateField(int x0, int y0, int x1, int y1, int playerNumber)
        {
            gameField.fields[y1, x1].Player = playerNumber;
            gameField.fields[y0, x0].Player = playerNumber;
        }

        private int SimulateMoveAndGetPlayerFieldCount(int currentX, int currentY, Direction direction)
        {
            var copy = gameField.Copy();
            int playerId = gameField.fields[currentY, currentX].Player.Value;
            var nextCoordinates = NextField(currentX, currentY, direction);
            if (nextCoordinatesDeath(nextCoordinates))
                return 0;
            var nextField = copy.fields[nextCoordinates.newY, nextCoordinates.newX];
            nextField.Player = playerId;
            var calc = new VoronoiCalculator(copy);
            return calc.FieldsBelongToPlayer(playerId);
        }

        private bool nextCoordinatesDeath((int newX, int newY) nextCoordinates)
        {
            bool outOfRange = nextCoordinates.newX < 0 || nextCoordinates.newX >= gameField.Width || nextCoordinates.newY < 0 || nextCoordinates.newY >= gameField.Height;
            if (outOfRange)
                return true;
            bool playerOnField = gameField.fields[nextCoordinates.newY, nextCoordinates.newX].Player != null;
            return playerOnField;

        }

        private (int newX, int newY) NextField(int currentX, int currrentY, Direction direction)
        {
            switch (direction)
            {
                case Direction.RIGHT:
                    return (currentX + 1, currrentY);
                case Direction.LEFT:
                    return (currentX - 1, currrentY);
                case Direction.UP:
                    return (currentX, currrentY - 1);
                case Direction.DOWN:
                    return (currentX, currrentY + 1);
            }
            throw new InvalidOperationException("Direction unbekannt");
        }

    }

    [DebuggerTypeProxy(typeof(GameFieldDebugProxy))]
    [Serializable]
    public class GameField
    {
        private const int WIDTH = 30;
        private const int HEIGHT = 20;

        public Field[,] fields;
        public int Width { get { return fields.GetLength(1); } }
        public int Height { get { return fields.GetLength(0); } }
        public GameField(int width = WIDTH, int height = HEIGHT)
        {
            fields = new Field[height, width];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    fields[y, x] = new Field(x, y);
                }
            }
        }

        public GameField Copy()
        {
            var copy = new GameField(this.Width, this.Height);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    copy.fields[y, x] = new Field(x, y);
                    copy.fields[y, x].Player = this.fields[y, x].Player;
                }
            }
            return copy;
        }
    }

    [Serializable]
    public class Field
    {
        public int? Player = null;
        public int X;
        public int Y;

        public Field(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "Player:" + Player;
        }
    }

    enum Direction
    {
        RIGHT, LEFT, UP, DOWN
    }

    [DebuggerDisplay("fields = {gameField}")]
    public class VoronoiCalculator
    {
        public GameField gameField;

        public VoronoiCalculator(GameField fields)
        {
            this.gameField = fields;
            CalcVoronoi();
        }

        private VoronoiCalculator CalcVoronoi()
        {
            var takenFields = TakenFields();
            foreach (var field in gameField.fields)
            {
                var minDistFieldsGroup = takenFields.GroupBy(takenField => ManhattanDistance(field, takenField)).OrderBy(group => group.Key).First();
                if (minDistFieldsGroup.Count() == 1)
                {
                    field.Player = minDistFieldsGroup.First().Player;
                }
                else
                {
                    field.Player = null;
                }
            }
            return this;
        }


        private static int ManhattanDistance(Field a, Field b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            return dx + dy;
        }
        private List<Field> TakenFields()
        {
            List<Field> result = new List<Field>();
            foreach (var field in gameField.fields)
            {
                if (field.Player != null)
                {
                    result.Add(field);
                }
            }
            return result;
        }

        public int FieldsBelongToPlayer(int player)
        {
            int result = 0;
            foreach (var field in gameField.fields)
            {
                if (field.Player == player)
                    result++;
            }
            return result;
        }
    }

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
                for (int y = 0; y < _gameFields.Height; y++)
                {
                    for (int x = 0; x < _gameFields.Width; x++)
                    {
                        result += printField(_gameFields.fields[y, x]);
                    }
                    result += "<br>";
                }

                return result;
            }
        }

        private string printField(Field field)
        {
            if (field.Player == null)
                return ColorString("0", Color.Black);
            else if (field.Player == 0)
                return ColorString((field.Player + 1).ToString(), Color.Green);
            else
                return ColorString((field.Player + 1).ToString(), Color.Red);
        }

        private string ColorString(string str, Color color)
        {
            return $"<span style = \"color: {ColorTranslator.ToHtml(color)};\">{str}</span>";
        }

    }

}