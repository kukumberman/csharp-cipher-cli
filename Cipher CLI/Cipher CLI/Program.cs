using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Sharprompt;
using CommandLine;

namespace Cipher_CLI
{
    class Program
    {
        class Options
        {
            [Option("action", Required = false)]
            public string Action { get; set; }

            [Option("ouput", Required = false)]
            public string OutputPath { get; set; }

            [Option("key", Required = false)]
            public string Key { get; set; }

            [Option("file", Required = false)]
            public string FilePath { get; set; }
        }

        class Entry
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<string> Data { get; set; }
            public string CreatedAt { get; set; }
            public string UpdatedAt { get; set; }
        }

        private static List<Entry> entries;

        private static readonly string Version = "1.0.0";

        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(Run);
        }

        private static string PromptKey()
        {
            return Prompt.Password("enter key");
        }

        private static string PromptList(string[] names)
        {
            return Prompt.Select("select entry", names, names.Length);
        }

        private static string PromptEntry(Entry entry)
        {
            return Prompt.Select("select", entry.Data, entry.Data.Count, "", (value) => "***");
        }

        private static List<Entry> GetEntries(string key, string encrypted)
        {
            try
            {
                Cipher cipher = new Cipher(key);
                string json = cipher.Decrypt(encrypted);

                var jsonOptions = new JsonSerializerOptions();
                jsonOptions.PropertyNameCaseInsensitive = true;

                var entries = JsonSerializer.Deserialize<List<Entry>>(json, jsonOptions);
                return entries;
            }
            catch
            {
                Console.WriteLine("wrong key or invalid json format");
                return GetEntries(PromptKey(), encrypted);
            }
        }

        private static bool IsValidPath(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        private static void Run(Options options)
        {
            Console.Title = $"Cipher CLI {Version}";

            string path = options.FilePath;

            if (!IsValidPath(path))
            {
                using (var dialog = new OpenFileDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK && IsValidPath(dialog.FileName))
                    {
                        path = dialog.FileName;
                    }
                    else
                    {
                        Console.WriteLine("file doesnt exists");
                        return;
                    }
                }
            }

            string contents = File.ReadAllText(path);

            string key = options.Key?.Length > 0 ? options.Key : PromptKey();

            if (string.IsNullOrEmpty(options.Action))
            {
                entries = GetEntries(key, contents);

                ShowEntries();
            }
            else
            {
                // todo: output argument as save path
                if (options.Action == "decrypt")
                {
                    string decrypted = new Cipher(key).Decrypt(contents);
                    string savedPath = SaveContents(decrypted);
                    Console.WriteLine(savedPath);
                }
                else if (options.Action == "encrypt")
                {
                    string encrypted = new Cipher(key).Encrypt(contents);
                    string savedPath = SaveContents(encrypted);
                    Console.WriteLine(savedPath);
                }
                else
                {
                    Console.WriteLine("unkwown action");
                }

                Console.ReadKey();
            }
        }

        private static void ShowEntries()
        {
            Console.Clear();

            string[] names = entries.Select(e => e.Name).ToArray();
            string selected = PromptList(names);
            var entry = entries.Find(e => e.Name == selected);

            Console.WriteLine(entry.Description);
            HandleEntry(entry);
        }

        private static void HandleEntry(Entry entry)
        {
            string data = PromptEntry(entry);
            Clipboard.SetText(data);

            if (Prompt.Confirm("continue with this entry?"))
            {
                HandleEntry(entry);
            }
            else
            {
                ShowEntries();
            }
        }

        private static string SaveContents(string contents)
        {
            string fileName = Guid.NewGuid().ToString() + ".txt";
            string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllText(path, contents);
            return path;
        }
    }
}
