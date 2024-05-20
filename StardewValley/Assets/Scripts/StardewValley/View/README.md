# ViewManager
entity의 해당되는 것중 render가 필요한 부분을 생성, 관리해주는 클래스입니다.<br/>
pure ECS이기 때문에 필요시 view를 추가 해줘야 하며, 해당 매니저는 ecs object event로 통신 하여 관리됩니다.(이벤트는 별도로 구현해야 합니다.)

## View
aspect를 화면에 표현해주는 class입니다.<br/>
프리팹을 내부에 포함하고 있으며 late load(비동기) 합니다.<br/>

[View](../../Base/View/View.cs)

- 생성 부분<br/>
각 view에 맞는 prefab을 지정하여 로드 할 수 있게 설계되었습니다.<br/>
prefab은 Transform 속성에 저장됩니다.<br/>
```cs
 public virtual async UniTask<Transform> LoadAsync(string customPrefabPath, Transform parent, CancellationTokenSource cancellationToken)
 {
     if (!isPrefab)
     {
         PrefabPath = customPrefabPath ?? "";
         Transform = new GameObject().transform;
         Transform.SetParent(parent, true);
         Transform.position = entity.Position;

         await AssetLoader.InstantiateAsync(PrefabPath, Transform, default, default, default, cancellationToken);
         if (cancellationToken.IsCancellationRequested) return null;

         isPrefab = true;

         if (!isAlreadyDisposed && entity != null)
             OnLoad();
     }

     return isAlreadyDisposed && entity != null ? null : Transform;
 }
```

pure ecs이기 때문에 prefab을 표현할 view를 따로 구현하였으며 structural change시 재 참조된 데이터를 받아야 합니다.<br/><br/>


## Create Prefab

[ViewManager](../../Base/View/ViewManager.cs)

- 등록 부분<br/>
먼저 entity 생성, 제거시 event를 등록합니다.
```cs
private void Bind()
{
    ECSController.ServiceEvents.On<EventCreateEntity>(OnEventCreateEntity);
    ECSController.ServiceEvents.On<EventDeleteEntity>(OnEventDeleteEntity);
    ...
}

private void UnBind()
{
    ECSController.ServiceEvents.Off<EventCreateEntity>(OnEventCreateEntity);
    ECSController.ServiceEvents.Off<EventDeleteEntity>(OnEventDeleteEntity);
    ...
}

void OnEventCreateEntity(EventCreateEntity e)
{
    CreateObj(e.Entity);
}

void OnEventDeleteEntity(EventDeleteEntity e)
{
    RemoveObj(e.Entity);
}

```

---


- 생성 부분<br/>
각 aspect에 맞는 Class를 찾아 구현해줍니다.<br/>
제거 또한 동일 방식으로 처리됩니다.<br/>

```cs
private async void CreateObj(IWATPObjectAspect entityData)
{
    try
    {
        ///맞는 class 제작
        IView prs = InitObj(entityData);

        if (prs == null)
            throw new Exception("Not view manager init class");
        
        prsDic.Add(entityData.Index, prs);
        ViewController.ServiceEvents.Emit(new ViewCreateEvent(prs));

        var prefab = prs as IPrefabHandler;
        await prefab.LoadAsync(prefab.PrefabPath, prefab.Parent, cancellationToken);
    }
    catch (Exception e)
    {
        throw new OperationCanceledException("Prefab(view) Load Error\n" + e);
    }
}
```