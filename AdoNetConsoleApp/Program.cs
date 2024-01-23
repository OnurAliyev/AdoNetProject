using AdoNet.Business.Helpers;
using AdoNet.Business.Services;
using AdoNet.Core.Entities;

string appStart = "Application started...";
string Welcome = "Welcome!";
Console.SetCursorPosition((Console.WindowWidth - appStart.Length) / 2, Console.CursorTop);
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine(appStart);
Console.SetCursorPosition((Console.WindowWidth - Welcome.Length) / 2, Console.CursorTop);
Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine(Welcome);
Console.ResetColor();
PostServices postServices = new();
bool runApp = true;
while (runApp)
{
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine("1 == Add object to database\n" +
                      "2 == Show objects not exist in database\n" +
                      "3 == Show user's posts count\n" +
                      "4 == Show all objects in database\n" +
                      "0 == Close App\n" +
                      " ");
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("Chosse the option >> ");
    Console.ResetColor();

    string? option = Console.ReadLine();
    int IntOption;
    bool IsInt = int.TryParse(option, out IntOption);
    if (IsInt)
    {
        if (IntOption >= 0 && IntOption <= 4)
        {
            switch (IntOption)
            {
                case (int)Menu.Add:
                    try
                    {
                        Console.Write("Enter post's ID: ");
                        int postId = Convert.ToInt32(Console.ReadLine());
                        Post? post = await postServices.GetByIdAsync(postId);
                        if (post is not null)
                        {
                            await postServices.AddPostToDbAsync(post);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Post with this ID {postId} is not found");
                            Console.ResetColor();
                            goto case (int)Menu.Add;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                        goto case (int)Menu.Add;
                    }
                    break;
                case (int)Menu.GetMiss:
                    try
                    {
                        await postServices.NotExistPostsDbAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                        goto case (int)Menu.GetMiss;
                    }
                    break;
                case (int)Menu.ShowCount:
                    try
                    {
                        Console.Write("Enter user id: ");
                        int userId;
                        if (!int.TryParse(Console.ReadLine(),out userId )|| (userId < 0))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nWrong format id! Please try again...\n");
                            Console.ResetColor();
                        }
                        else
                        {
                            int result = await postServices.GetUserPostCountsAsync(userId);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"User ID : {userId}\n" +
                                              $"User's post count : {result}\n" +
                                              " ");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                        goto case (int)Menu.ShowCount;
                    }
                    break;
                case (int)Menu.GetAll:
                    try
                    {
                        await postServices.ShowAllPostsInDbAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                        goto case (int)Menu.GetAll;
                    }
                    break;
                case 0:
                    runApp = false;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Application closed!\n" +
                                      $"Press any key to close window...");
                    Console.ResetColor();
                    break;

            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nPlease enter correct number!\n");
            Console.ResetColor();
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nPlease enter correct format!\n");
        Console.ResetColor();
    }
}