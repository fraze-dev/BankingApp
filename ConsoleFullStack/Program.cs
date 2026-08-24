using System;
using System.Collections.Generic;

namespace BankingApp;

class Program
{
    static void Main()
    {
        Console.Clear();
        WelcomeMessage();

        Bank bank = new Bank();
        bank.SeedData();

        User loggedInUser = Login(bank);

        if (loggedInUser == null)
        {
            ConcludingMessage();
            return;
        }

        switch (loggedInUser)
        {
            case Admin admin:
                AdminDashboard(admin, bank);
                break;
            case Customer customer:
                CustomerDashboard(customer, bank);
                break;
        }

        ConcludingMessage();
    }

    static void WelcomeMessage()
    {
        Console.WriteLine("Welcome to Banking Application");
        Console.WriteLine("===============================\n");
    }

    static User Login(Bank bank)
    {
        const int maxAttempts = 3;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            User user = bank.Authenticate(username, password);
            if (user != null)
            {
                Console.WriteLine($"\nLogin successful. Welcome, {user.Username}!\n");
                return user;
            }

            attempts++;
            int remaining = maxAttempts - attempts;
            string attemptWord = remaining == 1 ? "attempt" : "attempts";
            Console.WriteLine($"Invalid credentials. {remaining} {attemptWord} remaining.\n");
        }

        Console.WriteLine("Too many failed login attempts.\n");
        return null;
    }

    // ---------------- Admin ----------------

    static void AdminDashboard(Admin admin, Bank bank)
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine($"--- Admin Dashboard ({admin.Username}) ---");
            Console.WriteLine("1. View All Customers");
            Console.WriteLine("2. View All Accounts");
            Console.WriteLine("3. Add New Customer");
            Console.WriteLine("4. Add New Account");
            Console.WriteLine("5. Update Customer Password");
            Console.WriteLine("6. Delete Customer");
            Console.WriteLine("7. Logout");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAllCustomers(bank);
                    break;
                case "2":
                    ViewAllAccounts(bank);
                    break;
                case "3":
                    AddCustomerFlow(bank);
                    break;
                case "4":
                    AddAccountFlow(bank);
                    break;
                case "5":
                    UpdateCustomerPasswordFlow(bank);
                    break;
                case "6":
                    DeleteCustomerFlow(bank);
                    break;
                case "7":
                    exit = true;
                    Console.WriteLine("Logging out...\n");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.\n");
                    break;
            }
        }

    static void ViewAllCustomers(Bank bank)
    {
        List<Customer> customers = new List<Customer>();

        foreach (User user in bank.GetAllUsers())
        {
            if (user is Customer customer)
            {
                customers.Add(customer);
            }
        }

        if (customers.Count == 0)
        {
            Console.WriteLine("No customers found.\n");
            return;
        }

        Console.WriteLine("All Customers:");
        foreach (Customer customer in customers)
        {
            Console.WriteLine($"  {customer} — {customer.Accounts.Count} account(s)");
        }
        Console.WriteLine();
    }

    static void ViewAllAccounts(Bank bank)
    {
        List<Account> accounts = bank.GetAllAccounts();

        if (accounts.Count == 0)
        {
            Console.WriteLine("No accounts found.\n");
            return;
        }

        Console.WriteLine("All Accounts:");
        foreach (Account account in accounts)
        {
            Console.WriteLine($"  {account}");
        }
        Console.WriteLine();
    }

    static void AddCustomerFlow(Bank bank)
    {
        Console.Write("New username: ");
        string username = Console.ReadLine();

        if (bank.FindUser(username) != null)
        {
            Console.WriteLine("That username is already taken.\n");
            return;
        }

        Console.Write("New password: ");
        string password = Console.ReadLine();

        Customer newCustomer = new Customer(username, password);
        bank.AddUser(newCustomer);
        Console.WriteLine($"Customer '{username}' created.\n");
    }

    static void AddAccountFlow(Bank bank)
    {
        Console.Write("Customer username to open the account for: ");
        string username = Console.ReadLine();

        User user = bank.FindUser(username);
        if (user is not Customer)
        {
            Console.WriteLine("No customer found with that username.\n");
            return;
        }

        Console.Write("Account type (1 = Savings, 2 = Checking): ");
        string typeChoice = Console.ReadLine();

        Console.Write("Initial deposit amount: ");
        decimal.TryParse(Console.ReadLine(), out decimal initialBalance);

        Account newAccount;
        switch (typeChoice)
        {
            case "1":
                newAccount = new SavingsAccount(bank.GenerateAccountNumber("SAV"), username, initialBalance);
                break;
            case "2":
                newAccount = new CheckingsAccount(bank.GenerateAccountNumber("CHK"), username, initialBalance);
                break;
            default:
                Console.WriteLine("Invalid account type.\n");
                return;
        }

        bank.AddAccount(newAccount);
        Console.WriteLine($"Account {newAccount.AccountNumber} created for {username}.\n");
    }

    static void UpdateCustomerPasswordFlow(Bank bank)
    {
        Console.Write("Customer username: ");
        string username = Console.ReadLine();

        User user = bank.FindUser(username);
        if (user == null)
        {
            Console.WriteLine("No user found with that username.\n");
            return;
        }

        Console.Write("New password: ");
        string newPassword = Console.ReadLine();

        user.UpdatePassword(newPassword);
        Console.WriteLine($"Password updated for '{username}'.\n");
    }

    static void DeleteCustomerFlow(Bank bank)
    {
        Console.Write("Username to delete: ");
        string username = Console.ReadLine();

        User user = bank.FindUser(username);
        if (user is not Customer customer)
        {
            Console.WriteLine("No customer found with that username.\n");
            return;
        }

        List<Account> accountsCopy = new List<Account>(customer.Accounts);
        foreach (Account account in accountsCopy)
        {
            bank.RemoveAccount(account.AccountNumber);
        }

        bank.RemoveUser(username);
        Console.WriteLine($"Customer '{username}' and their accounts were deleted.\n");
    }
    }

    // ---------------- Customer ----------------

    static void CustomerDashboard(Customer customer, Bank bank)
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine($"--- Customer Dashboard ({customer.Username}) ---");
            Console.WriteLine("1. View My Accounts");
            Console.WriteLine("2. Deposit");
            Console.WriteLine("3. Withdraw");
            Console.WriteLine("4. Transfer");
            Console.WriteLine("5. Logout");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewAccounts(customer);
                    break;
                case "2":
                    DepositFlow(customer);
                    break;
                case "3":
                    WithdrawFlow(customer);
                    break;
                case "4":
                    TransferFlow(customer, bank);
                    break;
                case "5":
                    exit = true;
                    Console.WriteLine("Logging out...\n");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.\n");
                    break;
            }
        }
    }

    static void ViewAccounts(Customer customer)
    {
        if (customer.Accounts.Count == 0)
        {
            Console.WriteLine("You have no accounts.\n");
            return;
        }

        Console.WriteLine("Your Accounts:");
        for (int i = 0; i < customer.Accounts.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {customer.Accounts[i]}");
        }
        Console.WriteLine();
    }

    // Shared by deposit/withdraw/transfer — lets the customer pick which of their accounts to use
    static Account SelectAccount(Customer customer)
    {
        if (customer.Accounts.Count == 0)
        {
            Console.WriteLine("You have no accounts.\n");
            return null;
        }

        ViewAccounts(customer);
        Console.Write("Select an account by number: ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int index) && index >= 1 && index <= customer.Accounts.Count)
        {
            return customer.Accounts[index - 1];
        }

        Console.WriteLine("Invalid selection.\n");
        return null;
    }

    static void DepositFlow(Customer customer)
    {
        Account account = SelectAccount(customer);
        if (account == null) return;

        Console.Write("Enter deposit amount: ");
        string input = Console.ReadLine();

        if (decimal.TryParse(input, out decimal amount) && account is AccountOperations ops)
        {
            ops.Deposit(amount);
            Console.WriteLine($"Deposit successful. New balance: {account.Balance:C}\n");
        }
        else
        {
            Console.WriteLine("Invalid amount.\n");
        }
    }

    static void WithdrawFlow(Customer customer)
    {
        Account account = SelectAccount(customer);
        if (account == null) return;

        Console.Write("Enter withdrawal amount: ");
        string input = Console.ReadLine();

        if (decimal.TryParse(input, out decimal amount) && account is AccountOperations ops)
        {
            bool success = ops.Withdraw(amount);
            Console.WriteLine(success
                ? $"Withdrawal successful. New balance: {account.Balance:C}\n"
                : "Withdrawal failed.\n");
        }
        else
        {
            Console.WriteLine("Invalid amount.\n");
        }
    }

    static void TransferFlow(Customer customer, Bank bank)
    {
        Console.WriteLine("Transfer from one of your accounts to any account number.");
        Account fromAccount = SelectAccount(customer);
        if (fromAccount == null) return;

        Console.Write("Enter destination account number: ");
        string toAccountNumber = Console.ReadLine();
        Account toAccount = bank.FindAccount(toAccountNumber);

        if (toAccount == null)
        {
            Console.WriteLine("Destination account not found.\n");
            return;
        }

        if (toAccount.AccountNumber == fromAccount.AccountNumber)
        {
            Console.WriteLine("Cannot transfer to the same account.\n");
            return;
        }

        Console.Write("Enter transfer amount: ");
        string input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal amount))
        {
            Console.WriteLine("Invalid amount.\n");
            return;
        }

        if (fromAccount is AccountOperations fromOps && toAccount is AccountOperations toOps)
        {
            bool withdrawn = fromOps.Withdraw(amount);
            if (withdrawn)
            {
                toOps.Deposit(amount);
                Console.WriteLine($"Transfer successful. New balance: {fromAccount.Balance:C}\n");
            }
            else
            {
                Console.WriteLine("Transfer failed.\n");
            }
        }
    }

    static void ConcludingMessage()
    {
        Console.WriteLine("Thank you for using the Banking Application. Goodbye!");
    }
}