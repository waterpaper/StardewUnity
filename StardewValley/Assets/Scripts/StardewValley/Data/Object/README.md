# MapObjectManager
진행 Map에 해당하는 Aspect를 생성 및 관리하는 클래스 입니다.<br/>
entity 만으로는 관리하기 쉽지 않다고 판단하여서 aspect로 component를 모아서 관리합니다.<br/>
판단한 이유는 다음과 같습니다.<br/>

- 해당하는 entity의 component 생성이 한눈에 알기 쉽지 않음
- 필요 entity 조회시 조회가 쉽지 않음 (view 혹은 다른 class에서 사용)
- structural change 발생을 제어하기 위해 entity 생성, 제거, 재참조를 모아서 처리 필요
- 기존 OOP의 개념을 일부 이용 가능


프로젝트에서 Aspect는 하나의 Object 개념으로 사용되며 모든 제어는 Framework에 따라 제어됩니다.
Create, Destroy 이후 재 참조 로직이 실행됩니다.


[MapObjectManager](./MapObjectManager.cs)

## Create
aspect의 생성 제어를 위해 event를 통해 aspect builder를 전달 받습니다.<br/>
이 곳에서는 Builder를 모아 한번에 aspect를 생성합니다.<br/>
발생 순서는 System 동작 전입니다.<br/>


예시<br/>
[CropsAspect](../../ECS/Entity/CropsAspect.cs)
```cs
public class CropsAspectBuilder : IEntityBuilder
{
    float3 pos = float3.zero;
    int id;
    int day;
    EventActionComponent eventActionComponent = new();
    CropsAspect cropsAspect;

    // 나머지 선택 멤버는 메서드로 설정
    public CropsAspectBuilder()
    {
    }

    public CropsAspectBuilder SetPos(float3 pos)
    {
        this.pos = pos;
        return this;
    }

    public CropsAspectBuilder SetId(int id)
    {
        this.id = id;
        return this;
    }

    public CropsAspectBuilder SetDay(int day)
    {
        this.day = day;
        return this;
    }

    public EventActionComponent GetEventSystem()
    {
        return eventActionComponent;
    }

    public IWATPObjectAspect GetObjectAspect()
    {
        return cropsAspect;
    }

    public Entity Build()
    {
        var entity = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity();

        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<TransformComponent>(entity);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<EventComponent>(entity);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ColliderComponent>(entity);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<PhysicsComponent>(entity);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<InteractionComponent>(entity);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<CropsDataComponent>(entity);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<DeleteComponent>(entity);
        World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entity, eventActionComponent);

        var eventComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EventComponent>(entity);
        eventComponent.events = World.DefaultGameObjectInjectionWorld.EntityManager.AddBuffer<EventBuffer>(entity);
        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(entity, eventComponent);

        cropsAspect = World.DefaultGameObjectInjectionWorld.EntityManager.GetAspect<CropsAspect>(entity);
        cropsAspect.Init(pos, id, day);

        return entity;
    }
}
```

## Delete
Aspect의 DeleteReservation 의 값을 통해 제어하며 property를 통해 내부 설정합니다.<br/>
(Component 이용 및 상수 설정은 Aspect마다 다름)

삭제 로직
```cs
private void DestroyUpdate()
{
    foreach (var aspect in aspects)
    {
        if (aspect.DeleteReservation)
        {
            removeList.Add(aspect);
            aspect.OnDestroy();
            ECSController.ServiceEvents.Emit(new EventDeleteEntity(aspect));
        }
    }

    foreach (var aspect in removeList)
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(aspect.Entity);
        aspects.Remove(aspect);
        isRefUpdate = true;
    }

    removeList.Clear();
}
```