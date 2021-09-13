using ConsoleTables;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Task3
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
                byte[] bytes_comp_move = Encoding.UTF8.GetBytes(move);
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
            if (arglen < 3)
            {
                Console.WriteLine("Слишком мало аргументов.Количество аргументов должно быть больше двух");
            }
            else if (arglen % 2 != 1)
            {
                Console.WriteLine("Количество аргументов чётное.Необходимо ввести нечётное количество аргументов.");
            }
            else
            {
                string[] moves = args;
                int comp_move = SafeRandom.GetSafeRandomInt32(1, args.Length + 1);
                var key = RandomKey.Generate();
                var strkey = HexToSringConverter.Convert(key);
                var strhashValue = HexToSringConverter.Convert(HMACGenerator.Generate(key, args[comp_move - 1]));
                Console.WriteLine($"HMAC: {strhashValue}");
                bool correct = false;
                while (!correct)
                {
                    AvailableMovementsTable.Show(args, arglen);
                    int player_move = Convert.ToInt32(Console.ReadLine());
                    if (player_move < 0 || player_move > arglen+1)
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
                        string result = WinnerDeterminer.Determine(arglen, player_move, comp_move);
                        Console.WriteLine($"Your move: {args[player_move - 1]}");
                        Console.WriteLine($"Computer move: {args[comp_move - 1]}");
                        Console.WriteLine("You " + result);
                        Console.WriteLine("HMAC key: " + strkey);
                    }
                }
            }
            Console.ReadKey();
        }
    }
}




