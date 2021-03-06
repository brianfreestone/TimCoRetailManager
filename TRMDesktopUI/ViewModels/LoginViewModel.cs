using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Helpers;
using TRMDesktopUI.Library.Api;
using IEventAggregator = Caliburn.Micro.IEventAggregator;

namespace TRMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName = "test@gmail.com";
        private string _password = "Pwd12345.";
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool IsErrorVisible
        {
            get 
            {
                bool returnValue = false;
                if (ErrorMessage?.Length > 0)
                {
                    returnValue = true;
                }
                return returnValue; 
            }

            //set { }
      
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool CanLogIn
        {
            get
            {
                bool returnValue = false;

                if (UserName?.Length > 0 && Password?.Length > 0)
                {
                    returnValue = true;
                }

                return returnValue;
            }
        }

        public async Task LogIn()
        {

            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(UserName, Password);

                // capture more info about the user

                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                await _events.PublishOnUIThreadAsync(new LogOnEvent(), new CancellationToken());

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

        }
    }
}
