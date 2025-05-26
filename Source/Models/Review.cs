using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BusinessLayer.Models
{
    public class Review : INotifyPropertyChanged
    {
        private int _reviewIdentifier;
        private string _reviewTitleText = string.Empty;
        private string _reviewContentText = string.Empty;
        private bool _isRecommended;
        private double _numericRatingGivenByUser;
        private int _totalHelpfulVotesReceived;
        private int _totalFunnyVotesReceived;
        private int _totalHoursPlayedByReviewer;
        private DateTime _dateAndTimeWhenReviewWasCreated;
        private int _userIdentifier;
        private int _gameIdentifier;
        private string _userName = string.Empty;
        private string _titleOfGame = string.Empty;


        public byte[]? ProfilePictureBlob { get; set; }



        // REMOVE!! NO GUI IN MODEL!!!!
        public BitmapImage? ProfileImage
        {
            get
            {
                try
                {
                    if (ProfilePictureBlob == null || ProfilePictureBlob.Length == 0)
                        return null;

                    var bitmap = new BitmapImage();
                    using var stream = new MemoryStream(ProfilePictureBlob);
                    var ras = stream.AsRandomAccessStream();
                    bitmap.SetSource(ras);
                    return bitmap;
                }
                catch
                {
                    return new BitmapImage(new Uri("ms-appx:///Assets/default_avatar.png"));
                }
            }
        }



        public bool HasVotedHelpful { get; set; } = false; // temporary flag
        public bool HasVotedFunny { get; set; } = false;

        public int ReviewIdentifier
        {
            get => _reviewIdentifier;
            set => SetProperty(ref _reviewIdentifier, value);
        }

        public string ReviewTitleText
        {
            get => _reviewTitleText;
            set => SetProperty(ref _reviewTitleText, value);
        }

        public string ReviewContentText
        {
            get => _reviewContentText;
            set => SetProperty(ref _reviewContentText, value);
        }

        public bool IsRecommended
        {
            get => _isRecommended;
            set => SetProperty(ref _isRecommended, value);
        }

        public double NumericRatingGivenByUser
        {
            get => _numericRatingGivenByUser;
            set => SetProperty(ref _numericRatingGivenByUser, value);
        }

        public int TotalHelpfulVotesReceived
        {
            get => _totalHelpfulVotesReceived;
            set => SetProperty(ref _totalHelpfulVotesReceived, value);
        }

        public int TotalFunnyVotesReceived
        {
            get => _totalFunnyVotesReceived;
            set => SetProperty(ref _totalFunnyVotesReceived, value);
        }

        public int TotalHoursPlayedByReviewer
        {
            get => _totalHoursPlayedByReviewer;
            set => SetProperty(ref _totalHoursPlayedByReviewer, value);
        }

        public DateTime DateAndTimeWhenReviewWasCreated
        {
            get => _dateAndTimeWhenReviewWasCreated;
            set => SetProperty(ref _dateAndTimeWhenReviewWasCreated, value);
        }

        public int UserIdentifier
        {
            get => _userIdentifier;
            set => SetProperty(ref _userIdentifier, value);
        }

        public int GameIdentifier
        {
            get => _gameIdentifier;
            set => SetProperty(ref _gameIdentifier, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string TitleOfGame
        {
            get => _titleOfGame;
            set => SetProperty(ref _titleOfGame, value);
        }

        public void AddVote(string typeOfVote)
        {
            if (typeOfVote == "Helpful") TotalHelpfulVotesReceived++;
            else if (typeOfVote == "Funny") TotalFunnyVotesReceived++;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}


