using Microsoft.UI.Xaml.Data;
using SteamHub.ViewModels;
using System.Collections.ObjectModel;
using System;
using SteamHub.ApiContract.Models;

namespace SteamHub.Pages.Converters;
public class PostIdToCommentsConverter : IValueConverter
{
	public NewsPageViewModel ViewModel { get; set; }

	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is int postId && ViewModel != null)
		{
			return ViewModel.GetCommentsForPost(postId);
		}
		return new ObservableCollection<Comment>();
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotImplementedException();
}
