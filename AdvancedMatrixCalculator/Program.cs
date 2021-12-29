using System;

namespace AdvancedMatrixCalculator
{
    /// <summary>
    /// Main class of program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// This method checks if input command is loading matrix from file and input file exists.
        /// </summary>
        /// <param name="inputString">Input command.</param>
        /// <returns>Returns "0" if input command is not loading matrix from file, "1" if selected file is not txt, 
        /// "2" if file does not exist, another string - selected file name.</returns>
        static string CheckFileNameInput(string inputString)
        {
            if (inputString.Length < 4) return "0";
            if (inputString[0] != 'f' || inputString[1] != 'i' || inputString[2] != 'l' || inputString[3] != 'e')
                return "0";
            if (inputString[inputString.Length - 4] != '.' || inputString[inputString.Length - 3] != 't' ||
                inputString[inputString.Length - 2] != 'x' || inputString[inputString.Length - 1] != 't')
                return "1";
            string fileName = "";
            for (int index = 5; index < inputString.Length; index++) fileName += inputString[index];
            if (System.IO.File.Exists(fileName)) return fileName;
            return "2";
        }

        /// <summary>
        /// Method returns float value converted to string (if number is
        /// too long it will be presented in exponential form).
        /// </summary>
        /// <param name="number">Returns float number to be converted.</param>
        /// <returns>String value of float number.</returns>
        static string ConvertNumberToString(double number)
        {
            if (Math.Abs(number) >= 1e12) return String.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0:0.######E+0}", number);
            return number.ToString("F4");
        }

        /// <summary>
        /// Method requests the way to input matrix.
        /// </summary>
        /// <returns>Returns "0" if matrix should be filled randomly, "1" if matrix should be entered manual,
        /// or filename if matrix should be loaded from text file with corresponding filename.</returns>
        static string GetTypeOfMatrixFilling()
        {
            do
            {
                Console.Write("Method of filling matrix, random/keyboard/file filename.txt: ");
                string inputString = Console.ReadLine();
                if (inputString == "random") return "0";
                if (inputString == "keyboard") return "1";
                if (CheckFileNameInput(inputString) != "0" && CheckFileNameInput(inputString) != "1" &&
                    CheckFileNameInput(inputString) != "2") return CheckFileNameInput(inputString);
                if (CheckFileNameInput(inputString) == "1") Console.WriteLine("Chosen file is not .txt!!!");
                else if (CheckFileNameInput(inputString) == "2") Console.WriteLine("Chosen file does not exist!!!");
                else Console.WriteLine("Input type was not recognized!!!");
            } while (true);
            return "0";
        }

        /// <summary>
        /// Method requests the matrix size.
        /// </summary>
        /// <param name="needToBeSquared">Flag that states if matrix should be square or not.</param>
        /// <returns>Returns cortege (int, int) - matrix size.</returns>
        static (uint, uint) GetMatrixSize(bool needToBeSquared)
        {
            do
            {
                Console.Write("Enter amount of rows and columns (both values are integer and in [1; 20]): ");
                string[] separatedStrings = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (separatedStrings.Length != 2)
                {
                    Console.WriteLine("There are not two numbers!!!");
                    continue;
                }
                uint amountRows, amountColumns;
                if (!uint.TryParse(separatedStrings[0], out amountRows) || amountRows <= 1 || amountRows > 20)
                {
                    Console.WriteLine("First number is not correct!!!");
                    continue;
                }
                if (!uint.TryParse(separatedStrings[1], out amountColumns) || amountColumns <= 1 || amountColumns > 20)
                {
                    Console.WriteLine("Second number is not correct!!!");
                    continue;
                }
                if (needToBeSquared && amountColumns != amountRows)
                {
                    Console.WriteLine("Matrix should be square!!!");
                    continue;
                }
                return (amountRows, amountColumns);
            } while (true);
            return (0, 0);
        }

        /// <summary>
        /// Method requests the interval for random generation of matrix elements.
        /// </summary>
        /// <returns>Returns cortege (int, int) - interval for random generation.</returns>
        static (double, double) GetRangeOfRandomNumbers()
        {
            do
            {
                Console.Write("Enter minimum and maximum value for random generating matrix " +
                    "(both numbers must be less than 1e9 by absolute value): ");
                string[] separatedStrings = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (separatedStrings.Length != 2)
                {
                    Console.WriteLine("There are not two numbers!!!");
                    continue;
                }
                double minValue, maxValue;
                if (!double.TryParse(separatedStrings[0], out minValue) || Math.Abs(minValue) > (double)1e9)
                {
                    Console.WriteLine("First number is not correct!!!");
                    continue;
                }
                if (!double.TryParse(separatedStrings[1], out maxValue) || Math.Abs(maxValue) > (double)1e9)
                {
                    Console.WriteLine("Second number is not correct!!!");
                    continue;
                }
                if (minValue > maxValue)
                {
                    Console.WriteLine("Lower bound is more than upper bound!!!");
                    continue;
                }
                return (minValue, maxValue);
            } while (true);
            return (0, 0);
        }

        /// <summary>
        /// Method fills matrix randomly.
        /// </summary>
        /// <param name="amountRows">Amount of rows.</param>
        /// <param name="amountColumns">Amount of columns.</param>
        /// <param name="minMatrixValue">The smallest possible number in matrix.</param>
        /// <param name="maxMatrixValue">The biggest possible number in matrix.</param>
        /// <returns>Returns randomly generated matrix.</returns>
        static double[,] RandomFillMatrix(uint amountRows, uint amountColumns,
            double minMatrixValue, double maxMatrixValue)
        {
            double[,] generatedMatrix = new double[amountRows, amountColumns];
            Random randomNumberGenerator = new Random();
            for (uint indexOfRow = 0; indexOfRow < amountRows; indexOfRow++)
                for (uint indexOfColumn = 0; indexOfColumn < amountColumns; indexOfColumn++)
                    generatedMatrix[indexOfRow, indexOfColumn] = randomNumberGenerator.NextDouble() *
                        (maxMatrixValue - minMatrixValue) + minMatrixValue;
            return generatedMatrix;
        }

        /// <summary>
        /// Method tries to read matrix from file.
        /// </summary>
        /// <param name="amountRows">Amount of rows.</param>
        /// <param name="amountColumns">Amount of columns.</param>
        /// <param name="isInputCorrect">Flag that states if input was finished successfully.</param>
        /// <param name="needToBeSquared">Flag that states if matrix should be square or not.</param>
        /// <returns>Returns matrix if reading finished successfully.</returns>
        static double[,] TryInputMatrix(uint amountRows, uint amountColumns,
            out bool isInputCorrect, bool needToBeSquared)
        {
            double[,] inputMatrix = new double[amountRows, amountColumns];
            isInputCorrect = true;
            Console.WriteLine("Enter matrix row by row (according to matrix size), separate numbers by space.");
            for (uint indexOfRow = 0; indexOfRow < amountRows; indexOfRow++)
            {
                string[] separatedString = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (separatedString.Length != amountColumns)
                {
                    isInputCorrect = false;
                    break;
                }
                for (int indexOfColumn = 0; indexOfColumn < amountColumns; indexOfColumn++)
                {
                    double currentMatrixElement;
                    if (!double.TryParse(separatedString[indexOfColumn], out currentMatrixElement) ||
                        Math.Abs(currentMatrixElement) > (double)1e9)
                    {
                        isInputCorrect = false;
                        break;
                    }
                    inputMatrix[indexOfRow, indexOfColumn] = currentMatrixElement;
                }
                if (!isInputCorrect) break;
            }
            return inputMatrix;
        }

        /// <summary>
        /// Method reads matrix until matrix will not be read successfully.
        /// </summary>
        /// <param name="amountRows">Amount of rows.</param>
        /// <param name="amountColumns">Amount of columns.</param>
        /// <param name="needToBeSquare">Flag that states if input was finished successfully.</param>
        /// <returns>Returns input matrix.</returns>
        static double[,] InputFillMatrix(uint amountRows, uint amountColumns, bool needToBeSquare)
        {
            bool isInputCorrect = false;
            double[,] inputMatrix = new double[amountRows, amountColumns];
            do
            {
                inputMatrix = TryInputMatrix(amountRows, amountColumns, out isInputCorrect, needToBeSquare);
                if (!isInputCorrect) Console.WriteLine("Matrix reading was finished because of incorrect input." +
                    " Input restarted.");
            } while (!isInputCorrect);
            return inputMatrix;
        }

        /// <summary>
        /// Method tries to read matrix from file, if file does not exist returns null.
        /// </summary>
        /// <param name="fileName">Filename.</param>
        /// <param name="needToBeSquared">Flag that states if input was finished successfully.</param>
        /// <returns>Returns input matrix or null if input is not successful.</returns>
        static double[,] TryFileInputMatrix(string fileName, bool needToBeSquared)
        {
            try
            {
                System.IO.StreamReader fileReader = new System.IO.StreamReader(fileName);
                if (fileReader.EndOfStream) return null;
                string[] inputSize = fileReader.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                uint amountOfRows, amountOfColumns;
                if (!uint.TryParse(inputSize[0], out amountOfRows) ||
                    !uint.TryParse(inputSize[1], out amountOfColumns)) return null;
                if (amountOfRows <= 0 || amountOfRows > 20 || amountOfColumns <= 0 || amountOfColumns > 20)
                    return null;
                if (needToBeSquared && amountOfRows != amountOfColumns) return null;
                double[,] matrix = new double[amountOfRows, amountOfColumns];
                for (uint indexOfRow = 0; indexOfRow < amountOfRows; indexOfRow++)
                {
                    if (fileReader.EndOfStream) return null;
                    string[] separatedString = fileReader.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (separatedString.Length != amountOfColumns) return null;
                    for (int indexOfColumn = 0; indexOfColumn < amountOfColumns; indexOfColumn++)
                    {
                        double currentMatrixElement;
                        if (!double.TryParse(separatedString[indexOfColumn], out currentMatrixElement) ||
                            Math.Abs(currentMatrixElement) > (double)1e9) return null;
                        matrix[indexOfRow, indexOfColumn] = currentMatrixElement;
                    }
                }
                return matrix;
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// Method reads the matrix size and matrix itself from console.
        /// </summary>
        /// <param name="needToBeSquared">Flag that states if input was finished successfully.</param>
        /// <returns>Returns matrix.</returns>
        static double[,] TerminalInputMatrix(bool needToBeSquared)
        {
            (uint, uint) matrixSize = GetMatrixSize(needToBeSquared);
            double[,] totalMatrix = InputFillMatrix(matrixSize.Item1, matrixSize.Item2, needToBeSquared);
            Console.WriteLine("The matrix was entered successfully.");
            return totalMatrix;
        }

        /// <summary>
        /// Method reads the matrix size, interval of random numbers and fills matrix randomly.
        /// </summary>
        /// <param name="needToBeSquared">flag that states if input was finished successfully.</param>
        /// <returns>Returns randomly generated matrix.</returns>
        static double[,] TerminalRandomMatrix(bool needToBeSquared)
        {
            (uint, uint) matrixSize = GetMatrixSize(needToBeSquared);
            (double, double) numberRange = GetRangeOfRandomNumbers();
            double[,] totalMatrix = RandomFillMatrix(matrixSize.Item1,
                matrixSize.Item2, numberRange.Item1, numberRange.Item2);
            Console.WriteLine("The matrix was generated successfully: ");
            OutputMatrix(totalMatrix);
            return totalMatrix;
        }

        /// <summary>
        /// Method reads matrix from file.
        /// </summary>
        /// <param name="fileName">Filename.</param>
        /// <param name="needToBeSquared">Flag that states if input was finished successfully.</param>
        /// <returns>Returns matrix or null if input is not successful.</returns>
        static double[,] TerminalFileReadMatrix(string fileName, bool needToBeSquared)
        {
            double[,] matrix = TryFileInputMatrix(fileName, needToBeSquared);
            if (matrix == null) Console.WriteLine("Input type was not recognized!!!");
            else Console.WriteLine("The matrix was read from file successfully.");
            return matrix;
        }

        /// <summary>
        /// Method requests input type for matrix and reads/generates matrix.
        /// </summary>
        /// <param name="needToBeSquared">Flag that states if input was finished successfully.</param>
        /// <returns>Returns input/generated matrix.</returns>
        static double[,] ReadMatrix(bool needToBeSquared)
        {
            double[,] matrix = null;
            do
            {
                string inputType = GetTypeOfMatrixFilling();
                if (inputType == "1") matrix = TerminalInputMatrix(needToBeSquared);
                else if (inputType == "0") matrix = TerminalRandomMatrix(needToBeSquared);
                else matrix = TerminalFileReadMatrix(inputType, needToBeSquared);
            } while (matrix == null);
            return matrix;
        }

        /// <summary>
        /// Method prints matrix beautifully.
        /// </summary>
        /// <param name="matrix">Matrix for printing.</param>
        static void OutputMatrix(double[,] matrix)
        {
            uint[] maxElementLength = new uint[matrix.GetLength(1)];
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfRow++)
                for (uint indexOfColumn = 0; indexOfColumn < matrix.GetLength(1); indexOfColumn++)
                {
                    if (Math.Abs(matrix[indexOfRow, indexOfColumn]) < 1e-4) matrix[indexOfRow, indexOfColumn] = 0;
                    maxElementLength[indexOfColumn] = Math.Max(maxElementLength[indexOfColumn],
                        (uint)(ConvertNumberToString(matrix[indexOfRow, indexOfColumn]).Length) + 1);
                }
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfRow++)
            {
                for (uint indexOfColumn = 0; indexOfColumn < matrix.GetLength(1); indexOfColumn++)
                {
                    Console.Write(ConvertNumberToString(matrix[indexOfRow, indexOfColumn]) + " ");
                    for (uint amountSpace = 0; amountSpace < maxElementLength[indexOfColumn] -
                        ConvertNumberToString(matrix[indexOfRow, indexOfColumn]).Length; amountSpace++)
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Method finds trace of matrix.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <returns>Returns double value, matrix trace.</returns>
        static double CountTrace(double[,] matrix)
        {
            double trace = 0;
            for (uint indexDiagonal = 0; indexDiagonal < matrix.GetLength(0); indexDiagonal++)
                trace += matrix[indexDiagonal, indexDiagonal];
            return trace;
        }

        /// <summary>
        /// Method transposes matrix.
        /// </summary>
        /// <param name="matrix">Matrix to transpose.</param>
        /// <returns>Returns transposed matrix.</returns>
        static double[,] TransposeMatrix(double[,] matrix)
        {
            double[,] transponsedMatrix = new double[matrix.GetLength(1), matrix.GetLength(0)];
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfRow++)
                for (uint indexOfColumn = 0; indexOfColumn < matrix.GetLength(1); indexOfColumn++)
                    transponsedMatrix[indexOfColumn, indexOfRow] = matrix[indexOfRow, indexOfColumn];
            return transponsedMatrix;
        }

        /// <summary>
        /// Method counts sum of two matrices.
        /// </summary>
        /// <param name="firstMatrix">Matrix, first term.</param>
        /// <param name="secondMatrix">Matrix, second term.</param>
        /// <returns>Returns sum of two matrices.</returns>
        static double[,] GetMatrixSum(double[,] firstMatrix, double[,] secondMatrix)
        {
            double[,] sumOfMatrices = new double[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];
            for (uint indexOfRow = 0; indexOfRow < firstMatrix.GetLength(0); indexOfRow++)
                for (uint indexOfColumn = 0; indexOfColumn < firstMatrix.GetLength(1); indexOfColumn++)
                    sumOfMatrices[indexOfRow, indexOfColumn] = firstMatrix[indexOfRow, indexOfColumn] +
                        secondMatrix[indexOfRow, indexOfColumn];
            return sumOfMatrices;
        }

        /// <summary>
        /// Method counts difference of two matrices.
        /// </summary>
        /// <param name="firstMatrix">Matrix, minoend.</param>
        /// <param name="secondMatrix">Matrix, subtrahend.</param>
        /// <returns>Returns difference of two matrices.</returns>
        static double[,] GetMatrixDifference(double[,] firstMatrix, double[,] secondMatrix)
        {
            double[,] differenceOfMatrices = new double[firstMatrix.GetLength(0), firstMatrix.GetLength(1)];
            for (uint indexOfRow = 0; indexOfRow < firstMatrix.GetLength(0); indexOfRow++)
                for (uint indexOfColumn = 0; indexOfColumn < firstMatrix.GetLength(1); indexOfColumn++)
                    differenceOfMatrices[indexOfRow, indexOfColumn] = firstMatrix[indexOfRow, indexOfColumn] -
                        secondMatrix[indexOfRow, indexOfColumn];
            return differenceOfMatrices;
        }

        /// <summary>
        /// Method counts product of two matrices.
        /// </summary>
        /// <param name="firstMatrix">First matrix.</param>
        /// <param name="secondMatrix">Second matrix.</param>
        /// <returns>Returns product of two matrices.</returns>
        static double[,] GetMatrixProduct(double[,] firstMatrix, double[,] secondMatrix)
        {
            double[,] multiplicationOfMatrices = new double[firstMatrix.GetLength(0), secondMatrix.GetLength(1)];
            for (uint indexOfRow = 0; indexOfRow < firstMatrix.GetLength(0); indexOfRow++)
                for (uint indexOfColumn = 0; indexOfColumn < secondMatrix.GetLength(1); indexOfColumn++)
                    for (uint indexScaledSpace = 0; indexScaledSpace < firstMatrix.GetLength(1); indexScaledSpace++)
                        multiplicationOfMatrices[indexOfRow, indexOfColumn] += firstMatrix[indexOfRow, 
                            indexScaledSpace] * secondMatrix[indexScaledSpace, indexOfColumn];
            return multiplicationOfMatrices;
        }

        /// <summary>
        /// Method counts matrix and number product.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <param name="multiplier">Number, multiplier.</param>
        /// <returns>Returns product of matrix and number.</returns>
        static double[,] GetMatrixAndNumberProduct(double[,] matrix, double multiplier)
        {
            double[,] answer = new double[matrix.GetLength(0), matrix.GetLength(1)];
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfRow++)
                for (uint indexOfColumn = 0; indexOfColumn < matrix.GetLength(1); indexOfColumn++)
                    answer[indexOfRow, indexOfColumn] = matrix[indexOfRow, indexOfColumn] * multiplier;
            return answer;
        }

        /// <summary>
        /// Method swaps two rows in matrix.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <param name="firstRow">Index of first row.</param>
        /// <param name="secondRow">Index of second row.</param>
        static void SwapRows(double[,] matrix, uint firstRow, uint secondRow)
        {
            for (uint indexOfColumn = 0; indexOfColumn < matrix.GetLength(1); indexOfColumn++)
            {
                double buffer = matrix[firstRow, indexOfColumn];
                matrix[firstRow, indexOfColumn] = matrix[secondRow, indexOfColumn];
                matrix[secondRow, indexOfColumn] = buffer;
            }
        }

        /// <summary>
        /// Method counts matrix determinant with using Gauss method.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <returns>Returns float number, determinant value.</returns>
        static double CountDeterminant(double[,] matrix)
        {
            double determinant = 1.0;
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfRow++)
            {
                //Finds row with the most absolute value.
                uint maxAbsIndex = indexOfRow;
                for (uint indexMaxColumn = indexOfRow + 1; indexMaxColumn < matrix.GetLongLength(0); indexMaxColumn++)
                    if (Math.Abs(matrix[indexMaxColumn, indexOfRow]) > Math.Abs(matrix[maxAbsIndex, indexOfRow]))
                        maxAbsIndex = indexMaxColumn;
                if (Math.Abs(matrix[maxAbsIndex, indexOfRow]) < 1e-8)
                {
                    determinant = 0;
                    break;
                }
                SwapRows(matrix, indexOfRow, maxAbsIndex);
                if (indexOfRow != maxAbsIndex) determinant = -determinant;
                determinant *= matrix[indexOfRow, indexOfRow];
                //Scaling every row by [indexOfRow, indexOfRow] value.
                for (uint indexOfColumn = indexOfRow + 1; indexOfColumn < matrix.GetLength(1); indexOfColumn++)
                    matrix[indexOfRow, indexOfColumn] /= matrix[indexOfRow, indexOfRow];
                //Zeroing indexOfRow column.
                for (uint indexOfRow2 = 0; indexOfRow2 < matrix.GetLength(0); indexOfRow2++)
                    if (indexOfRow2 != indexOfRow && Math.Abs(matrix[indexOfRow2, indexOfRow]) > 1e-8)
                        for (uint indexOfColumn = indexOfRow + 1; indexOfColumn < matrix.GetLength(1); indexOfColumn++)
                            matrix[indexOfRow2, indexOfColumn] -= matrix[indexOfRow, indexOfColumn] *
                                matrix[indexOfRow2, indexOfRow];
            }
            return determinant;
        }

        /// <summary>
        /// Method solves SLAE with using Gauss method.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        static void SolveSLAE(double[,] matrix)
        {
            uint indexOfColumn = 0;
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfColumn++)
            {
                if (indexOfColumn >= matrix.GetLength(1) - 1) break;
                //Finds row with the most absolute value.
                uint idxMaxVal = indexOfRow;
                for (uint indexOfRow2 = indexOfRow; indexOfRow2 < matrix.GetLength(0); indexOfRow2++)
                    if (Math.Abs(matrix[indexOfRow2, indexOfColumn]) > Math.Abs(matrix[idxMaxVal, indexOfColumn]))
                        idxMaxVal = indexOfRow2;
                if (Math.Abs(matrix[idxMaxVal, indexOfColumn]) < 1e-8) continue;
                if (indexOfRow != idxMaxVal) SwapRows(matrix, indexOfRow, idxMaxVal);
                //Zeroing indexOfRow column.
                for (uint indexOfRow2 = 0; indexOfRow2 < matrix.GetLength(0); indexOfRow2++)
                    if (indexOfRow2 != indexOfRow)
                    {
                        double scaleCoeff = matrix[indexOfRow2, indexOfColumn] / matrix[indexOfRow, indexOfColumn];
                        for (uint indexOfColumn2 = 0; indexOfColumn2 < matrix.GetLength(1); indexOfColumn2++)
                            matrix[indexOfRow2, indexOfColumn2] -= matrix[indexOfRow, indexOfColumn2] * scaleCoeff;
                    }
                indexOfRow++;
            }
            //Transforming matrix to canonical form.
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfRow++)
            {
                double leader = 0;
                for (indexOfColumn = 0; indexOfColumn < matrix.GetLength(1); indexOfColumn++)
                {
                    if (Math.Abs(leader) < 1e-8 && Math.Abs(matrix[indexOfRow, indexOfColumn]) > 1e-8)
                        leader = matrix[indexOfRow, indexOfColumn];
                    if (Math.Abs(leader) > 1e-8) matrix[indexOfRow, indexOfColumn] /= leader;
                }
            }
        }

        /// <summary>
        /// Method checks if SLAE has solution.
        /// </summary>
        /// <param name="matrix">Matrix.</param>
        /// <returns>Boolean value that states if matrix has solution.</returns>
        static bool CheckSolutionExists(double[,] matrix)
        {
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfRow++)
            {
                bool nonZeroVariable = false;
                for (uint indexOfColumn = 0; indexOfColumn < matrix.GetLength(1) - 1; indexOfColumn++)
                    if (Math.Abs(matrix[indexOfRow, indexOfColumn]) > 1e-9) nonZeroVariable = true;
                if (!nonZeroVariable && Math.Abs(matrix[indexOfRow, matrix.GetLength(1) - 1]) > 1e-9) return false;
            }
            return true;
        }

        /// <summary>
        /// Method returns array of boolean values (is variable free).
        /// </summary>
        /// <param name="matrix">Matrix after applying Gauss method.</param>
        /// <returns>Returns array of boolean values.</returns>
        static bool[] GetParametricVariables(double[,] matrix)
        {
            bool[] parametricVariable = new bool[matrix.GetLength(1) - 1];
            for (uint indexOfRow = 0, wasLeader = 0; indexOfRow < matrix.GetLength(0); indexOfRow++, wasLeader = 0)
                for (uint indexOfColumn = 0; indexOfColumn < matrix.GetLength(1) - 1; indexOfColumn++)
                {
                    if (Math.Abs(matrix[indexOfRow, indexOfColumn]) > 1e-9 && wasLeader > 0)
                        parametricVariable[indexOfColumn] = true;
                    if (Math.Abs(matrix[indexOfRow, indexOfColumn]) > 1e-9) wasLeader++;
                }
            return parametricVariable;
        }

        /// <summary>
        /// Method prints SLAE solutions.
        /// </summary>
        /// <param name="matrix">Matrix after applying Gauss method.</param>
        static void OutputSolution(double[,] matrix)
        {
            for (uint indexOfRow = 0; indexOfRow < matrix.GetLength(0); indexOfRow++)
            {
                bool wasLeader = false;
                for (uint indexOfColumn = 0; indexOfColumn < matrix.GetLength(1) - 1; indexOfColumn++)
                    if (Math.Abs(matrix[indexOfRow, indexOfColumn]) > 1e-9)
                    {
                        if (wasLeader)
                        {
                            if (matrix[indexOfRow, indexOfColumn] < 0) Console.Write("+");
                            Console.Write($"{ConvertNumberToString(-matrix[indexOfRow, indexOfColumn])}" +
                            $"*X{indexOfColumn + 1}");
                        }
                        else
                        {
                            wasLeader = true;
                            Console.Write($"X{indexOfColumn + 1}=" +
                                $"{ConvertNumberToString(matrix[indexOfRow, matrix.GetLength(1) - 1])}");
                        }
                    }
                if (wasLeader) Console.WriteLine();
            }
        }

        /// <summary>
        /// Method prints all solutions of SLAE.
        /// </summary>
        /// <param name="matrix">Matrix after applying Gauss method.</param>
        static void ShowSolutionOfSLAE(double[,] matrix)
        {
            Console.WriteLine("Solution of SLAE: ");
            if (!CheckSolutionExists(matrix))
            {
                Console.WriteLine("This SLAE has no solutions.");
                return;
            }
            uint amountParametricVariable = 0;
            bool[] parametricVariable = GetParametricVariables(matrix);
            for (uint indexOfColumn = 0; indexOfColumn < parametricVariable.Length; indexOfColumn++)
                if (parametricVariable[indexOfColumn]) amountParametricVariable++;
            if (amountParametricVariable > 0)
            {
                Console.Write("For all variables values ");
                for (uint indexOfColumn = 0; indexOfColumn < parametricVariable.Length; indexOfColumn++)
                    if (parametricVariable[indexOfColumn]) Console.Write($"X{indexOfColumn + 1} ");
                Console.WriteLine();
            }
            OutputSolution(matrix);
        }

        /// <summary>
        /// Method requests command for matrix calculator until method receives first correct command.
        /// </summary>
        /// <returns>Returns first correct command for calculator.</returns>
        static string GetCalculatorCommand()
        {
            string inputCommand;
            do
            {
                Console.Write("Choose command (to get list of commands enter 'info'): ");
                inputCommand = Console.ReadLine();
                if (inputCommand == "trace" || inputCommand == "transponse" || inputCommand == "sum" ||
                    inputCommand == "diff" || inputCommand == "multmatrix" || inputCommand == "multnum" ||
                    inputCommand == "det" || inputCommand == "exit" || inputCommand == "info" ||
                    inputCommand == "slae") break;
                Console.WriteLine("Input command was not recognized!!!");
            } while (true);
            return inputCommand;
        }

        /// <summary>
        /// Terminal of trace method.
        /// </summary>
        static void TerminalTrace()
        {
            Console.WriteLine("You need to enter one matrix.");
            double[,] matrix = ReadMatrix(true);
            double traceValue = CountTrace(matrix);
            Console.WriteLine($"The trace of matrix: {ConvertNumberToString(traceValue)}.");
        }

        /// <summary>
        /// Terminal of transpose method.
        /// </summary>
        static void TerminalTransposal()
        {
            Console.WriteLine("You need to enter one matrix.");
            double[,] matrix = ReadMatrix(false);
            double[,] matrixTransponsed = TransposeMatrix(matrix);
            Console.WriteLine("This matrix but transposed: ");
            OutputMatrix(matrixTransponsed);
        }

        /// <summary>
        /// Terminal of sum method.
        /// </summary>
        static void TerminalSumOfMatrices()
        {
            Console.WriteLine("You need to enter 2 matrices.");
            double[,] firstMatrix = ReadMatrix(false), secondMatrix = ReadMatrix(false);
            while (firstMatrix.GetLength(0) != secondMatrix.GetLength(0) ||
                firstMatrix.GetLength(1) != secondMatrix.GetLength(1))
            {
                Console.WriteLine("Matrices have difeerent sizes!!! Repeat matrices input.");
                firstMatrix = ReadMatrix(false);
                secondMatrix = ReadMatrix(false);
            }
            double[,] sumOfMatrices = GetMatrixSum(firstMatrix, secondMatrix);
            Console.WriteLine("The sum of matrices: ");
            OutputMatrix(sumOfMatrices);
        }

        /// <summary>
        /// Terminal of difference method.
        /// </summary>
        static void TerminalDifferenceOfMatrices()
        {
            Console.WriteLine("You need to enter 2 matrices (former - decreasing, latter - subtraction).");
            double[,] firstMatrix = ReadMatrix(false), secondMatrix = ReadMatrix(false);
            while (firstMatrix.GetLength(0) != secondMatrix.GetLength(0) ||
                firstMatrix.GetLength(1) != secondMatrix.GetLength(1))
            {
                Console.WriteLine("Matrices have difeerent sizes!!! Repeat matrices input.");
                firstMatrix = ReadMatrix(false);
                secondMatrix = ReadMatrix(false);
            }
            double[,] differenceOfMatrices = GetMatrixDifference(firstMatrix, secondMatrix);
            Console.WriteLine("Matrices difference: ");
            OutputMatrix(differenceOfMatrices);
        }

        /// <summary>
        /// Terminal of matrices product method.
        /// </summary>
        static void TerminalProductOfMatrices()
        {
            Console.WriteLine("You need to enter 2 matrices.");
            double[,] firstMatrix = ReadMatrix(false), secondMatrix = ReadMatrix(false);
            while (firstMatrix.GetLength(1) != secondMatrix.GetLength(0))
            {
                Console.WriteLine("Sizes of matrices are such that they can not be producted!!! " +
                    "Repeat matrices input.");
                firstMatrix = ReadMatrix(false);
                secondMatrix = ReadMatrix(false);
            }
            double[,] matrixMultiplication = GetMatrixProduct(firstMatrix, secondMatrix);
            Console.WriteLine("Product of two matrices: ");
            OutputMatrix(matrixMultiplication);
        }

        /// <summary>
        /// Terminal of matrix and number prouct method.
        /// </summary>
        static void TerminalProductMatrixAndNumber()
        {
            Console.WriteLine("You need to enter matrix and after that number.");
            double[,] matrix = ReadMatrix(false);
            double multiplier;
            do
            {
                Console.Write("Enter number (absolute value must be less than 1e9): ");
                string inputString = Console.ReadLine();
                if (double.TryParse(inputString, out multiplier))
                {
                    if (Math.Abs(multiplier) <= 1e9) break;
                }
            } while (true);
            double[,] answer = GetMatrixAndNumberProduct(matrix, multiplier);
            Console.WriteLine("Product of matrices: ");
            OutputMatrix(answer);
        }

        /// <summary>
        /// Terminal of counting determinant method.
        /// </summary>
        static void TerminalGetMatrixDeterminant()
        {
            Console.WriteLine("You need to enter matrix.");
            double[,] matrix = ReadMatrix(true);
            double det = CountDeterminant(matrix);
            if (Math.Abs(det) >= 1e9 && false)
                Console.WriteLine($"The determinant of matrix: {ConvertNumberToString(det)}.");
            else Console.WriteLine($"The determinant of matrix: {ConvertNumberToString(det)}.");
        }

        /// <summary>
        /// Terminal of solving SLAE method.
        /// </summary>
        static void TerminalSolveSLAE()
        {
            Console.WriteLine("You need to enter SLAE as matrix.");
            double[,] matrix = ReadMatrix(false);
            SolveSLAE(matrix);
            Console.WriteLine("Solution of SLAE:");
            OutputMatrix(matrix);
            ShowSolutionOfSLAE(matrix);
        }

        /// <summary>
        /// Method shows info menu.
        /// </summary>
        static void ShowCalculatorInformation()
        {
            Console.WriteLine("Welcome to the matrix calculator. It supports following operations: ");
            Console.WriteLine("\tTo see info enter 'info';\n\tTo find trace enter 'trace';\n\tTo transpose" +
                " matrix enter 'transponse';\n\tTo find sum of matrices enter 'sum';\n\tTo find difference of" +
                " matrices enter 'diff';\n\tTo find matrices product enter 'multmatrix';\n\tTo find " +
                "product of matrix and number enter 'multnum';\n\tTo find determinant enter 'det';\n\tTo " +
                "get SLAE solution enter 'slae';\n\tTo exit program enter 'exit'.");
            Console.WriteLine("Remember that accuracy of calculations is no more than 4 digits after comma.");
        }

        /// <summary>
        /// Method executes command depending on command name.
        /// </summary>
        /// <returns>Returns boolean value that states if program should continue working.</returns>
        static bool CalculatorTerminal()
        {
            string calculatorCommand = GetCalculatorCommand();
            if (calculatorCommand == "trace") TerminalTrace();
            else if (calculatorCommand == "transponse") TerminalTransposal();
            else if (calculatorCommand == "sum") TerminalSumOfMatrices();
            else if (calculatorCommand == "diff") TerminalDifferenceOfMatrices();
            else if (calculatorCommand == "multmatrix") TerminalProductOfMatrices();
            else if (calculatorCommand == "multnum") TerminalProductMatrixAndNumber();
            else if (calculatorCommand == "det") TerminalGetMatrixDeterminant();
            else if (calculatorCommand == "exit") return false;
            else if (calculatorCommand == "info") ShowCalculatorInformation();
            else if (calculatorCommand == "slae") TerminalSolveSLAE();
            else Console.WriteLine("Input command was not recognized!!!");
            return true;
        }

        /// <summary>
        /// The main method of program.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        static void Main(string[] args)
        {
            while (CalculatorTerminal()) ;
        }
    }
}
