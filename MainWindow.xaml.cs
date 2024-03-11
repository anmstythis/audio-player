using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using System.Reflection.PortableExecutable;
using System.Windows.Data;

namespace audio_player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random rnd = new Random(); //рандомайзер для перемешивания музыки
        List<string> historyList = new List<string>(); //список, в который записывается история прослушивания
        bool shuffleOn = false; //переменная для регулирования перемешивания музыки
        bool pause = false; //переменная для регулирования паузы
        bool repeat = false;
        public MainWindow()
        {
            InitializeComponent();
            media.Volume = 0.5;
        }

        private void addHistory(string path) //метод для записи пути, прослушиваемой музыки, в список
        {
            historyList.Add(path);
        }

        private void audioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            media.Play();
            media.Position = new TimeSpan(Convert.ToInt64(audioSlider.Value));
        }

        private void shufflledMusic() //перемешивание музыки
        {
            if (shuffleOn == true)
            {
                int max = musicList.Items.Count;

                int mus = rnd.Next(0, max);
                media.Source = new Uri((string)musicList.Items[mus]);
                musicList.SelectedIndex = mus;
                media.Play();
            }
        }

        private void replayAudio()
        {
            if (musicList.ItemsSource != null || musicList.SelectedItem != null)
            {
                repeat = true;
                media.Source = new Uri((string)musicList.SelectedItem);
                media.Play();
            }
        }

        private void replay_Click(object sender, RoutedEventArgs e) //повтор
        {
            if (repeat == false)
            {
                repeat = true;
            }
            else
            {
                repeat = false;
            }
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            pause = !pause;
            if (pause == true) 
            {
                media.Pause();
            }
            else if (pause == false)
            {
                media.Play();
            }
        }

        private void musicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (repeat == true) //отключение повтора, если включен
            {
                repeat = false;
            }
            if (shuffleOn == true) //отключение перемешивания, если включено
            {
                shuffleOn = false;
            }

            media.Source = new Uri((string)musicList.SelectedItem);
            media.Play();
            addHistory(musicList.SelectedItem.ToString());
        }

        private void musicFold_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog window = new CommonOpenFileDialog { IsFolderPicker = true};
            var result = window.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string[]files = Directory.GetFiles(window.FileName, "*mp3");
                musicList.ItemsSource = files;
            }
            if (musicList.ItemsSource != null)
            {
                musicList.SelectedIndex = 0;
                media.Source = new Uri((string)musicList.Items[0]);
                media.Play();
            }
        }
        
        private void switchNext()
        {
            int index = musicList.SelectedIndex;
            if (index == musicList.Items.Count - 1)
            {
                index = 0;
                media.Source = new Uri((string)musicList.Items[index]);
                musicList.SelectedIndex = index;
                media.Play();
            }
            else
            {
                media.Source = new Uri((string)musicList.Items[index + 1]);
                musicList.SelectedIndex = index + 1;
                media.Play();
            }
        }

        private void shuffle_Click(object sender, RoutedEventArgs e)
        {
            if (shuffleOn == false)
            {  
                shuffleOn = true;
                shufflledMusic();
            }
            else
            {
                shuffleOn = false;
            }
        }

        private void volumeSlide_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (media != null)
            {
                media.Volume = volumeSlide.Value;
            }
        }

        private void media_MediaOpened(object sender, RoutedEventArgs e)
        {
            audioSlider.Value = 0;
            audioSlider.Maximum = media.NaturalDuration.TimeSpan.Ticks;
        }

        private void history_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow wind = new HistoryWindow();
            wind.outputHistory.ItemsSource = historyList;
            var result = wind.ShowDialog();
            if (result == true)
            {
                media.Source = new Uri(wind.Path);
                media.Play();
                musicList.SelectedItem = wind.Path;
            }
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            switchNext();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            int index = musicList.SelectedIndex;
            if (index == 0)
            {
                index = musicList.Items.Count - 1;
                media.Source = new Uri((string)musicList.Items[index]);
                musicList.SelectedIndex = index;
                media.Play();
            }
            else
            {
                media.Source = new Uri((string)musicList.Items[index - 1]);
                musicList.SelectedIndex = index - 1;
                media.Play();
            }
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (shuffleOn == true)
            {
                shufflledMusic();
            }
            else if (repeat == true)
            {
                replayAudio();
            }
            else
            {
                switchNext();
            }
        }
    }
}