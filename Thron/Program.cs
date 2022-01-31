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
                Stopwatch stopwatch = Stopwatch.StartNew();
                inputs = ReadConsole().Split(' ');
                int N = int.Parse(inputs[0]); // total number of players (2 to 4).
                int P = int.Parse(inputs[1]); // your player number (0 to 3).
                gameField.PlayerId = P;
                gameField.AllHeads = new Dictionary<int, Field>();
                for (int i = 0; i < N; i++)
                {
                    inputs = ReadConsole().Split(' ');
                    int X0 = int.Parse(inputs[0]); // starting X coordinate of lightcycle (or -1)
                    int Y0 = int.Parse(inputs[1]); // starting Y coordinate of lightcycle (or -1)
                    int X1 = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
                    int Y1 = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)

                    UpdateField(X0, Y0, X1, Y1, i);
                    gameField.AllHeads[i] = gameField.fields[Y1, X1];
                }
                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");

                var bestDirection = gameField.PossibleNeighborsPerDirectionOf(gameField.PlayerHead())
                    .Select(next =>
                    {
                        var copy = gameField.Copy();
                        copy.fields[next.nextField.Y,next.nextField.X].Player=gameField.PlayerId;
                        copy.SetPlayerHeadTo(next.nextField.X, next.nextField.Y);
                        var calc = new VoronoiCalculator(copy);

                        var result = new { Direction = next.direction, playerFields = calc.FieldsBelongToPlayer(gameField.PlayerId), enemyFields = calc.FieldsBelongToNotPlayer(gameField.PlayerId) };
                        Console.Error.WriteLine($"{result.Direction} bringt: {result.playerFields} gegner:{result.enemyFields}");
                        return result;
                    })
                    .OrderByDescending(calcResult => calcResult.playerFields)
                    .ThenBy(calcResult => calcResult.enemyFields).First().Direction;

                Console.WriteLine(bestDirection.ToString()); // A single line with UP, DOWN, LEFT or RIGHT
                stopwatch.Stop();
                Console.Error.WriteLine($"gebraucht: {stopwatch.ElapsedMilliseconds}");
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

        public static (int newX, int newY) Coordinates(Field start, Direction direction)
        {
            int currentX = start.X;
            int currentY = start.Y;
            switch (direction)
            {
                case Direction.RIGHT:
                    return (currentX + 1, currentY);
                case Direction.LEFT:
                    return (currentX - 1, currentY);
                case Direction.UP:
                    return (currentX, currentY - 1);
                case Direction.DOWN:
                    return (currentX, currentY + 1);
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

        public Dictionary<int, Field> AllHeads = new Dictionary<int, Field>();
        public int PlayerId;
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
            copy.AllHeads = this.AllHeads;
            copy.PlayerId = PlayerId;
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
            return PossibleNeighborsPerDirectionOf(start).Select(neighborDirection => neighborDirection.nextField).ToList();
        }

        public List<(Direction direction, Field nextField)> PossibleNeighborsPerDirectionOf(Field start)
        {
            return Thron.AllDirections()
                .Select(dir => new { Direction = dir, Coordinates = Thron.Coordinates(start, dir) })
                .Where(nextCoordinates => !NextCoordinatesDeath(nextCoordinates.Coordinates))
                 .Select(coordinates => (direction: coordinates.Direction, nextField: fields[coordinates.Coordinates.newY, coordinates.Coordinates.newX]))
                 .ToList();
        }

        public Field PlayerHead()
        {
            return AllHeads[PlayerId];
        }

        internal void SetPlayerHeadTo(int x, int y)
        {
            AllHeads[PlayerId] = fields[y, x];
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

        public override bool Equals(object obj)
        {
            return obj is Field field &&
                   X == field.X &&
                   Y == field.Y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Y},{X} = {Player}";
        }


    }

    public enum Direction
    {
        RIGHT, LEFT, UP, DOWN
    }

    [DebuggerDisplay("fields = {gameField}")]
    public class VoronoiCalculator
    {
        public GameField gameField;
        Dictionary<Field, int[,]> distsPerPlayer = new Dictionary<Field, int[,]>();

        public VoronoiCalculator(GameField fields)
        {
            this.gameField = fields;
            foreach (var playerHead in gameField.AllHeads.Values)
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
                var minDistPlayerHeadGroup = gameField.AllHeads.Values.GroupBy(playerHead => MazeDistance(playerHead, field)).OrderBy(group => group.Key).First();

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

            string result;
            if (field.Player == null)
                result = ColorString("0", Color.Black);
            else if (field.Player == 0)
                result = ColorString((field.Player + 1).ToString(), Color.Green);
            else
                result = ColorString((field.Player + 1).ToString(), Color.Red);

            if (FieldIsHead(field))
                result = $"<b>{result}</b>";
            return result;
        }

        private bool FieldIsHead(Field field)
        {
            if (field.Player != null && _gameFields.AllHeads.ContainsKey(field.Player.Value))
                return _gameFields.AllHeads[field.Player.Value].Equals(field);
            return false;
        }

        private string ColorString(string str, Color color)
        {
            return $"<span style = \"color: {ColorTranslator.ToHtml(color)};\">{str}</span>";
        }

    }
}