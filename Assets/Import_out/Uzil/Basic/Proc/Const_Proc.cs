namespace Uzil.Proc {


    public class Const_Proc {
        public const string PROC_JSON_ROOT = "Data/Proc";

        public const string PROC_PREFAB_ROOT = "Prefab/Proc";
    }

    
    public enum ProcNodeState {
        /*未啟用*/Inactive,
        /*啟用中*/Active,
        /*已達成條件*/Complete
    }

}