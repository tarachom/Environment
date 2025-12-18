
using System.Runtime.Versioning;
using Microsoft.Win32;

[SupportedOSPlatform("windows")]
class Program
{
    static void Main()
    {
        string regKeyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";

        string newPath = "";
        string findPath = @"C:\msys64\ucrt64\bin".Trim().ToLower();

        if (findPath.EndsWith('\\') || findPath.EndsWith('/'))
            findPath = findPath[..^1];

        bool update = true;

        RegistryKey? key = Registry.LocalMachine.OpenSubKey(regKeyName);
        string? value = (string?)key?.GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);

        if (value != null)
        {
            string[] paths = value.Split([';'], StringSplitOptions.RemoveEmptyEntries);
            foreach (string path in paths)
            {
                newPath += path + ";";

                if (path.ToLower() == findPath)
                    update = false;
            }

            if (update == true)
                try
                {
                    Environment.SetEnvironmentVariable("Path", newPath + findPath, EnvironmentVariableTarget.Machine);
                    Console.WriteLine($"OK. Path {findPath} added");
                }
                catch (System.Security.SecurityException)
                {
                    Console.WriteLine("Помилка: Необхідні права адміністратора для запису системної змінної.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Сталася помилка: {ex.Message}");
                }
            else
                Console.WriteLine($"OK. Path {findPath} exist");
        }

        Console.WriteLine($"Press Enter to exit program");
        Console.ReadLine();
    }
}