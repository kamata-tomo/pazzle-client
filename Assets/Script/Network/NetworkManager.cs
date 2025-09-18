using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// APIとの通信を管理するクラス
/// - 全てのAPIエンドポイントに対応
/// - 通信はIEnumerator + コールバック(Action<T>)形式
/// - 成功時はJSONをパースして返す
/// </summary>
public class NetworkManager : MonoBehaviour
{
#if DEBUG
    const string API_BASE_URL = "http://localhost:8000/api/";
#else
    const string API_BASE_URL = "https://ge202405.japaneast.cloudapp.azure.com/api/";
#endif


    private string userName;
    private string apiToken;

    public string UserName => this.userName;
    public string ApiToken => this.apiToken;

    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObj = new GameObject("NetworkManager");
                instance = gameObj.AddComponent<NetworkManager>();
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    // ========================================
    // ユーザー関連
    // ========================================

    // ユーザー存在確認
    public IEnumerator CheckUserExists(Action<bool> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/show");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest();

        bool exists = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            exists = true;
        }
        callback?.Invoke(exists);
    }

    // ユーザー登録
    public IEnumerator RegistUser(string name, Action<bool> callback)
    {
        RegistUserRequest requestData = new RegistUserRequest { Name = name };
        string json = JsonConvert.SerializeObject(requestData);

        UnityWebRequest request = UnityWebRequest.Post(
            API_BASE_URL + "users/store", json, "application/json");

        yield return request.SendWebRequest();

        bool isSuccess = false;
        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            string resultJson = request.downloadHandler.text;
            RegistUserResponse response = JsonConvert.DeserializeObject<RegistUserResponse>(resultJson);

            this.userName = name;
            this.apiToken = response.APIToken;
            SaveUserData();

            isSuccess = true;
        }

        callback?.Invoke(isSuccess);
    }


    // ユーザー情報取得
    public IEnumerator GetUser(Action<ShowUserResponse> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/show");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<ShowUserResponse>(request.downloadHandler.text);
            callback?.Invoke(response);
        }
        else callback?.Invoke(null);
    }

    // ユーザー情報更新
    public IEnumerator UpdateUser(string name, int? experience, int? item_quantity, Action<ShowUserResponse> callback)
    {
        UpdateUserRequest requestData = new UpdateUserRequest();
        if (name != null) requestData.Name = name;
        if (experience != null) requestData.Experience = experience.Value;
        if (item_quantity != null) requestData.ItemQuantity = item_quantity.Value;

        string json = JsonConvert.SerializeObject(requestData);

        UnityWebRequest request = UnityWebRequest.Post(
            API_BASE_URL + "users/update", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<ShowUserResponse>(request.downloadHandler.text);
            if (name != null) this.userName = name;
            SaveUserData();
            callback?.Invoke(response);
        }
        else callback?.Invoke(null);
    }

    // 称号一覧取得
    public IEnumerator GetTitles(Action<List<TitleData>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/show-title");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var acqs = JsonConvert.DeserializeObject<List<AcquisitionResponse>>(request.downloadHandler.text);
            var result = new List<TitleData>();

            foreach (var a in acqs)
            {
                if (a.Title != null)
                {
                    result.Add(new TitleData
                    {
                        Id = a.Title.Id,     // ← TitleのIDを使う
                        Name = a.Title.Name  // ← Titleの名前を使う
                    });
                }
            }

            callback?.Invoke(result);
        }
        else callback?.Invoke(null);
    }


    // 称号登録
    public IEnumerator StoreTitle(int titleId, Action<bool> callback)
    {
        StoreTitleRequest req = new StoreTitleRequest { TitleId = titleId };
        string json = JsonConvert.SerializeObject(req);

        UnityWebRequest request = UnityWebRequest.Post(
            API_BASE_URL + "users/store-title", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        callback?.Invoke(request.result == UnityWebRequest.Result.Success);
    }

    // フレンドリクエスト送信
    public IEnumerator StoreFriendRequest(int recipientId, Action<bool> callback)
    {
        StoreFriendRequest req = new StoreFriendRequest { RecipientId = recipientId };
        string json = JsonConvert.SerializeObject(req);

        UnityWebRequest request = UnityWebRequest.Post(
            API_BASE_URL + "users/store-friend-request", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        callback?.Invoke(request.result == UnityWebRequest.Result.Success);
    }

    // フレンドリクエスト一覧
    public IEnumerator GetFriendRequests(Action<string> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/show-friend-request");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        callback?.Invoke(request.downloadHandler.text);
    }

    // フレンド承認
    public IEnumerator StoreFriend(int requestingUserId, Action<bool> callback)
    {
        StoreFriendAcceptRequest req = new StoreFriendAcceptRequest { RequestingUserId = requestingUserId };
        string json = JsonConvert.SerializeObject(req);

        UnityWebRequest request = UnityWebRequest.Post(
            API_BASE_URL + "users/store-friend", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        callback?.Invoke(request.result == UnityWebRequest.Result.Success);
    }

    // フレンド一覧
    public IEnumerator GetFriends(Action<List<FriendData>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/show-friend");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<List<FriendData>>(request.downloadHandler.text);
            callback?.Invoke(response);
        }
        else callback?.Invoke(null);
    }

    // スタミナ自動回復
    public IEnumerator StaminaAutoRecovery(Action<StaminaData> callback)
    {
        UnityWebRequest request = UnityWebRequest.PostWwwForm(API_BASE_URL + "users/stamina-auto-recovery", "");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<StaminaData>(request.downloadHandler.text);
            callback?.Invoke(response);
        }
        else callback?.Invoke(null);
    }

    // スタミナ増減（reason_id指定）
    // NetworkManager.cs 内
    public IEnumerator StaminaChangesByReason(int reasonId, int? amount,Action<StaminaData> onSuccess,Action<string> onError)
    {
        StaminaChangeRequest req = new StaminaChangeRequest { ReasonId = reasonId, Amount = amount };
        string json = JsonConvert.SerializeObject(req);

        UnityWebRequest request = UnityWebRequest.Post(
            API_BASE_URL + "users/stamina-changes-by-reasons", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success && request.responseCode == 200)
        {
            // スタミナ正常更新
            var response = JsonConvert.DeserializeObject<StaminaData>(request.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else
        {
            // エラーJSONを取得
            var errorJson = request.downloadHandler.text;
            string message = "通信エラー";
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorJson);
                if (dict != null && dict.ContainsKey("error"))
                    message = dict["error"]; // "スタミナ不足です" など
            }
            catch { }

            onError?.Invoke(message);
        }
    }


    // フレンドからスタミナ受け取り
    public IEnumerator ProviderStamina(int friendId, Action<bool> callback)
    {
        ProviderStaminaRequest req = new ProviderStaminaRequest { FriendId = friendId };
        string json = JsonConvert.SerializeObject(req);

        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "users/provider-stamina", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        callback?.Invoke(request.result == UnityWebRequest.Result.Success);
    }

    // ログインボーナス取得
    public IEnumerator GetLoginBonus(Action<LoginBonusResponse> callback)
    {
        // API: POST /api/login-bonus
        UnityWebRequest request = UnityWebRequest.PostWwwForm(API_BASE_URL + "login-bonus", "");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<LoginBonusResponse>(request.downloadHandler.text);
            callback?.Invoke(response);
        }
        else
        {
            callback?.Invoke(null);
        }
    }

    // 他ユーザー全件取得
    public IEnumerator GetAllOtherUsers(Action<List<OtherUserData>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "users/show-others");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<List<OtherUserData>>(request.downloadHandler.text);
            callback?.Invoke(response);
        }
        else callback?.Invoke(null);
    }

    // IDでユーザー取得
    public IEnumerator GetUserById(int userId, Action<OtherUserData> callback)
    {
        var payload = new { user_id = userId };
        string json = JsonConvert.SerializeObject(payload);

        UnityWebRequest request = UnityWebRequest.Post(
            API_BASE_URL + "users/show-by-id", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<OtherUserData>(request.downloadHandler.text);
            callback?.Invoke(response);
        }
        else callback?.Invoke(null);
    }



    // ========================================
    // ステージ関連
    // ========================================

    public IEnumerator ShowStages(Action<List<StageData>> callback)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_BASE_URL + "stages/show-stage");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<List<StageData>>(request.downloadHandler.text);
            callback?.Invoke(response);
        }
        else callback?.Invoke(null);
    }

    public IEnumerator ClearStage(int stageId, int evaluation, bool collectibles, Action<bool> callback)
    {
        ClearStageRequest req = new ClearStageRequest { StageId = stageId, Evaluation = evaluation, Collectibles = collectibles };
        string json = JsonConvert.SerializeObject(req);

        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "stages/clear-stage", json, "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + this.apiToken);
        yield return request.SendWebRequest();

        callback?.Invoke(request.result == UnityWebRequest.Result.Success);
    }

    public IEnumerator GetCells(int stageId, Action<List<StageCellData>> callback)
    {
        GetCellsRequest req = new GetCellsRequest { StageId = stageId };
        string json = JsonConvert.SerializeObject(req);

        UnityWebRequest request = UnityWebRequest.Post(API_BASE_URL + "stages/getCells", json, "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var response = JsonConvert.DeserializeObject<List<StageCellData>>(request.downloadHandler.text);
            callback?.Invoke(response);
        }
        else callback?.Invoke(null);
    }

    // ========================================
    // ユーザーデータ保存・読込
    // ========================================
    private void SaveUserData()
    {
        SaveData saveData = new SaveData
        {
            UserName = this.userName,
            APIToken = this.apiToken,
        };
        string json = JsonConvert.SerializeObject(saveData);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", json);
    }

    public bool LoadUserData()
    {
        string path = Application.persistentDataPath + "/saveData.json";
        if (!File.Exists(path)) return false;

        string json = File.ReadAllText(path);
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.userName = saveData.UserName;
        this.apiToken = saveData.APIToken;
        return true;
    }
}
