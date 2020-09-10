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
                System.Console.WriteLine("Result is : {0} ", result);
            }


            //if the conversion doesn't exist into the file
            else
            {
               string  intermediateCurrency ="";
               decimal intermediateresult = 0;
               bool first = false;
                //while the final result wasn't found
                while (result.Equals(0))
                {
                    bool foundMatch = false;
                    foreach (var conversion in rates)
                    {
                        string[] infos = conversion.Split(";");
                        if (infos[0].Equals(firstcurrency) && first.Equals(false))
                        {
                            intermediateCurrency = infos[1];
                            intermediateresult = program.convertCurrencies(rates, firstcurrency, intermediateCurrency, amounttoconvert);
                            first = true;
                            foundMatch = true;
                        }
                        else if ((infos[0].Equals(intermediateCurrency)) || (infos[1].Equals(intermediateCurrency)))
                        {
                          //final result
                            if((infos[1].Equals(goalcurrency)) || (infos[0].Equals(goalcurrency)))
                            {
                                result = program.convertCurrencies(rates, intermediateCurrency, goalcurrency, intermediateresult);
                                Console.WriteLine("The result is : {0} ", decimal.Round(result));
                                foundMatch = true;
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
                    if(foundMatch == false)
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
                        return decimal.Round(result);
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
                        return decimal.Round(result);
                    }
                }
            }
            return 0;
        }
    }
}
