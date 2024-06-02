using Godot;
using System;
using System.Collections.Generic;


public static class Util
{
    /// <summary>
    /// Return offset from the grid center to the center of this cell
    /// </summary>
    static public Vector2 GetCellPositionAt(Vector2I gridRowCol, float cellSize, Vector2I cellRowCol)
    {
        // (row, col) is mapped to (y, x)
        Vector2 cellPos = new Vector2(cellRowCol.X, cellRowCol.Y) * cellSize + Vector2.One * cellSize / 2;
        Vector2 gridCenter = new Vector2(gridRowCol.X, gridRowCol.Y) * cellSize / 2;
        Vector2 offset = cellPos - gridCenter;
        Vector2 swapped = new Vector2(offset.Y, offset.X);
        return swapped;
    }

    /// <summary>
    /// Linearly interpolate the angle and distance from the center
    /// in the direction specified by clockwise.
    /// Expect t in the range of [0, 1]
    /// </summary>
    static public Vector2 CurvedInterpolate(Vector2 start, Vector2 end, Vector2 center, bool clockwise, float t)
    {
        Vector2 startToCenter = start - center;
        Vector2 endToCenter = end - center;

        float startLength = startToCenter.Length();
        float endLength = endToCenter.Length();
        float interpolatedLength = startLength * (1 - t) + endLength * t;

        // find angle from start to end
        // ref: https://stackoverflow.com/questions/68829498/lerp-rotation-angles-clock-or-counter-clockwise
        double startAngle = Math.Atan2(startToCenter.X, startToCenter.Y);
        double endAngle = Math.Atan2(endToCenter.X, endToCenter.Y);
        if (clockwise)
        {
            // Angle should be decreasing. If endAngle is larger, make it rotate one circle
            if (endAngle > startAngle) { endAngle -= Math.PI * 2; }
        }
        else
        {
            // Angle should be increasing. If endAngle is smaller, make it rotate one circle
            if (endAngle < startAngle) { endAngle += Math.PI * 2; }
        }
        float angle = (float)(startAngle - endAngle);
        float interpolatedAngle = angle * t;

        // find rotated vector of start by some angles
        float cosTheta = Mathf.Cos(interpolatedAngle);
        float sinTheta = Mathf.Sin(interpolatedAngle);
        float rotatedX = start.X * cosTheta - start.Y * sinTheta;
        float rotatedY = start.X * sinTheta + start.Y * cosTheta;
        Vector2 rotatedVector = new Vector2(rotatedX, rotatedY);

        Vector2 scaledRotatedVector = rotatedVector / rotatedVector.Length() * interpolatedLength;

        return scaledRotatedVector + center;
    }
}
