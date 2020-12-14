using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Amazon;
using Amazon.CognitoSync;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;
    public static int GAME_TIME_DIFFERENCE;
    public static UnityEngine.XR.Management.XRManagerSettings VR_MANAGER = null;
    public static void VR_INIT()
    {
        if (VR_MANAGER == null) { Debug.LogError($"Can't init VR, its not loaded!"); }
        else VR_MANAGER.StartSubsystems();
    }
    public static void VR_DEINIT()
    {
        if (VR_MANAGER == null) { Debug.LogError($"Can't deinit VR, its not loaded!"); }
        else VR_MANAGER.StopSubsystems();
    }

    public float testLat;
    public float EnergyPts;
    public Login LoginData;
    public float UpdateDelayServer = 3, UpdateDelayUser = 3;
    public List<Transform> StartLocationsPerTeam = new List<Transform>();
    
    public string UserName { get { return LoginData.UserName; } }
    public ulong BankAccountNumber { get { return LoginData.BankAccountNumber; } }
    public int Team { get { return LoginData.Team; } }

    DynamoDBContext DynamoContext;
    CognitoAWSCredentials CognitoCred;
    float delayBetweenDecryptAttempts = 15;
    bool canAttemptDecrypt = true;
    [SerializeField]
    int MoneyPts, DataPts, HeatPts;

    public IntStringEvent AddHeatEvent = new IntStringEvent();
    public IntStringEvent AddMoneyEvent = new IntStringEvent();
    public IntStringEvent AddDataEvent = new IntStringEvent();
    public IntStringEvent AddInfoEvent = new IntStringEvent();

    void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        UnityInitializer.AttachToGameObject(gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        GAME_TIME_DIFFERENCE = 0; //TODO: Set game time to real timer
        AddHeatEvent.AddListener((i, s) => HeatPts += i);
        AddMoneyEvent.AddListener((i, s) => MoneyPts += i);
        AddDataEvent.AddListener((i, s) => DataPts += i);
        StartCoroutine(VRLoaderInitRoutine());
    }
    IEnumerator VRLoaderInitRoutine()
    {
        yield return UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Failed to load VR!");
        }
        else
        {
            VR_MANAGER = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager;
        }
    }
    void OnDestroy()
    {
        if (INSTANCE == this) INSTANCE = null;
        if (VR_MANAGER != null)
        {
            VR_MANAGER.StopSubsystems();
            VR_MANAGER.DeinitializeLoader();
            VR_MANAGER = null;
        }
    }
    void Start()
    {
        CognitoCred = new CognitoAWSCredentials("eu-central-1:222adfcf-8e89-478f-b00b-a5deca2d6a4d", RegionEndpoint.EUCentral1);
        AmazonDynamoDBClient client = new AmazonDynamoDBClient(CognitoCred, RegionEndpoint.EUCentral1);
        Debug.Log($"Client: {client}");
        DynamoContext = new DynamoDBContext(client);
        Debug.Log($"Context: {DynamoContext}");
        StartCoroutine(UserUpdateRoutine());
        Debug.Log(CognitoCred.IdentityPoolId);
        //Already in Cognito pool but unauthorized
        LoginData = new Login();
    }
    public void UserLoggedIn()
    {
        Debug.Log($"Login by ID: , name: {UserName}, team: {Team}");
        if (StartLocationsPerTeam.Count > Team)
        {
            MapController.INSTANCE.transform.position = StartLocationsPerTeam[Team].position;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SendUsersOwnData();
    }
    IEnumerator UserUpdateRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(UpdateDelayUser);
            ReadServerMsgs(UserParse, new ScanCondition("UserName", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, UserName));
        }
    }
    IEnumerator ServerUpdateRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(UpdateDelayServer);
            ReadServerMsgs(ServerParse, new ScanCondition("ToServer", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, true));
        }
    }
    void OnResult(AmazonDynamoDBResult res)
    {
        if (res.Exception == null)
        {
            Debug.Log($"Success: {res.State}");
        }
        else
        {
            Debug.LogError($"Request failed");
        }
    }
    void TestReadUser(string userName)
    {
        DynamoContext.LoadAsync<User>(userName, (res) =>
        {
            if (res.Exception == null)
            {
                Debug.Log($"Loaded user: {userName}");
                Debug.Log($"Name: {res.Result._UserName}");
            }
            else
            {
                Debug.LogError($"Failed to load user: {userName}");
            }
        });
    }
    public void SendUsersOwnData()
    {
        if (UserName == "")
        {
            Debug.LogWarning($"Login first!");
            return;
        }
        User u = new User();
        u._UserName = UserName;
        u.Location = MapController.INSTANCE.LocationString;
        u.Lat = MapController.INSTANCE.Lat;
        u.Lon = MapController.INSTANCE.Lon;
        States.BasicState.GET_ALL(StateType.InfoState, u.Infos, u.InfoValues); //users knowledge of infos
        States.BasicState.GET_ALL(StateType.UserVariable, u.Variables, u.VarValues); //users internal variables
        DynamoContext.SaveAsync(u, (res) => { if (res.Exception == null) Debug.Log($"Save {u._UserName} at {u.Location}. Infos: {u.Infos.Count}, Variables: {u.Variables.Count}."); else Debug.LogError(res.Exception); });
    }
    public void GetInfo(string iName, Action<Info> callback)
    {
        DynamoContext.LoadAsync<Info>(iName, (res) =>
        {
            if (res.Exception == null)
            {
                callback.Invoke(res.Result);
            }
            else
            {
                callback.Invoke(null);
            }
        });
    }
    void UpdateInfo(string iName, Action<Info> updateAction = null, int retries = 0)
    {
        DynamoContext.LoadAsync<Info>(iName, (loaded) =>
        {
            if (loaded.Exception != null)
            {
                if (retries > 10)
                {
                    Debug.LogError("Failed to communicate I have decoded info!");
                    return;
                }
                Debug.LogError("Error trying to communicate decoding info, retrying");
                UpdateInfo(iName, updateAction, retries++);
                return;
            }
            Info i = loaded.Result;
            InfoUnit.ALL_INFOS[iName].SetToStatus(i.Status);
            if (updateAction == null) { return; } //no need to send back, as nothing has been updated
            updateAction(i);
            DynamoContext.SaveAsync(i, (res) => { Debug.Log($"Saved I have decoded info: {i.InfoName}"); });
        });
    }
    public void UserCheckMyBankAccount(Action<int> onSuccess, Action onFailure) //called from UI
    {
        DynamoContext.LoadAsync<Account>(UserName, (r)=>
            {
                if (r.Exception != null) { onFailure.Invoke(); return; }
                onSuccess.Invoke(r.Result.Balance); 
        });
    }
    public void ServerUpdateBankAccount(string userName, int value)
    {
        DynamoContext.LoadAsync<Account>(userName, (r) =>
        {
            if (r.Exception != null) { Debug.LogError($"Failed to update bank account! {userName} : {value}"); return; }
            var a = r.Result;
            a.Balance += value;
            DynamoContext.SaveAsync(a, GenericCallback);
        });
    }
    public void SendServerMessage(Dictionary<string, int> args, bool isToServer, string userName)
    {
        Debug.Log($"Sending Server Message with {args.Count} arguments");
        if (userName.Length > 0 && isToServer)
        {
            Debug.LogError($"Trying to send ambiguous message addressed to server and to user: {userName}!");
            return;
        }
        ServerMsg m = new ServerMsg(args, true);
        m.ToServer = isToServer;
        m.UserName = userName;
        DynamoContext.SaveAsync(m, GenericCallback);
    }
    public void ReadServerMsgs(Action<ServerMsg> parse, params ScanCondition[] conditions)
    {
        IEnumerable<ServerMsg> msgs = DynamoContext.ScanAsync<ServerMsg>(conditions) as IEnumerable<ServerMsg>;
        foreach (ServerMsg m in msgs)
        {
            Debug.Log($"Parsing msg: {m.Timestamp}");
            parse.Invoke(m);
            Debug.Log($"Delesting...");
            DynamoContext.DeleteAsync(m, GenericCallback);
        }
    }
    void UserParse(ServerMsg m)
    {
        Debug.Log($"User parsing: {m}, args: {m.Arguments.Count}");
        if (m.Arguments.Count < 1) return;
        if (m.Arguments.Count > 2)
        {
            if (m.Arguments[0] == "money_transfer")
            {
                // money_transfer value
                // sender
                // recepient
                GotMoneyTransfer(m.Values[1], m.Arguments[0]);
            }
        }
    }
    void GotMoneyTransfer(int money, string from)
    {
        Debug.LogWarning($"Money transfer received: {from} for: {money}ED");
    }
    void ServerParse(ServerMsg m)
    {
        Debug.Log($"Server parsing: {m}, args: {m.Arguments.Count}");
        if (m.Arguments.Count < 1) return;
        if (m.Arguments.Count > 1)
        {
            if (m.Arguments[0] == "game_state_change") ServerGotGameStateChange(m);
        }
        if (m.Arguments.Count > 2)
        {
            if (m.Arguments[0] == "money_transfer") ServerGotMoneyTransferRequest(m);
        }
    }
    void ServerGotMoneyTransferRequest(ServerMsg m)
    {
        Debug.Log($"Money transfer from: {m.Arguments[1]} to: {m.Arguments[2]}, value: {m.Values[0]}");
        SendServerMessage(m.GetValues(), false, m.Arguments[2]); //send notification to recepient
        ServerUpdateBankAccount(m.Arguments[1], -m.Values[0]); //take from sender
        ServerUpdateBankAccount(m.Arguments[2], m.Values[0]); //give to recepient
    }
    void ServerGotGameStateChange(ServerMsg m)
    {
        Debug.Log($"Game state change: {m.Arguments[1]}: {m.Values[1]}");
        States.BasicState bs = States.BasicState.GET_STATE(StateType.GameState, m.Arguments[1]);
        if (bs == null)
        {
            Debug.LogError($"Invalid game state in {m}: {m.Arguments[1]}");
            return;
        }
        bs.SetValue(m.Values[1], true);
        DynamoContext.SaveAsync(new State(bs), GenericCallback);
    }
    public void StartLooting(Location l)
    {
        Debug.Log($"User now hacks {this}, it was already hacked before: {l.IsAlreadyHacked}");
        HeatPts = 0;
        if (l.IsAlreadyHacked)
        {
            AddHeatEvent.Invoke(10, "Deck signature was detected!");
        }
        MoneyPts = 0;
        EnergyPts = 0;
        if (l.mLootable != null)
        {
            var i = l.mLootable.mInfoLoot;
        }
        PlayerPrefs.SetInt($"Hacked{l.LocationName}", 1);
    }
    public void GenericSave<T>(T payload) => DynamoContext.SaveAsync(payload, GenericCallback);
    public void GenericLoad<T>(string hashKey, Action<T> onLoadedAction) => DynamoContext.LoadAsync<T>(hashKey, (res) => onLoadedAction.Invoke(res.Result));
    AmazonDynamoDBCallback GenericCallback = (res) => { if (res.Exception != null) { Debug.LogError("Dynamo operation failed"); Debug.LogError(res.Exception); } else Debug.Log("Dynamo operation succeeded"); };
    public void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}