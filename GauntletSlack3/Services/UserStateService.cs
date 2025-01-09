using GauntletSlack3.Services.Interfaces;

namespace GauntletSlack3.Services
{
    public class UserStateService : IUserStateService
    {
        private readonly IUserService _userService;
        private int? _currentUserId;

        public event Action OnUserStateChanged;

        public int? CurrentUserId 
        { 
            get => _currentUserId;
            set
            {
                _currentUserId = value;
                OnUserStateChanged?.Invoke();
            }
        }

        public UserStateService(IUserService userService)
        {
            _userService = userService;
        }

        public string? CurrentUserName { get; set; }

        public async Task InitializeAsync(string email, string name)
        {
            CurrentUserId = await _userService.GetOrCreateUserAsync(email, name);
            CurrentUserName = name;
        }

        public void Clear()
        {
            CurrentUserId = null;
            CurrentUserName = null;
        }
    }
} 