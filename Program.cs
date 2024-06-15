using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using Microsoft.Web.Administration;
using System.Security.Principal;


class Program
{
    static void Main()
    {
        Console.WriteLine("Adding IIS AppPool users to Performance Log Users and Performance Monitor Users groups.");
        if (!IsAdministrator())
        {
            Console.WriteLine("Run this tool as an administrator on the IIS instance directly.");
            return;
        }

        Console.WriteLine("Running as administrator.");
        Console.WriteLine("Press any key to continue...");
        Console.ReadLine();


        using (ServerManager serverManager = new ServerManager())
        {
            foreach (ApplicationPool appPool in serverManager.ApplicationPools)
            {
                //User is constructed as a combination of 'IIS AppPool\' and the app pool name
                string appPoolUser = @"IIS AppPool\" + appPool.Name;
                if (!string.IsNullOrEmpty(appPoolUser))
                {
                    //English group names
                    AddUserToGroup(appPoolUser, "Performance Log Users");
                    AddUserToGroup(appPoolUser, "Performance Monitor Users");
                    //Dutch group names
                    //AddUserToGroup(appPoolUser, "Prestatielogboekgebruikers");
                    //AddUserToGroup(appPoolUser, "Prestatiemetergebruikers");
                    //Add other languages as needed
                }
            }
        }
    }


    static bool IsAdministrator()
    {
        using WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }


    static void AddUserToGroup(string username, string groupName)
    {
        using (PrincipalContext pc = new PrincipalContext(ContextType.Machine))
        {
            GroupPrincipal group = GroupPrincipal.FindByIdentity(pc, groupName);
            if (group == null)
            {
                Console.WriteLine($"{groupName} group not found.");
                return;
            }

            NTAccount user = new NTAccount(username);
            var sid = ((SecurityIdentifier)user.Translate(typeof(SecurityIdentifier))).ToString();

            if (!string.IsNullOrWhiteSpace(sid) && !group.Members.Contains(pc, IdentityType.Sid, sid))
            {
                group.Members.Add(pc, IdentityType.Sid, sid);
                group.Save();
                Console.WriteLine($"Added {username} to {groupName} group.");
            }
            else
            {
                Console.WriteLine($"User {username} not found or already a member of Performance Log Users group.");
            }
        }
    }
}
