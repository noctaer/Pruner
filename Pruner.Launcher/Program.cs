using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pruner.Launcher;

internal static class Program
{
    private const string AppFolderName = "Pruner";
    private const string AppExeName    = "Pruner.App.exe";

    [STAThread]
    private static void Main()
    {
        try
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppFolderName);

            string appExePath = Path.Combine(appDataPath, AppExeName);

            if (!File.Exists(appExePath))
            {
                MessageBox.Show(
                    $"Pruner nao foi encontrado em:\n{appExePath}\n\n" +
                    "Tente reinstalar o Pruner.",
                    "Pruner — Erro de inicializacao",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName         = appExePath,
                UseShellExecute  = true,
                WorkingDirectory = appDataPath,
            };

            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erro ao iniciar o Pruner:\n\n{ex.Message}",
                "Pruner — Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}