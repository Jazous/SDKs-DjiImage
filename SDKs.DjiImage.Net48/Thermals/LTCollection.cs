using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 位置温度集合
    /// </summary>
    public sealed class LTCollection : IAreaTemperature, IReadOnlyCollection<LTEntry>, IReadOnlyList<LTEntry>, IEnumerable<LTEntry>, IEnumerable
    {
        List<LTEntry> _entries;
        int _leftsum;
        int _topsum;
        float _tempsum;
        float _mintemp;
        float _maxtemp;
        int _left;
        int _top;
        int _right;
        int _bottom;
        bool _hasEntry;
        Location? _baryCentre;

        /// <summary>
        /// 创建集合新实例
        /// </summary>
        public LTCollection()
        {
            _entries = new List<LTEntry>();
            _leftsum = 0;
            _topsum = 0;
            _mintemp = float.NaN;
            _maxtemp = float.NaN;
            _tempsum = 0;
            _hasEntry = false;
        }
        /// <summary>
        /// 创建集合新实例
        /// </summary>
        /// <param name="capacity">初始化容量</param>
        public LTCollection(int capacity)
        {
            _entries = new List<LTEntry>(capacity);
            _leftsum = 0;
            _topsum = 0;
            _mintemp = float.NaN;
            _maxtemp = float.NaN;
            _tempsum = 0;
            _hasEntry = false;
        }
        /// <summary>
        /// 获取指定位置的元素
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <returns></returns>
        public LTEntry this[int index]
        {
            get
            {
                return _entries[index];
            }
        }

        /// <summary>
        /// 重心位置
        /// </summary>
        public Location BaryCentre
        {
            get
            {
                if (_baryCentre == null)
                    _baryCentre = _entries.Count == 0 ? new Location(0, 0) : new Location(_leftsum / _entries.Count, _topsum / _entries.Count);
                return _baryCentre.Value;
            }
        }
        /// <summary>
        /// 最低温度
        /// </summary>
        public float MinTemp
        {
            get { return _mintemp; }
        }
        /// <summary>
        /// 最高温度
        /// </summary>
        public float MaxTemp
        {
            get { return _maxtemp; }
        }
        /// <summary>
        /// 平均温度
        /// </summary>
        public float AvgTemp
        {
            get
            {
                return _entries.Count == 0 ? float.NaN : float.Parse((_tempsum / _entries.Count).ToString("f1"));
            }
        }
        /// <summary>
        /// 水平方向最小位置
        /// </summary>
        public int Left => _left;
        /// <summary>
        /// 垂直方向最小位置
        /// </summary>
        public int Top => _top;
        /// <summary>
        /// 水平方向最大位置
        /// </summary>
        public int Right => _right;
        /// <summary>
        /// 垂直方向最大位置
        /// </summary>
        public int Bottom => _bottom;
        /// <summary>
        /// 集合元素数量
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        /// <summary>
        /// 获取最高温度位置数组
        /// </summary>
        public Location[] MaxTempLocs()
        {
            return _entries.Where(c => c.Temp == _maxtemp).Select(c => new Location(c.Left, c.Top)).ToArray();
        }
        /// <summary>
        /// 获取最低温度位置数组
        /// </summary>
        public Location[] MinTempLocs()
        {
            return _entries.Where(c => c.Temp == _mintemp).Select(c => new Location(c.Left, c.Top)).ToArray();
        }
        /// <summary>
        /// 添加位置温度
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="temp"></param>
        public void Add(int left, int top, float temp)
        {
            Add(new LTEntry() { Left = left, Top = top, Temp = temp });
        }
        /// <summary>
        /// 添加位置温度
        /// </summary>
        /// <param name="entry"></param>
        public void Add(LTEntry entry)
        {
            if (!_hasEntry)
            {
                _mintemp = entry.Temp;
                _maxtemp = entry.Temp;
                _left = entry.Left;
                _top = entry.Top;
                _right = entry.Left;
                _bottom = entry.Top;
                _hasEntry = true;
            }

            if (entry.Left < _left)
                _left = entry.Left;

            if (entry.Left > _right)
                _right = entry.Left;

            if (entry.Top < _top)
                _top = entry.Top;

            if (entry.Top > _bottom)
                _bottom = entry.Top;

            _leftsum += entry.Left;
            _topsum += entry.Top;
            _tempsum += entry.Temp;

            if (_mintemp > entry.Temp)
                _mintemp = entry.Temp;

            if (_maxtemp < entry.Temp)
                _maxtemp = entry.Temp;

            _entries.Add(entry);
        }
        /// <summary>
        /// 添加位置温度
        /// </summary>
        /// <param name="entries"></param>
        public void AddRange(IEnumerable<LTEntry> entries)
        {
            foreach (var entry in entries)
                Add(entry);
        }
        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns></returns>
        public IEnumerator<LTEntry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        /// <summary>
        /// 是否元素相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is LTCollection)
                return this.SequenceEqual((LTCollection)obj);
            return false;
        }
        /// <summary>
        /// 获取当前对象的哈希代码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _entries.GetHashCode();
        }
        /// <summary>
        /// 返回表示当前对象区域温度的字符串
        /// </summary>
        /// <returns></returns>
        /// <remarks>如：{"minTemp":100,"maxTemp":100,"avgTemp":36.7}</remarks>
        public override string ToString()
        {
            return "{\"minTemp\":" + this._mintemp + ",\"maxTemp\":" + this._maxtemp + ",\"avgTemp\":" + this.AvgTemp + "}";
        }
    }
}