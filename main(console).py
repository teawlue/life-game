import os
import time
import random
import hashlib

def create_board(rows, cols):
    """Create a new game board with dead buffer around it."""
    return [[0] * (cols + 2) for _ in range(rows + 2)]

def count_neighbors(board, x, y):
    """Count the number of live neighbors around the cell at position (x, y), optimized with buffer."""
    neighbors = 0
    for dx in range(-1, 2):
        for dy in range(-1, 2):
            if dx != 0 or dy != 0:
                neighbors += board[x + dx][y + dy]
    return neighbors

def next_generation(current):
    """Calculate the next generation of the game state."""
    rows, cols = len(current) - 2, len(current[0]) - 2
    new_board = create_board(rows, cols)
    for x in range(1, rows + 1):
        for y in range(1, cols + 1):
            neighbors = count_neighbors(current, x, y)
            if current[x][y] == 1 and neighbors in (2, 3):
                new_board[x][y] = 1
            elif current[x][y] == 0 and neighbors == 3:
                new_board[x][y] = 1
    return new_board

def print_board(board):
    """Print the current board state to the screen, omitting the dead buffer."""
    os.system('cls' if os.name == 'nt' else 'clear')
    for row in board[1:-1]:
        print(' '.join('🟩' if cell else '⬛' for cell in row[1:-1]))

def initialize_board(rows, cols):
    """Let the user choose the initial state of the board."""
    board = create_board(rows, cols)
    choice = input("Type 'manual' to enter live cells manually, or anything else to generate randomly: ").strip().lower()
    if choice == 'manual':
        print("Enter live cell coordinates (e.g., '1 3' for row 1, column 3); type 'done' when finished:")
        while True:
            entry = input().strip()
            if entry.lower() == 'done':
                break
            try:
                x, y = map(int, entry.split())
                if 1 <= x <= rows and 1 <= y <= cols:
                    board[x][y] = 1
                else:
                    print("Coordinates out of bounds. Try again.")
            except ValueError:
                print("Invalid input. Use two integers separated by space.")
    else:
        for x in range(1, rows + 1):
            for y in range(1, cols + 1):
                board[x][y] = random.choice([0, 1])
    return board

def get_dimensions():
    """Ask user for the dimensions of the game board."""
    while True:
        try:
            print("Enter the number of rows for the board:")
            rows = int(input())
            if rows < 1:
                raise ValueError
            print("Enter the number of columns for the board:")
            cols = int(input())
            if cols < 1:
                raise ValueError
            return rows, cols
        except ValueError:
            print("Invalid input. Please enter positive integers.")

def menu():
    """Display menu options and get user choices for game settings."""
    rows, cols = get_dimensions()
    while True:
        speed = input("Enter 'fast', 'normal', 'slow', or 'step' for step-by-step execution with ENTER: ").lower()
        if speed in ['fast', 'normal', 'slow', 'step']:
            break
        print("Invalid speed. Choose from 'fast', 'normal', 'slow', 'step'.")
    return rows, cols, speed

def game_of_life():
    """Run the Game of Life with menu for initial settings."""
    rows, cols, speed = menu()
    initial_state = initialize_board(rows, cols)
    current = initial_state
    history = set()

    try:
        while True:
            print_board(current)
            current_hash = hashlib.sha256(str(current).encode()).hexdigest()
            if current_hash in history or all(all(cell == 0 for cell in row[1:-1]) for row in current[1:-1]):
                print("Game ended: Stable or repeating configuration detected.")
                break
            history.add(current_hash)
            current = next_generation(current)
            if speed == 'fast':
                time.sleep(0.1)
            elif speed == 'normal':
                time.sleep(0.5)
            elif speed == 'slow':
                time.sleep(1)
            elif speed == 'step':
                input("Press ENTER for the next generation...")
    except KeyboardInterrupt:
        print("Game interrupted.")

# Run the game
game_of_life()