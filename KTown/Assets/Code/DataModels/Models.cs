using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Amazon.DynamoDBv2.DataModel;

/// <summary>
/// Self reporting by users to server.
/// </summary>
[DynamoDBTable("KTownUsers")]
public class User
{
    /// <summary>
    /// Unique name
    /// </summary>
    [DynamoDBHashKey]
    public string _UserName { get; set; }
    [DynamoDBProperty]
    public string Location { get; set; }
    [DynamoDBProperty]
    public double Lat { get; set; }
    [DynamoDBProperty]
    public double Lon { get; set; }
    /// <summary>
    /// State of Users knowledge. Omits all Infos with 0 value.
    /// </summary>
    [DynamoDBProperty]
    public List<string> Infos { get; set; }
    /// <summary>
    /// Values for Infos.
    /// </summary>
    [DynamoDBProperty]
    public List<int> InfoValues { get; set; }
    [DynamoDBProperty]
    public List<string> Variables { get; set; }
    [DynamoDBProperty]
    public List<int> VarValues { get; set; }
}
[DynamoDBTable("KTownState")]
public class State
{
    [DynamoDBHashKey]
    public string _StateName { get; set; }
    [DynamoDBProperty]
    public int Value { get; set; }

    public State() { }
    public State(States.BasicState bs)
    {
        _StateName = bs.StateName;
        Value = bs.Value;
    }
}

[DynamoDBTable("KTownInfos")][Obsolete]
public class Info
{
    [DynamoDBHashKey]
    public string InfoName { get; set; }
    [DynamoDBProperty]
    public int Status { get; set; }
    [DynamoDBProperty("KnownBy")]
    public List<ulong> KnownByUsers { get; set; }
    [DynamoDBProperty("RevealedBy")]
    public ulong RevealedBy { get; set; }
}

/// <summary>
/// Server reads all ToServer=true messages and then destroys them.
/// Users read all 
/// </summary>
[DynamoDBTable("KtownMessages")]
public class ServerMsg
{
    /// <summary>
    /// unique non-human-readable ID
    /// </summary>
    [DynamoDBHashKey]
    public string MsgName { get; set; }
    [DynamoDBRangeKey]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// value = true => msg is to server
    /// </summary>
    [DynamoDBProperty]
    public bool ToServer { get; set; }
    /// <summary>
    /// value != null => msg is to user
    /// </summary>
    [DynamoDBProperty]
    public string UserName { get; set; }
    [DynamoDBProperty]
    public List<string> Arguments { get; set; }
    [DynamoDBProperty]
    public List<int> Values { get; set; }
    
    public ServerMsg()
    {
        MsgName = Guid.NewGuid().ToString();
        Timestamp = DateTime.Now;
        Arguments = new List<string>();
        Values = new List<int>();
    }
    public ServerMsg(Dictionary<string, int> values, bool isToServer)
    {
        ToServer = isToServer;
        Timestamp = DateTime.Now;
        MsgName = Guid.NewGuid().ToString();
        Values = new List<int>();
        Arguments = new List<string>();
        foreach (var v in values)
        {
            Arguments.Add(v.Key);
            Values.Add(v.Value);
        }
    }
    public Dictionary<string, int> GetValues()
    {
        Dictionary<string, int> result = new Dictionary<string, int>();
        for (int i = 0; i < Arguments.Count; i++)
        {
            result.Add(Arguments[i], Values[i]);
        }
        return result;
    }
    public override string ToString()
    {
        return $"ServerMsg time: {Timestamp}, with arguments: {Arguments.Count}";
    }
}
[DynamoDBTable("KtownBank")]
public class Account
{
    [DynamoDBHashKey]
    public string _UserName { get; set; }
    [DynamoDBProperty]
    public ulong Number { get; set; }
    [DynamoDBProperty]
    public int Balance { get; set; }
}