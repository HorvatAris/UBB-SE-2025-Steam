using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;

namespace SteamHub.Pages
{
    /// <summary>
    /// Represents the user registration page of the application.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        /// <summary>
        /// Gets or sets the user service to handle user-related operations.
        /// </summary>
        private IUserService UserService { get; set; }

        /// <summary>
        /// Gets or sets the navigation frame used for page navigation.
        /// </summary>
        private readonly Frame navigationFrame;

        /// <summary>
        /// Gets the ViewModel used for data binding on this page.
        /// </summary>
        public RegisterViewModel ViewModel { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterPage"/> class.
        /// </summary>
        /// <param name="navigationFrame">The frame used for navigation between login/register pages.</param>
        /// <param name="userService">The user service injected via dependency injection.</param>
        public RegisterPage(Frame navigationFrame, IUserService userService)
        {
            this.navigationFrame = navigationFrame;
            UserService = userService;
            this.InitializeComponent();

            // Subscribe to the Loaded event to initialize the ViewModel after the UI is ready
            this.Loaded += RegisterPage_Loaded;
        }

        /// <summary>
        /// Handles the Loaded event of the page.
        /// Instantiates the RegisterViewModel and sets it as the DataContext for data binding.
        /// </summary>
        private void RegisterPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new RegisterViewModel(navigationFrame, UserService);
            this.DataContext = ViewModel;
        }
    }
}