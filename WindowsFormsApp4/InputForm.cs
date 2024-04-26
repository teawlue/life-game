using System;
using System.Drawing;
using System.Windows.Forms;

public class InputForm : Form
{
    private bool[,] grid;
    private int cellSize;
    private const int rows = 50; // Те же значения, что и в основной форме
    private const int cols = 50;

    public InputForm(bool[,] grid, int size)
    {
        this.grid = grid;
        this.cellSize = size;
        this.ClientSize = new Size(cols * size, rows * size);
        this.Text = "Initialize Grid Manually";
        this.DoubleBuffered = true;
        this.MouseDown += new MouseEventHandler(Form_MouseDown);
    }

    private void Form_MouseDown(object sender, MouseEventArgs e)
    {
        int x = e.X / cellSize;
        int y = e.Y / cellSize;
        if (x >= 0 && x < cols && y >= 0 && y < rows)
        {
            grid[y, x] = !grid[y, x];
            this.Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Brush brush = grid[i, j] ? Brushes.Black : Brushes.White;
                g.FillRectangle(brush, j * cellSize, i * cellSize, cellSize - 1, cellSize - 1);
            }
        }
    }
}
