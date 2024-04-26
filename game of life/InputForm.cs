using System;
using System.Drawing;
using System.Windows.Forms;

public class InputForm : Form
{
    private bool[,] grid;
    private int cellSize;
    private Button initializeButton;
    private const int rows = 50; // Use the same values as in the main form
    private const int cols = 50;

    public InputForm(bool[,] grid, int size)
    {
        this.grid = grid;
        this.cellSize = size;
        this.ClientSize = new Size(cols * size, rows * size + 40);
        this.Text = "Initialize Grid Manually";
        this.DoubleBuffered = true;

        initializeButton = new Button
        {
            Text = "Initialize",
            Location = new Point(10, rows * size + 10),
            Size = new Size(100, 30)
        };
        initializeButton.Click += new EventHandler(InitializeGrid);
        this.Controls.Add(initializeButton);
    }

    private void InitializeGrid(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        ToggleCellState(e.X, e.Y);
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ToggleCellState(e.X, e.Y);
        }
        base.OnMouseMove(e);
    }

    private void ToggleCellState(int x, int y)
    {
        int row = y / cellSize;
        int col = x / cellSize;
        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            grid[row, col] = !grid[row, col];
            this.Invalidate(new Rectangle(col * cellSize, row * cellSize, cellSize, cellSize));
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
