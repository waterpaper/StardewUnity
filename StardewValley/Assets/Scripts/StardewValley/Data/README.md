# GameState
전체 게임 진행 정보를 가지고 있는 클래스입니다.<br/>
게임 시간 및 일자별 업데이트 로직이 처리됩니다.<br/>
세이브, 로드 데이터는 해당 부분에 모두 모여있으며 바이너리 파일로 저장, 로드를 구현했습니다.<br/>
옵저버 패턴을 이용하여 UI등 변경점이 필요한 곳에 연결되게 제작했습니다.<br/>

## Observer Pattern
SubjectData에 action을 등록하여 필요한 곳에 event를 처리하도록 구현했습니다.

- GameState
```cs
 public partial class GameState
 {
     public SubjectData<int> month;
     public SubjectData<int> day;
     public SubjectData<DayOfWeek> dayOfWeek;
     public SubjectData<int> time;
     public float timer;

     public PlayerInfo player = new();
     public Inventory inventory = new();
     ...
 }
```


- SubjectData

```cs
 public struct SubjectData<T>
 {
     private T v;
     public T Value
     {
         get
         {
             return this.v;
         }
         set
         {
             this.v = value;
             this.onChange?.Invoke(value);
         }
     }
     public Action<T> onChange;
 }

```


## Save & Load
유지 보수를 위해 확장 메서드를 이용하여 save & load를 구현햇습니다.</br>
바이너리 파일 및 json 형식을 이용한 데이터를 사용합니다.</br>

```cs
 public static void Save(this GameState gameState, int index)
 {
     BinaryFormatter bf = new BinaryFormatter();
     FileStream file = File.Create(Application.persistentDataPath + $"/SaveData_{index}.dat");
     SaveTableData saveData = new SaveTableData(gameState);
     bf.Serialize(file, saveData);
     file.Close();
     Debug.Log("Game data saved!");
 }

 public static bool Load(this GameState gameState, int index)
 {
     if (File.Exists(Application.persistentDataPath + $"/SaveData_{index}.dat") == false)
         return false;

     BinaryFormatter bf = new BinaryFormatter();
     FileStream file = File.Open(Application.persistentDataPath + $"/SaveData_{index}.dat", FileMode.Open);

     try
     {
                SaveTableData saveTableData = (SaveTableData)bf.Deserialize(file);
     }
     catch
     {

     }
    ...
 }

```