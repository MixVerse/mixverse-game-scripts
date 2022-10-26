using static Config.ItemConfig;
using Config;

/// <summary>
/// 此结构体作为每个物品同步结构体
/// </summary>
public partial struct ZverseItem
{
    public string itemId;   //表示此物品的唯一ID，无论配置文件还是数据库都需要此ID

   // public int durability;    //当前物品的耐久度

    public ZverseItem(string itemId)
    {
        this.itemId = itemId;
       // durability = 100;
    }


    public Item_Config data
    {
        get
        {
            return  GetData(itemId); 
        }
    }


    public string Id => data.Id;
    public string Name => data.Name;
    public string Introduce => data.Introduce;
    public string NameKey => data.NameKey;
    public string IntroduceKey => data.IntroduceKey;
    public EItemType Type => data.Type;
    public bool Sell => data.Sell;
    public int Price => data.Price;
    public int MaxStack => data.MaxStack;
    public bool IsHang => data.IsHang;
    public string WeaponID => data.WeaponID;
    public bool IsGreen => data.IsGreen;
    public bool Is => data.IsChangeColor;
}