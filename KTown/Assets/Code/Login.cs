using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;

public class Login
{
    public string UserName { get; private set; }
    public ulong BankAccountNumber { get; private set; }
    public int Team { get; private set; }
    public bool LoggedIn { get; private set; }

    public Login()
    {

    }
    public bool TryLogin(int userIndex, string password)
    {
        
        Debug.LogWarning("Wrong password!");
        return false;
    }
    public bool GetName(ulong id, out string name)
    {
        name = "";
        return false;
    }
    public bool GetID(string name, out ulong id)
    {
        id = 0;
        return false;
    }
}