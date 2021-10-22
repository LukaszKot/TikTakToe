using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    public char WinPerson { get; set; }

    public bool IsWin { get; set; }

    private bool isX;

    public bool isY
    {
        get { return isX; }
        set { isX = value; }
    }

    public char[,] Boxes { get; set; } = new char[3, 3];

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

    private bool IsPlayerAWinner(char player, int size)
    {
        return IsPlayerWinnerInAnyColumn(player, size)
            || IsPlayerWinnerInAnyRow(player, size)
            || IsPlayerWinnerInLeftCrossing(player, size)
            || IsPlayerWinnerInRightCrossing(player, size);
    }

    public void CheckWin()
    {
        CheckWinForPlayer('X', 3);
        CheckWinForPlayer('Y', 3);
    }

    private void CheckWinForPlayer(char player, int size)
    {
        if (IsPlayerAWinner(player, size))
        {
            MarkPlayerAsAWinner(player);
        }
    }

    private void MarkPlayerAsAWinner(char player)
    {
        IsWin = true;
        WinPerson = player;
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

    public bool HasError;

    public void InitializeData(int size)
    {
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                Boxes[i, j] = ' ';
            }
        }
    }

    private static void MoveToField(Program prog, int fieldNumber, char player, ref int moveCount, int size)
    {
        fieldNumber--;
        var row = fieldNumber / size;
        var column = fieldNumber % size;
        if (prog.Boxes[row, column] == ' ')
        {
            prog.Boxes[row, column] = player;
            moveCount++;
        }
        else
        {
            prog.NotVacantError();
        }
    }

    static void Main()
    {
        int moveCount = 0; // check loss
        char askMove; // display X or Y in question
        int selTemp;
        Program prog = new Program();
        prog.HasError = false;
        prog.InitializeData(3);
        prog.isY = true;
        Console.WriteLine(" -- Tic Tac Toe -- ");
        Console.Clear();
        while (!prog.IsWin)
        {
            if (moveCount == 9)
            {
                prog.DisplayLoss();
            }
            if ((prog.isY) == true) // if is X
            {
                askMove = 'X';
            }
            else
            {
                askMove = 'Y';
            }
            Console.Clear();
            prog.WriteBoard(3);
            Console.WriteLine();
            Console.WriteLine("What box do you want to place {0} in? (1-9)", askMove);
            Console.Write("> ");
            selTemp = int.Parse(Console.ReadLine());

            if (selTemp < 10 && selTemp > 0)
            {
                MoveToField(prog, selTemp, askMove, ref moveCount, 3);
            }
            else
            {
                Console.WriteLine("Wrong selection entered!");
                Console.WriteLine("Press any key to try again..");
                Console.ReadKey();
                prog.HasError = true;
            }
            if (prog.HasError)
            {
                prog.CheckWin(); // if error, just check win
                prog.HasError = !prog.HasError;
            }
            else
            {
                prog.isY = !prog.isY; // flip boolean
                prog.CheckWin();
            }
        }
        Console.Clear();
        prog.WriteBoard(3);
        Console.WriteLine();
        Console.WriteLine("The winner is {0}!", prog.WinPerson);
        Console.ReadKey();
    }
}