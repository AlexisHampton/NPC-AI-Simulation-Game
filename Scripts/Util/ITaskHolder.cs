using Godot.Collections;
using System;

//ITaskHolder provides a new way for tasks to be seen by npcs in a more timely manner
public partial interface ITaskHolder {
    public Array<Task> GetTasks();
}
