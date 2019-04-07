using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HS_Feed_Manager.DataModels
{
    public enum Status
    {
        New,
        Ongoing,
        Finished
    }

    public enum AutoDownload
    {
        On,
        Off
    }

    public class LocalSeries
    {
        private string _name;
        private List<Episode> _episodeList;
        private Status _status;
        private int _episodes;
        private AutoDownload _autoDownloadStatus;
        private int _localEpisodesCount;
        private int _rating;
        private DateTime _latestDownload; 

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public List<Episode> EpisodeList
        {
            get { return _episodeList; }
            set
            {
                if (value == _episodeList) return;
                _episodeList = value;
                OnPropertyChanged();
            }
        }

        public Status Status
        {
            get { return _status; }
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }

        public int Episodes
        {
            get { return _episodes; }
            set
            {
                if (value == _episodes) return;
                _episodes = value;
                OnPropertyChanged();
            }
        }

        public AutoDownload AutoDownloadStatus
        {
            get { return _autoDownloadStatus; }
            set
            {
                if (value == _autoDownloadStatus) return;
                _autoDownloadStatus = value;
                OnPropertyChanged();
            }
        }

        public int LocalEpisodesCount
        {
            get { return _localEpisodesCount; }
            set
            {
                if (value == _localEpisodesCount) return;
                _localEpisodesCount = value;
                OnPropertyChanged();
            }
        }

        public int Rating
        {
            get { return _rating; }
            set
            {
                if (value == _rating) return;
                _rating = value;
                OnPropertyChanged();
            }
        }

        public DateTime LatestDownload
        {
            get { return _latestDownload; }
            set
            {
                if (value == _latestDownload) return;
                _latestDownload = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
