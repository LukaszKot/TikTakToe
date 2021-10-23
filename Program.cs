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

public class Game
{
    public Board Board;
    public Player WinPerson { get; private set; }
    public int MoveCount { get; private set; } = 0;
    public Queue<Player> Players { get; private set; }
        = new Queue<Player>(new List<Player> { new Player('X'), new Player('Y') });
    public Game(int size)
    {
        Board = new Board(size);
    }

    public bool IsADraw()
        => MoveCount == Board.Size * Board.Size;
    public Player GetCurrentTurnPlayer()
        => Players.Peek();
    public void ChangeTurn()
    {
        Players.Enqueue(Players.Dequeue());
        MoveCount++;
    }

    public void Play()
    {
        while (WinPerson == null)
        {
            if (IsADraw())
            {
                Display.DisplayLoss();
            }
            Display.WriteBoard(Board);

            var selTemp = Display.AskForFieldToPlace(GetCurrentTurnPlayer(), 1, Board.Size * Board.Size);
            if (Board.IsProvidedFieldNumberVaild(selTemp))
            {
                if (Board.MoveToField(selTemp, GetCurrentTurnPlayer()))
                    ChangeTurn();
            }
            else
            {
                Display.InvalidKeyMessage();
            }
            CheckWin();
        }
        Display.WriteBoard(Board);
        Display.DisplayWinner(WinPerson);
    }

    public void CheckWin()
    {
        foreach (var player in Players)
        {
            CheckWinForPlayer(player, Board.Size);
        }
    }
    private void MarkPlayerAsAWinner(Player player)
    {
        WinPerson = player;
    }
    private void CheckWinForPlayer(Player player, int size)
    {
        if (Board.IsPlayerAWinner(player))
        {
            MarkPlayerAsAWinner(player);
        }
    }
}

public class Player
{
    public char Character { get; private set; }
    public Player(char character)
    {
        Character = character;
    }
}

public class Board
{
    public char[,] Boxes { get; private set; }
    public int Size { get; private set; }
    public Board(int size)
    {
        Boxes = new char[size, size];
        InitializeData(size);
        Size = size;
    }
    public bool IsProvidedFieldNumberVaild(int number)
        => number > 0 && number < Size * Size + 1;

    public bool IsPlayerAWinner(Player player)
    {
        return IsPlayerWinnerInAnyColumn(player)
            || IsPlayerWinnerInAnyRow(player)
            || IsPlayerWinnerInLeftCrossing(player)
            || IsPlayerWinnerInRightCrossing(player);
    }

    public bool MoveToField(int fieldNumber, Player player)
    {
        fieldNumber--;
        var row = fieldNumber / Size;
        var column = fieldNumber % Size;
        if (Boxes[row, column] == ' ')
        {
            Boxes[row, column] = player.Character;
            return true;
        }
        Display.NotVacantError();
        return false;
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

    private bool IsPlayerWinnerInAnyRow(Player player)
    {
        for (var i = 0; i < Size; i++)
        {
            if (IsWinnerInRow(player, i, Size))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsWinnerInRow(Player player, int rowIndex, int numberOfColumns)
    {
        for (var i = 0; i < numberOfColumns; i++)
        {
            if (Boxes[rowIndex, i] != player.Character)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPlayerWinnerInAnyColumn(Player player)
    {
        for (var i = 0; i < Size; i++)
        {
            if (IsWinnerInColumn(player, i, Size))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsWinnerInColumn(Player player, int columnIndex, int numberOfRows)
    {
        for (var i = 0; i < numberOfRows; i++)
        {
            if (Boxes[i, columnIndex] != player.Character)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPlayerWinnerInRightCrossing(Player player)
    {
        for (int i = 0; i < Size; i++)
        {
            if (Boxes[i, i] != player.Character)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPlayerWinnerInLeftCrossing(Player player)
    {
        for (var i = 0; i < Size; i++)
        {
            if (Boxes[i, Size - 1 - i] != player.Character)
            {
                return false;
            }
        }
        return true;
    }
}

public static class Display
{
    public static void DisplayGameTitle()
    {
        Console.WriteLine(" -- Tic Tac Toe -- ");
    }

    public static void DisplayLoss()
    {
        Console.WriteLine();
        Console.WriteLine("No one won.");
        Console.ReadKey();
        Environment.Exit(1);
    }

    public static void WriteBoard(Board board)
    {
        Console.Clear();
        var table = new List<string>();
        for (var i = 0; i < board.Size; i++)
        {
            var row = new List<string>();
            for (var j = 0; j < board.Size; j++)
            {
                row.Add($" {board.Boxes[i, j]} ");
            }
            table.Add(string.Join('|', row));
        }
        var tableWith = table[0].Length - 2;
        var border = string.Join("", Enumerable.Repeat("-", tableWith));
        var tableAsString = string.Join($"\n {border} \n", table);
        Console.WriteLine(tableAsString);
    }

    public static int AskForFieldToPlace(Player player, int minNumber, int maxNumber)
    {
        Console.WriteLine();
        Console.WriteLine($"What box do you want to place {player.Character} in? ({minNumber}-{maxNumber})");
        Console.Write("> ");
        return int.Parse(Console.ReadLine());
    }
    public static void NotVacantError()
    {
        Console.WriteLine();
        Console.WriteLine("Error: box not vacant!");
        Console.WriteLine("Press any key to try again..");
        Console.ReadKey();
    }

    public static void InvalidKeyMessage()
    {
        Console.WriteLine("Wrong selection entered!");
        Console.WriteLine("Press any key to try again..");
        Console.ReadKey();
    }
    public static void DisplayWinner(Player player)
    {
        Console.WriteLine();
        Console.WriteLine("The winner is {0}!", player.Character);
        Console.ReadKey();
    }
}