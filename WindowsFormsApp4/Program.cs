using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameOfLife
{
    public class GameOfLifeForm : Form
    {
        private Timer timer;
        private bool[,] grid;
        private HashSet<string> previousStates;
        private int generation;
        private const int size = 10; // Размер клетки
        private const int rows = 50;
        private const int cols = 50;
        private Button startButton;
        private Button initializeButton;
        private Label generationLabel;
        private Label populationLabel;
        private TrackBar speedTrackBar;

        public GameOfLifeForm()
        {
            grid = new bool[rows, cols];
            previousStates = new HashSet<string>();
            timer = new Timer();
            timer.Tick += new EventHandler(UpdateGrid);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Width = cols * size + 40;
            this.Height = rows * size + 160;
            this.Text = "Game of Life";

            startButton = new Button
            {
                Text = "Start",
                Location = new Point(10, rows * size + 10),
                Size = new Size(80, 30)
            };
            startButton.Click += new EventHandler(ToggleTimer);
            this.Controls.Add(startButton);

            initializeButton = new Button
            {
                Text = "Initialize",
                Location = new Point(10, rows * size + 50),
                Size = new Size(80, 30)
            };
            initializeButton.Click += new EventHandler(InitializeGrid);
            this.Controls.Add(initializeButton);

            generationLabel = new Label
            {
                Text = $"Generation: {generation}",
                Location = new Point(100, rows * size + 10),
                Size = new Size(180, 30)
            };
            this.Controls.Add(generationLabel);

            populationLabel = new Label
            {
                Text = "Population: 0",
                Location = new Point(100, rows * size + 40),
                Size = new Size(180, 30)
            };
            this.Controls.Add(populationLabel);

            speedTrackBar = new TrackBar
            {
                Minimum = 1,
                Maximum = 10,
                Value = 5,
                Location = new Point(300, rows * size + 10),
                Size = new Size(180, 30)
            };
            speedTrackBar.ValueChanged += new EventHandler(AdjustSpeed);
            this.Controls.Add(speedTrackBar);
        }

        private void ToggleTimer(object sender, EventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
                startButton.Text = "Start";
            }
            else
            {
                timer.Start();
                startButton.Text = "Stop";
            }
        }

        private void InitializeGrid(object sender, EventArgs e)
        {
            var initDialog = MessageBox.Show("Do you want to manually initialize the grid? Click 'Yes' to manually initialize, 'No' to randomize.", "Initialize Grid", MessageBoxButtons.YesNo);
            if (initDialog == DialogResult.Yes)
            {
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        grid[i, j] = false;

                var inputForm = new InputForm(grid, size);
                inputForm.ShowDialog();
            }
            else
            {
                RandomizeGrid();
            }
            this.Invalidate();
        }

        private void UpdateGrid(object sender, EventArgs e)
        {
            bool[,] next = new bool[rows, cols];
            int population = 0;
            string currentHash = GetGridHash();

            if (previousStates.Contains(currentHash))
            {
                timer.Stop();
                MessageBox.Show("Repeating configuration detected. Simulation stopped.", "Game of Life");
                startButton.Text = "Start";
                return;
            }

            previousStates.Add(currentHash);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int neighbors = CountNeighbors(i, j);
                    bool isAlive = grid[i, j];

                    if (isAlive && (neighbors == 2 || neighbors == 3))
                        next[i, j] = true;
                    else if (!isAlive && neighbors == 3)
                        next[i, j] = true;

                    if (next[i, j]) population++;
                }
            }

            grid = next;
            generation++;
            generationLabel.Text = $"Generation: {generation}";
            populationLabel.Text = $"Population: {population}";
            this.Invalidate();
        }

        private string GetGridHash()
        {
            var hash = new System.Text.StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    hash.Append(grid[i, j] ? '1' : '0');
                }
            }
            return hash.ToString();
        }

        private int CountNeighbors(int x, int y)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    int nx = (x + i + rows) % rows;
                    int ny = (y + j + cols) % cols;
                    if (grid[nx, ny]) count++;
                }
            }
            return count;
        }

        private void RandomizeGrid()
        {
            Random rand = new Random();
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    grid[i, j] = rand.Next(2) == 1;
        }

        private void AdjustSpeed(object sender, EventArgs e)
        {
            timer.Interval = 1100 - speedTrackBar.Value * 100;
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
                    g.FillRectangle(brush, j * size, i * size, size - 1, size - 1);
                }
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameOfLifeForm());
        }
    }
}
