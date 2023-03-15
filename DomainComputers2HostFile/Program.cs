using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net;

namespace DomainQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (IsComputerInDomain())
                {
                    Console.WriteLine("Joined to domain: " + Environment.UserDomainName);

                    using (var context = new PrincipalContext(ContextType.Domain))
                    {
                        var computerPrincipal = new ComputerPrincipal(context);
                        computerPrincipal.Enabled = true;

                        using (var searcher = new PrincipalSearcher(computerPrincipal))
                        {
                            foreach (Principal result in searcher.FindAll())
                            {
                                var computer = result as ComputerPrincipal;

                                if (computer != null)
                                {
                                    try
                                    {
                                        var ipAddresses = Dns.GetHostAddresses(computer.Name);
                                        foreach (var ipAddress in ipAddresses)
                                        {
                                            if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                            {
                                                Console.WriteLine($"{ipAddress} {computer.Name}");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error resolving IP address for {computer.Name}: {ex.Message}");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("The computer is not joined to a domain.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static bool IsComputerInDomain()
        {
            return Environment.UserDomainName != Environment.MachineName;
        }
    }
}
