# ECS
Dots 적용을 편하게 하기 위해 data와 logic을 분리하여 구현한 인게임 오브젝트 클래스.<br/>
entity - 각 component를 가진 object.<br/>
component - 각 data를 가진 클래스로 원하는 데이터를 내부 구현 .<br/>
service - 각 component들의 기능을 구현한 클래스.<br/>


### Entity
각 component를 가진 object.<br/>
여러 component interface를 상속 받아 내부에 component를 구현하고 초기화시킨다.<br/>
dots ecs에서의 entity랑 개념과 내부 구현은 다르다.<br/>

예시
```cs
 public class FarmerEntity : Entity, IPlayerComponent, IMoveComponent, IColliderComponent, ISleepComponent, ITargetableComponent, IPhysicsComponent, IMoveInputComponent, IInteractionInputComponent, IUsingInputComponent, IWarpComponent, IFsmComponent
 {
     #region Property

     private PlayerComponent playerComponent;
     public PlayerComponent PlayerComponent { get => playerComponent; }


     private MoveComponent moveComponent;
     public MoveComponent MoveComponent { get => moveComponent; }


     private ColliderComponent colliderComponent;
     public ColliderComponent ColliderComponent { get => colliderComponent; }


     private SleepComponent sleepComponent;
     public SleepComponent SleepComponent { get => sleepComponent; }

     ......
 }
```