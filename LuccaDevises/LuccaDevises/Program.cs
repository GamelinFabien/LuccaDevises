using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace LuccaDevises
{
    class Program
    {
        static void Main(string[] args)
        {

           if (args.Length == 0)
            {
                Console.WriteLine("Please specify a file path");
                Environment.Exit(-1);
            }
            string filepath = args[0];
            int counter = 0;
            string line;
            string convertGoal = "";
            int numberRates = 0;
            List<string> rates = new List<string>();

            // Read the file and get the informations from it
            System.IO.StreamReader file = new System.IO.StreamReader(@filepath);
            while ((line = file.ReadLine()) != null)
            {
                counter++;
                if (counter.Equals(1))
                {
                    convertGoal = line;
                }
                else if (counter.Equals(2))
                {
                    numberRates = int.Parse(line);
                }
                else
                {
                    rates.Add(line);
                }
            }
            file.Close();

            string[] goalspart = convertGoal.Split(";");
            string firstcurrency = goalspart[0];
            int amounttoconvert = int.Parse(goalspart[1]);
            string goalcurrency = goalspart[2];
            
            //if currencies are the same
            if(goalspart[0] == goalspart[2])
            {
                Console.WriteLine("The result is : {0} ", goalspart[1]);
                Environment.Exit(0);
            }
            //try to convert the desired currencies
            Program program = new Program();
            decimal result = program.convertCurrencies(rates, firstcurrency, goalcurrency, amounttoconvert);

            //if the conversion exists into the file
            if(result != 0)
            {
                result = decimal.Round(result, 2);
                System.Console.WriteLine("Result is : {0} ", result);
            }


            //if the conversion doesn't exist into the file
            else
            {
               string  intermediateCurrency ="";
               decimal intermediateresult = 0;
               bool first = true;
               bool done = false;
                //while the final result wasn't found
                while (done.Equals(false))
                {
                    bool foundMatch = false;
                    foreach (var conversion in rates)
                    {
                        string[] infos = conversion.Split(";");
                        if ((infos[0].Equals(firstcurrency)) || (infos[1].Equals(firstcurrency)))
                        {
                            if(first.Equals(true))
                            {
                                if(infos[0].Equals(firstcurrency))
                                {
                                    intermediateCurrency = infos[1];
                                    intermediateresult = program.convertCurrencies(rates, firstcurrency, intermediateCurrency, amounttoconvert);
                                }
                                else
                                {
                                    intermediateCurrency = infos[0];
                                    intermediateresult = program.convertCurrencies(rates, firstcurrency, intermediateCurrency, amounttoconvert);
                                }
                                
                                
                                first = false;
                                foundMatch = true;
                              /*  Console.WriteLine("info O = {0}, info 1 = {1}, firstcur = {2}, goal = {3} ", infos[0], infos[1], firstcurrency, goalcurrency);
                                Console.ReadLine();*/
                            }
                        }
                        else if((infos[0].Equals(goalcurrency)) || (infos[1].Equals(goalcurrency)))
                        {
                            if (first.Equals(true))
                            {
                                if (infos[0].Equals(goalcurrency))
                                {
                                    intermediateCurrency = infos[1];
                                    intermediateresult = program.convertCurrencies(rates, intermediateCurrency, goalcurrency, amounttoconvert);
                                }
                                else
                                {
                                    intermediateCurrency = infos[0];
                                    intermediateresult = program.convertCurrencies(rates, intermediateCurrency, goalcurrency, amounttoconvert);
                                }


                                first = false;
                                foundMatch = true;
                                /*  Console.WriteLine("info O = {0}, info 1 = {1}, firstcur = {2}, goal = {3} ", infos[0], infos[1], firstcurrency, goalcurrency);
                                  Console.ReadLine();*/
                            }
                        }
                        else if ((infos[0].Equals(intermediateCurrency)) || (infos[1].Equals(intermediateCurrency)))
                        {
                          //final result
                            if((infos[1].Equals(goalcurrency)) || (infos[0].Equals(goalcurrency)))
                            {
                                result = program.convertCurrencies(rates, intermediateCurrency, goalcurrency, intermediateresult);
                                result = decimal.Round(result, 2);
                                Console.WriteLine("The result is : {0} ", result);
                                foundMatch = true;
                                done = true;
                            }
                            //another intermediate step 
                            else
                            {
                                if(intermediateCurrency.Equals(infos[1]))
                                {
                                    intermediateresult = program.convertCurrencies(rates, intermediateCurrency, infos[0], intermediateresult);
                                    intermediateCurrency = infos[0];
                                    foundMatch = true;
                                }
                                else 
                                {
                                    intermediateresult = program.convertCurrencies(rates, intermediateCurrency, infos[1], intermediateresult);
                                    intermediateCurrency = infos[1];
                                    foundMatch = true;
                                }
                            }
                        }
                    }
                    if (foundMatch == false)
                    { Console.WriteLine("the system can't find any conversions for this currencies"); Environment.Exit(0); }
                }
            }



            // Suspend the screen.  
            System.Console.ReadLine();
        }







        decimal convertCurrencies(List<string> rates, string firstcurrency, string goalcurrency, decimal amounttoconvert)
        {
            for (int i = 0; i < rates.Count; i++)
            {
                string[] rateinfos = rates[i].Split(";");
                if (rateinfos[0].Equals(firstcurrency))
                {
                    if (rateinfos[1].Equals(goalcurrency))
                    {
                        decimal rate = decimal.Parse(rateinfos[2], CultureInfo.InvariantCulture);
                        decimal result = amounttoconvert * rate;
                       //return decimal.Round(result);
                        return result;
                    }
                }
            }
            //try if currencies are reversed
            for (int i = 0; i < rates.Count; i++)
            {
                string[] rateinfos = rates[i].Split(";");
                if (rateinfos[1].Equals(firstcurrency))
                {
                    if (rateinfos[0].Equals(goalcurrency))
                    {
                        decimal rate = decimal.Parse(rateinfos[2], CultureInfo.InvariantCulture);
                        decimal result = amounttoconvert * (1 / rate);
                       // return decimal.Round(result);
                        return result;
                    }
                }
            }
            return 0;
        }
    }
}
