﻿using Microsoft.UI.Xaml.Controls;
using RMP.App.Common;
using RMP.App.Dialogs;
using RMP.App.Settings.ViewModels;
using RMP.App.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationViewItemBase = Microsoft.UI.Xaml.Controls.NavigationViewItemBase;

namespace RMP.App.Windows
{
    /// <summary>
    /// Main app page, hosts the NavigationView and ContentFrame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        #region Variables
        private SettingsViewModel ViewModel => App.SViewModel;
        public static MainPage Current;

        private readonly NavigationHelper navigationHelper;
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }

        public ObservableCollection<Crumb> Breadcrumbs { get; set; }
            = new ObservableCollection<Crumb>();

        private MainTitleBar MainTitleBarHandle { get; set; }

        public SettingsDialog SDialog { get; }
            = new SettingsDialog();
        #endregion

        #region Classes
        public class Crumb
        {
            public string Title { get; set; }

            public override string ToString()
            {
                return Title;
            }
        }
        #endregion

        #region NavView Icons (TODO: FIX THIS TERRIBLE CODE)
        private readonly ImageIcon homeIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/At a glance.png")) };

        private readonly FontIcon homeIconMono =
            new FontIcon() { Glyph = "\uECA5" };

        private readonly ImageIcon playlistsIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Playlists.png")) };

        private readonly FontIcon playlistsIconMono =
            new FontIcon() { Glyph = "\uE8FD" };

        private readonly ImageIcon devicesIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Devices.png")) };

        private readonly FontIcon devicesIconMono =
            new FontIcon() { Glyph = "\uE1C9" };

        private readonly ImageIcon songsIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Songs.png")) };

        private readonly FontIcon songsIconMono =
            new FontIcon() { Glyph = "\uEC4F" };

        private readonly ImageIcon artistsIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Artists.png")) };

        private readonly FontIcon artistsIconMono =
            new FontIcon() { Glyph = "\uE125" };

        private readonly ImageIcon albumsIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Albums.png")) };

        private readonly FontIcon albumsIconMono =
            new FontIcon() { Glyph = "\uE93C" };

        private readonly ImageIcon genresIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Genres.png")) };

        private readonly FontIcon genresIconMono =
            new FontIcon() { Glyph = "\uE138" };

        private readonly ImageIcon videosIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Local Videos.png")) };

        private readonly FontIcon videosIconMono =
            new FontIcon() { Glyph = "\uE8B7" };

        private readonly ImageIcon streamingIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Online Videos.png")) };

        private readonly FontIcon streamingIconMono =
            new FontIcon() { Glyph = "\uE12B" };

        private readonly ImageIcon helpIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/DiscyHelp.png")) };

        private readonly FontIcon helpIconMono =
            new FontIcon() { Glyph = "\uE9CE" };

        private readonly ImageIcon playingIconColor =
            new ImageIcon() { Source = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Now Playing.png")) };

        private readonly FontIcon playingIconMono =
            new FontIcon() { Glyph = "\uE768" };
        #endregion

        public MainPage()
        {
            InitializeComponent();
            Current = this;

            MainTitleBarHandle = new MainTitleBar();

            Loaded += async (s, e) => await ApplyStartupSettings();
            DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Required;
            navigationHelper = new NavigationHelper(this);
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
            => MainTitleBarHandle.UpdateTitleBarItems(sender);

        #region NavigationHelper registration
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
            => navigationHelper.OnNavigatedTo(e);

        protected override void OnNavigatedFrom(NavigationEventArgs e)
            => navigationHelper.OnNavigatedFrom(e);
        #endregion

        #region Navigation
        private async void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string navTo = args.InvokedItemContainer.Tag.ToString();
            if (args.IsSettingsInvoked || navTo == "SettingsPage")
            {
                _ = await SDialog.ShowAsync();
                FinishNavigation();
                return;
            }

            if (navTo == ContentFrame.CurrentSourcePageType.ToString())
            {
                FinishNavigation();
                return;
            }

            if (navTo != null)
            {
                await Navigate(navTo);
            }
        }

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
            => ContentFrame.GoBack();

        private async Task Navigate(string navTo)
        {
            UnavailableDialog dialog;

            switch (navTo)
            {
                case "AlbumsPage":
                    _ = ContentFrame.Navigate(typeof(AlbumsPage));
                    break;

                case "ArtistsPage":
                    _ = ContentFrame.Navigate(typeof(ArtistsPage));
                    break;

                case "DevicesPage":
                    // _ = ContentFrame.Navigate(typeof(DevicesPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Device view and sync is not available yet.",
                        Description = "This will be coming in a future update.",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Devices.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                case "DiscyPage":
                    _ = ContentFrame.Navigate(typeof(DiscyPage));
                    break;

                case "GenresPage":
                    // _ = ContentFrame.Navigate(typeof(GenresPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "You can't check out the genres yet.",
                        Description = "Hopefully you can start soon!",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/NavigationView/Genres.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                case "HomePage":
                    _ = ContentFrame.Navigate(typeof(HomePage));
                    break;

                case "LocalVideosPage":
                    // _ = ContentFrame.Navigate(typeof(LocalVideosPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Sadly, local video is not available but is coming very soon.",
                        Description = "We can't wait for you to relive old memories.",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/Videos.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                case "NowPlayingPage":
                    _ = await GeneralControl.CreateWindow(typeof(NowPlaying),
                        AppWindowPresentationKind.Default, 320, 300);
                    break;

                case "PlaylistsPage":
                    // _ = ContentFrame.Navigate(typeof(PlaylistsPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Unfortunately, playlists aren't available yet. Go to your music library instead.",
                        Description = "Hopefully we won't be long adding them!",
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/Playlists.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                case "SongsPage":
                    _ = ContentFrame.Navigate(typeof(SongsPage));
                    break;

                case "StreamingPage":
                    // _ = ContentFrame.Navigate(typeof(StreamingPage));
                    dialog = new UnavailableDialog
                    {
                        Header = "Streaming services for videos, films and TV are not available yet.",
                        Description = "We are prioritising other features first, including local playback.",
                        LeftHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/Netflix.png")),
                        CenterHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/Prime.png")),
                        RightHero = new BitmapImage(new Uri("ms-appx:///Assets/Unavailable/YouTube.png"))
                    };

                    _ = await dialog.ShowAsync();
                    break;

                default:
                    break;
            }
        }

        public void FinishNavigation()
        {
            string type = ContentFrame.CurrentSourcePageType.ToString();
            string tag = type.Split('.').Last();

            Breadcrumbs.Clear();
            if (tag == "AlbumSongsPage" || tag == "ArtistSongsPage")
            {
                Breadcrumbs.Add(new Crumb
                {
                    Title = ""
                });
                return;
            }

            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    NavView.SelectedItem = item;
                    Breadcrumbs.Add(new Crumb
                    {
                        Title = item.Content.ToString()
                    });
                    return;
                }
            }

            foreach (NavigationViewItemBase item in NavView.FooterMenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == tag)
                {
                    NavView.SelectedItem = item;
                    Breadcrumbs.Add(new Crumb
                    {
                        Title = item.Content.ToString()
                    });
                    return;
                }
            }
        }
        #endregion

        #region Settings
        private async Task ApplyStartupSettings()
        {
            // Sidebar icon colors
            UpdateIconColor(ViewModel.IconPack);

            // Startup setting
            await Navigate(ViewModel.Open);

            FinishNavigation();
            PlayerElement.SetMediaPlayer(App.PViewModel.Player);
        }

        /// <summary>
        /// TERRIBLE FUNCTION, REMOVE ASAP!!!
        /// </summary>
        /// <param name="icons">WHY WOULD YOU WANT TO KNOW WHAT THIS DOES???</param>
        public void UpdateIconColor(int icons)
        {
            if (icons == 1)
            {
                SettingsPageItem.Visibility = Visibility.Visible;
                NavView.IsSettingsVisible = false;

                HomePageItem.Icon = homeIconColor;
                PlaylistsPageItem.Icon = playlistsIconColor;
                DevicesPageItem.Icon = devicesIconColor;
                SongsPageItem.Icon = songsIconColor;
                ArtistsPageItem.Icon = artistsIconColor;
                AlbumsPageItem.Icon = albumsIconColor;
                GenresPageItem.Icon = genresIconColor;
                LocalVideosPageItem.Icon = videosIconColor;
                StreamingPageItem.Icon = streamingIconColor;
                DiscyPageItem.Icon = helpIconColor;
                NowPlayingPageItem.Icon = playingIconColor;
                return;
            }

            SettingsPageItem.Visibility = Visibility.Collapsed;
            NavView.IsSettingsVisible = true;

            HomePageItem.Icon = homeIconMono;
            PlaylistsPageItem.Icon = playlistsIconMono;
            DevicesPageItem.Icon = devicesIconMono;
            SongsPageItem.Icon = songsIconMono;
            ArtistsPageItem.Icon = artistsIconMono;
            AlbumsPageItem.Icon = albumsIconMono;
            GenresPageItem.Icon = genresIconMono;
            LocalVideosPageItem.Icon = videosIconMono;
            StreamingPageItem.Icon = streamingIconMono;
            DiscyPageItem.Icon = helpIconMono;
            NowPlayingPageItem.Icon = playingIconMono;
        }
        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
            => _ = await FileHelpers.LaunchURIAsync(URLs.Feedback);

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem click = (MenuFlyoutItem)sender;
            HideItem(click.Tag.ToString(), false);
        }

        private void HideItem(string item, bool value)
        {
            int visibilityCheck = 0;
            switch (item)
            {
                case "Home":
                    ViewModel.ShowAtAGlance = value;
                    break;

                case "Playlists":
                    ViewModel.ShowPlaylists = value;
                    break;

                case "Devices":
                    ViewModel.ShowDevices = value;
                    break;

                case "Songs":
                    ViewModel.ShowSongs = value;
                    visibilityCheck = 1;
                    break;

                case "Artists":
                    ViewModel.ShowArtists = value;
                    visibilityCheck = 1;
                    break;

                case "Albums":
                    ViewModel.ShowAlbums = value;
                    visibilityCheck = 1;
                    break;

                case "Genres":
                    ViewModel.ShowGenres = value;
                    visibilityCheck = 1;
                    break;

                case "LocalVideos":
                    ViewModel.ShowLocalVideos = value;
                    visibilityCheck = 2;
                    break;

                case "Streaming":
                    ViewModel.ShowStreaming = value;
                    visibilityCheck = 2;
                    break;

                case "Help":
                    ViewModel.ShowHelpCentre = value;
                    break;

                case "NowPlaying":
                    ViewModel.ShowNowPlaying = value;
                    break;
            }

            ViewModel.ChangeHeaderVisibility(visibilityCheck);
        }

        private void Button_RightTapped(object sender, RightTappedRoutedEventArgs e)
            => App.MViewModel.Sync();

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
            => FinishNavigation();
    }
}
