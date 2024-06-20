using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace MiSPIC.Xunit
{
    public enum LoginOption
    {
        Login,
        WrongPasswordLogin,
        Register
    }
    public enum UserStatus
    {
        Admin,
        Sport,
        VIP,
        PokerClubMember,
        Ludik
    }

    public class ForTestsClass
    {
        public const string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\FrogmoneyCasinoDatabase.mdf;Integrated Security=True";
        
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public LoginOption ReturnUserRegistrationStatus(string username, string password, SqlConnection connection)
        {
            using (connection)
            {
                connection.Open();

                string isUserRegistered = "SELECT COUNT( * ) FROM UserList WHERE Name = @Name AND Password = @Password;";
                using (SqlCommand command = new SqlCommand(isUserRegistered, connection))
                {
                    command.Parameters.AddWithValue("@Name", username);
                    command.Parameters.AddWithValue("@Password", HashPassword(password));
                    int count = (int)command.ExecuteScalar();

                    if (count == 1)
                    {
                        return LoginOption.Login;
                    }
                }

                string isPasswordWrong = "SELECT COUNT( * ) FROM UserList WHERE Name = @Name;";
                using (SqlCommand command = new SqlCommand(isPasswordWrong, connection))
                {
                    command.Parameters.AddWithValue("@Name", username);
                    int count = (int)command.ExecuteScalar();

                    if (count == 1)
                    {
                        return LoginOption.WrongPasswordLogin;
                    }
                }
                return LoginOption.Register;
            }
        }

    }


    public class ReturnUserRegistrationStatus
    {
        [Fact]
        public void ReturnUserRegistrationStatus_ExistingUserCorrectPassword_ReturnsLoginOptionLogin()
        {
            // Arrange
            string usernameText = "existingUser";
            string passwordText = "correctPassword";

            var mockConnection = new Mock<SqlConnection>();
            var mockCommand = new Mock<SqlCommand>();

            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
            mockCommand.Setup(cmd => cmd.ExecuteScalar()).Returns(1);

            var forTestsClass = new ForTestsClass();

            // Act
            var result = forTestsClass.ReturnUserRegistrationStatus(usernameText, passwordText, mockConnection.Object);

            // Assert
            Assert.Equal(LoginOption.Login, result);
        }

        [Fact]
        public void ReturnUserRegistrationStatus_ExistingUserIncorrectPassword_ReturnsLoginOptionWrongPasswordLogin()
        {
            // Arrange
            string usernameText = "existingUser";
            string passwordText = "incorrectPassword";

            var mockConnection = new Mock<SqlConnection>();
            var mockCommand = new Mock<SqlCommand>();

            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
            mockCommand.Setup(cmd => cmd.ExecuteScalar()).Returns(1);

            var forTestsClass = new ForTestsClass();

            // Act
            var result = forTestsClass.ReturnUserRegistrationStatus(usernameText, passwordText, mockConnection.Object);

            // Assert
            Assert.Equal(LoginOption.Login, result);
        }

        [Fact]
        public void ReturnUserRegistrationStatus_NewUser_ReturnsLoginOptionRegister()
        {
            // Arrange
            string usernameText = "newUser";
            string passwordText = "newPassword";

            var mockConnection = new Mock<SqlConnection>();
            var mockCommand = new Mock<SqlCommand>();

            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
            mockCommand.Setup(cmd => cmd.ExecuteScalar()).Returns(1);

            var forTestsClass = new ForTestsClass();

            // Act
            var result = forTestsClass.ReturnUserRegistrationStatus(usernameText, passwordText, mockConnection.Object);

            // Assert
            Assert.Equal(LoginOption.Login, result);
        }
    }
}