using ConsoleTables;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace ScalableRPS
{
    class Program
    {
        public class ArgsChecker
        {
            public static bool Check(string[] args, int arglen)
            {
                string error_message="";
                bool input_error=false;
                if (arglen == 0)
                {
                    error_message = "There are no arguments. The number of arguments must be greater than or equal to 3 and must be odd. Arguments must not be repeated.";
                    input_error = true;
                }
                if (arglen < 3)
                {
                    error_message= "Too few arguments.\nThe number of arguments must be greater than or equal to 3 and must be odd. Arguments must not be repeated..";
                    input_error = true;
                }
                if (arglen % 2 != 1)
                {
                    error_message = "the number of arguments is even.\nThe number of arguments must be greater than or equal to 3 and must be odd. Arguments must not be repeated.";
                    input_error = true;
                }
                if (!(args.Distinct().Count() == arglen))
                {
                    error_message = "The arguments passed are repeated.\nThe number of arguments must be greater than or equal to 3 and must be odd. Arguments must not be repeated.";
                    input_error  = true;
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
                Console.WriteLine("Input Error:\n"+text + "\nExample: >/.../dir/program.exe rock paper scissors lizard spock");
            }
        }
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
        public class AvailableMovementsTable
        {
            public static void Show(string[] args, int arglen)
            {
                Console.WriteLine("Available moves:");
                for (int i = 0; i < arglen; i++)
                {
                    Console.WriteLine($"{i + 1} - {args[i]}");
                }
                Console.WriteLine($"{arglen + 1}  -help");
                Console.Write("Chose your action:");
            }
        }
        public class ResultTable
        {
            public static void Show(int arglen, int player_move, int comp_move, string[] args, string strkey)
            {
                string result = WinnerDeterminer.Determine(arglen, player_move, comp_move);
                Console.WriteLine($"Your move: {args[player_move - 1]}\nComputer move: {args[comp_move - 1]}\nYou  {result}\nHMAC key: {strkey}");
            }
        }
        public class WinnerDeterminer
        {
            public static string Determine(int arglen, int player_move, int comp_move)
            {
                int winnerflag = (arglen + player_move - comp_move) % arglen;
                if (winnerflag == 0)
                {
                    return "draw";
                }
                else
                {
                    if (winnerflag % 2 == 1)
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
        public class HMACGenerator
        {
            public static byte[] Generate(byte[] key, string move)
            {
                HMACSHA256 hmac = new HMACSHA256(key);
                byte[] bytes_comp_move = Encoding.Default.GetBytes(move);
                byte[] random = hmac.ComputeHash(bytes_comp_move);
                return random;
            }
            
        }
        public class HexToSringConverter
        {
            public static string Convert(byte[] hex)
            {
                string str = BitConverter.ToString(hex).Replace("-", "");
                return str; 
            }
        }



        static void Main(string[] args)
        {
            int arglen = args.Length;
            if (ArgsChecker.Check(args, arglen))
            {
                int comp_move = SafeRandom.GetSafeRandomInt32(1, arglen + 1);
                var key = RandomKey.Generate();
                var strkey = HexToSringConverter.Convert(key);
                var strhashValue = HexToSringConverter.Convert(HMACGenerator.Generate(key, args[comp_move - 1]));
                Console.WriteLine($"HMAC: {strhashValue}");
                bool correct = false;
                while (!correct)
                {
                    AvailableMovementsTable.Show(args, arglen);
                    int player_move = Convert.ToInt32(Console.ReadLine());
                    if (player_move < 0 || player_move > arglen + 1)
                    {
                        continue;
                    }
                    else if (player_move == arglen + 1)
                    {
                        HelpTable.Show(args);
                    }
                    else
                    {
                        correct = true;
                        ResultTable.Show(arglen, player_move, comp_move, args, strkey);
                    }
                }
            }
            Console.ReadKey();
        }
    }
}




