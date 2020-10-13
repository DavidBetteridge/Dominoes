using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Dominoes
{
    enum Direction
    {
        Unknown,
        Left,
        Right,
        Up,
        Down
    }

    class Cell
    {
        public int NumberOfDots { get; }

        public Direction Direction { get; set; } = Direction.Unknown;

        public Cell(int numberOfDots)
        {
            NumberOfDots = numberOfDots;
        }
    }

    class PairOfCells
    {
        public PairOfCells(Cell cell1, Cell cell2, Direction direction)
        {
            Cell1 = cell1;
            Cell2 = cell2;
            Direction = direction;
        }

        public Cell Cell1 { get; }
        public Cell Cell2 { get; }
        public Direction Direction { get; }

        public string Key()
        {
            if (Cell1.NumberOfDots < Cell2.NumberOfDots)
                return $"{Cell1.NumberOfDots}-{Cell2.NumberOfDots}";
            else
                return $"{Cell2.NumberOfDots}-{Cell1.NumberOfDots}";
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var numbers = new int[][]
                {
                    new int[] { 2,4,1,6,6,0,0,2 },
                    new int[] { 2,3,6,3,6,0,1,3 },
                    new int[] { 6,5,5,3,0,5,2,1 },
                    new int[] { 4,4,3,1,2,4,2,4 },
                    new int[] { 5,6,5,5,1,6,2,2 },
                    new int[] { 4,0,0,6,1,3,4,5 },
                    new int[] { 5,3,1,3,0,1,0,4 },
                };

            var grid = numbers.Select(row => mapRow(row)).ToArray();

            var columns = numbers[0].Length;
            var rows = numbers.Length;

            var found = new Dictionary<string, bool>();
            for (int c = 0; c <= 6; c++)
            {
                for (int r = c; r <= 6; r++)
                {
                    found.Add($"{c}-{r}", false);
                }
            }

            while (true)
            {
                Console.Clear();

                FindUniqueDominoes(numbers, grid, columns, rows, found);

                SingleDirection(numbers, grid, columns, rows, found);

                DisplayGrid(numbers, grid, rows);

                Console.ReadKey(true);

            }
        }


        public static string Key(Cell cell1, Cell cell2)
        {
            if (cell1.NumberOfDots < cell2.NumberOfDots)
                return $"{cell1.NumberOfDots}-{cell2.NumberOfDots}";
            else
                return $"{cell2.NumberOfDots}-{cell1.NumberOfDots}";
        }

        private static bool SingleDirection(int[][] numbers, Cell[][] grid, int columns, int rows, Dictionary<string, bool> found)
        {
            for (int column = 0; column < numbers[0].Length; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    var cell = grid[row][column];
                    if (cell.Direction == Direction.Unknown)
                    {
                        var canGoLeft = (column > 0) && (grid[row][column - 1].Direction == Direction.Unknown);
                        var canGoRight = (column + 1 < columns) && (grid[row][column + 1].Direction == Direction.Unknown);
                        var canGoUp = (row > 0) && (grid[row - 1][column].Direction == Direction.Unknown);
                        var canGoDown = (row + 1 < rows) && (grid[row + 1][column].Direction == Direction.Unknown);

                        var numberOfDirections = (canGoLeft ? 1 : 0) +
                                                 (canGoRight ? 1 : 0) +
                                                 (canGoUp ? 1 : 0) +
                                                 (canGoDown ? 1 : 0);

                        if (numberOfDirections == 1)
                        {
                            if (canGoLeft)
                            {
                                cell.Direction = Direction.Left;
                                grid[row][column - 1].Direction = Direction.Right;

                                found[Key(cell, grid[row][column - 1])] = true;

                                Console.WriteLine($"Left only {cell.NumberOfDots}-{grid[row][column - 1].NumberOfDots}");

                                return true;
                            }
                            if (canGoRight)
                            {
                                cell.Direction = Direction.Right;
                                grid[row][column + 1].Direction = Direction.Left;

                                found[Key(cell, grid[row][column + 1])] = true;

                                Console.WriteLine($"Right only {cell.NumberOfDots}-{grid[row][column + 1].NumberOfDots}");

                                return true;
                            }
                            if (canGoUp)
                            {
                                cell.Direction = Direction.Up;
                                grid[row - 1][column].Direction = Direction.Down;

                                found[Key(cell, grid[row - 1][column])] = true;

                                Console.WriteLine($"Up only {cell.NumberOfDots}-{grid[row - 1][column].NumberOfDots}");
                                return true;
                            }
                            if (canGoDown)
                            {
                                cell.Direction = Direction.Down;
                                grid[row + 1][column].Direction = Direction.Up;

                                found[Key(cell, grid[row + 1][column])] = true;


                                Console.WriteLine($"Down only {cell.NumberOfDots}-{grid[row + 1][column].NumberOfDots}");
                                return true;
                            }
                        }


                    }
                }
            }

            return false;
        }

        private static bool FindUniqueDominoes(int[][] numbers, Cell[][] grid, int columns, int rows, Dictionary<string, bool> found)
        {
            var pairs = new List<PairOfCells>();

            for (int column = 0; column < numbers[0].Length; column++)
            {
                for (int row = 0; row < rows; row++)
                {
                    var cell = grid[row][column];
                    if (cell.Direction == Direction.Unknown)
                    {
                        if (column + 1 < columns)
                        {
                            var otherCell = grid[row][column + 1];
                            if (otherCell.Direction == Direction.Unknown)
                            {
                                pairs.Add(new PairOfCells(cell, otherCell, Direction.Right));
                            }
                        }

                        if (row + 1 < rows)
                        {
                            var otherCell = grid[row + 1][column];
                            if (otherCell.Direction == Direction.Unknown)
                            {
                                pairs.Add(new PairOfCells(cell, otherCell, Direction.Down));
                            }
                        }

                    }
                }
            }

            var singles = pairs.GroupBy(p => p.Key()).Where(g => g.Count() == 1);
            foreach (var singleGroup in singles)
            {
                var pair = singleGroup.Single();

                if (!found[pair.Key()])
                {
                    if (pair.Direction == Direction.Right)
                    {
                        pair.Cell1.Direction = Direction.Right;
                        pair.Cell2.Direction = Direction.Left;
                    }
                    else
                    {
                        pair.Cell1.Direction = Direction.Down;
                        pair.Cell2.Direction = Direction.Up;
                    }

                    found[pair.Key()] = true;

                    Console.WriteLine("Unique pair " + pair.Key());
                    return true;
                }

            }

            return false;
        }

        private static void DisplayGrid(int[][] numbers, Cell[][] grid, int rows)
        {
            for (int row = 0; row < rows; row++)
            {
                var line = "";
                var spacer = "";
                for (int column = 0; column < numbers[0].Length; column++)
                {
                    var cell = grid[row][column];
                    line += cell.NumberOfDots;
                    if (cell.Direction == Direction.Right)
                        line += "-";
                    else
                        line += " ";

                    if (cell.Direction == Direction.Down)
                        spacer += "| ";
                    else
                        spacer += "  ";
                }
                Console.WriteLine(line);
                Console.WriteLine(spacer);
            }
        }

        private static Cell[] mapRow(int[] row)
        {
            return row.Select(numberOfDots => new Cell(numberOfDots)).ToArray();
        }
    }
}
