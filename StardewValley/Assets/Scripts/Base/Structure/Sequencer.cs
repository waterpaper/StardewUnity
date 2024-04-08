using System.Collections.Generic;

namespace WATP.Structure
{
    /// <summary>
    /// 시간을 진행해 원하는 데이터를 순차적으로 반환하는 시퀀서입니다.<br/>
    /// </summary>
    /// <typeparam name="T"> 시퀀스에 들어갈 데이터 </typeparam>
    /// <example>
    /// 코드:
    /// <code>
    /// var sequencer = new Sequencer&lt;string&gt;();
    /// sequencer.Add(1, "1초 지점");
    /// sequencer.Add(2, "2초 지점");
    /// sequencer.Add(3, "3초 지점1", "3초 지점2");
    /// //
    /// while(gameLoop)
    /// {
    ///     sequencer.Accumulate(deltaTime);
    ///     foreach(var state in sequencer.ConsumeQueue())
    ///     {
    ///         Console.WriteLine(state);
    ///     }
    /// }
    /// //
    /// sequencer.Clear();
    /// </code> <br/>
    /// 출력:
    /// <code>
    /// =======================
    /// 1초 지점
    /// 2초 지점
    /// 3초 지점1
    /// 3초 지점2
    /// =======================
    /// </code>
    /// </example>
    public class Sequencer<T>
    {
        private readonly SortedList<float, List<T>> _list;
        private readonly IList<float> _timeLine;
        private int _pivot;
        public float AccumulatedTime { get; private set; } = 0;

        private readonly Queue<T> _queue;
        
        public bool ShouldConsumeQueue => _queue.Count > 0;
        public bool IsAccumulating => _pivot != 0;
        
        public bool IsFinished => _pivot == _timeLine.Count;

        public Sequencer()
        {
            _list = new(64);
            _timeLine = _list.Keys;
            _queue = new(16);
        }

        public Sequencer(int capacity, int queueCapacity)
        {
            _list = new(capacity);
            _timeLine = _list.Keys;
            _queue = new(queueCapacity);
        }
        
        /// <summary>
        /// 시퀀스에 아이템을 추가합니다.
        /// </summary>
        /// <param name="time">추가할 지점의 시간</param>
        /// <param name="value">추가할 아이템</param>
        public void Add(float time, T value)
        {
            if (_pivot != 0)
            {
                throw new System.InvalidOperationException("시퀀스가 진행중입니다.");
            }
            if (!_list.TryGetValue(time, out var list))
            {
                list = new List<T>(4);
                _list.Add(time, list);
            }
            list.Add(value);
        }
        
        /// <summary>
        /// 시퀀스를 진행합니다.<br/>
        /// 진행 전후 <see cref="ShouldConsumeQueue"/>로 큐가 비어있는지 확인해야합니다.<br/>
        /// 큐 소모 관련 자세한 내용은 <see cref="ConsumeQueue"/>를 참고하세요.
        /// </summary>
        /// <param name="time"></param>
        public void Accumulate(float time)
        {
            AccumulatedTime += time;
            if (_pivot == _timeLine.Count)
            {
                return;
            }
            var saved = _timeLine[_pivot];
            if (saved > AccumulatedTime)
            {
                return;
            }
            
            _pivot++;
            foreach (var item in _list[saved])
            {
                _queue.Enqueue(item);
            }
        }
        
        /// <summary>
        /// 시퀀스를 진행하면 큐에 진행된 시퀀스가 쌓이는데, 이를 소모합니다.<br/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> ConsumeQueue()
        {
            while (_queue.Count > 0)
            {
                yield return _queue.Dequeue();
            }
        }
        
        /// <summary>
        /// 시퀀스 진행을 초기화 합니다.<br/>
        /// 시쿼스 아이템들이 지워지지는 않습니다.
        /// </summary>
        public void Rewind()
        {
            AccumulatedTime = 0;
            _pivot = 0;
            _queue.Clear();
        }
        
        /// <summary>
        /// 시퀀스를 전부 지웁니다.
        /// </summary>
        public void Clear()
        {
            Rewind();
            _list.Clear();
        }
    }
}