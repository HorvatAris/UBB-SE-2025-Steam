using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public class AuthenticationService
    {
        private readonly IUsersRepository usersRepository;
        public AuthenticationService(IUsersRepository newUsersRepository)
        {
            this.usersRepository = newUsersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
        }
    }
}
