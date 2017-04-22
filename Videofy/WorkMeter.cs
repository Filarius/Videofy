using System;


namespace Videofy.Tools
{
    class WorkMeter
    {
        public float workTotal
        {
            get { return _workTotal; }
            set {
                if (value>0)
                {
                    _workTotal = value;
                }
            }
        }
        private float _workTotal;        
        private float _position;

        public float position
        {
            get { return _position; }
        }

        public WorkMeter()
        {
            workTotal = 0;
            _position = 0;
        }

        public void Add(float add)
        {
            if (add >= 0)
            {
                _position += add;
            }
            else
            {
                throw new Exception("Not implemented");
            }
        }

        public float GetDone()
        {
            if(workTotal == 0)
            {
                return 0;
            }
            else
            {
                return _position / _workTotal;
            }
        }

        public String GetDonePercent()
        {
            return String.Format("{0:F1}%",GetDone()*100);
        }

        public void Reset()
        {
            _workTotal = 0;
            _position = 0;
        }
    }
}
