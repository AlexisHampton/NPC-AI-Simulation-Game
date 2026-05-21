using System;
using Godot;
using Godot.Collections;

//InventoryUI is the base class for all Inventories with a UI component
public partial class InventoryUI : Panel {

    [Export] protected Label itemNameLabel;
    [Export] protected Label itemDescrLabel;
    [Export] protected Array<InventoryCellUI> inventoryCellUIs = new();
    [Export] protected Timer doubleClickTimer;

    //cannot be populated in the inspector, returns null everytime for the keys
    protected Dictionary<InventoryCellUI, bool> takenIndicies = [];
    protected Dictionary<InventoryCell, InventoryCellUI> cellsUI = new();

    protected InventoryCell currClickedCell;
    protected int currClickedCellIndex;

    public event Action<InventoryCell> OnInvCellClicked;

    public override void _Ready() {
        //Connect button Pressed signals to LoadItem
        for (int i = 0; i < inventoryCellUIs.Count; i++) {
            int buttonIndex = i;
            inventoryCellUIs[i].Pressed += () => LoadItem(buttonIndex);
        }

        //populate taken indicies
        for (int i = 0; i < inventoryCellUIs.Count; i++) {
            takenIndicies.Add(inventoryCellUIs[i], false);
        }
        TurnInventoryOn(false);
    }
    //checks if all UI slots are full
    public bool IsFull() {
        return FindNextAvailableUICell() is null;
    }

    //Checks if item ui is already rendered
    public bool HasItem(InventoryCell ic) {
        return cellsUI.ContainsKey(ic);
    }

    //Add or Update InventoryUI with inventory cell data
    public void AddToInventoryUI(InventoryCell ic) {
        if (HasItem(ic)) {
            //update ic ui
            InventoryCellUI cell = cellsUI[ic];
            cell.UpdateUI(ic.item.ItemR.Sprite, ic.itemAmount);
        } else { // assign a new inventory cell UI
            InventoryCellUI nextAvailableCell = FindNextAvailableUICell();
            if (nextAvailableCell is null) return;
            takenIndicies[nextAvailableCell] = true;
            cellsUI.Add(ic, nextAvailableCell);
            nextAvailableCell.UpdateUI(ic.item.ItemR.Sprite, ic.itemAmount);
        }
    }

    private void ClearInventoryUI() {
        //foreach ()
    }

    //Removes an item from the inventory
    public void RemoveFromInventory(InventoryCell ic) {
        if (!HasItem(ic)) return;
        //update ic ui
        InventoryCellUI cell = cellsUI[ic];
        cell.ResetCell();
        takenIndicies[cell] = false;
        if (ic.itemAmount <= 0)
            cellsUI.Remove(ic);
    }

    //Loads an item to the information box where the player can see its name and a description
    public virtual void LoadItem(int buttonIndex) {
        //if player double clicked a cell
        if (currClickedCellIndex == buttonIndex && doubleClickTimer.TimeLeft > 0) {
            OnInvCellClicked.Invoke(currClickedCell);
            ResetCurrClicked();
            return;
        }
        InventoryCellUI cellUI = inventoryCellUIs[buttonIndex];
        InventoryCell ic = null;
        currClickedCellIndex = buttonIndex;

        foreach (InventoryCell invCell in cellsUI.Keys)
            if (cellsUI[invCell] == cellUI) {
                ic = invCell;
                currClickedCell = ic;
                break;
            }

        //in case player clicks on empty cell
        if (ic is not null) {
            itemDescrLabel.Text = ic.itemR.ItemDescr + "\n" + ic.itemR.PrintStats();
            itemNameLabel.Text = ic.itemR.ItemName;
            doubleClickTimer.Start();
        } else {
            itemNameLabel.Text = itemDescrLabel.Text = "";
            currClickedCell = null;
            currClickedCellIndex = -1;
        }

    }

    //Find the first available InventoryUICell
    public InventoryCellUI FindNextAvailableUICell() {
        foreach (InventoryCellUI cellUI in takenIndicies.Keys) {
            if (!takenIndicies[cellUI])
                return cellUI;
        }
        return null;
    }

    //Hide inventory and show mouse
    public virtual void HandleXButtonPressed() {
        TurnInventoryOn(false);
    }

    //Make the inventory visible or invisible
    public virtual void TurnInventoryOn(bool isOn) {
        Visible = isOn;
        itemNameLabel.Text = "";
        itemDescrLabel.Text = "";
        ResetCurrClicked();
    }

    //Resets current clicked button
    public void ResetCurrClicked() {
        currClickedCell = null;
        currClickedCellIndex = -1;
    }


}
