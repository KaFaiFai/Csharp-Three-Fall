using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.HttpRequest;

/// <summary>
/// Contains core algorithms used in different phases of the game<br />
/// - <see cref="WillDropTo"/><br />
/// - <see cref="FindMatch3"/><br />
/// - <see cref="FindFlyingBlocks"/><br />
/// </summary>
public partial class Mechanics : RefCounted
{
    /// <summary>
    /// Return the start positions with respect to the original shape right above the top row, 
    /// final positions and types of the <paramref name="blocks"/> 
    /// that will drop into <paramref name="mainBoard"/>. 
    /// The leftmost column index of the blocks is given by <paramref name="leftIndex"/>
    /// </summary>
    static public List<(Vector2I, Vector2I, BlockType)> WillDropTo(BlockType?[,] mainBoard, BlockType?[,] blocks, int leftIndex)
    {
        // Top row is at 0 and bottom row is at NumRow - 1
        int numRow = mainBoard.GetLength(0);
        int numCol = mainBoard.GetLength(1);
        List<int> lowestEmptySpace = Enumerable.Repeat(0, numCol).ToList();

        for (int col = 0; col < numCol; col++)
        {
            for (int row = numRow - 1; row > -1; row--)
            {
                if (mainBoard[row, col] == null)
                {
                    lowestEmptySpace[col] = row;
                    break;
                }
            }
        }

        List<(Vector2I, Vector2I, BlockType)> dropTo = new List<(Vector2I, Vector2I, BlockType)>();
        // Search from bottom to top
        for (int i = blocks.GetLength(0) - 1; i > -1; i--)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                int col = leftIndex + j;
                if (blocks[i, j] != null)
                {
                    BlockType block = (BlockType)blocks[i, j];
                    Vector2I start = new Vector2I(i - blocks.GetLength(0), col);
                    Vector2I end = new Vector2I(lowestEmptySpace[col], col);
                    dropTo.Add((start, end, block));
                    lowestEmptySpace[col] -= 1;
                }
            }
        }

        return dropTo;
    }

    static public List<Vector2I> FindMatch3(BlockType?[,] mainBoard)
    {
        int numRow = mainBoard.GetLength(0);
        int numCol = mainBoard.GetLength(1);
        List<Vector2I> results = new List<Vector2I>();

        // scan horizontal lines
        for (int i = 0; i < numRow; i++)
        {
            List<Vector2I> curResults = new List<Vector2I>();
            for (int j = 0; j < numCol; j++)
            {
                BlockType? block = mainBoard[i, j];
                Vector2I rowCol = new Vector2I(i, j);
                if (block == null)
                {
                    if (curResults.Count >= 3)
                    {
                        results.AddRange(curResults);
                    }
                    curResults = new List<Vector2I>();
                }
                else if (curResults.Count == 0)
                {
                    curResults.Add(rowCol);
                }
                else
                {
                    if (block == mainBoard[curResults.Last().X, curResults.Last().Y])
                    {
                        curResults.Add(rowCol);
                    }
                    else
                    {
                        if (curResults.Count >= 3)
                        {
                            results.AddRange(curResults);
                        }
                        curResults = new List<Vector2I> { rowCol };
                    }
                }
            }
            if (curResults.Count >= 3)
            {
                results.AddRange(curResults);
            }
        }


        // scan vertical lines
        for (int j = 0; j < numCol; j++)
        {
            List<Vector2I> curResults = new List<Vector2I>();
            for (int i = 0; i < numRow; i++)
            {
                BlockType? block = mainBoard[i, j];
                Vector2I rowCol = new Vector2I(i, j);
                if (block == null)
                {
                    if (curResults.Count >= 3)
                    {
                        results.AddRange(curResults);
                    }
                    curResults = new List<Vector2I>();
                }
                else if (curResults.Count == 0)
                {
                    curResults.Add(rowCol);
                }
                else
                {
                    if (block == mainBoard[curResults.Last().X, curResults.Last().Y])
                    {
                        curResults.Add(rowCol);
                    }
                    else
                    {
                        if (curResults.Count >= 3)
                        {
                            results.AddRange(curResults);
                        }
                        curResults = new List<Vector2I> { rowCol };
                    }
                }
            }
            if (curResults.Count >= 3)
            {
                results.AddRange(curResults);
            }
        }
        return results;
    }

    static public List<(Vector2I, Vector2I, BlockType)> FindFlyingBlocks(BlockType?[,] mainBoard)
    {
        int numRow = mainBoard.GetLength(0);
        int numCol = mainBoard.GetLength(1);
        List<(Vector2I, Vector2I, BlockType)> flyingBlocks = new List<(Vector2I, Vector2I, BlockType)>();
        for (int j = 0; j < numCol; j++)
        {
            int numEmpty = 0;
            for (int i = numRow - 1; i > -1; i--)
            {
                if (mainBoard[i, j] == null)
                {
                    numEmpty++;
                }
                else if (numEmpty != 0)
                {
                    Vector2I start = new Vector2I(i, j);
                    Vector2I end = new Vector2I(i + numEmpty, j);
                    BlockType block = (BlockType)mainBoard[i, j];
                    flyingBlocks.Add((start, end, block));
                }
            }
        }
        return flyingBlocks;
    }

    static private List<List<int>> FindMatch3InLine(List<BlockType?> line)
    {
        List<List<int>> results = new List<List<int>>();
        List<int> curResults = new List<int>();
        for (int i = 0; i < line.Count; i++)
        {
            BlockType? block = line[i];
            if (block == null)
            {
                if (curResults.Count >= 3)
                {
                    results.Add(curResults);
                }
                curResults = new List<int>();
            }
            else if (curResults.Count == 0)
            {
                curResults.Add(i);
            }
            else
            {
                if (block == line[i - 1])
                {
                    curResults.Add(i);
                }
                else
                {
                    if (curResults.Count >= 3)
                    {
                        results.Add(curResults);
                    }
                    curResults = new List<int> { i };
                }
            }
        }
        if (curResults.Count >= 3)
        {
            results.Add(curResults);
        }
        return results;
    }

    static private List<List<T>> MergeSublist<T>(List<List<T>> lists) where T : IEquatable<T>
    {
        List<List<T>> mergedLists = new List<List<T>> { };
        foreach (List<T> list in lists)
        {
            bool hasAdded = false;
            foreach (List<T> sublist in mergedLists)
            {
                IEnumerable<T> intersection = list.Intersect(sublist);
                if (intersection.Any())
                {
                    sublist.AddRange(list.Except(sublist));
                    hasAdded = false;
                }
            }
            if (!hasAdded)
            {
                mergedLists.Add(list);
            }
        }

        return mergedLists;
    }
}
