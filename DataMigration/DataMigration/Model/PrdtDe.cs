using DataMigration.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.Model
{
    [Serializable]
    public class Debase :ModelBase, INotifyPropertyChanged
    {
        public int parm_seqn
        {
            get
            {
                return _parm_seqn;
            }
            set
            {
                this._parm_seqn = value;
                InvokePropertyChanged("parm_seqn");
            }
        }
        public string title
        {
            get
            {
                return _title;
            }
            set
            {
                this._title = value;
                InvokePropertyChanged("title");
            }
        }
        public string prdt_type
        {
            get
            {
                return _prdt_type;
            }
            set
            {
                this._prdt_type = value;
                InvokePropertyChanged("prdt_type");
            }
        }
        public string prdt_knd
        {
            get
            {
                return _prdt_knd;
            }
            set
            {
                this._prdt_knd = value;
                InvokePropertyChanged("prdt_knd");
            }
        }
        public long beg_date
        {
            get
            {
                return _beg_date;
            }
            set
            {
                this._beg_date = value;
                InvokePropertyChanged("beg_date");
            }
        }
        public long end_date
        {
            get
            {
                return _end_date;
            }
            set
            {
                this._end_date = value;
                InvokePropertyChanged("end_date");
            }
        }
        public string cif_type
        {
            get
            {
                return _cif_type;
            }
            set
            {
                this._cif_type = value;
                InvokePropertyChanged("cif_type");
            }
        }
        public string term_type
        {
            get
            {
                return _term_type;
            }
            set
            {
                this._term_type = value;
                InvokePropertyChanged("term_type");
            }
        }
        public long term
        {
            get
            {
                return _term;
            }
            set
            {
                this._term = value;
                InvokePropertyChanged("term");
            }
        }
        public string prdt_sts
        {
            get
            {
                return _prdt_sts;
            }
            set
            {
                this._prdt_sts = value;
                InvokePropertyChanged("prdt_sts");
            }
        }
        public string prdt_ind
        {
            get
            {
                return _prdt_ind;
            }
            set
            {
                this._prdt_ind = value;
                InvokePropertyChanged("prdt_ind");
            }
        }

        
        

        public string _prdt_ind { get; set; }

        public string _prdt_sts { get; set; }

        public long _term { get; set; }

        public string _term_type { get; set; }

        public string _cif_type { get; set; }

        public long _end_date { get; set; }

        public long _beg_date { get; set; }

        public string _prdt_type { get; set; }
        public string _prdt_knd { get; set; }

        public string _title { get; set; }

        public int _parm_seqn { get; set; }
    }
    [Serializable]
    public class Derate : ModelBase
    {
        public long parm_seqn { get; set; }
        public string rate_type { get; set; }
        public string rate_chg_type { get; set; }
        public string rate_chg_term { get; set; }
        public String get_rate_code { get; set; }
        //利率工厂参数，调用相应函数进行新增
        public string rate_rule_sys { get; set; }
        public string rate_no { get; set; }
        public string rule_cal_type { get; set; }
        public string prdt_flt_type { get; set; }
        public double ratio { get; set; }
        public Derate()
        {
           
        }
    }
    [Serializable]
    public class DeProduct :ModelBase, INotifyPropertyChanged
    {
        
        public DeProduct()
        {
            Cbase=new Debase();
            Crate = new Derate();
        }


        private string _prdt_no = string.Empty;
        private string _sts;
        private string _filler;
        public string prdt_no
        {
            get
            {
                return _prdt_no;
            }
            set
            {
                this._prdt_no = value;
                InvokePropertyChanged("prdt_no");
            }
        }
        public string sts
        {
            get
            {
                return _sts;
            }
            set
            {
                this._sts = value;
                InvokePropertyChanged("sts");
            }
        }
        public string filler
        {
            get
            {
                return _filler;
            }
            set
            {
                this._filler = value;
                InvokePropertyChanged("filler");
            }
        }


        public string group_no
        {
            get
            {
                return _group_no;
            }
            set
            {
                this._group_no = value;
                InvokePropertyChanged("group_no");
            }
        }
        public long parm_seqn
        {
            get
            {
                return _parm_seqn;
            }
            set
            {
                this._parm_seqn = value;
                InvokePropertyChanged("parm_seqn");
            }
        }
        public long beg_date
        {
            get
            {
                return _beg_date;
            }
            set
            {
                this._beg_date = value;
                InvokePropertyChanged("beg_date");
            }
        }
        public long end_date
        {
            get
            {
                return _end_date;
            }
            set
            {
                this._end_date = value;
                InvokePropertyChanged("end_date");
            }
        }



        public Debase Cbase
        {
            get
            {
                return _Cbase;
            }
            set
            {
                this._Cbase = value;
                InvokePropertyChanged("Cbase");
            }
        }

        public Derate Crate
        {
            get
            {
                return _Crate;
            }
            set
            {
                this._Crate = value;
                InvokePropertyChanged("Crate");
            }
        }

        

        private Derate _Crate;

        private Debase _Cbase;

        private long _end_date;

        private long _beg_date;

        private long _parm_seqn;

        private string _group_no;

        
    }
    [Serializable]
    public class ModelBase : ICloneable,INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void InvokePropertyChanged(String propertyName)
        {
            PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);

            PropertyChangedEventHandler changed = PropertyChanged;

            if (changed != null)
            {
                changed(this, e);
            }
        }
        public object Clone()
        {
            return CommonFunc.Clone(this);

        }
    }
}
