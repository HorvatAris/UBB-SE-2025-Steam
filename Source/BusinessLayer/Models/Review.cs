using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace BusinessLayer.Models
{
    public class Review : INotifyPropertyChanged
    {
        private int reviewIdentifier;
        private string reviewTitleText = string.Empty;
        private string reviewContentText = string.Empty;
        private bool isRecommended;
        private double numericRatingGivenByUser;
        private int totalHelpfulVotesReceived;
        private int totalFunnyVotesReceived;
        private int totalHoursPlayedByReviewer;
        private DateTime dateAndTimeWhenReviewWasCreated;
        private int userIdentifier;
        private int gameIdentifier;
        private string userName = string.Empty;
        private string titleOfGame = string.Empty;
        public byte[]? ProfilePictureBlob { get; set; } // store image data as byte array

        public bool HasVotedHelpful { get; set; } = false; // temporary flag
        public bool HasVotedFunny { get; set; } = false;

        public int ReviewIdentifier
        {
            get => reviewIdentifier;
            set => SetProperty(ref reviewIdentifier, value);
        }

        public string ReviewTitleText
        {
            get => reviewTitleText;
            set => SetProperty(ref reviewTitleText, value);
        }

        public string ReviewContentText
        {
            get => reviewContentText;
            set => SetProperty(ref reviewContentText, value);
        }

        public bool IsRecommended
        {
            get => isRecommended;
            set => SetProperty(ref isRecommended, value);
        }

        public double NumericRatingGivenByUser
        {
            get => numericRatingGivenByUser;
            set => SetProperty(ref numericRatingGivenByUser, value);
        }

        public int TotalHelpfulVotesReceived
        {
            get => totalHelpfulVotesReceived;
            set => SetProperty(ref totalHelpfulVotesReceived, value);
        }

        public int TotalFunnyVotesReceived
        {
            get => totalFunnyVotesReceived;
            set => SetProperty(ref totalFunnyVotesReceived, value);
        }

        public int TotalHoursPlayedByReviewer
        {
            get => totalHoursPlayedByReviewer;
            set => SetProperty(ref totalHoursPlayedByReviewer, value);
        }

        public DateTime DateAndTimeWhenReviewWasCreated
        {
            get => dateAndTimeWhenReviewWasCreated;
            set => SetProperty(ref dateAndTimeWhenReviewWasCreated, value);
        }

        public int UserIdentifier
        {
            get => userIdentifier;
            set => SetProperty(ref userIdentifier, value);
        }

        public int GameIdentifier
        {
            get => gameIdentifier;
            set => SetProperty(ref gameIdentifier, value);
        }

        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        public string TitleOfGame
        {
            get => titleOfGame;
            set => SetProperty(ref titleOfGame, value);
        }

        public void AddVote(string typeOfVote)
        {
            if (typeOfVote == "Helpful")
            {
                TotalHelpfulVotesReceived++;
            }
            else if (typeOfVote == "Funny")
            {
                TotalFunnyVotesReceived++;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
