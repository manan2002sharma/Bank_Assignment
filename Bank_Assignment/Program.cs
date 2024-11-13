using System;
using System.Collections;
using System.Text.RegularExpressions;

public class User
{
    public string username { get; set; }
    public string password { get; set; }
    public string fatherName { get; set; }
    public string mobile { get; set; }
    public string Aadhar { get; set; }
    public ArrayList accounts { get; set; } = new ArrayList();
}

public class Account
{
    public string account_number { get; set; }
    public string holder_name { get; set; }
    public string account_type { get; set; }
    public decimal balance { get; set; }
    public ArrayList transactions { get; set; } =new ArrayList();
    public DateTime last_interest_date { get; set; } =DateTime.Now;
}

public class Transaction
{
    public string transaction_id { get; set; }
    public DateTime date { get; set; }
    public string type { get; set; } 
    public decimal amount { get; set; }
}

public class banking_app
{
    private ArrayList users = new ArrayList();
    private User logged_in_user;

    public void register()
    {
        string username= string.Empty;
        string password=string.Empty;
        string fatherName = string.Empty;
        string AdharNo = string.Empty;
        string mobile =string.Empty;
        //here i implementd regex so that we check if user has entered strong password
        string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        string patternMobile = @"^\d{10}$";
        string patternAdhar = @"^\d{12}$";
        Regex regex = new Regex(pattern);
        Regex mob = new Regex(patternMobile);
        Regex pAdhar = new Regex(patternAdhar);

        try
        {
            Console.Write("Enter Username: ");
            username=Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("Username cannot be empty or whitespace.");
            }
            Console.Write("Enter Password (Atleast one uppercase , one lowercase , one digit, one special char, and minimum length 8): ");
            password= Console.ReadLine();
            if (!regex.IsMatch(password))
            {
                throw new Exception("Make sure it has :-(Atleast one uppercase , one lowercase , one digit, one special char, and minimum length 8)");
            }
            Console.Write("Enter Your father's name : ");
            fatherName = Console.ReadLine();
            Console.Write("write valid aadhar number :");
            AdharNo = Console.ReadLine();
            if (!pAdhar.IsMatch(AdharNo))
            {
                throw new Exception("Aadhar number should be valid (valid aadhar has 12 digits)");
            }
            Console.Write("enter valid mobile number :");
            mobile = Console.ReadLine();
            if (!mob.IsMatch(mobile))
            {
                throw new Exception("Enter valid 10 digit mobile number");
            }


            users.Add(new User { username = username, password = password, mobile=mobile, fatherName=fatherName, Aadhar = AdharNo });
            Console.WriteLine("Registration Successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }

    public bool login()
    {
        string username = string.Empty;

        string password = string.Empty;
        try
        {
            Console.Write("Enter Username: ");
            username = Console.ReadLine();
            if(string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("Username cannot be empty or whitespace.");
            }
            Console.Write("Enter Password: ");
            password= Console.ReadLine();

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Password cannot be empty or whitespace.");
            }
            logged_in_user =null;
            foreach (User u in users)
            {
                if (u.username==username && u.password == password)
                {
                    logged_in_user=u;
                    break;
                }
            }

            if (logged_in_user!=null)
            {
                Console.WriteLine("Login Successful!");
                return true;
            }
            else
            {
                Console.WriteLine("Invalid credentials.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return false;
        }
    }

    public void open_account()
    {
        if (logged_in_user ==null)
        {
            Console.WriteLine("Please log in first.");
            return;
        }

        try
        {
            Console.Write("Enter Account Holder's Name: ");
            string holder_name=Console.ReadLine();
            if (string.IsNullOrWhiteSpace(holder_name))
            {
                throw new Exception("Account holder's name cannot be empty or whitespace.");
            }

            Console.Write("Enter Account Type (Savings/Current): ");
            string account_type = Console.ReadLine().Trim();
            if (account_type != "Savings" && account_type != "Current")
            {
                throw new Exception("Invalid account type. Please enter either 'Savings' or 'Checking'.");
            }
            Console.Write("Enter Initial Deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal initial_deposit) || initial_deposit < 0)
            {
                throw new ArgumentException("Invalid deposit amount. Please enter a valid non-negative decimal value.");
            }
            string account_number = "ACC" + (logged_in_user.accounts.Count + 1).ToString();
            Account new_account = new Account
            {
                account_number = account_number,
                holder_name = holder_name,
                account_type = account_type,
                balance = initial_deposit
            };
            logged_in_user.accounts.Add(new_account);
            Console.WriteLine($"Account created successfully with Account Number: {account_number}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    public void deposit(string account_number,decimal amount)
    {
        var account =FindAccount(account_number);
        if (account!=null)
        {
            account.balance +=amount;
            log_transaction(account,"Deposit",amount);
            Console.WriteLine("Deposit Successful.");
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    public void withdraw(string account_number,decimal amount)
    {
        var account = FindAccount(account_number);
        if (account !=null)
        {
            if (account.balance>= amount)
            {
                account.balance-=amount;
                log_transaction(account, "Withdrawal", amount);
                Console.WriteLine("Withdrawal Successful.");
            }
            else
            {
                Console.WriteLine("Insufficient Balance.");
            }
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    private Account FindAccount(string account_number)
    {
        foreach (Account acc in logged_in_user.accounts)
        {
            if (acc.account_number==account_number)
            {
                return acc;
            }
        }
        return null;
    }

    private void log_transaction(Account account,string type , decimal amount)
    {
        Random random =new Random();
        int randomNumber=random.Next(1, 100566);
        string randomString =randomNumber.ToString();
        Transaction transaction= new Transaction
        {
            transaction_id = randomString,
            date =DateTime.Now,
            type=type,
            amount=amount
        };
        account.transactions.Add(transaction);
    }

    public void generate_statement(string account_number)
    {
        var account= FindAccount(account_number);
        if (account != null)
        {
            Console.WriteLine($"Transaction History for Account: {account_number}");
            foreach (Transaction transaction in account.transactions)
            {
                Console.WriteLine($"{transaction.date} | {transaction.type} | {transaction.amount}");
            }
        }
        else
        {
            Console.WriteLine("Account not found.");
        }
    }

    public void add_monthly_interest()
    {
        foreach (Account account in logged_in_user.accounts)
        {
            if (account.account_type.ToLower()=="savings")
            {
                if ((DateTime.Now - account.last_interest_date).Days >=30)
                {
                    decimal interest_rate = 0.03m;
                    decimal interest= account.balance * interest_rate;
                    account.balance += interest;
                    account.last_interest_date = DateTime.Now;
                    Console.WriteLine($"Interest of {interest} added to Account {account.account_number}.");
                }
            }
        }
    }

    public void check_balance(string account_number)
    {
        var account = FindAccount(account_number);
        if (account!=null)
        {
            Console.WriteLine($"{account_number}: {account.balance}");
        }
        else
        {
            Console.WriteLine("No account exist");
        }
    }


    //This is the main menu which will be always displaying until user enters 0 . which is to exit the code

    public void show_menu()
    {
        int option=-1;
        do
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Open Account");
            Console.WriteLine("4. Deposit");
            Console.WriteLine("5. Withdraw");
            Console.WriteLine("6. Generate Statement");
            Console.WriteLine("7. Add Interest");
            Console.WriteLine("8. Check Balance");
            Console.WriteLine("9. Logout");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");
            try
            {
                option =int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine("Please enter a valid integer.");
                continue;
            }
            switch (option)
            {
                case 1:
                    register();
                    break;
                case 2:
                    if (login())
                    {
                        Console.WriteLine("Welcome To our bank, " + logged_in_user.username);
                    }
                    break;
                case 3:
                    if (logged_in_user != null) open_account();
                    else Console.WriteLine("Please login first.");
                    break;
                case 4:
                    if (logged_in_user!= null)
                    {
                        Console.Write("Enter Account Number : ");
                        string account_number = Console.ReadLine();
                        Console.Write("Enter Deposit Amount : ");
                        if (decimal.TryParse(Console.ReadLine(),out decimal deposit_amount))
                        {
                            deposit(account_number,deposit_amount);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount.Please enter a valid number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;
                case 5:
                    if (logged_in_user != null)
                    {
                        Console.Write("Enter Account Number: ");
                        string account_number =Console.ReadLine();
                        Console.Write("Enter Withdrawal Amount: ");
                        if (decimal.TryParse(Console.ReadLine(),out decimal withdrawal_amount))
                        {
                            withdraw(account_number,withdrawal_amount);
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount. Please enter a valid number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;
                case 6:
                    if (logged_in_user!=null)
                    {
                        Console.Write("Enter Account Number : ");
                        string account_number = Console.ReadLine();
                        generate_statement(account_number);
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;
                case 7:
                    if (logged_in_user != null)
                    {
                        add_monthly_interest();
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;
                case 8:
                    if (logged_in_user != null)
                    {
                        Console.Write("Enter Account Number: ");
                        string account_number = Console.ReadLine();
                        check_balance(account_number);
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;
                case 9:
                    logged_in_user = null;
                    Console.WriteLine("Logged out successfully.");
                    break;
                case 0:
                    Console.WriteLine("Exiting application...");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        } while (option != 0);
    }
}

public class program
{
    public static void Main()
    {
        banking_app app=new banking_app();
        app.show_menu();
    }
}
