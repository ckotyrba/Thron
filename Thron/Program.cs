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
        private List<Field> currentPlayerHeads;

        public void Play()
        {
            string[] inputs;

            // game loop
            while (true)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                currentPlayerHeads = new List<Field>();
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
                    currentPlayerHeads.Add(gameField.fields[Y1, X1]);
                }
                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");

                while (stopwatch.ElapsedMilliseconds < 100) { 
                var bestDirection = AllDirections()
                    .Select(direction =>
                    {
                        var result = new { Direction = direction, playerFields = 0, enemyFields = int.MaxValue };
                        try
                        {
                            var simulatedGamefield = SimulateMove(currentX, currentY, direction);
                            var calc = new VoronoiCalculator(simulatedGamefield, currentPlayerHeads);
                            result = new { Direction = direction, playerFields = calc.FieldsBelongToPlayer(P), enemyFields = calc.FieldsBelongToNotPlayer(P) };

                            return result;
                        }
                        catch (InvalidOperationException e)
                        {
                            result = new { Direction = direction, playerFields = 0, enemyFields = int.MaxValue };
                        }

                        Console.Error.WriteLine($"{direction} bringt: {result.playerFields} gegner:{result.enemyFields}");
                        return result;

                    })
                    .OrderByDescending(calcResult => calcResult.playerFields)
                    .ThenBy(calcResult => calcResult.enemyFields).First().Direction;

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

        private GameField SimulateMove(int currentX, int currentY, Direction direction)
        {
            var copy = gameField.Copy();
            int playerId = gameField.fields[currentY, currentX].Player.Value;
            var nextCoordinates = NextField(currentX, currentY, direction);
            if (gameField.NextCoordinatesDeath(nextCoordinates))
                throw new InvalidOperationException("Move Lead to death");
            var nextField = copy.fields[nextCoordinates.newY, nextCoordinates.newX];
            nextField.Player = playerId;
            return copy;
        }

        public static (int newX, int newY) NextField(int currentX, int currrentY, Direction direction)
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

        public static List<Direction> AllDirections()
        {
            return Enum.GetValues(typeof(Direction)).OfType<Direction>().ToList();
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
                    copy.fields[y, x].Player = this.fields[y, x].Player + 0;
                }
            }
            return copy;
        }

        public bool NextCoordinatesDeath((int newX, int newY) nextCoordinates)
        {
            bool outOfRange = nextCoordinates.newX < 0 || nextCoordinates.newX >= Width || nextCoordinates.newY < 0 || nextCoordinates.newY >= Height;
            if (outOfRange)
                return true;
            bool playerOnField = fields[nextCoordinates.newY, nextCoordinates.newX].Player != null;
            return playerOnField;
        }

        public List<Field> PossibleNeighborsOf(Field start)
        {
            return Thron.AllDirections()
                .Select(dir => Thron.NextField(start.X, start.Y, dir))
                .Where(nextCoordinates => !NextCoordinatesDeath(nextCoordinates))
                 .Select(coordinates => fields[coordinates.newY, coordinates.newX])
                 .ToList();
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
            return $"{Y},{X} = {Player}";
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
        private List<Field> currentPlayerHeads;
        Dictionary<Field, int[,]> distsPerPlayer = new Dictionary<Field, int[,]>();

        public VoronoiCalculator(GameField fields, List<Field> currentPlayerHeads)
        {
            this.gameField = fields;
            this.currentPlayerHeads = currentPlayerHeads;
            foreach (var playerHead in currentPlayerHeads)
            {
                distsPerPlayer.Add(playerHead, DistToAllFields(playerHead));
            }
            CalcVoronoi();
        }

        private VoronoiCalculator CalcVoronoi()
        {

            foreach (var field in gameField.fields)
            {
                if (field.Player != null)
                    continue;
                var minDistPlayerHeadGroup = currentPlayerHeads.GroupBy(playerHead => MazeDistance(playerHead, field)).OrderBy(group => group.Key).First();

                if (minDistPlayerHeadGroup.Count() == 1)
                {
                    field.Player = minDistPlayerHeadGroup.First().Player;
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

        private int MazeDistance(Field playerHead, Field target)
        {
            return distsPerPlayer[playerHead][target.Y, target.X];
        }

        private int[,] DistToAllFields(Field start)
        {
            int[,] dists = new int[gameField.Height, gameField.Width];
            for (int y = 0; y < dists.GetLength(0); y++)
            {
                for (int x = 0; x < dists.GetLength(1); x++)
                {
                    dists[y, x] = int.MaxValue;
                }
            }
            bool[,] alreadyVisited = new bool[gameField.Height, gameField.Width];
            Queue<Field> nextToLookAt = new Queue<Field>();
            nextToLookAt.Enqueue(start);
            dists[start.Y, start.X] = 0;
            while (nextToLookAt.Count > 0)
            {
                var currentField = nextToLookAt.Dequeue();
                int currentDist = dists[currentField.Y, currentField.X];
                foreach (var neighbor in gameField.PossibleNeighborsOf(currentField))
                {
                    if (!alreadyVisited[neighbor.Y, neighbor.X])
                    {
                        nextToLookAt.Enqueue(neighbor);
                        dists[neighbor.Y, neighbor.X] = currentDist + 1;
                        alreadyVisited[neighbor.Y, neighbor.X] = true;
                    }
                }
            }
            return dists;
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

        public int FieldsBelongToNotPlayer(int player)
        {
            int result = 0;
            foreach (var field in gameField.fields)
            {
                if (field.Player != null && field.Player != player)
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