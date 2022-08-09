namespace SDKs.DjiImage.Thermals
{
    /// <summary>
    /// 位置温度集合
    /// </summary>
    public sealed class LTCollection
    {
        List<LTEntry> _entries;
        int _leftsum;
        int _topsum;
        float _tempsum;
        float _mintemp;
        float _maxtemp;
        System.Collections.Generic.List<Location> minList = new System.Collections.Generic.List<Location>();
        System.Collections.Generic.List<Location> maxList = new System.Collections.Generic.List<Location>();

        /// <summary>
        /// 创建对象位置温度集合实例
        /// </summary>
        public LTCollection()
        {
            _entries = new System.Collections.Generic.List<LTEntry>();
            _leftsum = 0;
            _topsum = 0;
            _mintemp = float.NaN;
            _maxtemp = float.NaN;
        }
        /// <summary>
        /// 获取集合中的实例
        /// </summary>
        public LTEntry[] Entries()
        {
            return _entries.ToArray();
        }
        /// <summary>
        /// 获取集合中的指定实例
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public LTEntry[] Entries(Func<LTEntry, bool> predicate)
        {
            return _entries.Where(predicate).ToArray();
        }
        /// <summary>
        /// 最左位置温度
        /// </summary>
        public LTEntry Left { get; private set; }
        /// <summary>
        /// 最上位置温度
        /// </summary>
        public LTEntry Top { get; private set; }
        /// <summary>
        /// 最右位置温度
        /// </summary>
        public LTEntry Right { get; private set; }
        /// <summary>
        /// 最下位置温度
        /// </summary>
        public LTEntry Bottom { get; private set; }
        /// <summary>
        /// 重心位置
        /// </summary>
        public Location BaryCentre
        {
            get
            {
                return new Location()
                {
                    Left = _leftsum / _entries.Count,
                    Top = _topsum / _entries.Count
                };
            }
        }
        /// <summary>
        /// 最低温度
        /// </summary>
        public float MinTemp { get; private set; }
        /// <summary>
        /// 最高温度
        /// </summary>
        public float MaxTemp { get; private set; }
        /// <summary>
        /// 平均温度
        /// </summary>
        public float AveTemp
        {
            get
            {
                return MathF.Round(_tempsum / _entries.Count, 1);
            }
        }
        /// <summary>
        /// 最高温度位置列表
        /// </summary>
        public Location[] MaxTempLocs
        {
            get { return minList.ToArray(); }
        }
        /// <summary>
        /// 最低温度位置列表
        /// </summary>
        public Location[] MinTempLocs
        {
            get { return maxList.ToArray(); }
        }
        /// <summary>
        /// 集合元素数量
        /// </summary>
        public int Count { get { return _entries.Count; } }

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
            if (this.Left.Left > entry.Left)
            {
                this.Left = entry;
            }
            else if (this.Right.Left < entry.Left)
            {
                this.Right = entry;
            }
            if (this.Top.Top > entry.Top)
            {
                this.Top = entry;
            }
            else if (this.Bottom.Top < entry.Top)
            {
                this.Bottom = entry;
            }
            this._leftsum++;
            this._topsum++;
            this._tempsum += entry.Temp;
            if (_mintemp > entry.Temp)
            {
                _mintemp = entry.Temp;
                minList.Clear();
                minList.Add(new Location() { Left = entry.Left, Top = entry.Top });
            }
            else if (_mintemp == entry.Temp)
            {
                minList.Add(new Location() { Left = entry.Left, Top = entry.Top });
            }
            if (_maxtemp < entry.Temp)
            {
                _maxtemp = entry.Temp;
                minList.Clear();
                maxList.Add(new Location() { Left = entry.Left, Top = entry.Top });
            }
            else if (_maxtemp == entry.Temp)
            {
                maxList.Add(new Location() { Left = entry.Left, Top = entry.Top });
            }
        }
    }
}