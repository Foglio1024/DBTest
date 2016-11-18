using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using Test;

namespace DBTest
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

        }
        private const UInt32 StdOutputHandle = 0xFFFFFFF5;
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(UInt32 nStdHandle);
        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        List<DailyQuest> DungQuests;
        List<DailyQuest> SoloQuests;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //AllocConsole();
            Test.Program.LoadItemsDB();
        }
        QuestStrip SetCompensations(QuestStrip qs, DailyQuest quest)
        {
            int i = 0;
            foreach (var comp in quest.CompensationList)
            {
                qs.compensations.RowDefinitions.Add(new RowDefinition());
                TextBlock nameTb = new TextBlock();
                TextBlock amountTb = new TextBlock();
                string name = Program.ItemList.Find(x => x.Id == comp.TemplateId).Name;
                Console.WriteLine(name);
                nameTb.Text = name;
                amountTb.Text = comp.Amount.ToString();
                Grid.SetRow(nameTb, i);
                Grid.SetRow(amountTb, i);
                Grid.SetColumn(nameTb, 0);
                Grid.SetColumn(amountTb, 1);
                nameTb.HorizontalAlignment = HorizontalAlignment.Left;
                amountTb.HorizontalAlignment = HorizontalAlignment.Left;
                nameTb.VerticalAlignment = VerticalAlignment.Center;
                amountTb.VerticalAlignment = VerticalAlignment.Center;
                if (comp.Type == CompensationType.reputationPoint)
                {
                    nameTb.FontWeight = FontWeights.Bold;
                    amountTb.FontWeight = FontWeights.Bold;
                    nameTb.Foreground = new SolidColorBrush(Colors.Purple);
                    amountTb.Foreground = new SolidColorBrush(Colors.Purple);
                }
                switch (quest.Event.type)
                {
                    case EventType.Field:
                        qs.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 134, 84));
                        break;
                    case EventType.Dungeon:
                        qs.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 125, 180));
                        break;
                    case EventType.BattleField:
                        qs.Foreground = new SolidColorBrush(Colors.DarkOrange);
                        break;
                    default:
                        break;
                }
                qs.compensations.Children.Add(nameTb);
                qs.compensations.Children.Add(amountTb);
                i++;
            }
            return qs;

        }
        private void click(object sender, RoutedEventArgs ev)
        {
            list.Items.Clear();
            List<DailyQuest> Quests = new List<DailyQuest>();

            Quests = Program.RetrieveQuests(EventType.Field, 65);
            //Quests = Quests.Concat(Program.RetrieveQuests(EventType.Dungeon, 65)).ToList();

            Thread asd = new Thread(() =>
            {
                foreach (DailyQuest quest in Quests)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        QuestStrip qs = new QuestStrip();
                        qs.name.Text = quest.Name;
                        qs.name.FontWeight = FontWeights.Bold;
                        qs.ilvl.Text = quest.Event.requiredItemLevel.ToString();
                        qs = SetCompensations(qs, quest);
                        list.Items.Add(qs);
                        bar.Value = (Quests.IndexOf(quest)+1) *100 / Quests.Count;
                    });
                }
            });
            
            asd.SetApartmentState(ApartmentState.STA);
            asd.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Thread asd = new Thread(() =>
            {
                foreach (var t in Program.ItemList)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        QuestStrip qs = new QuestStrip();
                        qs.name.Text = t.Name;
                        qs.name.FontWeight = FontWeights.Bold;
                        qs.ilvl.Text = t.Id.ToString() +" "+ t.Rarity.ToString();
                        qs.compensations.Children.Add(new TextBlock { Text = t.ToolTip, TextWrapping = TextWrapping.Wrap });
                        list.Items.Add(qs);
                        bar.Value = (Program.ItemList.IndexOf(t) + 1) * 100 / Program.ItemList.Count;
                    });
                }
            });
            asd.SetApartmentState(ApartmentState.STA);
            asd.Start();

        }
    }

}
    

