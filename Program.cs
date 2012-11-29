using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Consulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Console Calculator v.0.0.1\n");

            bool quit = false;
            //Main Loop
            while (!quit)
            {
                Console.WriteLine();
                Console.Write(" >> ");
                string get = Console.ReadLine();

                List<string> figures = new List<string>();
                List<int> digits = new List<int>();

                bool gotDecimal = false;
                bool isLastOperator = true;

                //What's in that string?
                for (int i = 0; i < get.Length && !Error.newError; i++)
                {
                    if (get[i] >= 48 && get[i] <= 57)
                    {
                        digits.Add(Convert.ToInt32(get[i] - 48));
                        isLastOperator = false;
                    }
                    else
                    {
                        if (gotDecimal)
                        {
                            double temp = 0;
                            for (int j = 0; j < digits.Count; j++)
                            { temp += (double)digits[j] * Math.Pow(10, -(j + 1)); }
                            figures[figures.Count - 1] = (Convert.ToInt32(figures[figures.Count - 1]) + temp).ToString();
                            digits = new List<int>();

                            gotDecimal = false;
                        }
                        else if (digits.Count != 0)
                        { resetDigits(digits, figures); }

                        if (get[i] == '.')
                        {
                            if (!gotDecimal)
                            { gotDecimal = true; }
                            else
                            { Error.makeError("Syntax error.", i); }
                            isLastOperator = false;
                        }
                        else if (get[i] == '+' || get[i] == '*' || get[i] == '/' || get[i] == '^')
                        {
                            figures.Add(get[i].ToString());
                            if (!isLastOperator)
                            { isLastOperator = true; }
                            else
                            { Error.makeError("Syntax error.", i); }
                        }
                        else if (get[i] == '(' || get[i] == ')')
                        {
                            try
                            {
                                if (get[i] == '(')
                                {
                                    if (!(get[i - 1] == '+' || get[i - 1] == '*' || get[i - 1] == '/' || get[i - 1] == '-' || get[i - 1] == '^'))
                                    {
                                        figures.Add("*");
                                        figures.Add("(");
                                    }
                                    else
                                    { figures.Add("("); }
                                }

                                if (get[i] == ')')
                                {
                                    if (!(get[i + 1] == '+' || get[i + 1] == '*' || get[i + 1] == '/' || get[i + 1] == '-' || get[i + 1] == '^'))
                                    {
                                        figures.Add(")");
                                        figures.Add("*");
                                    }
                                    else
                                    { figures.Add(")"); }
                                }
                            }
                            catch (IndexOutOfRangeException)
                            { figures.Add(get[i].ToString()); }
                        }
                        else if (get[i] == '-')
                        {
                            try
                            {
                                if (isLastOperator || figures[figures.Count - 1] == "(" || figures[figures.Count - 1] == ")")
                                {
                                    figures.Add("0");
                                    figures.Add(",");
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            { figures.Add("-"); }
                            if (!isLastOperator)
                            { figures.Add("-"); }
                            isLastOperator = true;
                        }
                        else if (get[i] == ' ')
                        { }
                        else
                        { Error.makeError("Invalid character.", i); }
                    }
                }

                if (gotDecimal)
                {
                    double temp = 0;
                    for (int j = 0; j < digits.Count; j++)
                    { temp += (double)digits[j] * Math.Pow(10, -(j + 1)); }
                    figures[figures.Count - 1] = (Convert.ToInt32(figures[figures.Count - 1]) + temp).ToString();
                    digits = new List<int>();

                    gotDecimal = false;
                }

                resetDigits(digits, figures);

                if (!Error.newError)
                {
                    List<List<string>> expressions = new List<List<string>>();


                    for (int i = figures.Count - 1; i >= 0; i--)
                    {
                        if (figures[i] == "(")
                        {
                            expressions.Add(new List<string>());
                            figures.RemoveAt(i);
                            try
                            {
                                while (figures[i] != ")")
                                {
                                    expressions[expressions.Count - 1].Add(figures[i]);
                                    figures.RemoveAt(i);
                                }
                                figures[i] = "x";
                            }
                            catch (ArgumentOutOfRangeException)
                            { Error.makeError("Missing closing parenthesis."); }
                        }
                    }
                    expressions.Add(figures);

                    for (int j = 0; j < expressions.Count; j++)
                    {
                        for (int i = 0; i < expressions[j].Count; i++)
                        {
                            if (expressions[j][i] == "x")
                            { expressions[j][i] = expressions[j - 1][0]; }
                        }

                        for (int i = 0; i < expressions[j].Count; i++)
                        {
                            if (expressions[j][i] == ",")
                            {
                                double temp = Convert.ToDouble(expressions[j][i - 1]) - Convert.ToDouble(expressions[j][i + 1]);
                                expressions[j].RemoveAt(i);
                                expressions[j].RemoveAt(i);
                                expressions[j][i - 1] = temp.ToString();
                                i--;
                            }
                        }

                        for (int i = 0; i < expressions[j].Count; i++)
                        {
                            if (expressions[j][i] == "^")
                            {
                                double temp = Math.Pow(Convert.ToDouble(expressions[j][i - 1]), Convert.ToDouble(expressions[j][i + 1]));
                                expressions[j].RemoveAt(i);
                                expressions[j].RemoveAt(i);
                                expressions[j][i - 1] = temp.ToString();
                                i--;
                            }
                        }

                        for (int i = 0; i < expressions[j].Count; i++)
                        {
                            if (expressions[j][i] == "*")
                            {
                                double temp = Convert.ToDouble(expressions[j][i - 1]) * Convert.ToDouble(expressions[j][i + 1]);
                                expressions[j].RemoveAt(i);
                                expressions[j].RemoveAt(i);
                                expressions[j][i - 1] = temp.ToString();
                                i--;
                            }
                        }

                        for (int i = 0; i < expressions[j].Count; i++)
                        {
                            if (expressions[j][i] == "/")
                            {
                                double temp = Convert.ToDouble(expressions[j][i - 1]) / Convert.ToDouble(expressions[j][i + 1]);
                                expressions[j].RemoveAt(i);
                                expressions[j].RemoveAt(i);
                                expressions[j][i - 1] = temp.ToString();
                                i--;
                            }
                        }

                        for (int i = 0; i < expressions[j].Count; i++)
                        {
                            if (expressions[j][i] == "+")
                            {
                                double temp = Convert.ToDouble(expressions[j][i - 1]) + Convert.ToDouble(expressions[j][i + 1]);
                                expressions[j].RemoveAt(i);
                                expressions[j].RemoveAt(i);
                                expressions[j][i - 1] = temp.ToString();
                                i--;
                            }
                        }

                        for (int i = 0; i < expressions[j].Count; i++)
                        {
                            if (expressions[j][i] == "-")
                            {
                                double temp = Convert.ToDouble(expressions[j][i - 1]) - Convert.ToDouble(expressions[j][i + 1]);
                                expressions[j].RemoveAt(i);
                                expressions[j].RemoveAt(i);
                                expressions[j][i - 1] = temp.ToString();
                                i--;
                            }
                        }
                    }

                    if (!Error.newError)
                    {
                        Console.Write("    = ");
                        Console.WriteLine(expressions[expressions.Count - 1][0]);
                    }
                }
                
                if (Error.newError)
                { Error.writeError(); }
            }
        }

        public static void resetDigits(List<int> digits, List<string> figures)
        {
            if (digits.Count != 0)
            {
                int temp = 0;
                for (int j = 0; j < digits.Count; j++)
                { temp += digits[j] * (int)Math.Pow(10, digits.Count - 1 - j); }
                figures.Add(temp.ToString());
                while (digits.Count != 0)
                { digits.RemoveAt(0); }
            }
        }
    }

    static class Error
    {
        private static string message;
        private static int place;
        public static bool newError;
        public static void makeError(string errorMessage, int errorPlace)
        {
            message = errorMessage;
            place = errorPlace;
            newError = true;
        }
        public static void makeError(string errorMessage)
        {
            message = errorMessage;
            place = -1;
            newError = true;
        }
        public static void writeError()
        {
            if (place != -1)
            {
                for (int i = 0; i < place + 4; i++)
                { Console.Write(" "); }
                Console.WriteLine("^");
            }
            Console.Write("Error: ");
            Console.WriteLine(message);
            newError = false;
        }
    }
}
