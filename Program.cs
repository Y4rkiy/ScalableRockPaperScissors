using ConsoleTables;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace ScalableRPS
{
    class Program
    {
        public class HelpTable
        {
            public static void Show(string[] moves)
            {
                string[] header = new string[moves.Length + 1];
                for (int i = 0; i <= moves.Length; i++)
                {
                    if (i == 0)
                    {
                        header[i] = "PC/USER";
                        continue;
                    }
                    header[i] = moves[i - 1];
                }
                var table = new ConsoleTable(header);
                for (int i = 0; i < moves.Length; i++)
                {
                    string[] row = new string[moves.Length + 1];
                    row[0] = moves[i];
                    for (int j = 1; j <= moves.Length; j++)
                    {
                        row[j] = WinnerDeterminer.Determine(moves.Length, i, j - 1);
                    }
                    table.AddRow(row);
                }
                table.Write();
                Console.WriteLine();
            }
        }
        public class WinnerDeterminer
        {
            public static string Determine(int argsLength, int playerMove, int computerMove)
            {
                int winnerFlag = (argsLength + playerMove - computerMove) % argsLength;
                if (winnerFlag == 0)
                {
                    return "draw";
                }
                else
                {
                    if (winnerFlag % 2 == 1)
                    {
                        return "win";
                    }
                    else
                    {
                        return "lose";
                    }
                }
            }
        }
        public class SafeRandom
        {
            public static byte[] GetSafeRandomByte(int length)
            {
                byte[] random = new Byte[length];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(random);
                return random;
            }
            public static int GetSafeRandomInt32(int lim1, int lim2)
            {
                int random = RNGCryptoServiceProvider.GetInt32(lim1,lim2);
                return random;
            }
        }
        public class RandomKey
        {
            public static byte[] Generate()
            {
                byte[] random = SafeRandom.GetSafeRandomByte(32);
                return random;
            }
        }
        public class ArgsChecker
        {
            public static bool Check(string[] args, int argsLength)
            {
                string error_message = "";
                bool input_error = false;
                if (argsLength == 0)
                {
                    error_message = "There are no arguments. The number of arguments must be greater than or equal to 3 and must be odd. Arguments must not be repeated.";
                    input_error = true;
                }
                if (argsLength < 3)
                {
                    error_message = "Too few arguments.\nThe number of arguments must be greater than or equal to 3 and must be odd. Arguments must not be repeated..";
                    input_error = true;
                }
                if (argsLength % 2 != 1)
                {
                    error_message = "the number of arguments is even.\nThe number of arguments must be greater than or equal to 3 and must be odd. Arguments must not be repeated.";
                    input_error = true;
                }
                if (!(args.Distinct().Count() == argsLength))
                {
                    error_message = "The arguments passed are repeated.\nThe number of arguments must be greater than or equal to 3 and must be odd. Arguments must not be repeated.";
                    input_error = true;
                }
                if (input_error)
                {
                    GetErrorMessage(error_message);
                    return !input_error;
                }
                return !input_error;

            }
            public static void GetErrorMessage(string text)
            {
                Console.WriteLine("Input Error:\n" + text + "\nExample: >/.../dir/ScalableRPS.exe rock paper scissors lizard spock");
            }
        }

        

        static void Main(string[] args)
        {
            int ArgsLength = args.Length;
            if (ArgsChecker.Check(args, ArgsLength))
            {
                int ComputerMove = SafeRandom.GetSafeRandomInt32(1, ArgsLength + 1);
                var key = RandomKey.Generate();
                var StringKey = HexToSringConvert(key);
                var StringHash = HexToSringConvert(HMACGenerate(key, args[ComputerMove - 1]));
                Console.WriteLine($"HMAC: {StringHash}");
                bool correct = false;
                while (!correct)
                {
                    ShowAvailableMovements(args, ArgsLength);
                    int PlayerMove = Convert.ToInt32(Console.ReadLine());
                    if (PlayerMove < 0 || PlayerMove > ArgsLength + 1)
                    {
                        continue;
                    }
                    else if (PlayerMove == ArgsLength + 1)
                    {
                        HelpTable.Show(args);
                    }
                    else
                    {
                        correct = true;
                        ShowResult(ArgsLength, PlayerMove, ComputerMove, args, StringKey);
                    }
                }
            }
            Console.ReadKey();
        }

        public static void ShowAvailableMovements(string[] args, int arglen)
        {
            Console.WriteLine("Available moves:");
            for (int i = 0; i < arglen; i++)
            {
                Console.WriteLine($"{i + 1} - {args[i]}");
            }
            Console.WriteLine($"{arglen + 1}  -help");
            Console.Write("Chose your action:");
        }
        public static byte[] HMACGenerate(byte[] key, string move)
        {
            HMACSHA256 hmac = new HMACSHA256(key);
            byte[] bytesComputerMove = Encoding.Default.GetBytes(move);
            byte[] random = hmac.ComputeHash(bytesComputerMove);
            return random;
        }
        public static string HexToSringConvert(byte[] hex)
        {
            string stringHex = BitConverter.ToString(hex).Replace("-", "");
            return stringHex;
        }
        public static void ShowResult(int argsLength, int playerMove, int computerMove, string[] args, string stringKey)
        {
            string result = WinnerDeterminer.Determine(argsLength, playerMove, computerMove);
            Console.WriteLine($"Your move: {args[playerMove - 1]}\nComputer move: {args[computerMove - 1]}\nYou  {result}\nHMAC key: {stringKey}");
        }
    }
}




