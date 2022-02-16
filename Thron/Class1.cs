using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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
        new ThereIsNoSpoon2(width, height, lines.ToArray());
    }

    public Field[,] fields;

    public ThereIsNoSpoon2(int width, int height, params string[] lines)
    {
        fields = new Field[width, height];

        for (int y = 0; y < height; y++)
        {
            string line = lines[y]; // width characters, each either a number or a '.'
            Console.Error.WriteLine(line);
            for (int x = 0; x < line.Length; x++)
            {
                fields[x, y] = new Field(line[x], x, y);
            }
        }

        SetNeighbors();

        // Two coordinates and one integer: a node, one of its neighbors, the number of links connecting them.
        Console.WriteLine("0 0 2 0 1");
    }

    public void SetNeighbors()
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
            if (fields[x, node.y].symbol != '.')
            {
                return fields[x, node.y];
            }
        }
        return null;
    }


    public Field getBottom(Field node)
    {
        for (int y = node.x + 1; y < fields.GetLength(1); y++)
        {
            if (fields[node.x, y].symbol != '.')
            {
                return fields[y, node.y];
            }
        }
        return null;
    }


    private List<Field> GetAllNodes()
    {
        List<Field> nodes = new List<Field>();
        foreach (var field in fields)
        {
            if (field.symbol != '.')
            {
                nodes.Add(field);
            }
        }
        return nodes;
    }
}

public class Field
{
    public char symbol;
    public int x;
    public int y;
    public List<Field> neighbors = new List<Field>();

    public Field(char symbol, int x, int y)
    {
        this.symbol = symbol;
        this.x = x;
        this.y = y;
    }
}