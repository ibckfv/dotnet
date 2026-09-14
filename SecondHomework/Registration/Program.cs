using Registration;

var userService = new UserService();

userService.CreateUser("Саша", "blabla@gmail.com", "12345");
userService.CreateUser("Даша", "abab@mail.ru", "qwerty");

User? user = userService.Authorize("blabla@gmail.com", "12345");
Console.WriteLine($"Вход выполнен: {user.Name}");

userService.UpdateUser("blabla@gmail.com", "Александр", null, "123456");

userService.DeleteUser("abab@mail.ru");
