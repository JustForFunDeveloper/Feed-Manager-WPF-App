﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HS_Feed_Manager.Core;
using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.DataModels.DbModels;
using HS_Feed_Manager.ViewModels.Handler;
using JetBrains.Annotations;
using MahApps.Metro.IconPacks;
using PropertyChanged;
using Serilog;

namespace HS_Feed_Manager.ViewModels
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class HomeViewModel : INotifyPropertyChanged
    {
        // ReSharper disable once NotAccessedField.Local
        public event PropertyChangedEventHandler PropertyChanged;

        #region private Member

        private readonly List<string> _feedSortModes = new List<string>
        {
            "Not sorted",
            "Auto Download On",
            "Auto Download Off",
            "Exists locally",
            "Don't exists locally",
            "Status Undefined",
            "Status Ongoing",
            "Status New",
            "Status Finished",
            "Sort By Name Asc.",
            "Sort By Name Desc."
        };

        private readonly List<string> _localSortModes = new List<string>
        {
            "Not sorted",
            "Sort Date Newest",
            "Sort Date Oldest",
            "Auto Download On",
            "Auto Download Off",
            "Status Undefined",
            "Status Ongoing",
            "Status New",
            "Status Finished",
            "Sort By Name Asc.",
            "Sort By Name Desc."
        };

        private readonly List<string> _episodeSortModes = new List<string>
        {
            "Not sorted",
            "By Number Asc.",
            "By Number Desc.",
            "By Rating Asc.",
            "By Rating Desc.",
            "By Date Asc.",
            "By Date Desc."
        };

        public List<string> SortModes { get; set; }

        private ICommand _downloadFeed;
        private ICommand _searchLocalFolder;
        private ICommand _copyFromDownload;
        private ICommand _stopcopyFromDownload;

        private ICommand _copyTvShowName;
        private ICommand _textBoxButtonCmd;
        private ICommand _editLocalInfo;
        private ICommand _toggleAutoDownload;
        private ICommand _deleteFeedFromList;

        private ICommand _episodeTextBoxButtonCmd;
        private ICommand _playEpisode;
        private ICommand _openFolder;
        private ICommand _editEpisodeInfo;
        private ICommand _deleteEpisode;

        private ICommand _moveToFirst;
        private ICommand _moveToLast;
        private ICommand _moveUp;
        private ICommand _moveDown;

        private ICommand _addToList;
        private ICommand _deleteFromList;
        private ICommand _downloadThis;
        private ICommand _downloadAll;
        private ICommand _deleteTvShow;

        private int _sortModesIndex;
        private string _filterString = "";

        public int TabIndex { get; set; }
        public string ClearSearch { get; set; }
        private int _episodeSortModesIndex;
        public List<string> EpisodeSortModes { get; set; }
        public string EpisodeClearSearch { get; set; }

        public Visibility DownloadListVisibility { get; set; }
        public ObservableCollection<Episode> DownloadList { get; set; }
        public int SelectedDownloadIndex { get; set; }
        private Episode _selectedDownloadItem;

        public ObservableCollection<Episode> EpisodeList { get; set; }
        public object SelectedEpisode { get; set; }
        public Visibility EpisodeListVisibility { get; set; }

        public bool IsProgressActive { get; set; }
        public string ProgressToolTip { get; set; }
        public bool IsSearchLocalEnabled { get; set; }
        public bool ShowCopyFromDownloadStart { get; set; }
        public bool ShowCopyFromDownloadStop { get; set; }

        public ICollectionView FeedListView { get; private set; }
        public Visibility FeedInfoVisibility { get; set; }
        public string FeedInfoName { get; set; }
        public string FeedInfoEpisode { get; set; }
        public string FeedInfoAutoDownload { get; set; }
        public string FeedInfoLocalEpisodeCount { get; set; }
        public ObservableCollection<string> FeedIcons { get; set; }

        private readonly ObservableCollection<string> _feedIcons = new ObservableCollection<string>
        {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString()
        };

        public ICollectionView LocalListView { get; private set; }
        public Visibility LocalInfoVisibility { get; set; }
        public string LocalInfoName { get; set; }
        public string LocalInfoStatus { get; set; }
        public string LocalInfoEpisodes { get; set; }
        public string LocalInfoAutoDownload { get; set; }
        public string LocalInfoLocalEpisodeCount { get; set; }
        public ObservableCollection<string> LocalIcons { get; set; }

        private readonly ObservableCollection<string> _localIcons = new ObservableCollection<string>
        {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString()
        };

        public Visibility EpisodeInfoVisibility { get; set; }

        public string EpisodeInfoName { get; set; }
        public string EpisodeInfoPath { get; set; }
        public string EpisodeInfoStatus { get; set; }
        public string EpisodeInfoEpisode { get; set; }
        public string EpisodeInfoAutoDownload { get; set; }
        public string EpisodeInfoLocalEpisodeCount { get; set; }
        public string EpisodeInfoDownloadDate { get; set; }
        public ObservableCollection<string> EpisodeIcons { get; set; }

        private readonly ObservableCollection<string> _episodeIcons = new ObservableCollection<string>
        {
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString(),
            PackIconMaterialDesignKind.StarBorder.ToString()
        };

        private ImageSource _imageSource;

        #endregion

        public HomeViewModel()
        {
            InitialiseListViews();
            InitialiseProperties();

            Mediator.Register(MediatorGlobal.TabControlSelectionChanged, TabControlSelectionChanged);
            Mediator.Register(MediatorGlobal.ListBoxSelectionChanged, ListBoxSelectionChanged);
            Mediator.Register(MediatorGlobal.OnRefreshListView, OnRefreshListViews);
            Mediator.Register(MediatorGlobal.FinishedSearchLocalFolder, OnFinishedSearchLocalFolder);
            Mediator.Register(MediatorGlobal.UpdateDownloadList, OnUpdateDownloadList);
            Mediator.Register(MediatorGlobal.ListBoxDoubleClick, OnListBoxDoubleClick);
            Mediator.Register(MediatorGlobal.CustomDialogReturn, OnCustomDialogReturn);
            Mediator.Register(MediatorGlobal.ProgressMessage, OnProgressMessage);
            Mediator.Register(MediatorGlobal.FinishedCopyDownload, OnFinishedCopyDownload);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnCustomPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitialiseProperties()
        {
            IsSearchLocalEnabled = true;

            SortModes = _feedSortModes;
            FeedInfoVisibility = Visibility.Visible;
            FeedIcons = _feedIcons;

            LocalInfoVisibility = Visibility.Hidden;
            LocalIcons = _localIcons;

            EpisodeInfoVisibility = Visibility.Hidden;
            EpisodeIcons = _episodeIcons;

            DownloadListVisibility = Visibility.Visible;
            DownloadList = new ObservableCollection<Episode>();

            EpisodeListVisibility = Visibility.Hidden;
            EpisodeSortModes = _episodeSortModes;

            ShowCopyFromDownloadStart = true;
        }

        private void OnCustomDialogReturn(object obj)
        {
            try
            {
                if (obj == null)
                    return;
                List<object> answer = obj as List<object>;

                if (answer == null)
                    return;

                string identifier = (string) answer[0];
                bool returnValue = (bool) answer[1];

                if (identifier == null)
                    return;

                switch (identifier)
                {
                    case "DeleteEpisode":
                        OnDeleteEpisodeAnswer(returnValue);
                        break;
                    case "DeleteTvShow":
                        OnDeleteTvShow(returnValue);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnCustomDialogReturn Error!");
            }
        }

        #region IconBar

        public ICommand DownloadFeed
        {
            get
            {
                if (_downloadFeed == null)
                    _downloadFeed = new RelayCommand(
                        param => DownloadFeedCommand(),
                        param => CanDownloadFeedCommand()
                    );
                return _downloadFeed;
            }
        }

        private bool CanDownloadFeedCommand()
        {
            return true;
        }

        private void DownloadFeedCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.DownloadFeed, null);
        }

        public ICommand SearchLocalFolder
        {
            get
            {
                if (_searchLocalFolder == null)
                    _searchLocalFolder = new RelayCommand(
                        param => SearchLocalFolderCommand(),
                        param => CanSearchLocalFolderCommand()
                    );
                return _searchLocalFolder;
            }
        }

        private bool CanSearchLocalFolderCommand()
        {
            return true;
        }

        private void SearchLocalFolderCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.SearchLocalFolder, null);
            IsProgressActive = true;
            IsSearchLocalEnabled = false;
            ProgressToolTip = "Searching local folders!";
        }

        private void OnFinishedSearchLocalFolder(object obj)
        {
            IsProgressActive = false;
            IsSearchLocalEnabled = true;
            ProgressToolTip = "";
        }
        
        public ICommand CopyFromDownload
        {
            get
            {
                if (_copyFromDownload == null)
                    _copyFromDownload = new RelayCommand(
                        param => CopyFromDownloadCommand(),
                        param => CanCopyFromDownload()
                    );
                return _copyFromDownload;
            }
        }

        private bool CanCopyFromDownload()
        {
            return true;
        }

        private void CopyFromDownloadCommand()
        {
            try
            {
                Mediator.NotifyColleagues(MediatorGlobal.CopyFromDownload, null);
                ShowCopyFromDownloadStart = false;
                ShowCopyFromDownloadStop = true;
                IsProgressActive = true;
                ProgressToolTip = "Started to copy from download folder";
            }
            catch (Exception ex)
            {
                Log.Error(ex,"CopyFromDownloadCommand Error!");
            }
        }
        
        public ICommand StopCopyFromDownload
        {
            get
            {
                if (_stopcopyFromDownload == null)
                    _stopcopyFromDownload = new RelayCommand(
                        param => StopCopyFromDownloadCommand(),
                        param => CanStopCopyFromDownload()
                    );
                return _stopcopyFromDownload;
            }
        }

        private bool CanStopCopyFromDownload()
        {
            return true;
        }

        private void StopCopyFromDownloadCommand()
        {
            try
            {
                Mediator.NotifyColleagues(MediatorGlobal.StopCopyFromDownload, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"StopCopyFromDownloadCommand Error!");
            }
        }

        private void OnProgressMessage(object obj)
        {
            if (obj != null)
            {
                var message = obj as string;
                ProgressToolTip = message;
            }
        }
        
        private void OnFinishedCopyDownload(object obj)
        {
            ShowCopyFromDownloadStart = true;
            ShowCopyFromDownloadStop = false;
            IsProgressActive = false;
            IsSearchLocalEnabled = true;
            ProgressToolTip = "";
        }

        #endregion

        #region Search and Filter

        #region SearchBox and ComboBox

        public ICommand TextBoxButtonCmd
        {
            get
            {
                if (_textBoxButtonCmd == null)
                    _textBoxButtonCmd = new RelayCommand<object>(
                        param => TextBoxButtonCommand(param),
                        param => CanTextBoxButtonCommand()
                    );
                return _textBoxButtonCmd;
            }
        }

        private bool CanTextBoxButtonCommand()
        {
            return true;
        }

        private void TextBoxButtonCommand(object param)
        {
            try
            {
                if (param == null)
                    return;

                if (param is string)
                {
                    _filterString = param as string;
                    LocalListView.Filter = SearchFilter;
                    FeedListView.Filter = SearchFilter;
                }
                else if (param.GetType() == typeof(TextBox))
                {
                    var textBox = param as TextBox;
                    if (textBox != null && textBox.Text.Length == 0)
                    {
                        _filterString = "";
                        LocalListView.Filter = SearchFilter;
                        FeedListView.Filter = SearchFilter;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"TextBoxButtonCommand Error!");
            }
        }

        [DoNotNotify]
        public int SortModesIndex
        {
            get => _sortModesIndex;
            set
            {
                _sortModesIndex = value;
                SortThisList(value);
                OnCustomPropertyChanged();
            }
        }

        #endregion

        #region Views, Filter and Sorting

        private void OnRefreshListViews(object obj)
        {
            FeedListView.Refresh();
            LocalListView.Refresh();
        }

        private void InitialiseListViews()
        {
            try
            {
                FeedListView = CollectionViewSource.GetDefaultView(Logic.FeedEpisodes);
                FeedListView.CurrentChanged += FeedListViewCurrentChanged;

                LocalListView = CollectionViewSource.GetDefaultView(Logic.LocalTvShows);
                LocalListView.CurrentChanged += LocalListViewCurrentChanged;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"InitialiseListViews Error!");
            }
        }

        private void SortThisList(int value)
        {
            try
            {
                if (value == -1)
                    return;
                using (LocalListView.DeferRefresh())
                {
                    using (FeedListView.DeferRefresh())
                    {
                        if (TabIndex == 0)
                        {
                            FeedListView.SortDescriptions.Clear();
                            switch (value)
                            {
                                case 0:
                                    FeedListView.Filter = null;
                                    break;
                                case 1:
                                    FeedListView.Filter = AutowDownloadFilterOn;
                                    break;
                                case 2:
                                    FeedListView.Filter = AutowDownloadFilterOff;
                                    break;
                                case 3:
                                    FeedListView.Filter = ExistsLocally;
                                    break;
                                case 4:
                                    FeedListView.Filter = DoesntExistsLocally;
                                    break;
                                case 5:
                                    FeedListView.Filter = StatusFilterUndefined;
                                    break;
                                case 6:
                                    FeedListView.Filter = StatusFilterOngoing;
                                    break;
                                case 7:
                                    FeedListView.Filter = StatusFilterNew;
                                    break;
                                case 8:
                                    FeedListView.Filter = StatusFilterFinished;
                                    break;
                                case 9:
                                    FeedListView.Filter = null;
                                    FeedListView.SortDescriptions.Add(new SortDescription("Name",
                                        ListSortDirection.Ascending));
                                    break;
                                case 10:
                                    FeedListView.Filter = null;
                                    FeedListView.SortDescriptions.Add(new SortDescription("Name",
                                        ListSortDirection.Descending));
                                    break;
                            }
                        }
                        else if (TabIndex == 1)
                        {
                            LocalListView.SortDescriptions.Clear();
                            switch (value)
                            {
                                case 0:
                                    LocalListView.Filter = null;
                                    break;
                                case 1:
                                    LocalListView.Filter = null;
                                    LocalListView.SortDescriptions.Add(new SortDescription("LatestDownload",
                                        ListSortDirection.Descending));
                                    break;
                                case 2:
                                    LocalListView.Filter = null;
                                    LocalListView.SortDescriptions.Add(new SortDescription("LatestDownload",
                                        ListSortDirection.Ascending));
                                    break;
                                case 3:
                                    LocalListView.Filter = AutowDownloadFilterOn;
                                    break;
                                case 4:
                                    LocalListView.Filter = AutowDownloadFilterOff;
                                    break;
                                case 5:
                                    LocalListView.Filter = StatusFilterUndefined;
                                    break;
                                case 6:
                                    LocalListView.Filter = StatusFilterOngoing;
                                    break;
                                case 7:
                                    LocalListView.Filter = StatusFilterNew;
                                    break;
                                case 8:
                                    LocalListView.Filter = StatusFilterFinished;
                                    break;
                                case 9:
                                    LocalListView.Filter = null;
                                    LocalListView.SortDescriptions.Add(new SortDescription("Name",
                                        ListSortDirection.Ascending));
                                    break;
                                case 10:
                                    LocalListView.Filter = null;
                                    LocalListView.SortDescriptions.Add(new SortDescription("Name",
                                        ListSortDirection.Descending));
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"SortThisList Error!");
            }
        }

        private bool AutowDownloadFilterOn(object item)
        {
            try
            {
                if (item.GetType() == typeof(Episode))
                    return Logic.LocalTvShows.Any(x =>
                        x.AutoDownloadStatus == AutoDownload.On && x.Name == ((Episode) item).Name);

                if (item.GetType() == typeof(TvShow))
                {
                    var localSeries = item as TvShow;
                    if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                    return localSeries.AutoDownloadStatus.Equals(AutoDownload.On);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"AutowDownloadFilterOn Error!");
                return false;
            }
        }

        private bool AutowDownloadFilterOff(object item)
        {
            try
            {
                if (item.GetType() == typeof(Episode))
                    return Logic.LocalTvShows.Any(x =>
                        x.AutoDownloadStatus == AutoDownload.Off && x.Name == ((Episode) item).Name);

                if (item.GetType() == typeof(TvShow))
                {
                    var localSeries = item as TvShow;
                    if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                    return localSeries.AutoDownloadStatus.Equals(AutoDownload.Off);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"AutowDownloadFilterOff Error!");
                return false;
            }
        }

        private bool ExistsLocally(object item)
        {
            try
            {
                return Logic.LocalTvShows.Any(x => x.Name == ((Episode) item).Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"ExistsLocally Error!");
                return false;
            }
        }

        private bool DoesntExistsLocally(object item)
        {
            try
            {
                return !Logic.LocalTvShows.Any(x => x.Name == ((Episode) item).Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DoesntExistsLocally Error!");
                return false;
            }
        }

        private bool StatusFilterUndefined(object item)
        {
            try
            {
                if (item.GetType() == typeof(Episode))
                    return Logic.LocalTvShows.Any(x => x.Status == Status.Undefined && x.Name == ((Episode) item).Name);

                if (item.GetType() == typeof(TvShow))
                {
                    var localSeries = item as TvShow;
                    if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                    return localSeries.Status.Equals(Status.Undefined);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"StatusFilterUndefined Error!");
                return false;
            }
        }

        private bool StatusFilterOngoing(object item)
        {
            try
            {
                if (item.GetType() == typeof(Episode))
                    return Logic.LocalTvShows.Any(x => x.Status == Status.Ongoing && x.Name == ((Episode) item).Name);

                if (item.GetType() == typeof(TvShow))
                {
                    var localSeries = item as TvShow;
                    if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                    return localSeries.Status.Equals(Status.Ongoing);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"StatusFilterOngoing Error!");
                return false;
            }
        }

        private bool StatusFilterNew(object item)
        {
            try
            {
                if (item.GetType() == typeof(Episode))
                    return Logic.LocalTvShows.Any(x => x.Status == Status.New && x.Name == ((Episode) item).Name);

                if (item.GetType() == typeof(TvShow))
                {
                    var localSeries = item as TvShow;
                    if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                    return localSeries.Status.Equals(Status.New);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"StatusFilterNew Error!");
                return false;
            }
        }

        private bool StatusFilterFinished(object item)
        {
            try
            {
                if (item.GetType() == typeof(Episode))
                    return Logic.LocalTvShows.Any(x => x.Status == Status.Finished && x.Name == ((Episode) item).Name);

                if (item.GetType() == typeof(TvShow))
                {
                    var localSeries = item as TvShow;
                    if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                    return localSeries.Status.Equals(Status.Finished);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"StatusFilterFinished Error!");
                return false;
            }
        }

        private bool SearchFilter(object item)
        {
            try
            {
                if (item.GetType() == typeof(Episode))
                {
                    var episode = item as Episode;
                    if (episode == null) throw new ArgumentNullException(nameof(episode));
                    return episode.Name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0;
                }

                if (item.GetType() == typeof(TvShow))
                {
                    var localSeries = item as TvShow;
                    if (localSeries == null) throw new ArgumentNullException(nameof(localSeries));
                    return localSeries.Name.IndexOf(_filterString, StringComparison.OrdinalIgnoreCase) >= 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"SearchFilter Error!");
                return false;
            }
        }

        #endregion

        #region CurrentChangedEvents

        private void FeedListViewCurrentChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null)
                    return;

                if (TabIndex == 0)
                {
                    FeedInfoVisibility = Visibility.Visible;
                    LocalInfoVisibility = Visibility.Hidden;
                    EpisodeInfoVisibility = Visibility.Hidden;
                    DownloadListVisibility = Visibility.Visible;
                    EpisodeListVisibility = Visibility.Hidden;
                }

                var feedEpisode = (Episode) FeedListView.CurrentItem;

                if (feedEpisode == null)
                    return;

                FeedInfoName = feedEpisode.Name;
                FeedInfoEpisode = feedEpisode.EpisodeNumber.ToString();

                var localSeries = Logic.LocalTvShows.SingleOrDefault(x => x.Name == feedEpisode.Name);

                if (localSeries != null)
                {
                    FeedInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                    FeedInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();

                    for (var i = 0; i < localSeries.Rating; i++)
                        FeedIcons[i] = PackIconMaterialDesignKind.Star.ToString();
                    for (var i = 4; i >= localSeries.Rating; i--)
                        FeedIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
                else
                {
                    FeedInfoAutoDownload = "None";
                    FeedInfoLocalEpisodeCount = "None";

                    for (var i = 4; i >= 0; i--) FeedIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"FeedListViewCurrentChanged Error!");
            }
        }

        private void LocalListViewCurrentChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender == null)
                    return;
                if (TabIndex == 1)
                {
                    FeedInfoVisibility = Visibility.Hidden;
                    LocalInfoVisibility = Visibility.Visible;
                    EpisodeInfoVisibility = Visibility.Hidden;
                    DownloadListVisibility = Visibility.Hidden;
                    EpisodeListVisibility = Visibility.Visible;
                }

                var localSeries = (TvShow) LocalListView.CurrentItem;

                if (localSeries != null)
                {
                    EpisodeSortModesIndex = 2;
                    EpisodeClearSearch = "";

                    LocalInfoName = localSeries.Name;
                    LocalInfoStatus = localSeries.Status.ToString();
                    LocalInfoEpisodes = localSeries.EpisodeCount.ToString();
                    LocalInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                    LocalInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();
                    if (localSeries.ImagePath == null)
                    {
                        ImageSource = null;
                    }
                    else
                    {
                        ImageSource = new BitmapImage(new Uri(localSeries.ImagePath));
                    }

                    for (var i = 0; i < localSeries.Rating; i++)
                        LocalIcons[i] = PackIconMaterialDesignKind.Star.ToString();
                    for (var i = 4; i >= localSeries.Rating; i--)
                        LocalIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
                else
                {
                    for (var i = 4; i >= 0; i--) LocalIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"LocalListViewCurrentChanged Error!");
            }
        }

        private void ListBoxSelectionChanged(object obj)
        {
            try
            {
                if (TabIndex == 1)
                {
                    FeedInfoVisibility = Visibility.Hidden;
                    LocalInfoVisibility = Visibility.Hidden;
                    EpisodeInfoVisibility = Visibility.Visible;
                }

                var listEpisode = (Episode) obj;

                if (listEpisode == null)
                {
                    for (var i = 4; i >= 0; i--) EpisodeIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();
                    return;
                }


                EpisodeInfoName = listEpisode.Name;
                EpisodeInfoPath = listEpisode.LocalPath;
                EpisodeInfoEpisode = listEpisode.EpisodeNumber.ToString();
                EpisodeInfoDownloadDate = listEpisode.DownloadDate.ToString("dd.MM.yyyy HH:mm:ss");

                for (var i = 0; i < listEpisode.Rating; i++)
                    EpisodeIcons[i] = PackIconMaterialDesignKind.Star.ToString();
                for (var i = 4; i >= listEpisode.Rating; i--)
                    EpisodeIcons[i] = PackIconMaterialDesignKind.StarBorder.ToString();

                var localSeries = Logic.LocalTvShows.Where(x => x.Name == listEpisode.Name).SingleOrDefault();

                if (localSeries != null)
                {
                    EpisodeInfoStatus = localSeries.Status.ToString();
                    EpisodeInfoAutoDownload = localSeries.AutoDownloadStatus.ToString();
                    EpisodeInfoLocalEpisodeCount = localSeries.LocalEpisodesCount.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"ListBoxSelectionChanged Error!");
            }
        }

        #endregion

        #endregion

        #region Tab

        private void TabControlSelectionChanged(object obj)
        {
            if (TabIndex == 0)
            {
                SortModes = _feedSortModes;
                SortModesIndex = 0;
                ClearSearch = "";
            }
            else if (TabIndex == 1)
            {
                SortModes = _localSortModes;
                SortModesIndex = 1;
                ClearSearch = "";
            }
        }

        #endregion

        #region Feed List

        public ICommand ToggleAutoDownload
        {
            get
            {
                if (_toggleAutoDownload == null)
                    _toggleAutoDownload = new RelayCommand<object>(
                        param => ToggleAutoDownloadCommand(param),
                        param => CanToggleAutoDownloadCommand()
                    );
                return _toggleAutoDownload;
            }
        }

        private bool CanToggleAutoDownloadCommand()
        {
            return true;
        }

        private void ToggleAutoDownloadCommand(object param)
        {
            try
            {
                TvShow currentTvShow = null;
                string listName = param as string;
                if (listName != null && listName.Equals("LocalList"))
                {
                    currentTvShow = (TvShow) LocalListView.CurrentItem;
                    currentTvShow.AutoDownloadStatus = currentTvShow.AutoDownloadStatus.Equals(AutoDownload.On)
                        ? AutoDownload.Off
                        : AutoDownload.On;
                    Mediator.NotifyColleagues(MediatorGlobal.ToggleAutoDownload, currentTvShow);
                }
                else if (listName != null && listName.Equals("FeedList"))
                {
                    Episode currentEpisode = (Episode) FeedListView.CurrentItem;
                    currentTvShow =
                        Logic.LocalTvShows.SingleOrDefault(tvShow => tvShow.Name.Equals(currentEpisode.Name));
                    if (currentTvShow == null)
                    {
                        currentTvShow = new TvShow()
                        {
                            AutoDownloadStatus = AutoDownload.On,
                            Name = currentEpisode.Name
                        };
                        if (!DownloadList.Any(x => x.Name.Equals(currentEpisode.Name)))
                            DownloadList.Add(currentEpisode);
                    }
                    else
                    {
                        currentTvShow.AutoDownloadStatus = currentTvShow.AutoDownloadStatus.Equals(AutoDownload.On)
                            ? AutoDownload.Off
                            : AutoDownload.On;
                        if (currentTvShow.AutoDownloadStatus.Equals(AutoDownload.On))
                            if (!DownloadList.Any(x => x.Name.Equals(currentEpisode.Name)))
                                DownloadList.Add(currentEpisode);
                    }
                }

                if (currentTvShow != null)
                    Mediator.NotifyColleagues(MediatorGlobal.ToggleAutoDownload, currentTvShow);

                OnRefreshListViews(null);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"ToggleAutoDownloadCommand Error!");
            }
        }

        public ICommand DeleteFeedFromList
        {
            get
            {
                if (_deleteFeedFromList == null)
                    _deleteFeedFromList = new RelayCommand(
                        param => DeleteFeedFromListCommand(),
                        param => CanDeleteFeedFromListCommand()
                    );
                return _deleteFeedFromList;
            }
        }

        private bool CanDeleteFeedFromListCommand()
        {
            return true;
        }

        private void DeleteFeedFromListCommand()
        {
            try
            {
                if (FeedListView.CurrentItem == null)
                    return;

                DownloadList.Remove((Episode) FeedListView.CurrentItem);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DeleteFeedFromListCommand Error!");
            }
        }

        private void OnListBoxDoubleClick(object obj)
        {
            try
            {
                Episode episode = (Episode) FeedListView.CurrentItem;

                if (episode == null)
                    return;

                TvShow localTvShow = Logic.LocalTvShows.SingleOrDefault(tvShow => tvShow.Name.Equals(episode.Name));
                if (localTvShow == null)
                    return;

                TabIndex = 1;
                LocalListView.MoveCurrentToNext();
                LocalListView.MoveCurrentTo(localTvShow);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnListBoxDoubleClick Error!");
            }
        }

        #endregion

        #region Local List

        public ICommand CopyTvShowName
        {
            get
            {
                if (_copyTvShowName == null)
                    _copyTvShowName = new RelayCommand(
                        param => CopyTvShowNameCommand(),
                        param => CanCopyTvShowNameCommand()
                    );
                return _copyTvShowName;
            }
        }

        private bool CanCopyTvShowNameCommand()
        {
            return true;
        }

        private void CopyTvShowNameCommand()
        {
            try
            {
                if (LocalListView.CurrentItem == null)
                    return;

                var localSeries = (TvShow) LocalListView.CurrentItem;
                Clipboard.SetText(localSeries.Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"CopyTvShowNameCommand Error!");
            }
        }

        public ICommand EditLocalInfo
        {
            get
            {
                if (_editLocalInfo == null)
                    _editLocalInfo = new RelayCommand(
                        param => EditLocalInfoCommand(),
                        param => CanEditLocalInfoCommand()
                    );
                return _editLocalInfo;
            }
        }

        private bool CanEditLocalInfoCommand()
        {
            return true;
        }

        private void EditLocalInfoCommand()
        {
            try
            {
                if (LocalListView.CurrentItem == null)
                    return;

                var localSeries = (TvShow) LocalListView.CurrentItem;
                Mediator.NotifyColleagues(MediatorGlobal.UpdateFlyoutValues, localSeries);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"EditLocalInfoCommand Error!");
            }
        }

        public ICommand DeleteTvShow
        {
            get
            {
                if (_deleteTvShow == null)
                    _deleteTvShow = new RelayCommand(
                        param => DeleteTvShowCommand(),
                        param => CanDeleteTvShowCommand()
                    );
                return _deleteTvShow;
            }
        }

        private bool CanDeleteTvShowCommand()
        {
            return true;
        }

        private void DeleteTvShowCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.CustomDialog, new List<string>()
            {
                "DeleteTvShow",
                "Delete Series?",
                "Warning! This will delete all local episode files and all database entries of this series! \n" +
                "Are you sure you want to delete this Series?"
            });
        }

        private void OnDeleteTvShow(bool answer)
        {
            try
            {
                if (LocalListView.CurrentItem == null)
                    return;
                var localTvShow = (TvShow) LocalListView.CurrentItem;
                if (answer)
                    Mediator.NotifyColleagues(MediatorGlobal.DeleteTvShow, localTvShow);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnDeleteTvShow Error!");
            }
        }

        #endregion

        #region Download List

        [DoNotNotify]
        public Episode SelectedDownloadItem
        {
            get => _selectedDownloadItem;
            set
            {
                _selectedDownloadItem = value;
                OnSelectedDownloadItem(value);
                OnCustomPropertyChanged();
            }
        }

        private void OnSelectedDownloadItem(Episode episode)
        {
            try
            {
                if (null == episode)
                    return;

                Episode feedEpisode = Logic.FeedEpisodes.SingleOrDefault(ep =>
                    ep.Name.Equals(episode.Name) && ep.EpisodeNumber.Equals(episode.EpisodeNumber));

                if (feedEpisode == null)
                    return;

                FeedListView.MoveCurrentToNext();
                FeedListView.MoveCurrentTo(feedEpisode);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnSelectedDownloadItem Error!");
            }
        }

        public ICommand MoveToFirst
        {
            get
            {
                if (_moveToFirst == null)
                    _moveToFirst = new RelayCommand(
                        param => MoveToFirstCommand(),
                        param => CanMoveToFirstCommand()
                    );
                return _moveToFirst;
            }
        }

        private bool CanMoveToFirstCommand()
        {
            return true;
        }

        private void MoveToFirstCommand()
        {
            try
            {
                if (SelectedDownloadIndex < 0 || DownloadList.Count <= 0)
                    return;

                var episode = DownloadList[SelectedDownloadIndex];
                DownloadList.RemoveAt(SelectedDownloadIndex);
                DownloadList.Insert(0, episode);
                SelectedDownloadIndex = 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"MoveToFirstCommand Error!");
            }
        }

        public ICommand MoveToLast
        {
            get
            {
                if (_moveToLast == null)
                    _moveToLast = new RelayCommand(
                        param => MoveToLastCommand(),
                        param => CanMoveToLastCommand()
                    );
                return _moveToLast;
            }
        }

        private bool CanMoveToLastCommand()
        {
            return true;
        }

        private void MoveToLastCommand()
        {
            try
            {
                if (SelectedDownloadIndex < 0 || DownloadList.Count <= 0)
                    return;

                var episode = DownloadList[SelectedDownloadIndex];
                DownloadList.RemoveAt(SelectedDownloadIndex);
                DownloadList.Add(episode);
                SelectedDownloadIndex = DownloadList.Count - 1;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"MoveToLastCommand Error!");
            }
        }

        public ICommand MoveUp
        {
            get
            {
                if (_moveUp == null)
                    _moveUp = new RelayCommand(
                        param => MoveUpCommand(),
                        param => CanMoveUpCommand()
                    );
                return _moveUp;
            }
        }

        private bool CanMoveUpCommand()
        {
            return true;
        }

        private void MoveUpCommand()
        {
            try
            {
                if (SelectedDownloadIndex < 0 || DownloadList.Count <= 0)
                    return;

                if (SelectedDownloadIndex - 1 >= 0)
                {
                    var episode = DownloadList[SelectedDownloadIndex];
                    if (SelectedDownloadIndex == DownloadList.Count - 1)
                    {
                        DownloadList.RemoveAt(SelectedDownloadIndex);
                        DownloadList.Insert(SelectedDownloadIndex, episode);
                        SelectedDownloadIndex = SelectedDownloadIndex - 1;
                    }
                    else
                    {
                        DownloadList.RemoveAt(SelectedDownloadIndex);
                        DownloadList.Insert(SelectedDownloadIndex - 1, episode);
                        SelectedDownloadIndex = SelectedDownloadIndex - 2;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"MoveUpCommand Error!");
            }
        }

        public ICommand MoveDown
        {
            get
            {
                if (_moveDown == null)
                    _moveDown = new RelayCommand(
                        param => MoveDownCommand(),
                        param => CanMoveDownCommand()
                    );
                return _moveDown;
            }
        }

        private bool CanMoveDownCommand()
        {
            return true;
        }

        private void MoveDownCommand()
        {
            try
            {
                if (SelectedDownloadIndex < 0 || DownloadList.Count <= 0)
                    return;

                if (SelectedDownloadIndex + 1 < DownloadList.Count)
                {
                    var episode = DownloadList[SelectedDownloadIndex];
                    DownloadList.RemoveAt(SelectedDownloadIndex);
                    DownloadList.Insert(SelectedDownloadIndex + 1, episode);
                    SelectedDownloadIndex = SelectedDownloadIndex + 1;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"MoveDownCommand Error!");
            }
        }

        public ICommand AddToList
        {
            get
            {
                if (_addToList == null)
                    _addToList = new RelayCommand(
                        param => AddToListCommand(),
                        param => CanAddToListCommand()
                    );
                return _addToList;
            }
        }

        private bool CanAddToListCommand()
        {
            return true;
        }

        private void AddToListCommand()
        {
            try
            {
                if (FeedListView.CurrentItem == null)
                    return;

                if (!DownloadList.Any(x => x.Name == ((Episode) FeedListView.CurrentItem).Name))
                {
                    var episode = (Episode) FeedListView.CurrentItem;
                    if (episode != null)
                        DownloadList.Add(episode);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"AddToListCommand Error!");
            }
        }

        public ICommand DeleteFromList
        {
            get
            {
                if (_deleteFromList == null)
                    _deleteFromList = new RelayCommand(
                        param => DeleteFromListCommand(),
                        param => CanDeleteFromListCommand()
                    );
                return _deleteFromList;
            }
        }

        private bool CanDeleteFromListCommand()
        {
            return true;
        }

        private void DeleteFromListCommand()
        {
            try
            {
                if (SelectedDownloadIndex >= 0)
                    DownloadList.RemoveAt(SelectedDownloadIndex);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DeleteFromListCommand Error!");
            }
        }

        public ICommand DownloadThis
        {
            get
            {
                if (_downloadThis == null)
                    _downloadThis = new RelayCommand(
                        param => DownloadThisCommand(),
                        param => CanDownloadThisCommand()
                    );
                return _downloadThis;
            }
        }

        private bool CanDownloadThisCommand()
        {
            return true;
        }

        private void DownloadThisCommand()
        {
            try
            {
                if (SelectedDownloadIndex < 0)
                    return;

                Mediator.NotifyColleagues(MediatorGlobal.StartDownloadEpisodes,
                    new List<object> {DownloadList[SelectedDownloadIndex]});
                DownloadList.Remove(DownloadList[SelectedDownloadIndex]);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DownloadThisCommand Error!");
            }
        }

        public ICommand DownloadAll
        {
            get
            {
                if (_downloadAll == null)
                    _downloadAll = new RelayCommand(
                        param => DownloadAllCommand(),
                        param => CanDownloadAllCommand()
                    );
                return _downloadAll;
            }
        }

        private bool CanDownloadAllCommand()
        {
            return true;
        }

        private void DownloadAllCommand()
        {
            try
            {
                Mediator.NotifyColleagues(MediatorGlobal.StartDownloadEpisodes,
                    new List<object>(DownloadList.ToList()));
                DownloadList.Clear();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DownloadAllCommand Error!");
            }
        }

        private void OnUpdateDownloadList(object obj)
        {
            try
            {
                List<object> autoEpisodes = obj as List<object>;
                DownloadList.Clear();
                if (autoEpisodes != null)
                    foreach (var autoEpisode in autoEpisodes)
                    {
                        DownloadList.Add((Episode) autoEpisode);
                    }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnUpdateDownloadList Error!");
            }
        }

        #endregion

        #region Episode List

        public ICommand EpisodeTextBoxButtonCmd
        {
            get
            {
                if (_episodeTextBoxButtonCmd == null)
                    _episodeTextBoxButtonCmd = new RelayCommand<object>(
                        param => EpisodeTextBoxButtonCommand(param),
                        param => CanEpisodeTextBoxButtonCommand()
                    );
                return _episodeTextBoxButtonCmd;
            }
        }

        private bool CanEpisodeTextBoxButtonCommand()
        {
            return true;
        }

        private void EpisodeTextBoxButtonCommand(object param)
        {
            try
            {
                if (param == null)
                    return;

                var value = param as string;
                var localSeries = (TvShow) LocalListView.CurrentItem;
                var episodes = new List<Episode>(localSeries.Episodes);

                if (value != null && value.Equals(""))
                {
                    EpisodeList = new ObservableCollection<Episode>(episodes);
                    return;
                }

                if (param is string)
                {
                    var episodeFiltered = episodes.Where(item => item.EpisodeNumber.ToString() == value).ToList();
                    EpisodeList = new ObservableCollection<Episode>(episodeFiltered);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"EpisodeTextBoxButtonCommand Error!");
            }
        }

        public ICommand PlayEpisode
        {
            get
            {
                if (_playEpisode == null)
                    _playEpisode = new RelayCommand<object>(
                        param => PlayEpisodeCommand(),
                        param => CanPlayEpisodenCommand()
                    );
                return _playEpisode;
            }
        }

        private bool CanPlayEpisodenCommand()
        {
            return true;
        }

        private void PlayEpisodeCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.PlayEpisode, SelectedEpisode);
        }

        public ICommand OpenFolder
        {
            get
            {
                if (_openFolder == null)
                    _openFolder = new RelayCommand<object>(
                        param => OpenFolderCommand(),
                        param => CanOpenFolderCommand()
                    );
                return _openFolder;
            }
        }

        private bool CanOpenFolderCommand()
        {
            return true;
        }

        private void OpenFolderCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.OpenFolder, SelectedEpisode);
        }

        public ICommand EditEpisodeInfo
        {
            get
            {
                if (_editEpisodeInfo == null)
                    _editEpisodeInfo = new RelayCommand(
                        param => EditEpisodeInfoCommand(),
                        param => CanEditEpisodeInfoCommand()
                    );
                return _editEpisodeInfo;
            }
        }

        private bool CanEditEpisodeInfoCommand()
        {
            return true;
        }

        private void EditEpisodeInfoCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.UpdateEpisodeFlyoutValues, SelectedEpisode);
        }

        public ICommand DeleteEpisode
        {
            get
            {
                if (_deleteEpisode == null)
                    _deleteEpisode = new RelayCommand<object>(
                        param => DeleteEpisodeCommand(),
                        param => CanDeleteEpisodeCommand()
                    );
                return _deleteEpisode;
            }
        }

        private bool CanDeleteEpisodeCommand()
        {
            return true;
        }

        private void DeleteEpisodeCommand()
        {
            Mediator.NotifyColleagues(MediatorGlobal.CustomDialog, new List<string>()
            {
                "DeleteEpisode",
                "Delete Episode?",
                "Warning! This will delete the local file and the database entry! \n" +
                "Are you sure you want to delete this Episode?"
            });
        }

        private void OnDeleteEpisodeAnswer(bool answer)
        {
            if (answer)
                Mediator.NotifyColleagues(MediatorGlobal.DeleteEpisode, SelectedEpisode);
        }

        [DoNotNotify]
        public int EpisodeSortModesIndex
        {
            get => _episodeSortModesIndex;
            set
            {
                _episodeSortModesIndex = value;
                SortEpisodeList(value);
                OnCustomPropertyChanged();
            }
        }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => _imageSource = value;
        }

        private void SortEpisodeList(int value)
        {
            try
            {
                var localSeries = (TvShow) LocalListView.CurrentItem;
                var episodes = new List<Episode>(localSeries.Episodes);
                switch (value)
                {
                    case 0:
                        EpisodeList = new ObservableCollection<Episode>(localSeries.Episodes);
                        break;
                    case 1:
                        episodes.Sort((ep1, ep2) => ep1.EpisodeNumber.CompareTo(ep2.EpisodeNumber));
                        EpisodeList = new ObservableCollection<Episode>(episodes);
                        break;
                    case 2:
                        episodes.Sort((ep1, ep2) => ep2.EpisodeNumber.CompareTo(ep1.EpisodeNumber));
                        EpisodeList = new ObservableCollection<Episode>(episodes);
                        break;
                    case 3:
                        episodes.Sort((ep1, ep2) => ep1.Rating.CompareTo(ep2.Rating));
                        EpisodeList = new ObservableCollection<Episode>(episodes);
                        break;
                    case 4:
                        episodes.Sort((ep1, ep2) => ep2.Rating.CompareTo(ep1.Rating));
                        EpisodeList = new ObservableCollection<Episode>(episodes);
                        break;
                    case 5:
                        episodes.Sort((ep1, ep2) => ep1.DownloadDate.CompareTo(ep2.DownloadDate));
                        EpisodeList = new ObservableCollection<Episode>(episodes);
                        break;
                    case 6:
                        episodes.Sort((ep1, ep2) => ep2.DownloadDate.CompareTo(ep1.DownloadDate));
                        EpisodeList = new ObservableCollection<Episode>(episodes);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"SortEpisodeList Error!");
            }
        }

        #endregion
    }
}