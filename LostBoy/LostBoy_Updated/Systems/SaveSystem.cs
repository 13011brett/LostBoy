using System.Xml;
using LostBoy.Core;
using LostBoy.Entities;
using LostBoy.Items;
using LostBoy.Maps;

namespace LostBoy.Systems;

/// <summary>
/// Handles saving and loading game state to XML files.
/// Extracted from the old Player class where serialization was mixed with gameplay.
/// </summary>
public static class SaveSystem
{
    public static void Save(Player player, Map map, string fileName)
    {
        try
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("Player");
            doc.AppendChild(root);

            // Stats node
            var stats = doc.CreateElement("Stats");
            root.AppendChild(stats);
            AddTextNode(doc, stats, "Name", player.Name);
            AddTextNode(doc, stats, "CurrentHealth", player.Stats.Health.ToString("F0"));
            AddTextNode(doc, stats, "MaxHealth", player.Stats.MaxHealth.ToString("F0"));
            AddTextNode(doc, stats, "Level", player.Level.ToString());
            AddTextNode(doc, stats, "Armor", player.Stats.Armor.ToString("F0"));
            AddTextNode(doc, stats, "Damage", player.Damage.ToString("F1"));
            AddTextNode(doc, stats, "Experience", player.Experience.ToString());
            AddTextNode(doc, stats, "ExperienceRequired", player.ExperienceRequired.ToString());

            // Map node
            var mapNode = doc.CreateElement("Map");
            root.AppendChild(mapNode);
            AddTextNode(doc, mapNode, "MapX", map.Width.ToString());
            AddTextNode(doc, mapNode, "MapY", map.Height.ToString());
            AddTextNode(doc, mapNode, "MapDifficulty", map.Difficulty.ToString());

            // Inventory
            var invNode = doc.CreateElement("InventoryItems");
            root.AppendChild(invNode);

            foreach (var item in player.Bag.Items)
            {
                var itemNode = doc.CreateElement("InventoryItem");
                AddAttribute(doc, itemNode, "ID", item.Id.ToString());
                AddAttribute(doc, itemNode, "Name", item.Name);
                AddAttribute(doc, itemNode, "ItemSlot", ((int)item.Slot).ToString());
                AddAttribute(doc, itemNode, "IsEquippable", item.IsEquippable.ToString());
                AddAttribute(doc, itemNode, "IsConsumable", item.IsConsumable.ToString());
                AddAttribute(doc, itemNode, "IsEquipped", item.IsEquipped.ToString());
                AddAttribute(doc, itemNode, "InventorySlot", item.InventorySlot.ToString());
                AddAttribute(doc, itemNode, "Quantity", item.Quantity.ToString());
                AddAttribute(doc, itemNode, "MaxQuantity", item.MaxQuantity.ToString());
                AddAttribute(doc, itemNode, "ItemType", item.GetType().FullName ?? "LostBoy.Items.Item");
                AddAttribute(doc, itemNode, "LevelRequired", item.BonusStats.RequiredLevel.ToString());
                AddAttribute(doc, itemNode, "IStrength", item.BonusStats.Strength.ToString());
                AddAttribute(doc, itemNode, "IArmor", item.BonusStats.Armor.ToString("F0"));
                AddAttribute(doc, itemNode, "IHealth", item.BonusStats.Health.ToString("F0"));
                invNode.AppendChild(itemNode);
            }

            using var writer = XmlWriter.Create(fileName + ".xml", new XmlWriterSettings { Indent = true });
            doc.Save(writer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Save failed: {ex.Message}");
        }
    }

    public static (Player player, Map map)? Load(string filePath)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(filePath));

            float hp = GetFloat(doc, "/Player/Stats/CurrentHealth");
            float maxHp = GetFloat(doc, "/Player/Stats/MaxHealth");
            int level = GetInt(doc, "/Player/Stats/Level");
            float damage = GetFloat(doc, "/Player/Stats/Damage");
            string name = GetString(doc, "/Player/Stats/Name");
            int armor = GetInt(doc, "/Player/Stats/Armor");
            int exp = GetInt(doc, "/Player/Stats/Experience");
            int expReq = GetInt(doc, "/Player/Stats/ExperienceRequired");

            int mapX = GetInt(doc, "/Player/Map/MapX");
            int mapY = GetInt(doc, "/Player/Map/MapY");
            int mapDiff = GetInt(doc, "/Player/Map/MapDifficulty");

            var player = Player.FromSaveData(name, hp, maxHp, armor, level, damage, exp, expReq);
            var map = new Map(mapX, mapY, mapDiff);

            // Load inventory items
            foreach (XmlNode node in doc.SelectNodes("/Player/InventoryItems/InventoryItem")!)
            {
                var id = Guid.Parse(GetAttr(node, "ID"));
                string itemName = GetAttr(node, "Name");
                var slot = (ItemSlot)int.Parse(GetAttr(node, "ItemSlot"));
                bool equippable = bool.Parse(GetAttr(node, "IsEquippable"));
                bool consumable = bool.Parse(GetAttr(node, "IsConsumable"));
                bool equipped = bool.Parse(GetAttr(node, "IsEquipped"));
                int quantity = int.Parse(GetAttr(node, "Quantity"));
                int maxQty = int.Parse(GetAttr(node, "MaxQuantity"));
                int lvlReq = int.Parse(GetAttr(node, "LevelRequired"));
                float iArmor = float.Parse(GetAttr(node, "IArmor"));
                float iHealth = float.Parse(GetAttr(node, "IHealth"));

                var item = new Item
                {
                    Id = id,
                    Name = itemName,
                    Slot = slot,
                    IsEquippable = equippable,
                    IsConsumable = consumable,
                    IsEquipped = equipped,
                    Quantity = quantity,
                    MaxQuantity = maxQty,
                    BonusStats = new StatsBuilder()
                        .SetRequiredLevel(lvlReq)
                        .SetArmor(iArmor)
                        .SetHealth(iHealth)
                        .Build()
                };

                player.Bag.AddItem(item, quantity);
            }

            return (player, map);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load failed: {ex.Message}");
            return null;
        }
    }

    private static void AddTextNode(XmlDocument doc, XmlNode parent, string name, string value)
    {
        var node = doc.CreateElement(name);
        node.AppendChild(doc.CreateTextNode(value));
        parent.AppendChild(node);
    }

    private static void AddAttribute(XmlDocument doc, XmlElement node, string name, string value)
    {
        var attr = doc.CreateAttribute(name);
        attr.Value = value;
        node.Attributes.Append(attr);
    }

    private static float GetFloat(XmlDocument doc, string xpath)
        => float.Parse(doc.SelectSingleNode(xpath)!.InnerText);

    private static int GetInt(XmlDocument doc, string xpath)
        => int.Parse(doc.SelectSingleNode(xpath)!.InnerText);

    private static string GetString(XmlDocument doc, string xpath)
        => doc.SelectSingleNode(xpath)!.InnerText;

    private static string GetAttr(XmlNode node, string name)
        => node.Attributes![name]!.Value;
}