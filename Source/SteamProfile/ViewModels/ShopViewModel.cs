using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.Views;

namespace SteamProfile.ViewModels
{
    public partial class ShopViewModel : ObservableObject
    {
        private readonly Frame shopViewModelFrame;

        public ShopViewModel(Frame frame)
        {
            shopViewModelFrame = frame;
        }

        [RelayCommand]
        private void NavigateBack()
        {
            shopViewModelFrame.GoBack();
        }
    }
}