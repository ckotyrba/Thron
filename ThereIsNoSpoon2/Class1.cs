using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
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
        //Console.Error.WriteLine($"Schaue {startNode}");
        if (startNode == null)
        {
            var obvious = takeObvious();
            if (obvious.Count > 0)
            {
                var saveLink = obvious[0];
                startNode = saveLink.b;
                lines.AddRange(obvious);
            }
            else
            {
                startNode = GetAllNodes()[0];
            }
        }
        var connectionsTriedInThisRun = startNode.PossibleConnections(lines);
        foreach (var connection in connectionsTriedInThisRun)
        {
            lines.Add(connection);
            Console.Error.WriteLine($"füge {connection}");
            // wir können knoten nicht mehr voll machen => tot
            if (startNode.FreeSlots(lines) > startNode.PossibleConnections(lines).Count)
                return false;
            if (DoNextMove(ref lines, connection.b))
                return true;
            lines.Remove(connection);
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

    public List<Line> takeObvious()
    {
        List<Line> result = new List<Line>();
        int startSize;
        do
        {
            startSize = result.Count;
            foreach (var node in GetAllNodes())
            {
                if (node.neighbors.Count == 1)
                {
                    var line = new Line(node, node.neighbors[0]);
                    if (node.symbol > 2)
                        throw new InvalidDataException("Darf eigentlich nicht setzen" + node);
                    if (!result.Contains(line))
                        for (int i = 0; i < node.symbol; i++)
                        {
                            result.Add(line);
                        }
                }

                // WENN mögliche linien == symbol, nehme alle
                var possible = node.PossibleConnections(result);
                if (possible.Count == node.FreeSlots(result))
                {
                    result.AddRange(NeededLinesToAdd(result, possible));
                }
            }
        } while (result.Count - startSize > 0);
        return result;
    }

    private static List<Line> NeededLinesToAdd(List<Line> alreadyTaken, List<Line> toAdd)
    {
        List<Line> result = new List<Line>();
        foreach (var targetLine in toAdd)
        {
            int neededLinksInTarget = toAdd.Where(line => line.Equals(targetLine)).Count();
            int alreadyTakenCount = alreadyTaken.Where(line => line.Equals(targetLine)).Count();
            int inResult = result.Where(line => line.Equals(targetLine)).Count();
            if (neededLinksInTarget > alreadyTakenCount + inResult)
            {
                result.Add(targetLine);
            }
        }
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
            int possibleLinks = node.PossibleConnections(lines).Count;
            int connections = node.linesToNode(lines);
            if (node.symbol - connections > possibleLinks)
                return false;

        }
        // keine linien schneiden sich
        foreach (var line in lines)
        {
            if (lines.Any(li => li.Intersect(line)))
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
        var pickedLines = new List<Line>(lines);
        if (linesToNode(lines) >= symbol)
            return result;
        foreach (var neighbor in neighbors)
        {
        elementCheck:
            if (neighbor.symbol == symbol && symbol == 1)
            {
                continue;
            }
            var connection = new Line(this, neighbor);

            int alreadyPickedConnections = pickedLines.Where(line => line.Equals(connection)).Count();
            if (alreadyPickedConnections < 2)
            {
                if (neighbor.FreeSlots(pickedLines) > 0)
                {
                    if (!connection.IntersectAny(lines))
                    {
                        result.Add(connection);
                        pickedLines.Add(connection);
                        if (symbol > 1)
                            goto elementCheck;
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
        return lines.Where(line => line.a.Equals(this) || line.b.Equals(this)).Count();
    }

}

public class Line : IEquatable<Line>
{
    public Field a;
    public Field b;

    public Line(Field a, Field b)
    {
        this.a = a;
        this.b = b;
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
        return $"{a.x} {a.y} {b.x} {b.y} 1";
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
        return $"{minField}=>{maxField}";
    }

    public bool Equals(Line other)
    {
        return a.Equals(other.a) && b.Equals(other.b) ||
                     a.Equals(other.b) && b.Equals(other.a);
    }
}