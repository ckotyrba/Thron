using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

/**
 * The machines are gaining ground. Time to show them what we're really made of...
 **/
public class ThereIsNoSpoon2
{
    static void Main(string[] args)
    {
        int width = int.Parse(Console.ReadLine()); // the number of cells on the X axis
        int height = int.Parse(Console.ReadLine()); // the number of cells on the Y axis

        List<string> lines = new List<string>();
        for (int y = 0; y < height; y++)
        {
            string line = Console.ReadLine(); // width characters, each either a number or a '.'
            lines.Add(line);
        }
        var game = new ThereIsNoSpoon2(lines.ToArray());


        List<Line> connections = new List<Line>();
        if (!game.DoNextMove(ref connections))
            throw new InvalidProgramException("No solution found");

        foreach (var connection in connections)
        {
            Console.WriteLine(connection.ToOutput());
        }
    }

    public Field[,] fields;

    public ThereIsNoSpoon2(params string[] gameField)
    {
        fields = new Field[gameField[0].Length, gameField.Length];

        for (int y = 0; y < gameField.Length; y++)
        {
            string line = gameField[y]; // width characters, each either a number or a '.'
            Console.Error.WriteLine(line);
            for (int x = 0; x < line.Length; x++)
            {
                fields[x, y] = new Field(line[x], x, y);
            }
        }

        RecalcNeighbors();
    }

    private int lastPrintedIndex = -1;

    public bool DoNextMove(ref List<Line> lines, Field startNode = null)
    {
        var obvious = takeObvious(lines);
        // lines.AddRange(obvious);
        obvious.ForEach(line => Console.WriteLine(line.ToOutput()));
        return true;
        Console.Error.WriteLine($"nehme offensichtlich: {string.Join(';', obvious)}");
        //Console.Error.WriteLine($"Schaue {startNode}");
        if (startNode == null)
        {
            if (obvious.Count > 0)
            {
                var saveLink = obvious[0];
                startNode = saveLink.b;
            }
            else
            {
                startNode = GetAllNodes()[0];
            }
        }
        var connectionsTriedInThisRun = startNode.PossibleConnections(lines);
        foreach (var connection in connectionsTriedInThisRun)
        {
            List<Line> saveLines = new List<Line>(lines);
            lines.Add(connection);
            Console.Error.WriteLine($"füge {connection}");
            if (SoluationValid(lines))
            {
                //         Console.WriteLine(connection.ToOutput());
                if (DoNextMove(ref lines, connection.b))
                    return true;
            }
            lines = saveLines;
            Console.Error.WriteLine($"entferne {connection}");
        }

        var linesAlreadyTaken = new List<Line>(lines);
        Field lastNodeWithPossilities = LinesToNodes(lines)
            .LastOrDefault(node => node.PossibleConnections(linesAlreadyTaken).Where(line => !connectionsTriedInThisRun.Contains(line)).Any());

        // keine züge mehr möglich und keine freien züge in aufrufkette, dann müssten wir gewonnen haben. sonst gehe solange zurück, biss alternativer zug noch möglich gewesen wäre
        if (lastNodeWithPossilities == null)
        {
            return SolutionWins(lines);
        }

        // freie züge noch möglich, springe zu freien knoten
        return DoNextMove(ref lines, lastNodeWithPossilities);
    }

    private List<Field> LinesToNodes(List<Line> lines)
    {
        List<Field> result = new List<Field>();
        foreach (var line in lines)
        {
            result.Add(line.a);
            result.Add(line.b);
        }
        return result;
    }

    public List<Line> takeObvious(List<Line> alreadyTaken = null)
    {
        if (alreadyTaken == null)
            alreadyTaken = new List<Line>();
        List<Line> result = new List<Line>(alreadyTaken);
        int startSize;
        do
        {
            startSize = result.Count;
            foreach (var node in GetAllNodes())
            {

                if (node.FreeSlots(result) == 0)
                    continue;

                result.AddRange(TakeSaveLines(node, result));
                result.AddRange(TakeNeighbor2Solution(node, result));
                result.AddRange(TakeRemainingConnectionSameAsNeighborsSum(node, result));

                var possibleNeighbors = node.neighbors.Where(neighbor => neighbor.FreeSlots(result) > 0).ToList();
                if (node.symbol == 1)
                {
                    possibleNeighbors = possibleNeighbors.Where(neighbor => neighbor.symbol != 1).ToList();
                }
                if (possibleNeighbors.Count() == 1)
                {
                    var neighbor = possibleNeighbors.First();
                    int freeSlots = node.FreeSlots(result);
                    // verbinde 2er nicht mit 2, mit 2er nachbar
                    if (node.symbol == 2 && neighbor.symbol == 2)
                    {
                        freeSlots--;
                    }
                    if (freeSlots > 0)
                    {
                        var line = new Line(node, neighbor, freeSlots);
                        result.Add(line);
                    }
                }
                else
                {
                    // WENN mögliche linien == symbol, nehme alle
                    int freeSlots = node.FreeSlots(result);
                    int neighborFreePossibleSlots = possibleNeighbors.Sum(neighbor => Math.Min(2, neighbor.FreeSlots(result)));
                    if (freeSlots == neighborFreePossibleSlots)
                    {
                        foreach (var neighbor in possibleNeighbors)
                        {
                            result.Add(new Line(node, neighbor, Math.Min(2, neighbor.FreeSlots(result))));
                        }
                    }
                }


            }

        } while (result.Count - startSize > 0);
        var temp = new List<Line>(result);
        result.RemoveAll(line => alreadyTaken.Contains(line));
        return result;
    }

    private IEnumerable<Line> TakeRemainingConnectionSameAsNeighborsSum(Field node, List<Line> result)
    {
        int possibleConnections = node.neighbors.Sum(neighbor => 2 - ConnectionsAlreadyTaken(result, node, neighbor));
        int remainingConnections = node.FreeSlots(result);
        if (possibleConnections == remainingConnections)
        {
            foreach (var neighbor in node.neighbors.Where(neighbor => 2 - ConnectionsAlreadyTaken(result, node, neighbor) > 0))
            {
                yield return new Line(node, neighbor, 2 - ConnectionsAlreadyTaken(result, node, neighbor));
            }
        }
        yield break;
    }

    public IEnumerable<Line> TakeNeighbor2Solution(Field node, List<Line> alreadyTaken)
    {
        if (node.symbol / 2.0 == node.neighbors.Count)
        {
            foreach (var neighbor in node.neighbors)
            {
                int missingConnections = 2 - ConnectionsAlreadyTaken(alreadyTaken, node, neighbor);
                int times = Math.Min(missingConnections, neighbor.FreeSlots(alreadyTaken));
                if (times > 0)
                {
                    var connection = new Line(node, neighbor, times);
                    yield return connection;
                }
            }
        }
        yield break;
    }

    public List<Line> TakeSaveLines(Field node, List<Line> alreadyTaken)
    {
        var result = new List<Line>();

        var alreadyTakenWithoutOwn = alreadyTaken.Where(line => !line.a.Equals(node) && !line.b.Equals(node)).ToList();

        //verteile maximal möglich an nachbar.falls alle nachbarn belegt sind => nehme alle
        int connextionsLeft = node.FreeSlots(alreadyTakenWithoutOwn);
        var orderedNeighbors = node.neighbors.Where(neighbor => neighbor.FreeSlots(alreadyTakenWithoutOwn) > 0).OrderByDescending(neighbor => neighbor.symbol).ToList();
        for (int i = 0; i < orderedNeighbors.Count; i++)
        {
            var currentNeighbor = orderedNeighbors[i];
            //sonderfall beachten 2er dürfen 2er nur mit 1 besuchen
            if (node.symbol == 2 && currentNeighbor.symbol == 2)
                connextionsLeft -= 1;
            else
                connextionsLeft -= Math.Min(2, currentNeighbor.FreeSlots(alreadyTakenWithoutOwn));
            if (connextionsLeft <= 0)
            {
                if (i == node.neighbors.Count - 1)
                {
                    foreach (var neighbor in node.neighbors)
                    {
                        //  was ist hier mit duplikaten? wie erkennen, dass
                        var connection = new Line(node, neighbor, 1);
                        if (ConnectionsAlreadyTaken(alreadyTaken, node, neighbor) < 1)
                            result.Add(new Line(node, neighbor, 1));
                    }
                }
                break;
            }
        }

        Console.Error.WriteLine($"schaue {node} und nehme {string.Join(',', result)}");

        return result;
    }


    public bool SolutionWins(List<Line> lines)
    {
        bool allNodesConnected = GetAllNodes().All(node => node.linesToNode(lines) == node.symbol);
        return allNodesConnected && SoluationValid(lines);
    }

    private bool SoluationValid(List<Line> lines)
    {

        foreach (var node in GetAllNodes())
        {
            // alle haben nachbarn
            if (node.linesToNode(lines) > node.symbol)
                return false;

            // jeder knoten kann noch ziel erreichen
            int neighborFreeSlots = node.neighbors.Sum(neighbor => neighbor.FreeSlots(lines));
            if (node.FreeSlots(lines) > neighborFreeSlots)
                return false;
        }

        // keine linien schneiden sich
        foreach (var line in lines)
        {
            if (lines.Any(li => li.Intersect(line)))
                return false;
            if (lines.Where(li => li.EqualsIgnoreTimes(line)).Sum(li => li.times) > 2)
                return false;
        }

        return true;
    }

    public void RecalcNeighbors()
    {
        GetAllNodes().ForEach(node => node.neighbors.Clear());
        foreach (var node in GetAllNodes())
        {
            Field right = getRight(node);
            if (right != null)
            {
                node.neighbors.Add(right);
                right.neighbors.Add(node);
            }

            Field bottom = getBottom(node);
            if (bottom != null)
            {
                node.neighbors.Add(bottom);
                bottom.neighbors.Add(node);
            }
        }
    }

    public Field getRight(Field node)
    {
        for (int x = node.x + 1; x < fields.GetLength(0); x++)
        {
            var currentNode = fields[x, node.y];
            if (currentNode.IsNode())
            {
                return currentNode;
            }
        }
        return null;
    }


    public Field getBottom(Field node)
    {
        for (int y = node.y + 1; y < fields.GetLength(1); y++)
        {
            var currentNode = fields[node.x, y];
            if (currentNode.IsNode())
            {
                return currentNode;
            }
        }
        return null;
    }

    private static List<Field> allNodes;

    private List<Field> GetAllNodes()
    {
        if (true || allNodes == null)
        {
            List<Field> nodes = new List<Field>();
            foreach (var field in fields)
            {
                if (field.IsNode())
                {
                    nodes.Add(field);
                }
            }
            allNodes = nodes;
        }
        return allNodes;
    }

    private static int ConnectionsAlreadyTaken(List<Line> connections, Field a, Field b)
    {
        return connections.Where(line => line.EqualsIgnoreTimes(new Line(a, b, 1))).Sum(line => line.times);
    }
}

public class Field
{
    public int symbol;
    public int x;
    public int y;
    public List<Field> neighbors = new List<Field>();

    public Field(char symbol, int x, int y)
    {
        if (symbol == '.')
            this.symbol = -1;
        else
            this.symbol = int.Parse(symbol.ToString());

        this.x = x;
        this.y = y;
    }

    public override bool Equals(object obj)
    {
        return obj is Field field &&
               x == field.x &&
               y == field.y;
    }

    public override string ToString()
    {
        return $"{x}/{y}";
    }

    public bool IsNode()
    {
        return symbol != -1;
    }

    public List<Line> PossibleConnections(List<Line> lines)
    {
        var result = new List<Line>();
        if (linesToNode(lines) >= symbol)
            return result;
        foreach (var neighbor in neighbors)
        {
            if (neighbor.FreeSlots(lines) > 0)
            {
                if (this.symbol == 1 && neighbor.symbol == 1)
                    continue;

                var connection = new Line(this, neighbor, 1);
                int connectionsAlreadyInUse = lines.Where(line => line.EqualsIgnoreTimes(connection)).Sum(line => line.times);

                if (!connection.IntersectAny(lines))
                {
                    if (this.symbol == 2 && neighbor.symbol == 2 && connectionsAlreadyInUse > 0)
                    {
                        continue;
                    }
                    result.Add(connection);
                    if (this.symbol == 2 && neighbor.symbol == 2)
                        continue;
                    if (this.FreeSlots(lines) > 1 && neighbor.FreeSlots(lines) > 1)
                    {
                        if (connectionsAlreadyInUse == 0)
                            result.Add(new Line(this, neighbor, 2));
                    }
                }
            }
        }
        return result;
    }


    public int FreeSlots(List<Line> lines)
    {
        return symbol - linesToNode(lines);
    }

    public int linesToNode(List<Line> lines)
    {
        return lines.Where(line => line.a.Equals(this) || line.b.Equals(this)).Select(line => line.times).Sum();
    }

}

public class Line : IEquatable<Line>
{
    public Field a;
    public Field b;

    public int times;

    public Line(Field a, Field b, int times = 1)
    {
        this.a = a;
        this.b = b;
        this.times = times;
        if (times == 0)
            throw new InvalidDataException("0 nicht erlaubt");
    }

    public bool NodeBetweenLine(Field node)
    {
        if (!node.Equals(a) && !node.Equals(b))
        {
            if (a.x == b.x && node.x == a.x)
            {
                return node.y > Math.Min(a.y, b.y) && node.y < Math.Max(a.y, b.y);
            }
            else if (a.y == b.y && node.y == a.y)
            {
                return node.x > Math.Min(a.x, b.x) && node.x < Math.Max(a.x, b.x);
            }
        }
        return false;
    }

    public bool IntersectAny(List<Line> others)
    {
        return others.Any(line => line.Intersect(this));
    }


    public bool Intersect(Line other)
    {
        if (other.Equals(this))
            return false;
        foreach (var node in NodesOnLine())
        {
            if (other.NodeBetweenLine(node))
                return true;
        }
        return false;
    }

    private List<Field> NodesOnLine()
    {
        List<Field> result = new List<Field>();
        Vector2 start = new Vector2(a.x, a.y);
        Vector2 end = new Vector2(b.x, b.y);

        Vector2 direction = Vector2.Normalize(end - start);
        int length = (int)(start - end).Length();
        for (int i = 1; i < length; i++)
        {
            Vector2 step = start + direction * i;
            result.Add(new Field('.', (int)step.X, (int)step.Y));
        }
        return result;
    }

    internal string ToOutput()
    {
        return $"{a.x} {a.y} {b.x} {b.y} {times}";
    }

    private Field MinField()
    {
        if (new Vector2(a.x, a.y).Length() < new Vector2(b.x, b.y).Length())
        {
            return a;
        }
        return b;
    }

    public override string ToString()
    {
        var minField = MinField();
        var maxField = a.Equals(minField) ? b : a;
        return $"{minField}=>{maxField}:{times}";
    }

    public bool Equals(Line other)
    {
        return EqualsIgnoreTimes(other) && times == other.times;
    }


    public bool EqualsIgnoreTimes(Line other)
    {
        return (a.Equals(other.a) && b.Equals(other.b)) || (a.Equals(other.b) && b.Equals(other.a));
    }
}