namespace GameOfTournaments.Data.Factories
{
    using GameOfTournaments.Data.Factories.Models;
    using GameOfTournaments.Data.Models;

    public static class ApplicationUserFactory
    {
        public static ApplicationUser Create(RegisterUserModel registerUserModel)
        {
            if (registerUserModel == null)
                return default;
            
            return new ApplicationUser
            {
                UserName = registerUserModel.Username,
                Email = registerUserModel.Email,
                FirstName = registerUserModel.FirstName,
                MiddleName = registerUserModel.MiddleName,
                LastName = registerUserModel.LastName,
                Age = registerUserModel.Age,
                Male = registerUserModel.Male,
                PhoneNumber = registerUserModel.PhoneNumber,
            };
        }
    }
}