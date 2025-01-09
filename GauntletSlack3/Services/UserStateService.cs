using GauntletSlack3.Services.Interfaces;

namespace GauntletSlack3.Services
{
    public class UserStateService : IUserStateService
    {
        private int? _currentUserId;
        private readonly IUserService _userService;

        public UserStateService(IUserService userService)
        {
            _userService = userService;
        }

        public int? CurrentUserId
        {
            get => _currentUserId;
            set
            {
                _currentUserId = value;
                OnUserStateChanged?.Invoke();
            }
        }
        public string? CurrentUserName { get; set; }
        public event Action OnUserStateChanged;

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