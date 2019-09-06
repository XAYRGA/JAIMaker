using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JaiMaker
{


    public static class INAFile
    {
        public static Dictionary<int, Dictionary<int,string>> parse(string file)
        {
            var RETL = new Dictionary<int, Dictionary<int, string>>();
            var STARR = File.ReadAllLines(file);

            var currentBank = 0;
            var BankDict = new Dictionary<int, string>();
            RETL[currentBank] = BankDict;

            for (int line = 0; line < STARR.Length; line++)
            {
                var currentLine = STARR[line];
                if (currentLine.Length > 1) // Ignore blank lines. 
                {
                    if (currentLine[0] == ':')
                    {
                        var newBank = currentLine.Substring(1);
                        var newBankNumber = Convert.ToInt32(newBank);
                        BankDict = new Dictionary<int, string>();
                        currentBank = newBankNumber;
                        RETL[currentBank] = BankDict;
                    } else if (currentLine[0]=='/' || currentLine[0] == '\r' || currentLine[0] == '\n') {
                        // do nothing,  comment.
                    }
                    else
                    {
                        if  (currentLine.Contains("="))
                        {
                            var args = currentLine.Split('=');
                            try
                            {
                                var indexNumber = Convert.ToInt32(args[0]);
                                var name = args[1];
                                Console.WriteLine("BANK {0} {1} {2}", currentBank, indexNumber,name);
                                BankDict[indexNumber] = name;
                            } catch {
                                Console.WriteLine("Malformmed line in {0}, line {1}", file, line);
                            };

                        } else
                        {
                            Console.WriteLine("Malformmed line in {0}, line {1}", file, line);
                        }
                    }
                }

            }
            return RETL;
        }

    }
}
