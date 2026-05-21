using Godot;
using System;

public interface ISaveData {
    public void OnLoad(SavedData savedData);
    public SavedData OnSave();
}
