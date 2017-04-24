using System;
using System.IO;
using Videofy.Settings;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace Videofy.Main
{
    [Serializable()]
    struct OptionsStruct
    {
        public ResolutionsEnum resolution;
        public Byte density; // bits per cell                           
        public Byte cellCount; // number of cells for DCT
        public int videoQuality; //video encoding quality
        public bool isEncodingCRF; //define if quality must be CRF based or Bitrate
        public EncodingPreset encodingPreset;
        public PixelFormat pxlFmtIn;
        public PixelFormat pxlFmtOut;

        public OptionsStruct(params Object[] args)
        {
            resolution = ResolutionsEnum.p720;
            density = 1;
            cellCount = 1;
            videoQuality = 25;
            isEncodingCRF = true;
            encodingPreset = EncodingPreset.medium;
            pxlFmtIn = PixelFormat.YUV420P;
            pxlFmtOut = PixelFormat.YUV444P;
        }
    }

    class OptionsManager
    {
        private String _path = "cfg.dat";
        public OptionsStruct props;
        private ComboBox _res, _dest, _inf, _outf;

        public OptionsManager(ComboBox res, 
                              ComboBox dest, 
                              ComboBox inf, 
                              ComboBox outf)
        {
            props = new OptionsStruct();
            _res = res;
            _dest = dest;
            _inf = inf;
            _outf = outf;
            if (File.Exists(_path))
            {
                Load();                
            }
            else
            {
                props.resolution = ResolutionsEnum.p720; 
                props.density = 1;
                props.pxlFmtIn = PixelFormat.YUV420P;
                props.pxlFmtOut = PixelFormat.YUV420P;
                Flush();                
            }
            UpdateUI();
            Flush();

        }

        public void Flush()
        {
            Save();

            Config.FrameHeigth = (int)props.resolution;
            Config.BitLevel = props.density;
            Config.PixFmt = props.pxlFmtIn;
            Config.PixFmtOut = props.pxlFmtOut;
        }

        public void UpdateUI()
        {
            int i;
            String s="";

            i = (int)props.resolution;
            _res.SelectedIndex = _res.FindStringExact(i.ToString() + "p");

            _dest.SelectedIndex = props.density - 1;
            
            switch (props.pxlFmtIn)
            {
                case PixelFormat.Gray: s = "Gray";break;
                case PixelFormat.YUV420P: s = "YUV420p"; break;
                case PixelFormat.YUV444P: s = "YUV444p"; break;
            }
            _inf.SelectedIndex = _inf.FindStringExact(s);
            
            switch (props.pxlFmtOut)
            {
                case PixelFormat.Gray: s = "Gray"; break;
                case PixelFormat.YUV420P: s = "YUV420p"; break;
                case PixelFormat.YUV444P: s = "YUV444p"; break;
            }
            _outf.SelectedIndex = _outf.FindStringExact(s);

        }

        public void SetResolution(int index)
        {
            switch (index)
            {
                case 0: props.resolution = ResolutionsEnum.p144; break;
                case 1: props.resolution = ResolutionsEnum.p240; break;
                case 2: props.resolution = ResolutionsEnum.p360; break;
                case 3: props.resolution = ResolutionsEnum.p480; break;
                case 4: props.resolution = ResolutionsEnum.p720;break;
                case 5: props.resolution = ResolutionsEnum.p1080; break;
                case 6: props.resolution = ResolutionsEnum.p1440; break;
                case 7: props.resolution = ResolutionsEnum.p2160; break;
            }
            Flush();
        }

        public void SetDensity(int index)
        {
            props.density = (byte)(index + 1);
            Flush();
        }

        public void SetPxlFmtIn(int index)
        {
            switch (index)
            {
                case 0:
                    props.pxlFmtIn = PixelFormat.Gray;
                    break;
                case 1:
                    props.pxlFmtIn = PixelFormat.YUV420P;
                    break;
                case 2:
                    props.pxlFmtIn = PixelFormat.YUV444P;
                    break;
            }
            Flush();
        }

        public void SetPxlFmtOut(int index)
        {
            switch (index)
            {
                case 0:
                    props.pxlFmtIn = PixelFormat.YUV420P;
                    break;
                case 1:
                    props.pxlFmtIn = PixelFormat.YUV444P;                              
                    break;
            }
            Flush();
        }
        
        public void Load()
        {            
            var f = new FileStream(_path, FileMode.Open);
            props = (OptionsStruct)(new BinaryFormatter()).Deserialize(f);            
            f.Close();
        }
        public void Save()
        {            
            var f = new FileStream(_path, FileMode.Create);
            (new BinaryFormatter()).Serialize(f, props);
            f.Close();           
        }


    }
}
