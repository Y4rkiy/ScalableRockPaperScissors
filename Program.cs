using System;
using System.Security.Cryptography;
using ConsoleTables;
using System.Text;

namespace Task3
{
    class Program
    {
        public static class TableGenerator {
            public static void GenereteTable(string[] moves)
            {
                Array.Resize(ref moves, moves.Length + 1);
                moves[moves.GetUpperBound(0)] = "PC/USER";
                var table = new ConsoleTable(moves);
                for (int i = 0; i < moves.Length; i++)
                {
                    table.AddRow(moves[i], "Draw", "Win", "Lose");
                }
                table.Write();
                Console.WriteLine();
            }
        }
        public class WinnerDeterminer
        {
            public static bool Determine(int arglen, int player_move, int comp_move)
            {
                int winnerflag = (arglen + player_move - comp_move) % arglen;
                if (winnerflag == 0)
                {
                    Console.WriteLine("ничья\n");
                }
                else
                {
                    if (winnerflag % 2 == 1)
                    {
                        Console.WriteLine("You win");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("You lose!");
                        return true;
                    }
                }
                return false;
            }
        }
        public class SafeRandom
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        }

        public class RandomKey { 
            public byte[] Generate() {
                byte[] random = new Byte[32];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(random);
                return random;
                }
            }
        public class HMACGenerator
        {
           public static byte[] Generete(byte[] key, string move)
            {
                HMACSHA256 hmac = new HMACSHA256(key);
                byte[] bytes_comp_move = Encoding.ASCII.GetBytes(move);
                return hmac.ComputeHash(bytes_comp_move);
            }
        }

        

        static void Main(string[] args)
        {
            int arglen = args.Length;
            Console.WriteLine(arglen);
            if (arglen < 3)
            {
                Console.WriteLine("Слишком мало аргументов.Количество аргументов должно быть больше двух");
            }
            else if(arglen % 2 != 1)
            {
                Console.WriteLine("Количество аргументов чётное.Необходимо ввести нечётное количество аргументов.");
            }
            else
            {
                RandomKey rnd = new RandomKey();
                var key = rnd.Generate();
                var strkey = BitConverter.ToString(key).Replace("-", "");

                string[] moves = args;
                bool final = false;
                while (!final)
                {
                    Console.WriteLine("HMAC " + strkey);
                    Console.WriteLine("Available moves:");
                    for (int i = 0; i < arglen; i++)
                    {
                        Console.WriteLine($"{i + 1} - {args[i]}");
                    }
                    Console.WriteLine($"{arglen + 1}  -help");
                    Console.Write("Chose your action:");
                    int player_move = 4;
                    bool start_game = false;

                    while (!start_game) {
                        int player_input = Convert.ToInt32(Console.ReadLine());
                        if (player_input == args.Length + 1)
                        {
                            TableGenerator.GenereteTable(args);
                        }
                        else
                        {
                            start_game = true;
                            player_move = player_input;
                        }
                    }
                    Console.WriteLine($"Your move: {args[player_move-1]}");
                    int comp_move = RNGCryptoServiceProvider.GetInt32(1, args.Length+1);
                    Console.WriteLine($"Computer move: {args[comp_move-1]}");
                    var strhashValue = BitConverter.ToString(HMACGenerator.Generete(key, args[comp_move - 1])).Replace("-", "");
                    Console.WriteLine($"HMAC: {strhashValue}");
                    final = WinnerDeterminer.Determine(arglen, player_move, comp_move);
                }
            }
            Console.ReadKey();
        }
    }
}




