using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;
using static Godot.GD;

//Holds an itemR and it's spawned item
public struct ItemInfo {
    public ItemR itemR;
    public Item item;

    public ItemInfo(ItemR ir, Item i) {
        itemR = ir;
        item = i;
    }
}

[GlobalClass]
/// An inventory that spawns IGatherables procedurally into the world
public partial class DynamicInventory : Inventory {

    [Export] private Array<MeshInstance3D> meshes = new Array<MeshInstance3D>();
    [Export] private Node3D meshSpawnPos;
    [Export] private Node3D itemParent;

    //offsets to keep spawned objects away from the edges of the mesh
    [Export] private Vector3 posOffset = new Vector3(0.25f, 0, 0.25f);
    [Export] private Vector3 endOffset = new Vector3(0.25f, 0.25f, 0.25f);
    [Export] private float xOffset = .4f;
    [Export] private float zOffset = .4f;

    private List<Vector3> takenPos = new List<Vector3>();

    //spawned objects
    private System.Collections.Generic.Dictionary<ItemR, List<Item>> itemObjects = new System.Collections.Generic.Dictionary<ItemR, List<Item>>();


    //current mesh items can spawn on
    private int meshIndex = 0;
    private Aabb currMeshAABB;

    //a dirty vector for detecting bad positions
    private Vector3 badVec = new Vector3(-1000, -1000, -1000);
    //amount of random positions tried
    private int randPosAmt = 0;

    public System.Collections.Generic.Dictionary<ItemR, List<Item>> GetItemObjects { get => itemObjects; }

    //Add items to the inventory and spawn them
    public override void AddToInventory(ItemR item, int amt) {
        //Spawn items
        if (!CanSpawnItems()) return;
        base.AddToInventory(item, amt);

        currMeshAABB = meshes[meshIndex].GetAabb();
        List<Item> itemObj = new List<Item>();

        //spawns each item if it can in the world
        for (; amt > 0; amt--) {
            if (!CanSpawnItems()) break;
            Node3D newItem = SpawnItem(item);
            Vector3 pos = RandomPosition(GetItemAABB(newItem));
            newItem.Position = pos;

            //if the position is null, go to the next mesh and pick a position
            PickAPosition(newItem, pos);
            itemObj.Add(newItem.GetNode<Item>("."));
        }
        //if we already have the item, add the new spawned objects to it
        if (itemObjects.ContainsKey(item)) {
            foreach (Item i in itemObj)
                itemObjects[item].Add(i);
        } else
            itemObjects.Add(item, itemObj);
    }

    //Add an already spawned item to the inventory
    public void AddToInventory(ItemR itemR, Item itemSpawned) {
        if (!CanSpawnItems()) return;

        base.AddToInventory(itemR, 1);

        currMeshAABB = meshes[meshIndex].GetAabb();
        itemSpawned.Reparent(itemParent);

        Vector3 pos = RandomPosition(GetItemAABB(itemSpawned));
        itemSpawned.Position = pos;
        itemSpawned.Visible = true;

        //if the position is null, go to the next mesh and pick a position
        PickAPosition(itemSpawned, pos);
        //if we already have the item, add the new spawned objects to it
        if (itemObjects.ContainsKey(itemR)) {
            itemObjects[itemR].Add(itemSpawned);

        } else
            itemObjects.Add(itemR, new List<Item> { itemSpawned });

    }

    //if the position is null, go to the next mesh and pick a position
    private void PickAPosition(Node3D newItem, Vector3 pos) {
        if (pos == badVec) {
            meshIndex++;
            if (!CanSpawnItems()) {
                isFull = true;
                return;
            }
            pos = RandomPosition(GetItemAABB(newItem));
            newItem.Position = pos;
        }
        takenPos.Add(pos);
    }

    //we can spawn items if we don't run out of meshes to spawn on
    private bool CanSpawnItems() {
        if (meshIndex >= meshes.Count) {
            PrintS(GetOwner<Node3D>().Name, "Cannot pack more items");
            return false;
        }
        return true;
    }


    //Gets a Node3D's AABB
    Aabb GetItemAABB(Node3D item) {
        foreach (Node child in item.GetChildren(true))
            if (child is MeshInstance3D mesh)
                return mesh.GetAabb();
        return new Aabb();
    }

    //Spawns the item into the world
    private Node3D SpawnItem(ItemR item) {
        Node3D newItem = item.GetPackedScene().Instantiate<Node3D>();
        itemParent.AddChild(newItem);
        newItem.Position = meshSpawnPos.Position;
        return newItem;
    }
    //randomly generates a position that is not taken or outside a mesh's bounds or is touching another item's mesh
    private Vector3 RandomPosition(Aabb objAABB) {
        //starts at the top left corner of the mesh with the offset from the beginning
        Vector3 startPos = currMeshAABB.Position - objAABB.Position + posOffset;
        //ends at the bottom right corner of the mesh with the offset from the end
        Vector3 endPos = currMeshAABB.End - objAABB.End - endOffset;

        //GD.PrintS(GetParent().Name, "meshIndex:", meshIndex, meshes.Count);
        //the Y sits just above thr meshAABB
        Vector3 randPos = new Vector3(
            (float)RandRange(startPos.X, endPos.X),
            currMeshAABB.End.Y + objAABB.End.Y + meshes[meshIndex].Position.Y,
            (float)RandRange(startPos.Z, endPos.Z)
        );

        //if this position is taken, get a new one
        if (CheckIfPosTaken(randPos, objAABB)) {
            //if we've checked 100 times, return "null"
            if (randPosAmt >= 100)
                return badVec;
            randPosAmt++;
            return RandomPosition(objAABB);
        }
        isFull = false;
        randPosAmt = 0;
        return randPos;
    }

    //Checks if a position will cause the new object to take the same space as another object
    private bool CheckIfPosTaken(Vector3 pos, Aabb objBounds) {
        foreach (Vector3 tp in takenPos) {
            float dist = tp.DistanceTo(pos);
            if (dist < objBounds.End.Z / 2 + zOffset || dist < objBounds.End.X / 2 + xOffset)
                return true;
        }
        return false;
    }

    //////////////////////////////////// remove from inventory //////////////////////////
    //Removes a specified amount of aspecified itemR from the inventory
    public override void RemoveFromInventory(ItemR item, int amt) {
        base.RemoveFromInventory(item, amt);
        if (!itemObjects.ContainsKey(item)) return;

        List<Item> itemObjs = itemObjects[item];
        //destroy objects
        for (int i = 0; i < amt; i++) {
            if (itemObjs.Count == 0)
                return;
            takenPos.Remove(itemObjs[0].Position);
            itemObjs[0].QueueFree();
            itemObjs.RemoveAt(0);
        }
    }

    //Removes a specified amount of aspecified itemR from the inventory
    public ItemInfo RemoveFromInventory(ItemR item) {
        base.RemoveFromInventory(item, 1);
        if (!itemObjects.ContainsKey(item))
            return new ItemInfo(null, null);

        Item itemSpawned = DespawnItem(item);

        return new ItemInfo(item, itemSpawned);

    }

    //random because don't know which key will get
    //Removes a random item but does not despawn it
    public ItemInfo RemoveRandomItemFromInventory() {

        if (itemObjects.Count <= 0) return new ItemInfo(null, null);

        ItemR key = itemObjects.Keys.First();
        base.RemoveFromInventory(key, 1);
        Item item = DespawnItem(key);

        return new ItemInfo(key, item);
    }

    public List<ItemInfo> RemoveAllFromInventory() {
        List<ItemInfo> itemInfos = new List<ItemInfo>();
        while (!IsEmpty())
            itemInfos.Add(RemoveRandomItemFromInventory());
        return itemInfos;
    }

    //Removes an item from inventory and returns it. 
    private Item DespawnItem(ItemR item) {
        List<Item> itemObjs = itemObjects[item];
        Item itemSpawned = itemObjs[0];
        takenPos.Remove(itemSpawned.Position);
        itemObjs.Remove(itemSpawned);
        itemSpawned.Visible = false;

        if (itemObjects[item].Count <= 0) {
            itemObjects.Remove(item);
        }
        meshIndex = 0;
        return itemSpawned;
    }


    /*   RemoveFromInventory(ic.item, ic.itemAmount);
            }
        //Removes all the specified itemR's from the inventory
        public override List<InventoryCell> EmptyInventory() {
            List<InventoryCell> removedItems = base.EmptyInventory();
            foreach (InventoryCell ic in removedItems) {
                PrintS("ic", ic, ic.item, ic.itemAmount);

            return removedItems;
        }
    */
}
