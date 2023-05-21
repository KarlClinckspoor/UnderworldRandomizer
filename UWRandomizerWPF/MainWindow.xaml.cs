using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UWRandomizerEditor.LEVdotARK;
using UWRandomizerTools;

namespace UWRandomizerWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ArkLoader ark;
    private int seed = 42;
    private ItemRandomizationSettings _itemSettings;

    public MainWindow()
    {
        Singletons.SeedRandomAndReset(seed);
        _itemSettings = new ItemRandomizationSettings();
        InitializeComponent();
    }

    private void Btn_LoadLevArk_Click(object sender, RoutedEventArgs e)
    {
        AddMsgToLog($"Attempting to load lev.ark");
        try
        {
            ark = new ArkLoader(TxtBoxPathToArk.Text);
            StackTools.IsEnabled = true;
            BtnSaveChanges.IsEnabled = true;
            AddMsgToLog($"Loaded lev.ark");
        }
        catch (Exception exp)
        {
            AddMsgToLog($"Error when loading lev.ark. Exception: {exp}. Choose another file.");
        }
    }

    private void Btn_SetSeed_Click(object sender, RoutedEventArgs e)
    {
        AddMsgToLog($"Attempting to interpret {TxtBoxSeedValue.Text} as an integer to use as seed");
        if (int.TryParse(TxtBoxSeedValue.Text, out var newSeed))
        {
            this.seed = newSeed;
            Singletons.SeedRandomAndReset(seed);
            AddMsgToLog($"Done replacing seed");
            return;
        }
        AddMsgToLog($"Couldn't interpret {TxtBoxSeedValue.Text} as an integer");
    }

    private void Btn_ExportSpoilerLog_Click(object sender, RoutedEventArgs e)
    {
        string log = CreateSpoilerLog();
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
        bool? result = saveFileDialog.ShowDialog();

        if (result == true)
        {
            File.WriteAllText(saveFileDialog.FileName, log);
            AddMsgToLog($"Saved spoiler log at {saveFileDialog.FileName}");
        }
        else
        {
            var tempname = "UWspoilerlog.txt";
            File.WriteAllText(log, tempname);
            AddMsgToLog($"Saved spoiler log to {Path.Join(Directory.GetCurrentDirectory(), tempname)}");
        }
    }

    private void Btn_ShuffleItems_Click(object sender, RoutedEventArgs e)
    {
        AddMsgToLog("In-level shuffling of items in all levels");
        ItemTools.ShuffleItemsInAllLevels(ark, Singletons.RandomInstance, _itemSettings);
        AddMsgToLog("Done. Check either UltimateUnderworldEditor or export the spoiler log if you want.");
    }

    private void Btn_RemoveAllLocks_Click(object sender, RoutedEventArgs e)
    {
        AddMsgToLog("Removing all locks from doors");
        RandoTools.RemoveAllDoorReferencesToLocks(ark);
        AddMsgToLog("Done");
    }

    private void Btn_BackupLevArk_Click(object sender, RoutedEventArgs e)
    {
        AddMsgToLog($"Saving LEV.ARK as LEV.ARK.BCK.");
        var tempArk = new ArkLoader(ark.Path);
        UWRandomizerEditor.Utils.SaveBuffer(tempArk, Path.GetDirectoryName(tempArk.Path), "LEV.ARK.BCK");
    }

    private void Btn_Browse_Click(object sender, RoutedEventArgs e)
    {
        var openFileDlg = new Microsoft.Win32.OpenFileDialog();
        bool? result = openFileDlg.ShowDialog();
        if (result == true)
        {
            TxtBoxPathToArk.Text = openFileDlg.FileName;
            AddMsgToLog($"Selected file {openFileDlg.FileName}");
        }
    }

    private void Btn_SaveChanges_Click(object sender, RoutedEventArgs e)
    {
        AddMsgToLog("Attempting to save the current lev.ark over the old one.");
        UWRandomizerEditor.Utils.SaveBuffer(ark, Path.GetDirectoryName(ark.Path), "LEV.ARK");
        AddMsgToLog("Save successful.");
    }

    private string CreateSpoilerLog()
    {
        var sb = new StringBuilder();
        AddMsgToLog("Creating spoiler log, 1-indexed.");
        sb.AppendLine($"Spoiler log: lev.ark, seed {seed}. Entries are XY pos (starts from bottom left) and the number is the object ID.");
        int level = 0; // 1 indexed
        foreach (var block in ark.TileMapObjectsBlocks)
        {
            level++;
            AddMsgToLog($"Processing level {level}");
            sb.AppendLine($"Level {level}");

            foreach (var tile in block.Tiles)
            {
                sb.Append($"\tTile: ({tile.XYPos[0]},{tile.XYPos[1]}): ");
                foreach (var obj in tile.ObjectChain)
                {
                    sb.Append($"({obj.ToString()},{obj.ItemID});");
                }

                sb.Append("\n");
            }
        }

        AddMsgToLog("Done creating spoiler log string");
        return sb.ToString();
    }

    private void Btn_RestoreLevArk_Click(object sender, RoutedEventArgs e)
    {
        string backupPath = Path.Join(Path.GetDirectoryName(ark.Path), "LEV.ARK.BCK");
        if (File.Exists(backupPath))
        {
            AddMsgToLog($"Found backup file LEV.ARK.BCK at '{backupPath}', deleting current lev.ark");
            File.Delete(ark.Path);
            AddMsgToLog($"Renaming backup to LEV.ARK");
            File.Copy(backupPath, ark.Path);
            AddMsgToLog($"Reloading LEV.ARK");
            ark = new ArkLoader(ark.Path);
            return;
        }
        AddMsgToLog("Couldn't find backup file LEV.ARK.BCK. Didn't do anything");
    }

    private void AddMsgToLog(string message)
    {
        ListViewLog.Items.Add(new ListViewItem() {Content = message});
    }

    private void BtnClearLog_OnClick(object sender, RoutedEventArgs e)
    {
        ListViewLog.Items.Clear();
    }
}