using System;
using System.Collections.Generic;
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
using static UWRandomizer.RandoTools;

namespace UWRandomizer;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>


public partial class MainWindow : Window
{
    ArkLoader ark;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ark = new ArkLoader(PathToArk.Text);
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        RandoTools.RemoveAllDoorReferencesToLocks(ark);
        ark.SaveBuffer(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(PathToArk.Text)), "LEV.ARK_test");
    }
}