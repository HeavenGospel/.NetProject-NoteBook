﻿
namespace PrismProject.Api.Context.Dtos
{
    public class UserDto : BaseDto
    {
		private string userName;

		public string UserName
		{
			get { return userName; }
			set { userName = value; OnPropertyChanged(); }
		}

		private string account;

		public string Account
		{
			get { return account; }
			set { account = value; OnPropertyChanged(); }
        }

		private string password;

		public string Password
		{
			get { return password; }
			set { password = value; OnPropertyChanged(); }
        }
	}
}
