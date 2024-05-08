using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SPD_Algorithms
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Create tasks (n, p), where n - task number, p - task duration
            Dictionary<int,int> Tasks = new Dictionary<int, int>();
            Tasks.Add(1, 1);
            Tasks.Add(2, 2);
            Tasks.Add(3, 3);
            Tasks.Add(4, 4);
            Tasks.Add(5, 5);
            Tasks.Add(6, 6);

            //LPT Algorithm
            int Cmax = LPTorLSA(Tasks, true);
            Console.WriteLine("LPT Cmax: " + Cmax + "\n");

            //LSA Algorithm
            Cmax = LPTorLSA(Tasks, false);
            Console.WriteLine("LSA Cmax: " + Cmax + "\n");

            //PD Algorithm
            Cmax = PD(Tasks);
            Console.WriteLine("PD Cmax: " + Cmax + "\n");

            //LPT - true, LSA - false
            int LPTorLSA(Dictionary<int,int> currentTasks, bool choice) {

                if (choice)
                {
                    Console.WriteLine("Starting LPT Algorithm...");
                    currentTasks = currentTasks.OrderByDescending(task => task.Value).ToDictionary();
                }
                else { Console.WriteLine("Starting LSA Algorithm..."); }
                Console.WriteLine("Tasks to be ordered:");
                foreach (var task in currentTasks)
                {
                    Console.WriteLine("n: "+task.Key+" p: "+task.Value);
                }

                List<int> machine1Tasks = new List<int>();
                List<int> machine2Tasks = new List<int>();
                int machine1CMax = 0;
                int machine2CMax = 0;

                foreach (var task in currentTasks)
                {
                    if (machine1CMax <= machine2CMax)
                    {
                        machine1Tasks.Add(task.Key);
                        machine1CMax += task.Value;
                    }
                    else
                    {
                        machine2Tasks.Add(task.Key);
                        machine2CMax += task.Value;
                    }
                }

                Console.Write("First machine tasks:");
                foreach (var task in machine1Tasks)
                {
                    Console.Write(" " + task);
                }
                Console.WriteLine();

                Console.Write("Second machine tasks:");
                foreach (var task in machine2Tasks)
                {
                    Console.Write(" " + task);
                }
                Console.WriteLine();

                return machine1CMax >= machine2CMax ? machine1CMax : machine2CMax;
            }
            int PD(Dictionary<int, int> currentTasks)
            {
                Console.WriteLine("Starting PD Algorithm...");
                int numberOfTasks = currentTasks.Count;
                int totalTime = 0;
                foreach(var task in currentTasks)
                {
                    totalTime += task.Value;
                }
                int rows = numberOfTasks + 1;
                int columns = (totalTime / 2) + 1;

                int[,] board = new int[rows, columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (j == 0)
                            board[i, j] = 1;
                        else
                            board[i, j] = 0;
                    }
                }

                for (int j = 1; j < rows; j++)
                {
                    for (int k = 1; k < columns; k++)
                    {
                        if (board[j - 1, k] == 1)
                        {
                            board[j, k] = 1;
                        }
                        if (k >= currentTasks.ElementAt(j - 1).Value)
                        {
                            if (board[j - 1, k - currentTasks.ElementAt(j - 1).Value] == 1)
                            {
                               board[j, k] = 1;
                            }
                        }
                    }
                }

                List<int> machine1Tasks = new List<int>();
                List<int> machine2Tasks = new List<int>();
                int machine1CMax = 0;
                int machine2CMax = 0;
                Console.WriteLine("Board for backtracking:");
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        Console.Write(board[i, j] + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                int currentRow = rows - 1;
                int currentCol = columns - 1;
                List<int> leftoverTasks = Enumerable.Range(1, numberOfTasks).ToList();
                while (currentCol > 0)
                {
                    if (board[currentRow, currentCol] == 0)
                    {
                        --currentCol;
                    }
                    else if (board[currentRow, currentCol] == 1 && board[currentRow - 1, currentCol] == 0)
                    {
                        machine1Tasks.Add(currentRow);
                        machine1CMax += currentTasks.ElementAt(currentRow-1).Value;
                        leftoverTasks.Remove(currentRow);

                        currentCol -= currentTasks.ElementAt(currentRow-1).Value;
                    }
                    else
                    {
                        --currentRow;
                    }
                }
                Console.Write("First machine tasks:");
                foreach (var task in machine1Tasks)
                {
                    Console.Write(" " + task);
                }
                Console.WriteLine();
                Console.Write("Second machine tasks:");
                foreach (int task in leftoverTasks)
                {
                    machine2Tasks.Add(task);
                    Console.Write(" " + task);
                    machine2CMax += currentTasks.ElementAt(task-1).Value;
                }
                Console.WriteLine();
                return machine1CMax >= machine2CMax ? machine1CMax : machine2CMax;
            }
        }
    }
}
