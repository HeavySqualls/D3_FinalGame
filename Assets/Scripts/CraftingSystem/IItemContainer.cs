
public interface IItemContainer 
{
    int ItemCount(string _itemID);
    sItem RemoveItem(string _itemID);
    bool RemoveItem(sItem _item);
    bool AddItem(sItem _item);
    bool IsFull();
}
