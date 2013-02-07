using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Consulator
{
    class Consulator
    {
        //Difference between operators and functions: operators cannot be consecutive,
        //Meaning operator-operator fails.
        public static string[] OPERATORS = new string[6];
        public static List<string> functions = new List<string>();

        public static void Main()
        {
            OPERATORS[0] = "^";
            OPERATORS[1] = "~";
            OPERATORS[2] = "*";
            OPERATORS[3] = "/";
            OPERATORS[4] = "%";
            OPERATORS[5] = "+";

            functions.Add("-");
            functions.Add("(");
            functions.Add(")");

            Console.WriteLine("Console Calculator v.0.0.2\n");

            while (true)
            {
                Console.WriteLine();
                Console.Write(" >> ");
                string input = Console.ReadLine();

                List<string> inputList = new List<string>();
                inputList.Add("");

                //First general pass while throwing it into a List.
                //Only catches unrecognized characters.
                for (int i = 0; i < input.Length && !Error.isRaised; ++i)
                {
                    if (input[i] >= '0' && input[i] <= '9' || input[i] == '.')
                    { inputList[inputList.Count - 1] += input[i]; }
                    else
                    {
                        if (checkIfOperator(input[i].ToString()))
                        { inputList.Add(input[i].ToString()); }
                        else if (checkIfFunction(input[i].ToString()))
                        { inputList.Add(input[i].ToString()); }
                        else if (input[i] == ' ')
                        { } //DO NOTHING. DO NOT ADD ANYTHING. THIS COULD BE INVALID THO. TODO
                        else
                        { Error.raise(i, "Syntax Error: Unrecognized symbol '" + input[i] + "'."); }

                        inputList.Add("");
                    }
                }

                while (inputList.Contains(""))
                { inputList.Remove(""); }
                List<List<string>> inputStack = new List<List<string>>();
                inputStack.Add(new List<string>());
                int stackCount = 0;

                if (inputList.Count == 0)
                { Error.raise("You didn't type anything."); }

                //Second pass for syntax errors.
                if (!Error.isRaised)
                {
                    //Check if there's an operator at the front.
                    if (checkIfOperator(inputList[0]))
                    { Error.raise(0, "Syntax Error: Cannot begin with an operator."); } //TODO rewrite

                    for (int i = 0; i < inputList.Count && !Error.isRaised; ++i)
                    {
                        //Check if consecutive operators.
                        if (checkIfOperator(inputList[i]))
                        {
                            if (i == inputList.Count - 1)
                            { Error.raise(getLocation(inputList, i), "Syntax Error: Cannot end with an operator."); }
                            else if (checkIfOperator(inputList[i + 1]))
                            { Error.raise(getLocation(inputList, i + 1), "Syntax Error: Cannot have consecutive operators."); } //TODO rewrite
                            else
                            { inputStack[stackCount].Add(inputList[i]); }
                        }
                        else if (checkIfFunction(inputList[i]))
                        {
                            if (inputList[i] == "(")
                            {
                                if (i != 0)
                                {
                                    if (!checkIfOperator(inputList[i - 1]) && !checkIfFunction(inputList[i - 1]))
                                    { inputStack[stackCount].Add("*"); }
                                }

                                if (i != inputList.Count - 1 && checkIfOperator(inputList[i + 1]))
                                { Error.raise(getLocation(inputList, i + 1), "Syntax Error: Cannot begin parentheses with an operator."); }

                                inputStack[stackCount].Add(inputList[i]);
                                ++stackCount;

                                if (inputStack.Count == stackCount)
                                { inputStack.Add(new List<string>()); }
                                else
                                { inputStack[stackCount].Add("$"); }                                
                            }
                            else if (inputList[i] == ")")
                            {
                                if (i != 0)
                                {
                                    if (checkIfOperator(inputList[i - 1]))
                                    { Error.raise(getLocation(inputList, i - 1), "Syntax Error: Cannot end parentheses with an operator."); }
                                    else if (inputList[i - 1] == "(" || inputList[i - 1] == ")")
                                    { }
                                    else if (checkIfFunction(inputList[i - 1]))
                                    { Error.raise(getLocation(inputList, i - 1), "Syntax Error: Cannot end parentheses with an operator."); }
                                }

                                if (stackCount > 0)
                                {
                                    --stackCount;
                                    inputStack[stackCount].Add(inputList[i]);
                                }
                                else
                                { Error.raise(getLocation(inputList, i), "Syntax Error: Too many end parentheses."); } //TODO rewrite

                                if (i != inputList.Count - 1)
                                {
                                    if (inputList[i + 1] == "(")
                                    { }
                                    else if (!checkIfOperator(inputList[i + 1]) && !checkIfFunction(inputList[i + 1]))
                                    { inputStack[stackCount].Add("*"); }
                                }
                            }
                            else if (inputList[i] == "-")
                            {                                
                                if (i == inputList.Count - 1)
                                { Error.raise(getLocation(inputList, i), "Syntax Error: Cannot end with an operator."); } //TODO rewrite
                                else if (checkIfOperator(inputList[i + 1]))
                                { Error.raise(getLocation(inputList, i + 1), "Syntax Error: Cannot have consecutive operators."); } //TODO rewrite
                                else if (i == 0 || checkIfOperator(inputList[i - 1]))
                                { inputStack[stackCount].Add("~"); }
                                else if (inputList[i - 1] == ")")
                                { inputStack[stackCount].Add("-"); }
                                else if (checkIfFunction(inputList[i - 1]))
                                { inputStack[stackCount].Add("~");}
                                else
                                { inputStack[stackCount].Add("-"); }
                            }
                        }
                        else
                        {
                            //Check if too many decimal places (in a string).
                            if (stringContains(inputList[i], ".") > 1)
                            { Error.raise(getLocation(inputList, i, inputList[i].LastIndexOf(".")), "Syntax Error: Too many decimal points"); } //TODO rewrite
                            else
                            { inputStack[stackCount].Add(inputList[i]); }
                        }
                    }

                    for (int i = 0; i < stackCount; ++i)
                    { inputStack[i].Add(")"); }
                }

                //Finally calculate.
                if (!Error.isRaised)
                {
                    Console.Write("\n\t");
                    for (int i = inputStack.Count - 1; i >= 0; --i)
                    {
                        //Console.Write('\t');
                        //for (int j = 0; j < inputStack[i].Count; ++j)
                        //{ Console.Write(inputStack[i][j]); }
                        //Console.WriteLine("\n");

                        for (int j = 0; j < inputStack[i].Count; ++j)
                        {
                            if (inputStack[i][j] == "(")
                            {
                                if (inputStack[i + 1].Count != 0)
                                {
                                    inputStack[i][j] = inputStack[i + 1][0];
                                    inputStack[i + 1].RemoveAt(0);
                                    if (inputStack[i + 1].Count > 1)
                                    { inputStack[i + 1].RemoveAt(0); }
                                }
                                else
                                { inputStack[i][j] = "0"; } //TODO what should happen here?
                                inputStack[i].RemoveAt(j + 1);
                            }
                        }

                        for (int j = 0; j < inputStack[i].Count; ++j)
                        {
                            if (inputStack[i][j] == "^")
                            {
                                if (inputStack[i][j + 1] == "~")
                                {
                                    inputStack[i][j + 2] = (Convert.ToDouble(inputStack[i][j + 2]) * -1).ToString();
                                    inputStack[i].RemoveAt(j + 1);
                                }
                            }
                        }

                        for (int j = 0; j < inputStack[i].Count; ++j)
                        {
                            if (inputStack[i][j] == "^")
                            {
                                inputStack[i][j - 1] = Math.Pow(Convert.ToDouble(inputStack[i][j - 1]), Convert.ToDouble(inputStack[i][j + 1])).ToString();
                                inputStack[i].RemoveAt(j + 1);
                                inputStack[i].RemoveAt(j);
                            }
                        }

                        for (int j = 0; j < inputStack[i].Count; ++j)
                        {
                            if (inputStack[i][j] == "~")
                            {
                                inputStack[i][j + 1] = (Convert.ToDouble(inputStack[i][j + 1]) * -1).ToString();
                                inputStack[i].RemoveAt(j);
                            }
                        }

                        for (int j = 0; j < inputStack[i].Count; ++j)
                        {
                            if (inputStack[i][j] == "*")
                            {
                                inputStack[i][j - 1] = (Convert.ToDouble(inputStack[i][j - 1]) * Convert.ToDouble(inputStack[i][j + 1])).ToString();
                                inputStack[i].RemoveAt(j + 1);
                                inputStack[i].RemoveAt(j);
                            }
                            else if (inputStack[i][j] == "/")
                            {
                                inputStack[i][j - 1] = (Convert.ToDouble(inputStack[i][j - 1]) / Convert.ToDouble(inputStack[i][j + 1])).ToString();
                                inputStack[i].RemoveAt(j + 1);
                                inputStack[i].RemoveAt(j);
                            }
                            else if (inputStack[i][j] == "%")
                            {
                                inputStack[i][j - 1] = (Convert.ToDouble(inputStack[i][j - 1]) % Convert.ToDouble(inputStack[i][j + 1])).ToString();
                                inputStack[i].RemoveAt(j + 1);
                                inputStack[i].RemoveAt(j);
                            }
                        }

                        for (int j = 0; j < inputStack[i].Count; ++j)
                        {
                            if (inputStack[i][j] == "+")
                            {
                                inputStack[i][j - 1] = (Convert.ToDouble(inputStack[i][j - 1]) + Convert.ToDouble(inputStack[i][j + 1])).ToString();
                                inputStack[i].RemoveAt(j + 1);
                                inputStack[i].RemoveAt(j);
                            }
                            else if (inputStack[i][j] == "-")
                            {
                                inputStack[i][j - 1] = (Convert.ToDouble(inputStack[i][j - 1]) - Convert.ToDouble(inputStack[i][j + 1])).ToString();
                                inputStack[i].RemoveAt(j + 1);
                                inputStack[i].RemoveAt(j);
                            }
                        }
                    }

                    Console.WriteLine(inputStack[0][0]);
                }
                else
                { Console.WriteLine(Error.get()); }
            }
        }

        public static bool checkIfOperator(string checkString)
        {
            for (int i = 0; i < OPERATORS.Length; ++i)
            {
                if (OPERATORS[i] == checkString)
                { return true; }
            }
            return false;
        }

        public static bool checkIfFunction(string checkString)
        {
            for (int i = 0; i < functions.Count; ++i)
            {
                if (functions[i] == checkString)
                { return true; }
            }
            return false;
        }

        public static int stringContains(string checkString, string value)
        {
            int count = 0;
            for (int i = 0; i < checkString.Length; ++i)
            {
                bool stillGood = true;
                for (int j = 0; j < value.Length && stillGood; ++j)
                {
                    if (checkString[i + j] != value[j])
                    { stillGood = false; }
                }

                if (stillGood)
                { ++count; }
            }

            return count;
        }

        public static int getLocation(List<string> stringList, int inList)
        {
            int location = 0;
            for (int i = 0; i < inList; ++i)
            {
                for (int j = 0; j < stringList[i].Length; ++j)
                { ++location; }
            }
            return location;
        }

        public static int getLocation(List<string> stringList, int inList, int inString)
        {
            int location = getLocation(stringList, inList);

            location += inString;

            return location;
        }
    }
}
