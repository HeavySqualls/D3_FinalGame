
public class LootBoxSlot : ItemSlot
{
    public bool isLooted;
    public override bool CanRecieveItem(sScrapItem _item)
    {
        if (_item == null && !isLooted)
        {
            return true;
        }

        return false;
    }

    public void IsLooted()
    {
        isLooted = true;
        Item = null;
    }
}
