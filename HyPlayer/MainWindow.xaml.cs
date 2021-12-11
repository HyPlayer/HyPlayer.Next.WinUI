using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using HyPlayer.Casper;
using HyPlayer.Casper.Model;
using HyPlayer.NCMProvider;
using HyPlayer.NCMProvider.Models;
using HyPlayer.NCMProvider.Returns;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HyPlayer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private NeteaseCloudMusicProvider _neteaseCloudMusicProvider;
        public AccountInfo AccountInfo = new AccountInfo();
        public PlayCore PlayCore; 
        public IntPtr WindowHandle;
        
        public MainWindow()
        {
            this.InitializeComponent();
            WindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            PlayCore = new PlayCore(WindowHandle);
            _neteaseCloudMusicProvider = new NeteaseCloudMusicProvider();
            PlayCore.RegisterMusicProvider(_neteaseCloudMusicProvider);
        }

        private async void ButtonLoadMusic_OnClick(object sender, RoutedEventArgs e)
        {
            PlayCore.MoveNext();
            await PlayCore.LoadNowPlayingItemMedia();
        }

        private void ButtonPlayMusic_OnClick(object sender, RoutedEventArgs e)
        {
            PlayCore.PlayService.Play();
        }

        private async void ButtonNextMusic_OnClick(object sender, RoutedEventArgs e)
        {
            PlayCore.PlayService.Stop();
            PlayCore.MoveNext();
            await PlayCore.LoadNowPlayingItemMedia();
        }

        private async void ButtonLoadPlayList_OnClick(object sender, RoutedEventArgs e)
        {
            var list = (await _neteaseCloudMusicProvider.GetPlayItem("pl" + AccountInfo.PlayListId)) as NCMUserPlayList;
            PlayCore.ReplacePlaySource(list);
            PlayCore.LoadPlaySource();
        }

        private void ButtonPauseMusic_OnClick(object sender, RoutedEventArgs e)
        {
            PlayCore.PlayService.Pause();
        }

        private async void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            var ret = await _neteaseCloudMusicProvider.RequestAsync<LoginReturn>(NeteaseApis.LoginCellphone,
                new Dictionary<string, object>()
                {
                    { "phone", AccountInfo.Account },
                    { "password", AccountInfo.Password }
                });
            AccountInfo.UserName = ret.Profile.Nickname;
        }
    }

    class PlayListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var playList = value as List<SingleSong>;
            if (playList == null)
                return null;
            return playList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AccountInfo : DependencyObject
    {
        public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(
            "UserName", typeof(string), typeof(AccountInfo), new PropertyMetadata(default(string)));

        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(
            "Account", typeof(string), typeof(AccountInfo), new PropertyMetadata(default(string)));

        public string Account
        {
            get { return (string)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register(
            "Password", typeof(string), typeof(AccountInfo), new PropertyMetadata(default(string)));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PlayListIdProperty = DependencyProperty.Register(
            "PlayListId", typeof(string), typeof(AccountInfo), new PropertyMetadata(default(string)));

        public string PlayListId
        {
            get { return (string)GetValue(PlayListIdProperty); }
            set { SetValue(PlayListIdProperty, value); }
        }
    }
}