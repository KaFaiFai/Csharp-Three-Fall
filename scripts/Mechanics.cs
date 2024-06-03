using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains core algorithms used in different phases of the game<br />
/// - <see cref="WillDropTo"/><br />
/// - <see cref="FindMatch3"/><br />
/// - <see cref="FindFlyingBlocks"/><br />
/// </summary>
public partial class Mechanics : RefCounted
{
    /// <summary>
    /// Return the final positions and types the <paramref name="blocks"/> 
    /// will drop into <paramref name="mainBoard"/>. 
    /// The leftmost column index of the blocks is given by <paramref name="leftIndex"/>
    /// </summary>
    static public List<(Vector2I, BlockType)> WillDropTo(BlockType?[,] mainBoard, BlockType?[,] blocks, int leftIndex)
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

        List<(Vector2I, BlockType)> dropTo = new List<(Vector2I, BlockType)>();
        // Search from bottom to top
        for (int i = blocks.GetLength(0) - 1; i > -1; i--)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                if (blocks[i, j] != null)
                {
                    BlockType block = (BlockType)blocks[i, j];
                    int col = leftIndex + j;
                    dropTo.Add((new Vector2I(lowestEmptySpace[col], col), block));
                    lowestEmptySpace[col] -= 1;
                }
            }
        }

        return dropTo;
    }

    static public List<(Vector2I, BlockType)> FindMatch3(BlockType?[,] mainBoard)
    {
        int numRow = mainBoard.GetLength(0);
        int numCol = mainBoard.GetLength(1);
        List<(Vector2I, BlockType)> results = new List<(Vector2I, BlockType)>();

        // scan horizontal lines
        for (int i = 0; i < numRow; i++)
        {
            List<(Vector2I, BlockType)> curResults = new List<(Vector2I, BlockType)>();
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
                    curResults = new List<(Vector2I, BlockType)>();
                }
                else if (curResults.Count == 0)
                {
                    curResults.Add((rowCol, (BlockType)block));
                }
                else
                {
                    if (block == curResults.Last().Item2)
                    {
                        curResults.Add((rowCol, (BlockType)block));
                    }
                    else
                    {
                        if (curResults.Count >= 3)
                        {
                            results.AddRange(curResults);
                        }
                        curResults = new List<(Vector2I, BlockType)> { (rowCol, (BlockType)block) };
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
            List<(Vector2I, BlockType)> curResults = new List<(Vector2I, BlockType)>();
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
                    curResults = new List<(Vector2I, BlockType)>();
                }
                else if (curResults.Count == 0)
                {
                    curResults.Add((rowCol, (BlockType)block));
                }
                else
                {
                    if (block == curResults.Last().Item2)
                    {
                        curResults.Add((rowCol, (BlockType)block));
                    }
                    else
                    {
                        if (curResults.Count >= 3)
                        {
                            results.AddRange(curResults);
                        }
                        curResults = new List<(Vector2I, BlockType)> { (rowCol, (BlockType)block) };
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
}
