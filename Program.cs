using System;
using System.Collections.Generic;
using System.Linq;

static class Program
{
    static void Main()
    {
        var game = new Game(3);
        Display.DisplayGameTitle();
        game.Play();
    }
}

class Game
{
    public Board Board;
    public Game(int size)
    {
        Board = new Board(size);
    }
    public int MoveCount { get; set; } = 0;

    public bool IsADraw()
        => MoveCount == Board.Size * Board.Size;

    public Queue<Player> Players { get; set; } = new Queue<Player>(new List<Player> { new Player('Y'), new Player('X') });
    public Player GetCurrentTurnPlayer()
        => Players.Peek();
    public void ChangeTurn()
    {
        Players.Enqueue(Players.Dequeue());
        MoveCount++;
    }
    public bool IsWin { get; set; }
    public char WinPerson { get; set; }

    public void Play()
    {
        while (!IsWin)
        {
            if (IsADraw())
            {
                Board.DisplayLoss();
            }
            Console.Clear();
            Board.WriteBoard(3);
            ChangeTurn();
            Console.WriteLine();
            Console.WriteLine("What box do you want to place {0} in? (1-9)", GetCurrentTurnPlayer().Character);
            Console.Write("> ");
            int selTemp = int.Parse(Console.ReadLine());

            if (selTemp < 10 && selTemp > 0)
            {
                Board.MoveToField(selTemp, GetCurrentTurnPlayer(), 3);
            }
            else
            {
                Console.WriteLine("Wrong selection entered!");
                Console.WriteLine("Press any key to try again..");
                Console.ReadKey();
                Board.HasError = true;
            }
            if (Board.HasError)
            {
                CheckWin(); // if error, just check win
                Board.HasError = !Board.HasError;
            }
            else
            {
                CheckWin();
            }
        }
        Console.Clear();
        Board.WriteBoard(3);
        Console.WriteLine();
        Console.WriteLine("The winner is {0}!", WinPerson);
        Console.ReadKey();
    }
    private void MarkPlayerAsAWinner(char player)
    {
        IsWin = true;
        WinPerson = player;
    }

    public void CheckWin()
    {
        CheckWinForPlayer('X', 3);
        CheckWinForPlayer('Y', 3);
    }
    private void CheckWinForPlayer(char player, int size)
    {
        if (Board.IsPlayerAWinner(player, size))
        {
            MarkPlayerAsAWinner(player);
        }
    }
}

class Player
{
    public char Character { get; private set; }
    public Player(char character)
    {
        Character = character;
    }
}

class Board
{
    public char[,] Boxes { get; private set; }
    public int Size { get; private set; }
    public Board(int size)
    {
        Boxes = new char[size, size];
        InitializeData(size);
        Size = size;
    }

    private void InitializeData(int size)
    {
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                Boxes[i, j] = ' ';
            }
        }
    }

    public void WriteBoard(int size)
    {
        var table = new List<string>();
        for (var i = 0; i < size; i++)
        {
            var row = new List<string>();
            for (var j = 0; j < size; j++)
            {
                row.Add($" {Boxes[i, j]} ");
            }
            table.Add(string.Join('|', row));
        }
        var tableWith = table[0].Length - 2;
        var border = string.Join("", Enumerable.Repeat("-", tableWith));
        var tableAsString = string.Join($"\n {border} \n", table);
        Console.WriteLine(tableAsString);
    }



    private bool IsPlayerWinnerInAnyRow(char player, int size)
    {
        for (var i = 0; i < size; i++)
        {
            if (IsWinnerInRow(player, i, size))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsPlayerAWinner(char player, int size)
    {
        return IsPlayerWinnerInAnyColumn(player, size)
            || IsPlayerWinnerInAnyRow(player, size)
            || IsPlayerWinnerInLeftCrossing(player, size)
            || IsPlayerWinnerInRightCrossing(player, size);
    }

    private bool IsWinnerInRow(char player, int rowIndex, int numberOfColumns)
    {
        for (var i = 0; i < numberOfColumns; i++)
        {
            if (Boxes[rowIndex, i] != player)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPlayerWinnerInAnyColumn(char player, int size)
    {
        for (var i = 0; i < size; i++)
        {
            if (IsWinnerInColumn(player, i, size))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsWinnerInColumn(char player, int columnIndex, int numberOfRows)
    {
        for (var i = 0; i < numberOfRows; i++)
        {
            if (Boxes[i, columnIndex] != player)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPlayerWinnerInRightCrossing(char player, int size)
    {
        for (int i = 0; i < size; i++)
        {
            if (Boxes[i, i] != player)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPlayerWinnerInLeftCrossing(char player, int size)
    {
        for (var i = 0; i < size; i++)
        {
            if (Boxes[i, size - 1 - i] != player)
            {
                return false;
            }
        }
        return true;
    }

    public void NotVacantError()
    {
        HasError = true;
        Console.WriteLine();
        Console.WriteLine("Error: box not vacant!");
        Console.WriteLine("Press any key to try again..");
        Console.ReadKey();
        return;
    }

    public void DisplayLoss()
    {
        Console.WriteLine();
        Console.WriteLine("No one won.");
        Console.ReadKey();
        Environment.Exit(1);
    }

    public bool HasError = false;

    public void MoveToField(int fieldNumber, Player player, int size)
    {
        fieldNumber--;
        var row = fieldNumber / size;
        var column = fieldNumber % size;
        if (Boxes[row, column] == ' ')
        {
            Boxes[row, column] = player.Character;
        }
        else
        {
            NotVacantError();
        }
    }
}

public static class Display
{
    public static void DisplayGameTitle()
    {
        Console.WriteLine(" -- Tic Tac Toe -- ");
    }
}