using Godot;
using System;

public class Main : Node2D
{
    [Export]
    int gridWidth = 188;
    [Export]
    int gridHeight = 188;

    float weight = 0.6f;
    int speed = 80;
    float resetTime = 5f;
    float elapsedTime = 0f;
    Point currentPoint = new Point(89, 89);
    Point previousPoint = new Point(0, 0);

    int numberOfPoints = 5;
    Point[] points;
    TileMap map;
    Random rand;

    TextEdit pointsEdit;
    TextEdit weightEdit;
    CheckBox ignoreLastCB;

    bool isDrawing = true;

    bool ignoreLast = true;

    public override void _Ready()
    {
        map = GetNode<TileMap>("TileMap");

        pointsEdit = GetNode<TextEdit>("Control/TextureRect/V1/H1/PointsEdit");
        weightEdit = GetNode<TextEdit>("Control/TextureRect/V2/H2/WeightEdit");
        ignoreLastCB = GetNode<CheckBox>("Control/TextureRect/H3/IgnoreLast");

        rand = new Random();

        ResetAndRandomize();
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        elapsedTime += delta;

        if (Input.IsActionJustPressed("reset"))
        {
            ResetAndRandomize();
        }

        if (elapsedTime > resetTime)
        {
            isDrawing = !isDrawing;
            elapsedTime -= resetTime;
        }

        if (isDrawing)
        {
            DrawShape(1);
        }
        else DrawShape(0);

    }

    public void DrawShape(int cellValue)
    {

        for (int i = 0; i < speed; i++)
        {
            int next = rand.Next(points.Length);
            if (previousPoint != points[next] || ignoreLast)
            {
                SetHalfWayPointToPoint(points[next]);
            }
            previousPoint = points[next];
            map.SetCell(currentPoint.xPos, currentPoint.yPos, cellValue);
        }

    }

    public void ResetAndRandomize()
    {
        numberOfPoints = pointsEdit.Text.ToInt();
        weight = weightEdit.Text.ToFloat();
        ignoreLast = ignoreLastCB.Pressed;

        currentPoint = new Point(gridWidth / 2, gridHeight / 2);
        previousPoint = new Point(0, 0);
        points = new Point[numberOfPoints];
        isDrawing = true;
        elapsedTime = 0;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                map.SetCell(x, y, 0);
            }
        }

        float startAngle = (float)GD.RandRange(0, 360);
        float startAngleInRadians = Mathf.Deg2Rad(startAngle);

        float angle = 360 / points.Length;
        float angleInRadians = Mathf.Deg2Rad(angle);
        int length = 80;

        int xStart = gridWidth / 2;
        int yStart = gridHeight / 2;

        for (int i = 0; i < points.Length; i++)
        {
            int myX = xStart + (int)(length * Mathf.Cos(startAngle + (angleInRadians * i)));
            int myY = yStart + (int)(length * Mathf.Sin(startAngle + (angleInRadians * i)));

            points[i] = new Point(myX, myY);
            map.SetCell(points[i].xPos, points[i].yPos, 2);
        }
    }

    void SetHalfWayPointToPoint(Point dest)
    {
        int middleX = Mathf.FloorToInt(Mathf.Lerp(currentPoint.xPos, dest.xPos, weight));
        int middleY = Mathf.FloorToInt(Mathf.Lerp(currentPoint.yPos, dest.yPos, weight));

        currentPoint.xPos = middleX;
        currentPoint.yPos = middleY;
    }

    Point GetRandomPoint()
    {
        int x = Mathf.FloorToInt((float)GD.RandRange(0, gridWidth));
        int y = Mathf.FloorToInt((float)GD.RandRange(0, gridHeight));
        return new Point(x, y);
    }


    class Point
    {
        public int xPos;
        public int yPos;

        public Point(int x, int y)
        {
            xPos = x;
            yPos = y;
        }
    }

}
