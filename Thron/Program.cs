using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


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
                    int fieldsBelongToPlayer = SimulateMoveAndGetPlayerFieldCount(currentX, currentY, Direction.DOWN);
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
            gameField.fields[x1, y1].Player = playerNumber;
        }

        private int SimulateMoveAndGetPlayerFieldCount(int currentX, int currentY, Direction direction)
        {
            var gamefieldCopy = gameField.Copy();
            int playerId = gameField.fields[currentX, currentY].Player.Value;

            var nextCoordinates = NextField(currentX, currentY, direction);
            var nextField = gamefieldCopy.fields[nextCoordinates.newX, nextCoordinates.newY];
            if (nextField.Player != null)
                return 0;
            nextField.Player = playerId;
            var calc = new VoronoiCalculator(gamefieldCopy);
            return calc.FieldsBelongToPlayer(playerId);
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
        public static int WIDTH = 30;
        public static int HEIGHT = 20;

        public Field[,] fields = new Field[WIDTH, HEIGHT];
        public GameField()
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    fields[x, y] = new Field(x, y);
                }
            }
        }

        public GameField Copy()
        {
            var copy = new GameField();
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    copy.fields[x, y] = new Field(x, y);
                    copy.fields[x, y].Player = this.fields[x, y].Player;
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
            foreach (var field in gameField.fields)
            {
                Console.Error.WriteLine($"BErechne: {field.X} {field.Y}");
                Field minDistField = null;
                foreach (var takenField in TakenFields())
                {
                    if (minDistField == null || ManhattanDistance(field, takenField) < ManhattanDistance(field, minDistField))
                    {
                        minDistField = takenField;
                    }

                    else if (minDistField != null && ManhattanDistance(field, takenField) == ManhattanDistance(field, minDistField))
                    {
                        // bei selber distanz, bekommt das noch keiner
                        minDistField = null;
                        break;
                    }
                }
                if (minDistField == null)
                    field.Player = null;
                else
                    field.Player = minDistField.Player;
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

}