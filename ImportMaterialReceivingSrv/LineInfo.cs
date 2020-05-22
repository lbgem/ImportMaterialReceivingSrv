using System;

namespace ImportMaterialReceivingSrv
{
    public class LineInfo
    {
        String _PRI_KEY;
        String _HEADER_PRI_KEY;
        String _ITEM_NUMBER;
        String _ITEM_DESCRIPTION;
        String _UNIT_OF_MEASURE;
        String _SUBINVENTORY_CODE;
        String _SUBINVENTORY_DESC;
        String _LOT_NUMBER;
        int _QUANTITY;
        String _BUDGET_PRI_KEY;
        String _HTBM_VENDOR_PRODUCT_ID;
        String _TERMINAL_CATEGORY;
        String _HTBM_LOT_NUM;

        public string PRI_KEY { get => _PRI_KEY; set => _PRI_KEY = value; }
        public string HEADER_PRI_KEY { get => _HEADER_PRI_KEY; set => _HEADER_PRI_KEY = value; }
        public string ITEM_NUMBER { get => _ITEM_NUMBER; set => _ITEM_NUMBER = value; }
        public string ITEM_DESCRIPTION { get => _ITEM_DESCRIPTION; set => _ITEM_DESCRIPTION = value; }
        public string UNIT_OF_MEASURE { get => _UNIT_OF_MEASURE; set => _UNIT_OF_MEASURE = value; }
        public string SUBINVENTORY_CODE { get => _SUBINVENTORY_CODE; set => _SUBINVENTORY_CODE = value; }
        public string SUBINVENTORY_DESC { get => _SUBINVENTORY_DESC; set => _SUBINVENTORY_DESC = value; }
        public string LOT_NUMBER { get => _LOT_NUMBER; set => _LOT_NUMBER = value; }
        public int QUANTITY { get => _QUANTITY; set => _QUANTITY = value; }
        public string BUDGET_PRI_KEY { get => _BUDGET_PRI_KEY; set => _BUDGET_PRI_KEY = value; }
        public string HTBM_VENDOR_PRODUCT_ID { get => _HTBM_VENDOR_PRODUCT_ID; set => _HTBM_VENDOR_PRODUCT_ID = value; }
        public string TERMINAL_CATEGORY { get => _TERMINAL_CATEGORY; set => _TERMINAL_CATEGORY = value; }
        public string HTBM_LOT_NUM { get => _HTBM_LOT_NUM; set => _HTBM_LOT_NUM = value; }
    }
}