# DataManager
게임의 정적 데이터를 미리 캐싱해 놓은 클래스<br/>
Table Data를 비롯하여 Atals, 환경설정을 관리합니다.<br/>

## Table Data
game에 필요한 전체 데이터를 프로젝트 시작시 로드하여 미리 저장합니다.<br/>
해당 프로세스는 json데이터를 이용하여 처리됩니다.<br/>

- 예시(npc data load)
```cs
    AddLoadProcess_Json<NPCTableData>("NPCData_Json", (datas) =>
    {
        foreach (var data in datas)
        {
            npcTables.Add(data.Id, data);
        }
    });
   
   ////

    void AddLoadProcess_Json<T>(string fileName, Action<List<T>> action, LoadingType loadingType = LoadingType.defaultProcess) where T : class
    {
        Loading.AddLoadProcess(loadingType,
            UniTask.Defer(async () => { await LoadAssetCoroutine_Json(fileName, action); }));
    }

    ////

    async UniTask LoadAssetCoroutine_Json<T>(string fileName, Action<List<T>> action) where T : class
    {
        var path = $"Address/Data/{fileName}.json";
        var tAsset = await AssetLoader.LoadAsync<TextAsset>(path);
        JsonDefaultForm<List<T>> form = Json.JsonToObject<JsonDefaultForm<List<T>>>(tAsset.text);
        action?.Invoke(form.data);

        AssetLoader.Unload(path, tAsset);
    }
```


## AssetLoader
ObjectBox를 관리하는 클래스로 필요한 path의 GameObject 요소를 저장하여 추후 사용시 Instantiate로 빠르게 사용할 수 있게 도와줍니다.

- ObjectBox<br/>
Resource를 효율적으로 관리하기 위해 제작한 GameObject 래퍼 클래스<br/>
스마트 포인터 역활도 수행하며 참조카운트 관리를 위해 제거시 참조 해제를 반드시 해줘야 합니다.<br/>

```cs
 internal sealed class ObjectBox
{
    private int refCount;
    private UnityEngine.Object obj;

    private readonly string assetPath;

    internal ObjectBox(string assetPath)
    {
        this.assetPath = assetPath;
    }

    public async UniTask<T> ReferencingAsync<T>(CancellationTokenSource cancellationToken = null) where T : UnityEngine.Object
    {
        try
        {
            refCount++;

            if (obj == null && refCount == 1)
            {
                var lhandle = Addressables.LoadAssetAsync<T>(assetPath);
                obj = await lhandle.Task;
            }
            else if (obj == null && refCount > 1)
            {
                await UniTask.WaitUntil(() => { return obj != null; });
            }

            //cancel
            if (cancellationToken != null && cancellationToken.IsCancellationRequested)
            {
                Unreferencing(obj);
                throw new Exception("Cancel");
            }


            if (obj is GameObject)
            {
                //메모리 관련 오버헤드로 생성 및 해제 제한 발생 가능(무시하고 addressable load 사용하고 싶을시 사용)
                //var handle = Addressables.InstantiateAsync(assetPath);
                //return await handle.ToUniTask() as T;

                return GameObject.Instantiate(obj) as T;
            }
            else
                return obj as T;
        }
        catch (InvalidKeyException e)
        {
            Debugger.LogError($"InvalidKeyException: {assetPath}");
            refCount--;
            throw new OperationCanceledException($"InvalidKeyException: {assetPath}");
        }
        catch (Exception e)
        {
            Debug.Log("error\n" + e);
            throw e;
        }
    }
}

```