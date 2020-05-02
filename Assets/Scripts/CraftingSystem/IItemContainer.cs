
public interface IItemContainer 
{
    int ItemCount(string _itemID);
    sScrapItem RemoveItem(string _itemID);
    bool RemoveItem(sScrapItem _item);
    bool AddItem(sScrapItem _item);
    bool IsFull();
}
