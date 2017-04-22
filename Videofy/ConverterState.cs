using System;


namespace Videofy.Main
{
    enum ConverterStateEnum
    {
        None,
        Error,
        Done,
        Working
    }

    class ConverterState {
        private ConverterStateEnum state;
        public ConverterState()
        {
            state = ConverterStateEnum.None;
        }

        public void Set(ConverterStateEnum NewState)
        {
            state = NewState;
        }

        public ConverterStateEnum Get()
        {
            return state;
        }
        
        override public String ToString()
        {
            switch(state)
            {
                case ConverterStateEnum.Done: return "Done";
                case ConverterStateEnum.Error: return "Error";
                case ConverterStateEnum.None: return "None";
                case ConverterStateEnum.Working: return "Working";
            }
            return "UNKNOWN";
        }
    }
}
