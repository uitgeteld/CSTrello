using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CSTrelloApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static Frame ContentFrame;
        public MainWindow()
        {
            InitializeComponent();
            ContentFrame = contentFrame;
            if (contentFrame != null) contentFrame.Navigate(typeof(OverviewPage));
        }
        private void contentSelectorBar_SelectionChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
        {
            SelectorBarItem selectedItem = sender.SelectedItem as SelectorBarItem;
            if (selectedItem != null && contentFrame != null)
            {

                if (selectedItem == createSelectorBarItem)
                {
                    contentFrame.Navigate(typeof(CreatePage));
                }
                else if (selectedItem == overviewSelectorBarItem)
                {
                    contentFrame.Navigate(typeof(OverviewPage));
                }
            }
        }
    }
}
