string[] cmdArgs = Environment.GetCommandLineArgs();
Dictionary<string, string> pairs = new Dictionary<string, string>();
for (int i = 1; i < cmdArgs.Length; i += 2)
{
    pairs.Add(cmdArgs[i], cmdArgs[i + 1]);
}
var ns=pairs["-n"].ToString();
var model = pairs["-m"].ToString();
var output = pairs["-o"].ToString();

Console.ReadLine();