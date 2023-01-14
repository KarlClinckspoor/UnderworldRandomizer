using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UWRandomizerEditor.LEVDotARK;
using UWRandomizerTools;

namespace UWRandomizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    ArkLoader ark;
    int seed = 42;

    public MainWindow()
    {
        Singletons.SeedRandomAndReset(seed);
        InitializeComponent();
    }

    private void Btn_LoadLevArk_Click(object sender, RoutedEventArgs e)
    {
        ark = new ArkLoader(TxtBox_PathToArk.Text);
        Stack_Tools.IsEnabled = true;
        Btn_SaveChanges.IsEnabled = true;
    }

    private void Btn_SetSeed_Click(object sender, RoutedEventArgs e)
    {
        int.TryParse(TxtBox_SeedValue.Text, out int newSeed);
        this.seed = newSeed;
        Singletons.SeedRandomAndReset(seed);
    }

    private void Btn_ExportSpoilerLog_Click(object sender, RoutedEventArgs e)
    {
        string log = CreateSpoilerLog();
        var openFileDlg = new Microsoft.Win32.OpenFileDialog();
        bool? result = openFileDlg.ShowDialog();

        if (result == true)
        {
            File.WriteAllText(openFileDlg.FileName, log);
        }
        else
        {
            File.WriteAllText(log, "./UWspoilerlog.txt");
        }
    }

    private void Btn_ShuffleItems_Click(object sender, RoutedEventArgs e)
    {
        ShuffleItems.ShuffleAllLevels(ark, Singletons.RandomInstance);
    }

    private void Btn_RemoveAllLocks_Click(object sender, RoutedEventArgs e)
    {
        RandoTools.RemoveAllDoorReferencesToLocks(ark);
    }

    private void Btn_BackupLevArk_Click(object sender, RoutedEventArgs e)
    {
        var tempArk = new ArkLoader(ark.Path);
        UWRandomizerEditor.Utils.StdSaveBuffer(tempArk, System.IO.Path.GetDirectoryName(tempArk.Path), "LEV.ARK.BCK");
    }

    private void Btn_Browse_Click(object sender, RoutedEventArgs e)
    {
        var openFileDlg = new Microsoft.Win32.OpenFileDialog();
        bool? result = openFileDlg.ShowDialog();
        if (result == true)
        {
            TxtBox_PathToArk.Text = openFileDlg.FileName;
        }
    }

    private void Btn_SaveChanges_Click(object sender, RoutedEventArgs e)
    {
        UWRandomizerEditor.Utils.StdSaveBuffer(ark, System.IO.Path.GetDirectoryName(ark.Path), "LEV.ARK");
    }

    private string CreateSpoilerLog()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Spoiler log: lev.ark, seed {seed}");
        int level = 0; // 1 indexed
        foreach (var block in ark.TileMapObjectsBlocks)
        {
            level++;
            sb.AppendLine($"Level {level}");

            foreach (var tile in block.TileInfos)
            {
                sb.Append($"\tTile: ({tile.XYPos[0]},{tile.XYPos[1]}): ");
                foreach (var obj in tile.ObjectChain)
                {
                    sb.Append($"({obj.ToString()},{obj.ItemID});");
                }

                sb.Append("\n");
            }
        }

        return sb.ToString();
    }

    private void Btn_RestoreLevArk_Click(object sender, RoutedEventArgs e)
    {
        string backupPath = System.IO.Path.Join(System.IO.Path.GetDirectoryName(ark.Path), "LEV.ARK.BCK");
        if (File.Exists(backupPath))
        {
            File.Delete(ark.Path);
            File.Copy(backupPath, ark.Path);
            ark = new ArkLoader(ark.Path);
        }
    }
}