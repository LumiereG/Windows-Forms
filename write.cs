enum Tool { None, Rectangle, Circle, Line }

Tool currentTool = Tool.None;
Color currentColor = Color.Black;
Point startPoint;
bool isDrawing = false;
Bitmap bitmap;

public Form1()
{
    InitializeComponent();
    bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
    pictureBox1.Image = bitmap;
}

private void Form1_Load(object sender, EventArgs e)
{
    // Nic do zrobienia na starcie
}

private void btnRectangle_Click(object sender, EventArgs e)
{
    currentTool = Tool.Rectangle;
    lblSelectedTool.Text = "Wybrane narzędzie: Prostokąt";
}

private void btnCircle_Click(object sender, EventArgs e)
{
    currentTool = Tool.Circle;
    lblSelectedTool.Text = "Wybrane narzędzie: Koło";
}

private void btnLine_Click(object sender, EventArgs e)
{
    currentTool = Tool.Line;
    lblSelectedTool.Text = "Wybrane narzędzie: Linia";
}

private void btnColorPicker_Click(object sender, EventArgs e)
{
    using (ColorDialog colorDialog = new ColorDialog())
    {
        if (colorDialog.ShowDialog() == DialogResult.OK)
        {
            currentColor = colorDialog.Color;
        }
    }
}

private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
{
    if (currentTool != Tool.None)
    {
        startPoint = e.Location;
        isDrawing = true;
    }
}

private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
{
    if (isDrawing && currentTool != Tool.None)
    {
        DrawPreview(e.Location);
    }
}

private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
{
    if (isDrawing && currentTool != Tool.None)
    {
        DrawShape(e.Location);
        isDrawing = false;
    }
}

private void DrawPreview(Point currentPoint)
{
    using (Graphics g = Graphics.FromImage(bitmap))
    {
        g.Clear(Color.White);

        switch (currentTool)
        {
            case Tool.Rectangle:
                g.DrawRectangle(new Pen(currentColor), GetRectangle(startPoint, currentPoint));
                break;
            case Tool.Circle:
                g.DrawEllipse(new Pen(currentColor), GetRectangle(startPoint, currentPoint));
                break;
            case Tool.Line:
                g.DrawLine(new Pen(currentColor), startPoint, currentPoint);
                break;
        }
    }
    pictureBox1.Invalidate();
}

private void DrawShape(Point endPoint)
{
    using (Graphics g = Graphics.FromImage(bitmap))
    {
        switch (currentTool)
        {
            case Tool.Rectangle:
                g.FillRectangle(new SolidBrush(currentColor), GetRectangle(startPoint, endPoint));
                break;
            case Tool.Circle:
                g.FillEllipse(new SolidBrush(currentColor), GetRectangle(startPoint, endPoint));
                break;
            case Tool.Line:
                g.DrawLine(new Pen(currentColor), startPoint, endPoint);
                break;
        }
    }
    pictureBox1.Invalidate();
}

private Rectangle GetRectangle(Point start, Point end)
{
    int width = Math.Abs(end.X - start.X);
    int height = Math.Abs(end.Y - start.Y);
    int x = Math.Min(start.X, end.X);
    int y = Math.Min(start.Y, end.Y);
    return new Rectangle(x, y, width, height);
}
