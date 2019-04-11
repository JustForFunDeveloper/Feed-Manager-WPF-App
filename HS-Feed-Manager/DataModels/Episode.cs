using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HS_Feed_Manager.DataModels
{
    public class Episode : INotifyPropertyChanged
    {
        private string _name;
        private double _episodeNumber;
        private string _link;
        private DateTime _downloadDate;

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

        public double EpisodeNumber
        {
            get { return _episodeNumber; }
            set
            {
                if (value == _episodeNumber) return;
                _episodeNumber = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get { return _link; }
            set
            {
                if (value == _link) return;
                _link = value;
                OnPropertyChanged();
            }
        }

        public DateTime DownloadDate
        {
            get { return _downloadDate; }
            set
            {
                if (value == _downloadDate) return;
                _downloadDate = value;
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
