namespace GameOfTournaments.Web.Factories
{
    using Ardalis.GuardClauses;
    using GameOfTournaments.Data.Models;
    using GameOfTournaments.Web.Models;

    public static class ApplicationUserFactory
    {
        public static ApplicationUser Create(RegisterUserModel registerUserModel)
        {
            Guard.Against.Null(registerUserModel, nameof(registerUserModel));

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