using sqljson.converters;
using static System.Console;

// See https://aka.ms/new-console-template for more information
// WriteLine($"{Environment.CurrentDirectory}");
string jpath = $"{Environment.CurrentDirectory}/../example.json";

WriteLine($"Reading JSON file: {jpath}");

PostgreSQLConverter converter = new(jpath);
string sql = converter.ProcessJson();
WriteLine(sql);

File.WriteAllText($"{Environment.CurrentDirectory}/../output.sql", sql);
