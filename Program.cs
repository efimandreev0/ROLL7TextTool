using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlliOlliTextTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0].Contains(".str"))
            {
                Extract(args[0]);
            }
            else
            {
                Rebuild(args[0]);
            }
        }
        public static void Extract(string str)
        {
            var reader = new BinaryReader(File.OpenRead(str));
            int count = reader.ReadInt32();
            string[] strings = new string[count];
            string[] magicS = new string[count];
            for (int i = 0; i < count; i++)
            {
                int size = reader.ReadInt32();
                magicS[i] = Encoding.UTF8.GetString(reader.ReadBytes(size));
                size = reader.ReadInt32();
                string[] chars = new string[size];
                for (int s = 0; s < size; s++)
                {
                    chars[s] = Encoding.Unicode.GetString(reader.ReadBytes(2)).Replace("\n","<lf>").Replace("\r","<br>");
                    reader.BaseStream.Position += 2;
                }
                strings[i] = string.Join("", chars);
            }
            File.WriteAllLines(Path.GetFileNameWithoutExtension(str), strings);
            File.WriteAllLines("variables.txt", magicS);
        }
        public static void Rebuild(string txt)
        {
            string[] strings = File.ReadAllLines(txt);
            string[] variables = File.ReadAllLines("variables.txt");
            using (BinaryWriter writer = new BinaryWriter(File.Create(Path.GetFileNameWithoutExtension(txt) + ".str")))
            {
                writer.Write(strings.Length);
                for (int i = 0; i < strings.Length; i++)
                {
                    writer.Write(Encoding.UTF8.GetBytes(variables[i]).Length);
                    writer.Write(Encoding.UTF8.GetBytes(variables[i]));
                    strings[i] = strings[i].Replace("<lf>", "\n").Replace("<br>", "\r");
                    writer.Write(strings[i].Length);
                    char[] chars = GetCharactersFromString(strings[i]);
                    for (int s = 0; s < chars.Length; s++)
                    {
                        string Char = string.Join("", chars[s]);
                        writer.Write(Encoding.Unicode.GetBytes(Char));
                        writer.Write(new byte[2]);
                    }
                }
                char[] chars1 = GetCharactersFromString(strings[0]);
                char[] chars2 = GetCharactersFromString(strings[1]);
                writer.Write(chars1.Length + chars2.Length);
                for (int s = 0; s < chars1.Length; s++)
                {
                    string Char = string.Join("", chars1[s]);
                    writer.Write(Encoding.Unicode.GetBytes(Char));
                    writer.Write(new byte[2]);
                }
                for (int s = 0; s < chars2.Length; s++)
                {
                    string Char = string.Join("", chars2[s]);
                    writer.Write(Encoding.Unicode.GetBytes(Char));
                    writer.Write(new byte[2]);
                }
            }
        }
        private static char[] GetCharactersFromString(string input)
        {
            char[] characters = new char[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                characters[i] = input[i];
            }

            return characters;
        }

    }
}
